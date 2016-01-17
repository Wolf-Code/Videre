using System.Collections;
using System.Collections.Generic;
using CookComputing.XmlRpc;
using VidereSubs.OpenSubtitles.Data;

namespace VidereSubs.OpenSubtitles.Outputs
{
    /// <summary>
    /// The result for the check movie hash(2) method.
    /// </summary>
    public class CheckMovieHashOutput : Output
    {
        /// <summary>
        /// The returned movie data in a dictionary where the hash is the key.
        /// </summary>
        public Dictionary<string, MovieData[ ]> MovieData { get; } = new Dictionary<string, MovieData[ ]>( );

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="output">The check movie hash(2) result.</param>
        public CheckMovieHashOutput( XmlRpcStruct output ) : base( output )
        {
            XmlRpcStruct data = ( XmlRpcStruct ) output[ "data" ];
            foreach ( DictionaryEntry hash in data )
            {
                XmlRpcStruct[ ] hashData = ( XmlRpcStruct[ ] ) hash.Value;
                MovieData[ ] movieDataArray = new MovieData[ hashData.Length ];

                int C = 0;
                foreach ( XmlRpcStruct moviedata in hashData )
                {
                    MovieData mData = new MovieData( moviedata );
                    movieDataArray[ C++ ] = mData;
                }

                MovieData.Add( ( string ) hash.Key, movieDataArray );
            }
        }
    }
}
