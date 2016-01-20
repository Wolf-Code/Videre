using System.Net.Sockets;

namespace VidereLib.EventArgs
{
    /// <summary>
    /// EventArgs for the OnClientConnected event.
    /// </summary>
    public class OnClientConnectedEventArgs : System.EventArgs
    {
        /// <summary>
        /// The connected client.
        /// </summary>
        public TcpClient Client { private set; get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="client">The connected client.</param>
        public OnClientConnectedEventArgs( TcpClient client )
        {
            this.Client = client;
        }
    }
}
