using CookComputing.XmlRpc;

namespace VidereSubs.OpenSubtitles.Interfaces
{
    /// <summary>
    /// The XML RPC interface for client methods.
    /// </summary>
    [XmlRpcUrl( "https://api.opensubtitles.org:443/xml-rpc" )]
    public interface IClient
    {
        /// <summary>
        /// Logs in to the opensubtitles.org server.
        /// </summary>
        /// <param name="username">The username to log in with.</param>
        /// <param name="password">The password to log in with. Can be plain-text or an MD5 hash.</param>
        /// <param name="language">The language to communicate with as an ISO 639-2 code.</param>
        /// <param name="useragent"></param>
        /// <returns>The output from the server.</returns>
        [XmlRpcMethod( "LogIn" )]
        XmlRpcStruct LogIn( string username, string password, string language, string useragent );

        /// <summary>
        /// Logs out the client.
        /// </summary>
        /// <param name="token">The token of the client.</param>
        /// <returns>The output from the server.</returns>
        [XmlRpcMethod( "LogOut" )]
        XmlRpcStruct LogOut( string token );
    }
}