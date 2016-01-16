using CookComputing.XmlRpc;
using VidereSubs.OpenSubtitles.Interfaces;
using VidereSubs.OpenSubtitles.Outputs;

namespace VidereSubs.OpenSubtitles
{
    /// <summary>
    /// Class used to communicate with the XML RPC server for actions that don't require a logged in client.
    /// </summary>
    public static class Server
    {
        /// <summary>
        /// Retrieves info about the opensubtitles.org server.
        /// </summary>
        /// <returns>Info about the opensubtitles.org server.</returns>
        public static ServerInfoOutput ServerInfo( )
        {
            IServer proxy = XmlRpcProxyGen.Create<IServer>( );
            XmlRpcStruct ret = proxy.ServerInfo( );

            ServerInfoOutput info = new ServerInfoOutput( ret );

            return info;
        }
    }
}
