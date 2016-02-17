using CookComputing.XmlRpc;

namespace VidereSubs.OpenSubtitles.Data
{
    /// <summary>
    /// Contains information about a subtitle language.
    /// </summary>
    public class SubtitleLanguage
    {
        /// <summary>
        /// The ISO-639 3 characters code of the language.
        /// </summary>
        public string ISO639_3 { get; }

        /// <summary>
        /// The name of the language.
        /// </summary>
        public string LanguageName { get; }

        /// <summary>
        /// The ISO-639 2 characters code of the language.
        /// </summary>
        public string ISO639_2 { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="output">The server's output.</param>
        public SubtitleLanguage( XmlRpcStruct output )
        {
            ISO639_3 = output.GetString( "SubLanguageID" );
            LanguageName = output.GetString( "LanguageName" );
            ISO639_2 = output.GetString( "ISO639" );
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString( )
        {
            return $@"ISO639_3: {ISO639_3}, 
LanguageName: {LanguageName}, 
ISO639_2: {ISO639_2}";
        }
    }
}
