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
        private readonly Dictionary<NetworkRequestAttribute.RequestIdentifier, List<MethodInfo>> hooks = new Dictionary<NetworkRequestAttribute.RequestIdentifier, List<MethodInfo>>( );

        /// <summary>
        /// The port on which the networking has been set up.
        /// </summary>
        public ushort Port { private set; get; }

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
            Assembly assembly = Assembly.GetExecutingAssembly( );
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
            QrEncoder qrEncoder = new QrEncoder( ErrorCorrectionLevel.H );
            byte[ ] ipBytes = IP.GetAddressBytes( );
            QrCode qrCode = qrEncoder.Encode( $"{string.Join( ",", ipBytes )},{Port}" );


            const int moduleSizeInPixels = 25;
            GraphicsRenderer renderer = new GraphicsRenderer( new FixedModuleSize( moduleSizeInPixels, QuietZoneModules.Two ), Brushes.Black, Brushes.White );

            DrawingSize dSize = renderer.SizeCalculator.GetSize( qrCode.Matrix.Width );

            Bitmap bmp = new Bitmap( dSize.CodeWidth, dSize.CodeWidth );
            using ( Graphics graphics = Graphics.FromImage( bmp ) )
                renderer.Draw( graphics, qrCode.Matrix );

            return bmp;
        }

        /// <summary>
        /// Sets up the network receiver on a port.
        /// </summary>
        /// <param name="port">The port to receive requests on.</param>
        public void SetUpNetworkReceiver( ushort port )
        {
            if ( server != null )
                if ( port == this.Port )
                    throw new Exception( "Attempting to start a server twice. Have you called ShutdownServer yet?" );
            
            this.Port = port;
            server = new Server( port );
            server.OnClientConnected += ServerOnOnClientConnected;
        }

        /// <summary>
        /// Shuts down the server.
        /// </summary>
        public void ShutdownServer( )
        {
            server.Stop( );
            server = null;
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
                                NetworkRequestAttribute.RequestIdentifier id = ( NetworkRequestAttribute.RequestIdentifier ) cmd;
                                Console.WriteLine( id );

                                if ( !hooks.ContainsKey( id ) )
                                    continue;

                                ViderePlayer.MainDispatcher.Invoke( ( ) =>
                                {
                                    foreach ( MethodInfo info in hooks[ id ] )
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
            SetUpNetworkReceiver( this.Port );
        }

        [NetworkRequest( NetworkRequestAttribute.RequestIdentifier.Play )]
        private void OnPlayRequest( BinaryReader reader )
        {
            Console.WriteLine( "Play request" );
            Player.GetComponent<StateComponent>( ).Play( );
        }

        [NetworkRequest( NetworkRequestAttribute.RequestIdentifier.Pause )]
        private void OnPauseRequest( BinaryReader reader )
        {
            Console.WriteLine( "Pause request" );
            Player.GetComponent<StateComponent>( ).Pause( );
        }

        [NetworkRequest( NetworkRequestAttribute.RequestIdentifier.PauseOrResume )]
        private void OnPauseOrResumeRequest( BinaryReader reader )
        {
            Console.WriteLine( "Pause / resume request" );
            Player.GetComponent<StateComponent>( ).ResumeOrPause( );
        }
    }
}