using System.Security.Cryptography;
using CookComputing.XmlRpc;
using VidereSubs.OpenSubtitles.Interfaces;
using VidereSubs.OpenSubtitles.Outputs;

namespace VidereSubs.OpenSubtitles
{
    /// <summary>
    /// Client to contact the XML RPC server at opensubtitles.org.
    /// </summary>
    public class Client
    {
        private readonly IClient clientProxy;
        private LogInOutput login;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Client( )
        {
            clientProxy = XmlRpcProxyGen.Create<IClient>( );
        }

        /// <summary>
        /// Logs in to the opensubtitles.org website.
        /// </summary>
        /// <param name="username">The client's username.</param>
        /// <param name="password">The client's password.</param>
        /// <returns>The result of the login.</returns>
        public LogInOutput LogIn( string username, string password )
        {
            MD5 alg = MD5.Create( );
            byte[ ] pwBytes = System.Text.Encoding.ASCII.GetBytes( password );
            byte[ ] hash = alg.ComputeHash( pwBytes );

            string hashedPassword = Hasher.ToHexadecimal( hash );

            XmlRpcStruct ret = clientProxy.LogIn( username, hashedPassword, "eng", "OSTestUserAgent" );

            this.login = new LogInOutput( ret )
            {
                Token = ( string ) ret[ "token" ]
            };

            return login;
        }

        /// <summary>
        /// Logs out the client.
        /// </summary>
        /// <returns>The output from the server.</returns>
        public LogOutOutput LogOut( )
        {
            if ( !login.LogInSuccesful )
                return null;

            XmlRpcStruct ret = clientProxy.LogOut( login.Token );

            LogOutOutput output = new LogOutOutput( ret );

            return output;
        }
    }
}