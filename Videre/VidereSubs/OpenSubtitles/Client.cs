using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography;
using CookComputing.XmlRpc;
using VidereSubs.OpenSubtitles.Data;
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
        /// The user agent to use on opensubtitles.org.
        /// </summary>
        public string UserAgent { private set; get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Client( string userAgent )
        {
            clientProxy = XmlRpcProxyGen.Create<IClient>( );
            this.UserAgent = userAgent;
        }

        /// <summary>
        /// Logs in to the opensubtitles.org website.
        /// </summary>
        /// <param name="username">The client's username.</param>
        /// <param name="password">The client's password.</param>
        /// <param name="encrypt">Whether or not to encrypt the password before sending. If the password is provided as plain-text, set this to true. If the password has already been encrypted with MD5, set it to false.</param>
        /// <returns>The result of the login.</returns>
        public LogInOutput LogIn( string username, string password, bool encrypt = true )
        {
            if ( encrypt )
            {
                MD5 alg = MD5.Create( );
                byte[ ] pwBytes = System.Text.Encoding.ASCII.GetBytes( password );
                byte[ ] hash = alg.ComputeHash( pwBytes );

                password = Hasher.ToHexadecimal( hash );

                Console.WriteLine( password );
            }
            XmlRpcStruct ret = clientProxy.LogIn( username, password, "eng", this.UserAgent );

            this.login = new LogInOutput( ret );

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

        /// <summary>
        /// Retrieves information about the movie hash.
        /// </summary>
        /// <param name="movieHashes">The hashes to check the information for.</param>
        public void CheckMovieHash2( params string[ ] movieHashes )
        {
            XmlRpcStruct ret = clientProxy.CheckMovieHash2( login.Token, movieHashes );

            CheckMovieHashOutput output = new CheckMovieHashOutput( ret );
            foreach( KeyValuePair<string, MovieData[]> pair in output.MovieData)
                foreach ( MovieData data in pair.Value )
                    Console.WriteLine( data.MovieName );
        }

        private void DebugStruct( XmlRpcStruct output, int tabs = 0 )
        {
            foreach ( var key in output.Keys )
            {
                if ( output[ key ] is XmlRpcStruct )
                    DebugStruct( ( XmlRpcStruct ) output[ key ], tabs + 1 );
                else if ( output[ key ] is XmlRpcStruct[ ] )
                    foreach ( var s in ( XmlRpcStruct[ ] ) output[ key ] )
                        DebugStruct( s, tabs + 1 );
                else
                    Console.WriteLine( $"{new String( '\t', tabs )}{key}: {output[ key ]}" );
            }
        }
    }
}