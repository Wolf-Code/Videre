using System;
using System.IO;
using System.Text;

namespace VidereSubs.OpenSubtitles
{
    /// <summary>
    /// Calculates the has for movie files.
    /// </summary>
    public static class Hasher
    {
        /// <summary>
        /// Computes the hash for a given movie file.
        /// </summary>
        /// <param name="filename">The path to the file.</param>
        /// <returns>A byte array containing the movie's hash.</returns>
        public static byte[ ] ComputeMovieHash( string filename )
        {
            byte[ ] result;
            using ( Stream input = File.OpenRead( filename ) )
                result = ComputeMovieHash( input );

            return result;
        }

        private static byte[ ] ComputeMovieHash( Stream input )
        {
            long lhash, streamsize;
            streamsize = input.Length;
            lhash = streamsize;

            long i = 0;
            byte[ ] buffer = new byte[ sizeof ( long ) ];
            while ( i < 65536 / sizeof ( long ) && ( input.Read( buffer, 0, sizeof ( long ) ) > 0 ) )
            {
                i++;
                lhash += BitConverter.ToInt64( buffer, 0 );
            }

            input.Position = Math.Max( 0, streamsize - 65536 );
            i = 0;
            while ( i < 65536 / sizeof ( long ) && ( input.Read( buffer, 0, sizeof ( long ) ) > 0 ) )
            {
                i++;
                lhash += BitConverter.ToInt64( buffer, 0 );
            }
            input.Close( );
            byte[ ] result = BitConverter.GetBytes( lhash );
            Array.Reverse( result );
            return result;
        }

        /// <summary>
        /// Turns the byte array containing the hash into a string containing the hexadecimal representation of the hash.
        /// </summary>
        /// <param name="bytes">The bytes containing the movie's hash.</param>
        /// <returns>The hexadecimal representation of the hash.</returns>
        public static string ToHexadecimal( byte[ ] bytes )
        {
            StringBuilder hexBuilder = new StringBuilder( );
            for ( int i = 0; i < bytes.Length; i++ )
                hexBuilder.Append( bytes[ i ].ToString( "x2" ) );

            return hexBuilder.ToString( );
        }
    }
}