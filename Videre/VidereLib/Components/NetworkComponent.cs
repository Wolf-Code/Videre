using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using VidereLib.Attributes;
using VidereLib.EventArgs;
using VidereLib.Networking;

namespace VidereLib.Components
{
    /// <summary>
    /// The component that handles networking.
    /// </summary>
    public class NetworkComponent : ComponentBase
    {
        private Server server;
        private readonly Dictionary<byte, List<MethodInfo>> hooks = new Dictionary<byte, List<MethodInfo>>( );

        /// <summary>
        /// The port on which the networking has been set up.
        /// </summary>
        public int Port { private set; get; }

        /// <summary>
        /// The ip address.
        /// </summary>
        public IPAddress IP => Server.IP;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="player">The videre player.</param>
        public NetworkComponent( ViderePlayer player ) : base( player )
        {

        }

        /// <summary>
        /// Gets called after all components have been added to the player.
        /// </summary>
        protected override void OnInitialize( )
        {
            Assembly assembly = Assembly.GetExecutingAssembly(  );
            foreach ( Type t in assembly.GetTypes( ) )
            {
                foreach ( MethodInfo info in t.GetMethods( BindingFlags.Instance | BindingFlags.NonPublic ) )
                {
                    NetworkRequestAttribute attribute = info.GetCustomAttributes<NetworkRequestAttribute>( ).FirstOrDefault( );

                    if ( attribute == null )
                        continue;

                    Console.WriteLine( info.Name );

                    if ( !hooks.ContainsKey( attribute.Identifier ) )
                        hooks.Add( attribute.Identifier, new List<MethodInfo>( ) );

                    hooks[ attribute.Identifier ].Add( info );
                }
            }
        }

        /// <summary>
        /// Gets the QR code containing the data of the network component.
        /// </summary>
        /// <returns>The QR code image.</returns>
        public Bitmap GetQRCode( )
        {
            QrEncoder encoder = new QrEncoder( );
            List<byte> bytes = new List<byte>( );
            bytes.AddRange( IP.GetAddressBytes( ) );
            bytes.AddRange( BitConverter.GetBytes( Port ) );
            QrCode code = encoder.Encode( bytes );

            const int moduleSizeInPixels = 5;
            GraphicsRenderer renderer = new GraphicsRenderer( new FixedModuleSize( moduleSizeInPixels, QuietZoneModules.Two ), Brushes.Black, Brushes.White );

            Point padding = new Point( 10, 16 );
            DrawingSize dSize = renderer.SizeCalculator.GetSize( code.Matrix.Width );

            Size size = new Size( dSize.CodeWidth, dSize.CodeWidth ) + new Size( 2 * padding.X, 2 * padding.Y );

            Bitmap img = new Bitmap( size.Width, size.Height );
            using ( Graphics graphics = Graphics.FromImage( img ) )
                renderer.Draw( graphics, code.Matrix, padding );

            return img;
        }

        /// <summary>
        /// Sets up the network receiver on a port.
        /// </summary>
        /// <param name="port">The port to receive requests on.</param>
        public void SetUpNetworkReceiver( int port )
        {
            if ( server != null )
                throw new Exception( "Attempting to start a server twice." );

            this.Port = port;
            server = new Server( port );
            server.OnClientConnected += ServerOnOnClientConnected;
        }

        private void ServerOnOnClientConnected( object Sender, OnClientConnectedEventArgs OnClientConnectedEventArgs )
        {
            BackgroundWorker worker = new BackgroundWorker( );
            worker.DoWork += ( O, Args ) =>
            {
                try
                {
                    using ( NetworkStream stream = OnClientConnectedEventArgs.Client.GetStream( ) )
                        using ( BinaryReader reader = new BinaryReader( stream ) )
                            while ( OnClientConnectedEventArgs.Client.Connected )
                            {
                                byte cmd = reader.ReadByte( );
                                Console.WriteLine( cmd );

                                if ( !hooks.ContainsKey( cmd ) )
                                    continue;

                                ViderePlayer.MainDispatcher.Invoke( ( ) =>
                                {
                                    foreach ( MethodInfo info in hooks[ cmd ] )
                                        info.Invoke( this, new object[ ] { reader } );
                                } );
                            }
                }
                catch ( Exception e )
                {
                    Console.WriteLine( e );
                    OnClientConnectedEventArgs.Client.Close( );
                }
            };
            worker.RunWorkerCompleted += ( O, Args ) => OnClientDisconnected( );
            worker.RunWorkerAsync( );
        }

        private void OnClientDisconnected( )
        {
            Console.WriteLine( "Client disconnected" );
            SetUpNetworkReceiver( 13337 );
        }
        
        [NetworkRequest( 0 )]
        private void OnPlayRequest( BinaryReader reader )
        {
            Console.WriteLine( "Play request" );
            Player.GetComponent<StateComponent>( ).Play( );
        }

        [NetworkRequest( 1 )]
        private void OnPauseRequest( BinaryReader reader )
        {
            Console.WriteLine( "Pause request" );
            Player.GetComponent<StateComponent>( ).Pause( );
        }
    }
}
