using System;
using System.Threading.Tasks;
using TMDbLib.Objects.TvShows;
using VidereLib.Data;

namespace VidereLib.NetworkingRequests
{
    /// <summary>
    /// A job for requesting movie info.
    /// </summary>
    public class RequestTvInfoJob : RequestBaseJob<TvShow>
    {
        private readonly int showID;
        private TheMovieDBRequest<TvShow> showRequest;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="media">The media to find information for.</param>
        /// <param name="showID">The ID of the show.</param>
        public RequestTvInfoJob( VidereMedia media, int showID ) : base( media )
        {
            this.showID = showID;
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
            this.showRequest?.Cancel( );
        }

        /// <summary>
        /// Initiates the request.
        /// </summary>
        public override async Task<TvShow> Request( )
        {
            showRequest = new TheMovieDBRequest<TvShow>( ( ) => Task.Run( ( ) => client.GetTvShow( showID ), this.token.Token ) );
            showRequest.OnRequestLimitReached += ( Sender, Args ) => this.OnRequestLimitReached?.Invoke( this, null );

            return await showRequest.Request( );
        }
    }
}