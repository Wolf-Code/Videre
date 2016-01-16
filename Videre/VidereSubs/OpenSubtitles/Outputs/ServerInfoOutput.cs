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
        public string Version { internal set; get; }

        /// <summary>
        /// XML-RPC interface URL.
        /// </summary>
        public string RequestURL { internal set; get; }

        /// <summary>
        /// Server's application name and version.
        /// </summary>
        public string Application { internal set; get; }

        /// <summary>
        /// Contact e-mail address for server related quuestions and problems.
        /// </summary>
        public string Contact { internal set; get; }

        /// <summary>
        /// Main server URL.
        /// </summary>
        public string WebsiteURL { internal set; get; }

        /// <summary>
        /// Number of users currently online.
        /// </summary>
        public int UsersOnlineTotal { internal set; get; }

        /// <summary>
        /// Number of users currently online using a client application (XML-RPC API).
        /// </summary>
        public int UsersOnlineProgram { internal set; get; }

        /// <summary>
        /// Number of currently logged-in users.
        /// </summary>
        public int UsersLoggedIn { internal set; get; }

        /// <summary>
        /// Maximum number of users throughout the history.
        /// </summary>
        public uint UsersOnlineMaxAllTime { internal set; get; }

        /// <summary>
        /// Number of registered users.
        /// </summary>
        public uint UsersRegistered { internal set; get; }

        /// <summary>
        /// Total number of subtitle downloads.
        /// </summary>
        public ulong SubsDownloads { internal set; get; }

        /// <summary>
        /// Total number of subtitle files stored on the server.
        /// </summary>
        public uint SubtitleFiles { internal set; get; }

        /// <summary>
        /// Total number of movies in the database.
        /// </summary>
        public uint MoviesTotal { internal set; get; }

        /// <summary>
        /// Total number of movie A.K.A. titles in the database.
        /// </summary>
        public uint MoviesAKA { internal set; get; }

        /// <summary>
        /// Total number of subtitle languages supported.
        /// </summary>
        public uint TotalSubtitleLanguages { internal set; get; }

        /// <summary>
        /// Structure containing information about last updates of translations.
        /// </summary>
        public XmlRpcStruct LastUpdateStrings { internal set; get; }

        /// <summary>
        /// The download limits.
        /// </summary>
        public XmlRpcStruct DownloadLimits { internal set; get; }

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
        }
    }
}


