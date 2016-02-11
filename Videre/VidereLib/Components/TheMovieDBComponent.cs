using System;
using System.Collections.Generic;
using System.Net.TMDb;
using System.Threading;
using System.Threading.Tasks;
using VidereLib.Data;

namespace VidereLib.Components
{
    /// <summary>
    /// The <see cref="ComponentBase"/> for accessing information from themoviedb.org.
    /// </summary>
    public class TheMovieDBComponent : ComponentBase
    {
        private const string APIKey = "51f2d94f857dd380ee6ae5f52e3c782f";
        private readonly ServiceClient client;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TheMovieDBComponent( )
        {
            client = new ServiceClient( APIKey );
        }

        /// <summary>
        /// Finds movie posters for given media.
        /// </summary>
        /// <param name="medias">The media to find movie posters for.</param>
        /// <returns>A <see cref="Dictionary{TKey,TValue}"/> containing the IMDB ID as the key and the URL as the value.</returns>
        public async Task<Dictionary<string, string>> GetMoviePosters( params VidereMedia[ ] medias )
        {
            CancellationToken token = new CancellationToken( );
            dynamic config = await client.Settings.GetConfigurationAsync( token );
            string baseUrl = config.images.base_url;
            string size = "w500";

            Dictionary<string, string> results = new Dictionary<string, string>( );
            for ( int x = 0; x < medias.Length; x++ )
            {
                VidereMedia media = medias[ x ];
                if ( media.IMDBID == null )
                    continue;

                if ( results.ContainsKey( media.IMDBID ) )
                    continue;

                try
                {
                    Resource idRes = await client.FindAsync( "tt" + media.IMDBID, "imdb_id", token );
                    if ( idRes == null ) continue;

                    Movie res = await client.Movies.GetAsync( idRes.Id, "en", false, token );
                    if ( res != null )
                        results.Add( media.IMDBID, baseUrl + size + res.Poster );
                }
                catch ( ServiceRequestException e )
                {
                    switch ( e.StatusCode )
                    {
                        case 429:
                            return results;

                        default:
                            Console.WriteLine( $"Error during retrieval of posters. Status code {e.StatusCode}." );
                            return results;
                    }
                }
            }

            return results;
        }
    }
}
