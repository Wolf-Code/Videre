using System;
using System.Text;
using CookComputing.XmlRpc;

namespace VidereSubs.OpenSubtitles.Data
{
    /// <summary>
    /// Contains information about a subtitle file.
    /// </summary>
    public class SubtitleData
    {
        /// <summary>
        /// The ID of the subtitle file on opensubtitles.org.
        /// </summary>
        public ulong IDSubtitleFile { private set; get; }

        /// <summary>
        /// The subtitle's file name.
        /// </summary>
        public string SubFileName { private set; get; }

        /// <summary>
        /// The size of the file in bytes.
        /// </summary>
        public uint SubFileSize { private set; get; }

        /// <summary>
        /// The subtitle's hash.
        /// </summary>
        public string SubFileHash { private set; get; }

        /// <summary>
        /// The ISO-639 identifier of the language.
        /// </summary>
        public string ISO639 { private set; get; }

        /// <summary>
        /// The name of the language.
        /// </summary>
        public string LanguageName { private set; get; }

        /// <summary>
        /// The date on which the subtitles have been added.
        /// </summary>
        public DateTime AddDate { private set; get; }

        /// <summary>
        /// The amount of times the subtitles have been downloaded.
        /// </summary>
        public uint DownloadsCount { private set; get; }

        /// <summary>
        /// The download link for the subtitles file as a .gz.
        /// </summary>
        public string SubDownloadLink { private set; get; }

        /// <summary>
        /// The encoding of the subtitle file.
        /// </summary>
        public Encoding SubEncoding { private set; get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="output">The output from the request.</param>
        public SubtitleData( XmlRpcStruct output )
        {
            SubFileName = output.GetString( "SubFileName" );
            AddDate = DateTime.Parse( output.GetString( "SubAddDate" ) );
            IDSubtitleFile = output.GetULong( "IDSubtitleFile" );
            DownloadsCount = output.GetUInt( "SubDownloadsCnt" );
            SubDownloadLink = output.GetString( "SubDownloadLink" );
            SubFileSize = output.GetUInt( "SubSize" );
            LanguageName = output.GetString( "LanguageName" );
            SubFileHash = output.GetString( "SubHash" );
            ISO639 = output.GetString( "ISO639" );

            SubEncoding = Encoding.GetEncoding( int.Parse( output.GetString( "SubEncoding" ).Substring( 2 ) ) );
        }
    }
}
