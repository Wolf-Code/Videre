using System;
using System.Net;
using System.Net.Sockets;
using VidereLib.EventArgs;

namespace VidereLib.Networking
{
    /// <summary>
    /// A TPC server class.
    /// </summary>
    public class Server
    {
        private readonly TcpListener server;
        
        /// <summary>
        /// The client.
        /// </summary>
        public TcpClient Client { private set; get; }

        /// <summary>
        /// Checks whether or not the client is currently connected.
        /// </summary>
        public bool IsClientConnected => Client != null && Client.Connected;

        /// <summary>
        /// Gets called whenever a client has connected.
        /// </summary>
        public event EventHandler<OnClientConnectedEventArgs> OnClientConnected; 

        /// <summary>
        /// The server's IP address it listens on.
        /// </summary>
        public static IPAddress IP
        {
            get
            {
                IPHostEntry host = Dns.GetHostEntry( Dns.GetHostName( ) );
                foreach ( IPAddress ip in host.AddressList )
                    if ( ip.AddressFamily.ToString( ) == "InterNetwork" )
                        return ip;

                return IPAddress.None;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="port">The port to listen on.</param>
        public Server( int port )
        {
            this.server = new TcpListener( IP, port );

            this.StartListening( );
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        public void Stop( )
        {
            this.server.Stop( );
        }

        private void AcceptTcpClientCallback( IAsyncResult result )
        {
            TcpListener srv = ( TcpListener ) result.AsyncState;
            if ( srv?.Server == null || !srv.Server.IsBound )
                return;

            try
            {
                TcpClient cl = srv.EndAcceptTcpClient( result );
                this.Client = cl;
                Console.WriteLine( "Client!" );
                OnClientConnected?.Invoke( this, new OnClientConnectedEventArgs( cl ) );
            }
            catch ( ObjectDisposedException )
            {
                // Object disposed exception, this is fine as it's happening because we're closing the socket.
            }
        }

        private void StartListening( )
        {
            this.server.Start( );
            this.server.BeginAcceptTcpClient( AcceptTcpClientCallback, this.server );
        }
    }
}
