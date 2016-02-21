using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TMDbLib.Client;
using TMDbLib.Objects.Find;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.TvShows;
using VidereLib.Data;
using VidereLib.EventArgs;
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
        private readonly TMDbClient client;

        /// <summary>
        /// The amount of time to wait if there have been too many requests. In seconds.
        /// </summary>
        public const int TheMovieDBRequestLimitPeriod = 10;

        /// <summary>
        /// The base url.
        /// </summary>
        public string BaseURL { private set; get; }

        /// <summary>
        /// Checks to see if the configuration has already been retrieved.
        /// </summary>
        public bool HasConfig => BaseURL != null;

        /// <summary>
        /// The poster size to use.
        /// </summary>
        public string PosterSize => "w500";

        /// <summary>
        /// Gets called whenever episode information has been received.
        /// </summary>
        public event EventHandler<Tuple<VidereMedia,SearchTvEpisode>> OnEpisodeInformationReceived; 

        /// <summary>
        /// Gets called whenever movie information has been received.
        /// </summary>
        public event EventHandler<Tuple<VidereMedia,MovieResult>> OnMovieInformationReceived;

        /// <summary>
        /// Gets called whenever tv show information has been received.
        /// </summary>
        public event EventHandler<Tuple<int, TvShow>> OnTvShowInformationReceived;

        /// <summary>
        /// Gets called whenever season information has been received.
        /// </summary>
        public event EventHandler<OnTvSeasonInformationReceivedEventArgs> OnSeasonInformationReceived;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TheMovieDBComponent( )
        {
            client = new TMDbClient( APIKey, true );
        }

        /// <summary>
        /// Retrieves the configuration for TheMovieDB.
        /// </summary>
        public async Task RetrieveConfiguration( )
        {
            await Task.Run( ( ) =>
            {
                client.GetConfig( );
                BaseURL = client.Config.Images.BaseUrl;
            } );
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

        private TheMovieDBRequest<FindContainer> RequestIMDBInfo( VidereMedia media )
        {
            if ( !media.HasImdbID )
                return null;

            TheMovieDBRequest<FindContainer> request = new TheMovieDBRequest<FindContainer>( ( ) =>
            {
                TMDbClient cl = new TMDbClient( APIKey );
                Task<FindContainer> container = cl.Find( FindExternalSource.Imdb, "tt" + media.MediaInformation.IMDBID );
                container.Wait( );

                return container.Result;
            } );

            return request;
        }

        /// <summary>
        /// Starts a request for movie information. The result will be returned via <see cref="OnMovieInformationReceived"/>.
        /// </summary>
        /// <param name="media">The <see cref="VidereMedia"/> to find the information for.</param>
        public TheMovieDBRequestBase RequestMovieInformation( VidereMedia media )
        {
            TheMovieDBRequest<FindContainer> request = RequestIMDBInfo( media );
            if ( request == null )
            {
                this.OnMovieInformationReceived?.Invoke( this, new Tuple<VidereMedia, MovieResult>( media, null ) );
                return null;
            }

            request.OnRequestCompleted += ( Sender, Container ) =>
            {
                MovieResult movie = Container.MovieResults.FirstOrDefault( );
                if ( movie == null )
                {
                    this.OnMovieInformationReceived?.Invoke( this, new Tuple<VidereMedia, MovieResult>( media, null ) );
                    return;
                }

                this.OnMovieInformationReceived?.Invoke( this, new Tuple<VidereMedia, MovieResult>( media, movie ) );
            };

            request.Request( );

            return request;
        }

        /// <summary>
        /// Starts a request for episode information. The result will be returned via <see cref="OnEpisodeInformationReceived"/>.
        /// </summary>
        /// <param name="media">The <see cref="VidereMedia"/> to find the information for.</param>
        public TheMovieDBRequestBase RequestEpisodeInformation( VidereMedia media )
        {
            TheMovieDBRequest<FindContainer> request = RequestIMDBInfo( media );
            if ( request == null )
            {
                this.OnEpisodeInformationReceived?.Invoke( this, new Tuple<VidereMedia, SearchTvEpisode>( media, null ) );
                return null;
            }

            request.OnRequestCompleted += ( Sender, Container ) =>
            {
                SearchTvEpisode episode = Container.TvEpisodes.FirstOrDefault( );
                if ( episode == null )
                {
                    this.OnEpisodeInformationReceived?.Invoke( this, new Tuple<VidereMedia, SearchTvEpisode>( media, null ) );
                    return;
                }

                this.OnEpisodeInformationReceived?.Invoke( this, new Tuple<VidereMedia, SearchTvEpisode>( media, episode ) );
            };

            request.Request( );

            return request;
        }

        /// <summary>
        /// Starts a request for season information. The result will be returned via <see cref="OnSeasonInformationReceived"/>.
        /// </summary>
        /// <param name="showID">The ID of the show.</param>
        /// <param name="season">The season of the show.</param>
        public TheMovieDBRequestBase RequestSeasonInformation( int showID, int season )
        {
            TheMovieDBRequest<TvSeason> request = new TheMovieDBRequest<TvSeason>( ( ) =>
            {
                Task<TvSeason> container = client.GetTvSeason( showID, season );
                container.Wait( );

                return container.Result;
            } );

            request.OnRequestCompleted += ( Sender, Season ) =>
            {
                if ( Season == null )
                {
                    this.OnSeasonInformationReceived?.Invoke( this, new OnTvSeasonInformationReceivedEventArgs( showID, season, null ) );
                    return;
                }

                this.OnSeasonInformationReceived?.Invoke( this, new OnTvSeasonInformationReceivedEventArgs( showID, season, Season ) );
            };

            request.Request( );

            return request;
        }

        /// <summary>
        /// Starts a request for tv show information. The result will be returned via <see cref="OnTvShowInformationReceived"/>.
        /// </summary>
        /// <param name="showID">The ID for the show.</param>
        public TheMovieDBRequestBase RequestTvShowInformation( int showID )
        {
            TheMovieDBRequest<TvShow> request = new TheMovieDBRequest<TvShow>( ( ) =>
            {
                TMDbClient cl = new TMDbClient( APIKey );
                Task<TvShow> container = cl.GetTvShow( showID );
                container.Wait( );

                return container.Result;
            } );

            request.OnRequestCompleted += ( Sender, Show ) =>
            {
                if ( Show == null )
                {
                    this.OnTvShowInformationReceived?.Invoke( this, new Tuple<int, TvShow>( showID, null ) );
                    return;
                }

                this.OnTvShowInformationReceived?.Invoke( this, new Tuple<int, TvShow>( showID, Show ) );
            };

            request.Request( );

            return request;
        }
    }
}
