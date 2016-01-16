using CookComputing.XmlRpc;

/*  
  (string) [xmlrpc_version],
  (string) [xmlrpc_url],
  (string) [application],
  (string) [contact],
  (string) [website_url],
  (string) [users_online_total],
  (string) [users_online_program],
  (string) [users_loggedin],
  (string) [users_max_alltime],
  (string) [users_registered],
  (string) [subs_downloads],
  (string) [subs_subtitle_files],
  (string) [movies_total],
  (string) [movies_aka],
  (string) [total_subtitles_languages],
  struct(
    (string) [<language ISO639 2-letter code>],
    ... more languages go here ...
  ) [last_update_strings],
  (double) [seconds]*/

namespace VidereSubs.OpenSubtitles.Outputs
{
    /// <summary>
    /// The return value of the ServerInfo method.
    /// </summary>
    public class ServerInfoOutput : Output
    {
        /// <summary>
        /// Version of server's XML-RPC API implementation.
        /// </summary>
        public string Version { private set; get; }

        /// <summary>
        /// XML-RPC interface URL.
        /// </summary>
        public string RequestURL { private set; get; }

        /// <summary>
        /// Server's application name and version.
        /// </summary>
        public string Application { private set; get; }

        /// <summary>
        /// Contact e-mail address for server related quuestions and problems.
        /// </summary>
        public string Contact { private set; get; }

        /// <summary>
        /// Main server URL.
        /// </summary>
        public string WebsiteURL { private set; get; }

        /// <summary>
        /// Number of users currently online.
        /// </summary>
        public int UsersOnlineTotal { private set; get; }

        /// <summary>
        /// Number of users currently online using a client application (XML-RPC API).
        /// </summary>
        public int UsersOnlineProgram { private set; get; }

        /// <summary>
        /// Number of currently logged-in users.
        /// </summary>
        public int UsersLoggedIn { private set; get; }

        /// <summary>
        /// Maximum number of users throughout the history.
        /// </summary>
        public uint UsersOnlineMaxAllTime { private set; get; }

        /// <summary>
        /// Number of registered users.
        /// </summary>
        public uint UsersRegistered { private set; get; }

        /// <summary>
        /// Total number of subtitle downloads.
        /// </summary>
        public ulong SubsDownloads { private set; get; }

        /// <summary>
        /// Total number of subtitle files stored on the server.
        /// </summary>
        public uint SubtitleFiles { private set; get; }

        /// <summary>
        /// Total number of movies in the database.
        /// </summary>
        public uint MoviesTotal { private set; get; }

        /// <summary>
        /// Total number of movie A.K.A. titles in the database.
        /// </summary>
        public uint MoviesAKA { private set; get; }

        /// <summary>
        /// Total number of subtitle languages supported.
        /// </summary>
        public uint TotalSubtitleLanguages { private set; get; }

        /// <summary>
        /// Structure containing information about last updates of translations.
        /// </summary>
        public XmlRpcStruct LastUpdateStrings { private set; get; }

        /// <summary>
        /// The download limits.
        /// </summary>
        public XmlRpcStruct DownloadLimits { private set; get; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString( )
        {
            return
                $@"Version: {Version}
Request URL: {RequestURL}
Application: {Application}
Contact: {Contact}
Website URL: {WebsiteURL}
Users Online Total: {UsersOnlineTotal}
Users Online Program: {UsersOnlineProgram}
Users Logged In: {UsersLoggedIn}
Users Online Max All-Time: {UsersOnlineMaxAllTime}
Users Registered: {UsersRegistered}
Subs Downloaded: {SubsDownloads}
Subtitle Files: {SubtitleFiles}
Movies Total: {MoviesTotal}
Movies A.K.A.: {MoviesAKA}
Total Subtitle Languages: {TotalSubtitleLanguages}
Seconds: {Seconds}";
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="output">The server request output.</param>
        public ServerInfoOutput( XmlRpcStruct output ) : base( output )
        {
            Version = ( string ) output[ "xmlrpc_version" ];
            RequestURL = ( string ) output[ "xmlrpc_url" ];
            Application = ( string ) output[ "application" ];
            Contact = ( string ) output[ "contact" ];
            WebsiteURL = ( string ) output[ "website_url" ];
            UsersOnlineTotal = ( int ) output[ "users_online_total" ];
            UsersOnlineProgram = ( int ) output[ "users_online_program" ];
            UsersLoggedIn = ( int ) output[ "users_loggedin" ];
            UsersOnlineMaxAllTime = uint.Parse( ( string ) output[ "users_max_alltime" ] );
            UsersRegistered = uint.Parse( ( string ) output[ "users_registered" ] );
            SubsDownloads = ulong.Parse( ( string ) output[ "subs_downloads" ] );
            SubtitleFiles = uint.Parse( ( string ) output[ "subs_subtitle_files" ] );
            MoviesTotal = uint.Parse( ( string ) output[ "movies_total" ] );
            MoviesAKA = uint.Parse( ( string ) output[ "movies_aka" ] );
            TotalSubtitleLanguages = uint.Parse( ( string ) output[ "total_subtitles_languages" ] );
            LastUpdateStrings = ( XmlRpcStruct ) output[ "last_update_strings" ];
            DownloadLimits = ( XmlRpcStruct ) output[ "download_limits" ];
        }
    }
}