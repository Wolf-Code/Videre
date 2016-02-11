using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VidereLib.Data;
using VidereSubs.OpenSubtitles;

namespace Videre
{
    public static class MediaInformationManager
    {
        private const string MediaDataFile = "MediaInfo.json";

        private static readonly Dictionary<string, MovieInformation> movieInformation = new Dictionary<string, MovieInformation>( );

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
                            Rating = movie.Rating
                        };

                        movieInformation.Add( ( string ) movie.Hash, info );
                    }
                }
        }

        public static void SaveMediaData( )
        {
            using ( FileStream FS = File.OpenWrite( MediaDataFile ) )
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

                        jsonWriter.WritePropertyName( "Hash" );
                        jsonWriter.WriteValue( hash );

                        jsonWriter.WriteEndObject( );
                    }

                    jsonWriter.WriteEndArray( );
                    jsonWriter.WriteEndObject( );

                    jsonWriter.Close( );
                }
        }

        public static void SetMovieInformation( MovieInformation info )
        {
            movieInformation[ info.Hash ] = info;
        }

        public static MovieInformation GetOrSaveMovieInformation( MovieInformation info )
        {
            MovieInformation loaded = GetMovieInformationByHash( info.Hash );
            if ( loaded != null ) return loaded;

            SetMovieInformation( info );
            return info;
        }

        public static MovieInformation GetMovieInformationByHash( string hash )
        {
            return !movieInformation.ContainsKey( hash ) ? null : movieInformation[ hash ];
        }

        public static bool ContainsMovieInformationForHash( string hash, out MovieInformation info )
        {
            info = GetMovieInformationByHash( hash );

            return info != null;
        }
    }
}