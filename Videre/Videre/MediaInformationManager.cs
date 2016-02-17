using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VidereLib.Data;

namespace Videre
{
    /// <summary>
    /// Handles loading and saving of media information as a cache.
    /// </summary>
    public static class MediaInformationManager
    {
        private const string MediaDataFile = "MediaInfo.json";

        private static readonly Dictionary<string, MovieInformation> movieInformation = new Dictionary<string, MovieInformation>( );

        /// <summary>
        /// Loads all media information from file.
        /// </summary>
        public static async void LoadMediaData( )
        {
            if ( !File.Exists( MediaDataFile ) )
                return;

            using ( FileStream FS = File.OpenRead( MediaDataFile ) )
                using ( TextReader fileReader = new StreamReader( FS ) )
                {
                    string text = await fileReader.ReadToEndAsync( );
                    dynamic mediaInfo = JObject.Parse( text );
                    foreach ( dynamic movie in mediaInfo.Movies )
                    {
                        MovieInformation info = new MovieInformation
                        {
                            Name = movie.Name,
                            IMDBID = movie.IMDB,
                            Poster = movie.Poster,
                            Rating = movie.Rating,
                            Episode = movie.Episode ?? ( ushort ) 0,
                            Season = movie.Season ?? ( ushort ) 0
                        };

                        movieInformation.Add( ( string ) movie.Hash, info );
                    }
                }
        }

        /// <summary>
        /// Saves all entries to file.
        /// </summary>
        public static void SaveMediaData( )
        {
            using ( FileStream FS = File.Create( MediaDataFile ) )
                using ( StreamWriter writer = new StreamWriter( FS ) )
                {
                    JsonWriter jsonWriter = new JsonTextWriter( writer );
                    jsonWriter.Formatting = Formatting.Indented;
                    
                    jsonWriter.WriteStartObject( );
                    jsonWriter.WritePropertyName( "Movies" );
                    jsonWriter.WriteStartArray( );

                    foreach ( var pair in movieInformation )
                    {
                        string hash = pair.Key;
                        MovieInformation movie = pair.Value;

                        jsonWriter.WriteStartObject( );

                        jsonWriter.WritePropertyName( "Name" );
                        jsonWriter.WriteValue( movie.Name );

                        jsonWriter.WritePropertyName( "Year" );
                        jsonWriter.WriteValue( movie.Year );

                        jsonWriter.WritePropertyName( "Rating" );
                        jsonWriter.WriteValue( movie.Rating );

                        jsonWriter.WritePropertyName( "Poster" );
                        jsonWriter.WriteValue( movie.Poster );

                        jsonWriter.WritePropertyName( "IMDB" );
                        jsonWriter.WriteValue( movie.IMDBID );

                        if ( movie.Episode > 0 && movie.Season > 0 )
                        {
                            jsonWriter.WritePropertyName( "Episode" );
                            jsonWriter.WriteValue( movie.Episode );

                            jsonWriter.WritePropertyName( "Season" );
                            jsonWriter.WriteValue( movie.Season );
                        }

                        jsonWriter.WritePropertyName( "Hash" );
                        jsonWriter.WriteValue( hash );

                        jsonWriter.WriteEndObject( );
                    }

                    jsonWriter.WriteEndArray( );
                    jsonWriter.WriteEndObject( );

                    jsonWriter.Close( );
                }
        }

        /// <summary>
        /// Sets the movie information in the <see cref="MediaInformationManager"/> to a new <see cref="MovieInformation"/>.
        /// </summary>
        /// <param name="info">The <see cref="MovieInformation"/> to set.</param>
        public static void SetMovieInformation( MovieInformation info )
        {
            movieInformation[ info.Hash ] = info;
        }

        /// <summary>
        /// Either gets the currently saved <see cref="MovieInformation"/> if it exists, otherwise it will first save <paramref name="info"/> and then return it as well. Use this to make changes to existing entries, or save the entry if it's not there yet.
        /// </summary>
        /// <param name="info">The movie information we want to get, by hash.</param>
        /// <returns>The either retrieved or newly saved <see cref="MovieInformation"/>.</returns>
        public static MovieInformation GetOrSaveMovieInformation( MovieInformation info )
        {
            MovieInformation loaded = GetMovieInformationByHash( info.Hash );
            if ( loaded != null ) return loaded;

            SetMovieInformation( info );
            return info;
        }

        /// <summary>
        /// Gets a <see cref="MovieInformation"/> by hash.
        /// </summary>
        /// <param name="hash">The hash to look for.</param>
        /// <returns>The <see cref="MovieInformation"/> for this hash.</returns>
        public static MovieInformation GetMovieInformationByHash( string hash )
        {
            return !movieInformation.ContainsKey( hash ) ? null : movieInformation[ hash ];
        }

        /// <summary>
        /// Checks if there is an entry for <paramref name="hash"/>.
        /// </summary>
        /// <param name="hash">The hash to check for.</param>
        /// <param name="info">The <see cref="MovieInformation"/> if there is an entry, null otherwise.</param>
        /// <returns>True if there is an entry, false otherwise.</returns>
        public static bool ContainsMovieInformationForHash( string hash, out MovieInformation info )
        {
            info = GetMovieInformationByHash( hash );

            return info != null;
        }
    }
}