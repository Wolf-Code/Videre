using System;
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
        /// Constructor.
        /// </summary>
        /// <param name="output">The output from the request.</param>
        public SubtitleData( XmlRpcStruct output )
        {
            this.SubFileName = output.GetString( "SubFileName" );
            this.AddDate = DateTime.Parse( output.GetString( "SubAddDate" ) );
            this.IDSubtitleFile = output.GetULong( "IDSubtitleFile" );
            this.DownloadsCount = output.GetUInt( "SubDownloadsCnt" );
            this.SubDownloadLink = output.GetString( "SubDownloadLink" );
            this.SubFileSize = output.GetUInt( "SubSize" );
            this.LanguageName = output.GetString( "LanguageName" );
            this.SubFileHash = output.GetString( "SubHash" );
            this.ISO639 = output.GetString( "ISO639" );
        }
    }
}
