using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using VidereLib.EventArgs;

namespace VidereLib.Networking
{
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

        private void StartListening( )
        {
            this.server.Start( );

            BackgroundWorker worker = new BackgroundWorker( );
            worker.DoWork += WorkerOnDoWork;
            worker.RunWorkerCompleted += ( Sender, Args ) =>
            {
                TcpClient cl = Args.Result as TcpClient;
                this.Client = cl;
                Console.WriteLine( "Client!" );
                OnClientConnected?.Invoke( this, new OnClientConnectedEventArgs( cl ) );
            };

            worker.RunWorkerAsync( );
        }

        private void WorkerOnDoWork( object Sender, DoWorkEventArgs WorkEventArgs )
        {
            TcpClient cl = this.server.AcceptTcpClient( );
            WorkEventArgs.Result = cl;
        }
    }
}
