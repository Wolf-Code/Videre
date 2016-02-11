using System.Net.TMDb;
using System.Threading;
using System.Threading.Tasks;
using VidereLib.NetworkingRequests;

namespace VidereLib.Components
{
    /// <summary>
    /// The <see cref="ComponentBase"/> for accessing information from themoviedb.org.
    /// </summary>
    public class TheMovieDBComponent : ComponentBase
    {
        /// <summary>
        /// The API key for themoviedb.org.
        /// </summary>
        public const string APIKey = "51f2d94f857dd380ee6ae5f52e3c782f";
        private readonly ServiceClient client;

        /// <summary>
        /// The amount of time to wait if there have been too many requests. In seconds.
        /// </summary>
        public const int TheMovieDBRequestLimitPeriod = 10;

        /// <summary>
        /// The base url.
        /// </summary>
        public string BaseURL { private set; get; }

        /// <summary>
        /// The poster size to use.
        /// </summary>
        public string PosterSize => "w500";

        /// <summary>
        /// Constructor.
        /// </summary>
        public TheMovieDBComponent( )
        {
            client = new ServiceClient( APIKey );
        }

        /// <summary>
        /// Retrieves the configuration for TheMovieDB.
        /// </summary>
        public async Task RetrieveConfiguration( )
        {
            CancellationToken token = new CancellationToken( );

            TheMovieDBRequest<dynamic> configRequest = new TheMovieDBRequest<dynamic>( async ( ) => await client.Settings.GetConfigurationAsync( token ) );
            dynamic config = await configRequest.Request( );

            this.BaseURL = config.images.base_url;
        }

        /// <summary>
        /// Returns the full poster URL from the partial poster URL.
        /// </summary>
        /// <param name="poster">The partial poster URL/</param>
        /// <returns>The full poster url.</returns>
        public string GetPosterURL( string poster )
        {
            return BaseURL + PosterSize + poster;
        }
    }
}
