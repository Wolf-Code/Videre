using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VidereLib.Data;
using VidereLib.Data.MediaData;

namespace Videre
{
    /// <summary>
    /// Handles loading and saving of media information as a cache.
    /// </summary>
    public static class MediaInformationManager
    {
        private const string MediaDataFile = "MediaInfo.json";

        private static readonly Dictionary<string, VidereMovieInformation> movieInformation = new Dictionary<string, VidereMovieInformation>( );
        private static readonly Dictionary<string, VidereEpisodeInformation> episodeInformation = new Dictionary<string, VidereEpisodeInformation>( );

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
                        VidereMovieInformation info = JsonConvert.DeserializeObject<VidereMovieInformation>( movie.ToString(  ) );

                        movieInformation.Add( info.Hash, info );
                    }

                    foreach ( dynamic episode in mediaInfo.Episodes )
                    {
                        VidereEpisodeInformation info = JsonConvert.DeserializeObject<VidereEpisodeInformation>( episode.ToString(  ) );

                        episodeInformation.Add( info.Hash, info );
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
                    {
                        jsonWriter.WriteStartArray( );

                        foreach ( var pair in movieInformation )
                            jsonWriter.WriteRawValue( JsonConvert.SerializeObject( pair.Value ) );

                        jsonWriter.WriteEndArray( );
                    }

                    jsonWriter.WritePropertyName( "Episodes" );
                    {
                        jsonWriter.WriteStartArray( );

                        foreach ( var pair in episodeInformation )
                            jsonWriter.WriteRawValue( JsonConvert.SerializeObject( pair.Value ) );

                        jsonWriter.WriteEndArray( );
                    }
                    jsonWriter.WriteEndObject( );

                    jsonWriter.Close( );
                }
        }

        #region Set

        /// <summary>
        /// Sets the movie information in the <see cref="MediaInformationManager"/> to a new <see cref="VidereMovieInformation"/>.
        /// </summary>
        /// <param name="info">The <see cref="VidereMovieInformation"/> to set.</param>
        public static void SetMovieInformation( VidereMovieInformation info )
        {
            movieInformation[ info.Hash ] = info;
        }

        /// <summary>
        /// Sets the episode information in the <see cref="MediaInformationManager"/> to a new <see cref="VidereEpisodeInformation"/>.
        /// </summary>
        /// <param name="info">The <see cref="VidereEpisodeInformation"/> to set.</param>
        public static void SetEpisodeInformation( VidereEpisodeInformation info )
        {
            episodeInformation[ info.Hash ] = info;
        }

        #endregion

        #region GetOrSave

        /// <summary>
        /// Either gets the currently saved <see cref="VidereMovieInformation"/> if it exists, otherwise it will first save <paramref name="info"/> and then return it as well. Use this to make changes to existing entries, or save the entry if it's not there yet.
        /// </summary>
        /// <param name="info">The movie information we want to get, by hash.</param>
        /// <returns>The either retrieved or newly saved <see cref="VidereMovieInformation"/>.</returns>
        public static VidereMovieInformation GetOrSaveMovieInformation( VidereMovieInformation info )
        {
            VidereMovieInformation loaded = GetMovieInformationByHash( info.Hash );
            if ( loaded != null ) return loaded;

            SetMovieInformation( info );
            return info;
        }

        /// <summary>
        /// Either gets the currently saved <see cref="VidereEpisodeInformation"/> if it exists, otherwise it will first save <paramref name="info"/> and then return it as well. Use this to make changes to existing entries, or save the entry if it's not there yet.
        /// </summary>
        /// <param name="info">The episode information we want to get, by hash.</param>
        /// <returns>The either retrieved or newly saved <see cref="VidereEpisodeInformation"/>.</returns>
        public static VidereEpisodeInformation GetOrSaveEpisodeInformation( VidereEpisodeInformation info )
        {
            VidereEpisodeInformation loaded = GetEpisodeInformationByHash( info.Hash );
            if ( loaded != null ) return loaded;

            SetEpisodeInformation( info );
            return info;
        }

        #endregion

        #region GetByHash

        /// <summary>
        /// Gets a <see cref="VidereMovieInformation"/> by hash.
        /// </summary>
        /// <param name="hash">The hash to look for.</param>
        /// <returns>The <see cref="VidereMovieInformation"/> for this hash.</returns>
        public static VidereMovieInformation GetMovieInformationByHash( string hash )
        {
            return !movieInformation.ContainsKey( hash ) ? null : movieInformation[ hash ];
        }

        /// <summary>
        /// Gets a <see cref="VidereEpisodeInformation"/> by hash.
        /// </summary>
        /// <param name="hash">The hash to look for.</param>
        /// <returns>The <see cref="VidereEpisodeInformation"/> for this hash.</returns>
        public static VidereEpisodeInformation GetEpisodeInformationByHash( string hash )
        {
            return !episodeInformation.ContainsKey( hash ) ? null : episodeInformation[ hash ];
        }

        /// <summary>
        /// Gets a <see cref="VidereMediaInformation"/> by hash, checking for both movies and episodes.
        /// </summary>
        /// <param name="hash">The hash to look for.</param>
        /// <returns>The <see cref="VidereMediaInformation"/> for this hash.</returns>
        public static VidereMediaInformation GetMediaInformationByHash( string hash )
        {
            VidereMovieInformation movieInfo;
            if ( ContainsMovieInformationForHash( hash, out movieInfo ) )
                return movieInfo;

            VidereEpisodeInformation episodeInfo;
            return ContainsEpisodeInformationForHash( hash, out episodeInfo ) ? episodeInfo : null;
        }
        #endregion

        #region ContainsForHash

        /// <summary>
        /// Checks if there is an entry for <paramref name="hash"/>.
        /// </summary>
        /// <param name="hash">The hash to check for.</param>
        /// <param name="info">The <see cref="VidereMovieInformation"/> if there is an entry, null otherwise.</param>
        /// <returns>True if there is an entry, false otherwise.</returns>
        public static bool ContainsMovieInformationForHash( string hash, out VidereMovieInformation info )
        {
            info = GetMovieInformationByHash( hash );

            return info != null;
        }

        /// <summary>
        /// Checks if there is an entry for <paramref name="hash"/>.
        /// </summary>
        /// <param name="hash">The hash to check for.</param>
        /// <param name="info">The <see cref="VidereEpisodeInformation"/> if there is an entry, null otherwise.</param>
        /// <returns>True if there is an entry, false otherwise.</returns>
        public static bool ContainsEpisodeInformationForHash( string hash, out VidereEpisodeInformation info )
        {
            info = GetEpisodeInformationByHash( hash );

            return info != null;
        }

        #endregion
    }
}