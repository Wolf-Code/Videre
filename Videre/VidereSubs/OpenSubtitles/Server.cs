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

            ServerInfoOutput info = new ServerInfoOutput( ret )
            {
                Version = ( string ) ret[ "xmlrpc_version" ],
                RequestURL = ( string ) ret[ "xmlrpc_url" ],
                Application = ( string ) ret[ "application" ],
                Contact = ( string ) ret[ "contact" ],
                WebsiteURL = ( string ) ret[ "website_url" ],
                UsersOnlineTotal = ( int ) ret[ "users_online_total" ],
                UsersOnlineProgram = ( int ) ret[ "users_online_program" ],
                UsersLoggedIn = ( int ) ret[ "users_loggedin" ],
                UsersOnlineMaxAllTime = uint.Parse( ( string ) ret[ "users_max_alltime" ] ),
                UsersRegistered = uint.Parse( ( string ) ret[ "users_registered" ] ),
                SubsDownloads = ulong.Parse( ( string ) ret[ "subs_downloads" ] ),
                SubtitleFiles = uint.Parse( ( string ) ret[ "subs_subtitle_files" ] ),
                MoviesTotal = uint.Parse( ( string ) ret[ "movies_total" ] ),
                MoviesAKA = uint.Parse( ( string ) ret[ "movies_aka" ] ),
                TotalSubtitleLanguages = uint.Parse( ( string ) ret[ "total_subtitles_languages" ] ),
                LastUpdateStrings = ( XmlRpcStruct ) ret[ "last_update_strings" ],
                DownloadLimits = ( XmlRpcStruct ) ret[ "download_limits" ],
            };

            return info;
        }
    }
}
