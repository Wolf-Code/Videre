using CookComputing.XmlRpc;

namespace VidereSubs.OpenSubtitles.Interfaces
{
    /// <summary>
    /// The XML RPC proxy for the server.
    /// </summary>
    [XmlRpcUrl( "http://api.opensubtitles.org:80/xml-rpc" )]
    public interface IServer : IXmlRpcProxy
    {
        /// <summary>
        /// Gets the server info from opensubtitles.org.
        /// </summary>
        /// <returns>Server info.</returns>
        [XmlRpcMethod( "ServerInfo" )]
        XmlRpcStruct ServerInfo( );
    }
}
