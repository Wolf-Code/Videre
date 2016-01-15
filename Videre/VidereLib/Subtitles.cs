using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace VidereLib
{
    /// <summary>
    /// Contains all <see cref="SubtitleData"/> for a given .srt file.
    /// </summary>
    public class Subtitles
    {
        private readonly Dictionary<TimeSpan, SubtitleData> SubtitleDatas = new Dictionary<TimeSpan, SubtitleData>( );
        private readonly List<TimeSpan> Keys;

        /// <summary>
        /// The amount of subtitles loaded.
        /// </summary>
        public int Count => Keys.Count;

        /// <summary>
        /// True if the subtitles were parsed succesfully, false otherwise.
        /// </summary>
        public bool SubtitlesParsedSuccesfully { private set; get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="FilePath">The path to the .srt file.</param>
        public Subtitles( string FilePath )
        {
            try
            {
                ParseFile( FilePath );
                SubtitlesParsedSuccesfully = true;
            }
            catch
            {
                SubtitlesParsedSuccesfully = false;
            }
            Keys = new List<TimeSpan>( SubtitleDatas.Keys );
        }

        private void ParseFile( string FilePath )
        {
            if ( !File.Exists( FilePath ) )
                return;

            CultureInfo france = new CultureInfo( "fr-Fr" );
            string[ ] Data;
            using ( FileStream FS = File.OpenRead( FilePath ) )
                using ( TextReader Reader = new StreamReader( FS ) )
                    Data = Reader.ReadToEnd( ).Split( '\n' );

            int X = 0;
            while ( X < Data.Length - 1 )
            {
                int ID = int.Parse( Data[ X++ ] );

                string[ ] SplitTime = Data[ X++ ].Split( new[ ] { "-->" }, StringSplitOptions.RemoveEmptyEntries );
                TimeSpan Start = TimeSpan.Parse( SplitTime[ 0 ].Trim( ), france );
                TimeSpan End = TimeSpan.Parse( SplitTime[ 1 ].Trim( ), france );

                List<string> subs = new List<string>( );
                int parsed;
                while ( Data[ X ].Length > 0 && X < Data.Length - 1 && !int.TryParse( Data[ X ], out parsed ) && parsed != ID + 1 )
                {
                    subs.Add( Data[ X ] );
                    X++;
                }

                SubtitleDatas.Add( Start, new SubtitleData( ID, Start, End, subs ) );
            }
        }

        /// <summary>
        /// Checks if there are subtitles left for a given time.
        /// </summary>
        /// <param name="CurrentTime">The time to check for.</param>
        /// <returns>True if there are subtitles after this time, false otherwise.</returns>
        public bool AnySubtitlesLeft( TimeSpan CurrentTime )
        {
            int Index = Keys.BinarySearch( CurrentTime );
            if ( Index < 0 )
                Index = ~Index;

            return Index < Keys.Count - 1;
        }

        /// <summary>
        /// Gets the subtitle data by index.
        /// </summary>
        /// <param name="Index">The index of the subtitle data.</param>
        /// <returns>The subtitle data.</returns>
        public SubtitleData GetData( int Index )
        {
            if ( Index < 1 )
                Index = 1;

            return SubtitleDatas[ Keys[ Index - 1 ] ];
        }

        /// <summary>
        /// Gets the subtitle data for the current time.
        /// </summary>
        /// <param name="CurrentTime">The current time.</param>
        /// <returns>The subtitle data.</returns>
        public SubtitleData GetData( TimeSpan CurrentTime )
        {
            int Index = Keys.BinarySearch( CurrentTime );
            if ( Index < 0 )
                Index = ~Index;

            return SubtitleDatas[ Keys[ Index ] ];
        }
    }
}
