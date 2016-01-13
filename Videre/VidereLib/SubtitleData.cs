using System;
using System.Collections.Generic;

namespace VidereLib
{
    public class SubtitleData
    {
        /// <summary>
        /// The subtitle data for when there is no subtitle data.
        /// </summary>
        public static SubtitleData Empty { private set; get; } = new SubtitleData( 0, TimeSpan.Zero, TimeSpan.Zero, new List<string>( ) );

        public int Index { private set; get; }
        public TimeSpan From { private set; get; }
        public TimeSpan To { private set; get; }
        public List<string> Lines { private set; get; } 

        public SubtitleData( int id, TimeSpan start, TimeSpan end, List<string> lines  )
        {
            this.Index = id;
            this.From = start;
            this.To = end;
            this.Lines = lines;
        }
    }
}
