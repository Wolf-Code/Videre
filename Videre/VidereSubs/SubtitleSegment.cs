using System;
using System.Collections.Generic;

namespace VidereSubs
{
    /// <summary>
    /// Contains data about a subtitle segment.
    /// </summary>
    public class SubtitleSegment
    {
        /// <summary>
        /// The subtitle data for when there is no subtitle data.
        /// </summary>
        public static SubtitleSegment Empty { private set; get; } = new SubtitleSegment( 0, TimeSpan.Zero, TimeSpan.Zero, new List<string>( ) );

        /// <summary>
        /// The subtitle's index.
        /// </summary>
        public int Index { private set; get; }

        /// <summary>
        /// The time at which the subtitle starts being shown.
        /// </summary>
        public TimeSpan From { private set; get; }

        /// <summary>
        /// The time at which the subtitles are done being shown.
        /// </summary>
        public TimeSpan To { private set; get; }

        /// <summary>
        /// The lines in the subtitle.
        /// </summary>
        public List<string> Lines { private set; get; } 

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">The subtitle's ID.</param>
        /// <param name="start">The start of the subtitle.</param>
        /// <param name="end">The end of the subtitle.</param>
        /// <param name="lines">The lines in the subtitle.</param>
        public SubtitleSegment( int id, TimeSpan start, TimeSpan end, List<string> lines  )
        {
            Index = id;
            From = start;
            To = end;
            Lines = lines;
        }
    }
}
