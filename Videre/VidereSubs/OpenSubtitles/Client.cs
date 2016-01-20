using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Timers;
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
        private readonly Timer keepAliveTimer;


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
            keepAliveTimer = new Timer( TimeSpan.FromMinutes( 10 ).TotalMilliseconds );
            keepAliveTimer.Elapsed += KeepAliveTimerOnElapsed;
        }

        private void KeepAliveTimerOnElapsed( object Sender, ElapsedEventArgs Args )
        {
            clientProxy.NoOperation( this.login.Token );
        }

        private void ResetTimer( )
        {
            keepAliveTimer.Stop( );
            keepAliveTimer.Start( );
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
            }
            XmlRpcStruct ret = clientProxy.LogIn( username, password, "eng", this.UserAgent );

            this.login = new LogInOutput( ret );
            keepAliveTimer.Start( );

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
            keepAliveTimer.Stop( );

            return output;
        }

        /// <summary>
        /// Retrieves information about the movie hash.
        /// </summary>
        /// <param name="movieHashes">The hashes to check the information for.</param>
        public CheckMovieHashOutput CheckMovieHash2( params string[ ] movieHashes )
        {
            XmlRpcStruct ret = clientProxy.CheckMovieHash2( login.Token, movieHashes );

            CheckMovieHashOutput output = new CheckMovieHashOutput( ret );
            ResetTimer( );

            return output;
        }

        /// <summary>
        /// Searches for subtitle information.
        /// </summary>
        /// <param name="languages"></param>
        /// <param name="movieFiles"></param>
        /// <returns></returns>
        public SubtitleData[ ] SearchSubtitles( string[ ] languages, params FileInfo[ ] movieFiles )
        {
            string requestLanguages = string.Join( ",", languages );
            XmlRpcStruct[ ] requests = new XmlRpcStruct[ movieFiles.Length ];
            for ( int Index = 0; Index < movieFiles.Length; Index++ )
            {
                FileInfo info = movieFiles[ Index ];
                string hash = Hasher.ComputeMovieHash( info.FullName );
                requests[ Index ] = new XmlRpcStruct { { "moviehash", hash }, { "moviesize", info.Length }, { "sublanguageid", requestLanguages } };
            }

            XmlRpcStruct parameters = new XmlRpcStruct { };

            XmlRpcStruct ret = clientProxy.SearchSubtitles( login.Token, requests, parameters );
            XmlRpcStruct[ ] data = ret[ "data" ] as XmlRpcStruct[ ];
            if ( data == null )
                return new SubtitleData[ 0 ];

            ResetTimer( );

            return data.Select( sub => new SubtitleData( sub ) ).ToArray( );
        }

        /// <summary>
        /// Gets an array of allowed subtitle languages.
        /// </summary>
        /// <param name="language">Array of enabled subtitle languages.</param>
        /// <returns>An array of allowed subtitle languages.</returns>
        public SubtitleLanguage[ ] GetSubLanguages( string language = "en" )
        {
            XmlRpcStruct ret = clientProxy.GetSubLanguages( language );

            GetSubLanguagesOutput outp = new GetSubLanguagesOutput( ret );
            ResetTimer( );

            return outp.Languages;
        }

        [Conditional( "DEBUG" )]
        private void DebugStruct( XmlRpcStruct output, int tabs = 0 )
        {
            foreach ( var key in output.Keys )
            {
                if ( output[ key ] is XmlRpcStruct )
                {
                    Console.WriteLine( key + ": " );
                    DebugStruct( ( XmlRpcStruct ) output[ key ], tabs + 1 );
                }
                else if ( output[ key ] is XmlRpcStruct[ ] )
                {
                    Console.WriteLine( key + ": " );
                    foreach ( var s in ( XmlRpcStruct[ ] ) output[ key ] )
                    {
                        DebugStruct( s, tabs + 1 );
                        Console.WriteLine( );
                    }
                }
                else
                    Console.WriteLine( $"{new string( '\t', tabs )}{key}: {output[ key ]}" );
            }
        }
    }
}