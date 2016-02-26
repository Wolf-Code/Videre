using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Timers;
using CookComputing.XmlRpc;
using VidereSubs.Exceptions;
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
        /// The amount of times we will attempt to retry the service if it is unavailable.
        /// </summary>
        public const int MaxServiceUnavailableRetries = 3;

        /// <summary>
        /// The user agent to use on opensubtitles.org.
        /// </summary>
        public string UserAgent { get; }

        /// <summary>
        /// Whether or not the client is logged in.
        /// </summary>
        public bool IsLoggedIn => login != null && login.LogInSuccesful;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Client( string userAgent )
        {
            clientProxy = XmlRpcProxyGen.Create<IClient>( );
            UserAgent = userAgent;
            keepAliveTimer = new Timer( TimeSpan.FromMinutes( 10 ).TotalMilliseconds );
            keepAliveTimer.Elapsed += KeepAliveTimerOnElapsed;
        }

        private void KeepAliveTimerOnElapsed( object Sender, ElapsedEventArgs Args )
        {
            clientProxy.NoOperation( login.Token );
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
            XmlRpcStruct ret = PerformRequest( ( ) => clientProxy.LogIn( username, password, "eng", UserAgent ) );

            login = new LogInOutput( ret );
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

            XmlRpcStruct ret = PerformRequest( ( ) => clientProxy.LogOut( login.Token ) );

            LogOutOutput output = new LogOutOutput( ret );
            keepAliveTimer.Stop( );

            return output;
        }

        /// <summary>
        /// Retrieves information about the movie hash.
        /// </summary>
        /// <param name="movieHashes">The hashes to check the information for.</param>
        public CheckMovieHashOutput CheckMovieHashAllGuesses( params string[ ] movieHashes )
        {
            XmlRpcStruct ret = PerformRequest( ( ) => clientProxy.CheckMovieHash2( login.Token, movieHashes ) );

            CheckMovieHashOutput output = new CheckMovieHashOutput( ret );
            ResetTimer( );

            return output;
        }


        /// <summary>
        /// Retrieves information about the movie hash.
        /// </summary>
        /// <param name="movieHashes">The hashes to check the information for.</param>
        public CheckMovieHashOutput CheckMovieHashBestGuessOnly( params string[ ] movieHashes )
        {
            XmlRpcStruct ret = PerformRequest( ( ) => clientProxy.CheckMovieHash( login.Token, movieHashes ) );

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
                requests[ Index ] = new XmlRpcStruct { { "moviehash", hash }, { "moviebytesize", info.Length }, { "sublanguageid", requestLanguages } };
            }

            XmlRpcStruct parameters = new XmlRpcStruct( );

            XmlRpcStruct ret = PerformRequest( ( ) => clientProxy.SearchSubtitles( login.Token, requests, parameters ) );
            XmlRpcStruct[ ] data = ret[ "data" ] as XmlRpcStruct[ ];

            ResetTimer( );

            return data?.Select( sub => new SubtitleData( sub ) ).ToArray( ) ?? new SubtitleData[ 0 ];
        }

        /// <summary>
        /// Gets an array of allowed subtitle languages.
        /// </summary>
        /// <param name="language">Array of enabled subtitle languages.</param>
        /// <returns>An array of allowed subtitle languages.</returns>
        public SubtitleLanguage[ ] GetSubLanguages( string language = "en" )
        {
            XmlRpcStruct ret = PerformRequest( ( ) => clientProxy.GetSubLanguages( language ) );

            GetSubLanguagesOutput outp = new GetSubLanguagesOutput( ret );
            ResetTimer( );

            return outp.Languages;
        }

        private static XmlRpcStruct PerformRequest( Func<XmlRpcStruct> func )
        {
            for ( int x = 0; x < MaxServiceUnavailableRetries; x++ )
            {
                try
                {
                    return func( );
                }
                catch
                {
                    // ignored
                }
            }

            throw new OpenSubtitlesServiceUnavailable( );
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