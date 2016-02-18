using System;
using System.Threading.Tasks;
using TMDbLib.Objects.TvShows;
using VidereLib.Data;

namespace VidereLib.NetworkingRequests
{
    /// <summary>
    /// A job for requesting movie info.
    /// </summary>
    public class RequestSeasonInfoJob : RequestBaseJob<TvSeason>
    {
        private readonly int showID;
        private readonly int season;
        private TheMovieDBRequest<TvSeason> tvSeason;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="media">The media to find information for.</param>
        /// <param name="showID">The ID of the show.</param>
        /// <param name="season">The season of the show.</param>
        public RequestSeasonInfoJob( VidereMedia media, int showID, int season ) : base( media )
        {
            this.showID = showID;
            this.season = season;
        }

        /// <summary>
        /// Called whenever the request are suspended due to too many requests having been sent.
        /// </summary>
        public override event EventHandler OnRequestLimitReached;

        /// <summary>
        /// Gets called whenever a request is cancelled.
        /// </summary>
        protected override void OnCancel( )
        {
            this.tvSeason?.Cancel( );
        }

        /// <summary>
        /// Initiates the request.
        /// </summary>
        public override async Task<TvSeason> Request( )
        {
            tvSeason = new TheMovieDBRequest<TvSeason>( ( ) => Task.Run( ( ) => client.GetTvSeason( showID, season ), this.token.Token ) );
            tvSeason.OnRequestLimitReached += ( Sender, Args ) => this.OnRequestLimitReached?.Invoke( this, null );
            tvSeason.OnExceptionThrown += ( Sender, e ) =>
            {
                throw new Exception( "Exception during tv season request.", e );
            };

            return await tvSeason.Request( );
        }
    }
}
