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

        /// <summary>
        /// Retrieves information about a hash.
        /// </summary>
        /// <param name="token">The token of the client.</param>
        /// <param name="movieHashes">The hashes.</param>
        /// <returns>The output from the server.</returns>
        [XmlRpcMethod( "CheckMovieHash" )]
        XmlRpcStruct CheckMovieHash( string token, string[ ] movieHashes );

        /// <summary>
        /// Retrieves information about a hash.
        /// </summary>
        /// <param name="token">The token of the client.</param>
        /// <param name="movieHashes">The hashes.</param>
        /// <returns>The output from the server.</returns>
        [XmlRpcMethod( "CheckMovieHash2" )]
        XmlRpcStruct CheckMovieHash2( string token, string[ ] movieHashes );

        /// <summary>
        /// Retrieves subtitle information.
        /// </summary>
        /// <param name="token">The token of the client.</param>
        /// <param name="subData">The data to search for.</param>
        /// <param name="parameters">Extra parameters.</param>
        /// <returns>The output from the server.</returns>
        [XmlRpcMethod( "SearchSubtitles" )]
        XmlRpcStruct SearchSubtitles( string token, XmlRpcStruct[ ] subData, XmlRpcStruct parameters );

        /// <summary>
        /// Gets all allowed subtitle languages.
        /// </summary>
        /// <param name="language">The language information.</param>
        /// <returns>The output from the server.</returns>
        [XmlRpcMethod( "GetSubLanguages" )]
        XmlRpcStruct GetSubLanguages( string language );

        /// <summary>
        /// Notifies the server to keep the session open.
        /// </summary>
        /// <param name="token">The token of the client.</param>
        /// <returns>The output from the server.</returns>
        [XmlRpcMethod( "NoOperation" )]
        XmlRpcStruct NoOperation( string token );
    }
}