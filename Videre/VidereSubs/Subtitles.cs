using System;
using System.Collections.Generic;

namespace VidereSubs
{
    /// <summary>
    /// Contains all <see cref="SubtitleSegment"/> for a given .srt file.
    /// </summary>
    public abstract class Subtitles
    {
        private Dictionary<TimeSpan, SubtitleSegment> SubtitleDatas = new Dictionary<TimeSpan, SubtitleSegment>( );
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
        protected Subtitles( string FilePath )
        {
            this.LoadSubtitles( FilePath );
            Keys = new List<TimeSpan>( SubtitleDatas.Keys );
        }

        private void LoadSubtitles( string filePath )
        {
            try
            {
                this.SubtitleDatas = ParseFile( filePath );
                SubtitlesParsedSuccesfully = this.SubtitleDatas != null;
            }
            catch
            {
                SubtitlesParsedSuccesfully = false;
            }
        }

        /// <summary>
        /// Parses the file and returns a dictionary containing the timespan at which it starts as the key, and the actual subtitle data as the value.
        /// </summary>
        /// <param name="FilePath">The path to the subtitle file.</param>
        /// <returns>The subtitle data.</returns>
        protected abstract Dictionary<TimeSpan, SubtitleSegment> ParseFile( string FilePath );

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
        public SubtitleSegment GetData( int Index )
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
        public SubtitleSegment GetData( TimeSpan CurrentTime )
        {
            int Index = Keys.BinarySearch( CurrentTime );
            if ( Index < 0 )
                Index = ~Index;

            return SubtitleDatas[ Keys[ Index ] ];
        }
    }
}
