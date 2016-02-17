using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using VidereSubs.Attributes;

namespace VidereSubs
{
    /// <summary>
    /// Contains all <see cref="SubtitleSegment"/> for a given .srt file.
    /// </summary>
    public abstract class Subtitles
    {
        private Dictionary<TimeSpan, SubtitleSegment> SubtitleDatas = new Dictionary<TimeSpan, SubtitleSegment>( );
        private readonly List<TimeSpan> Keys;

        private static readonly Dictionary<string, Type> SubtitleLoaders = new Dictionary<string, Type>( ); 

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
            LoadSubtitles( FilePath );
            Keys = new List<TimeSpan>( SubtitleDatas.Keys );
        }

        static Subtitles( )
        {
            Assembly assembly = Assembly.GetExecutingAssembly( );
            foreach ( Type t in assembly.GetTypes( ) )
            {
                foreach (  var info in t.GetCustomAttributes(true))
                {
                    SubtitleLoaderAttribute attribute = info as SubtitleLoaderAttribute;
                    if ( attribute == null )
                        continue;

                    foreach( string extension in attribute.Extensions)
                        if ( !SubtitleLoaders.ContainsKey( extension ) )
                            SubtitleLoaders.Add( extension, t );
                        else
                            throw new Exception( "Attempting to add two handlers for the same subtitle extension." );
                }
            }
        }

        private void LoadSubtitles( string filePath )
        {
            try
            {
                SubtitleDatas = ParseFile( filePath );
                SubtitlesParsedSuccesfully = SubtitleDatas != null;
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

        /// <summary>
        /// Loads the correct <see cref="Subtitles"/> based on the file extension.
        /// </summary>
        /// <param name="FilePath">The path to the subtitles file.</param>
        /// <returns>The <see cref="Subtitles"/> specific to this subtitle file extension.</returns>
        public static Subtitles LoadSubtitlesFile( string FilePath )
        {
            FileInfo file = new FileInfo( FilePath );
            string extension = file.Extension.Substring( 1 );

            if ( !SubtitleLoaders.ContainsKey( extension ) )
                return null;

            return ( Subtitles ) Activator.CreateInstance( SubtitleLoaders[ extension ], FilePath );
        }
    }
}
