using CookComputing.XmlRpc;
using VidereSubs.OpenSubtitles.Data;

namespace VidereSubs.OpenSubtitles.Outputs
{
    /// <summary>
    /// The output from the GetSubLanguages method.
    /// </summary>
    public class GetSubLanguagesOutput : Output
    {
        /// <summary>
        /// The allowed languages.
        /// </summary>
        public SubtitleLanguage[ ] Languages { private set; get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="output">The output from the server.</param>
        public GetSubLanguagesOutput( XmlRpcStruct output ) : base( output )
        {
            XmlRpcStruct[ ] data = output.GetXmlRpcStructArray( "data" );
            Languages = new SubtitleLanguage[ data.Length ];

            for ( int Index = 0; Index < data.Length; Index++ )
                Languages[ Index ] = new SubtitleLanguage( data[ Index ] );
        }
    }
}
