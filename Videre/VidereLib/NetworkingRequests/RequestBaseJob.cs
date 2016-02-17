using System;
using System.Threading;
using System.Threading.Tasks;
using TMDbLib.Client;
using TMDbLib.Objects.Find;
using VidereLib.Components;
using VidereLib.Data;

namespace VidereLib.NetworkingRequests
{
    /// <summary>
    /// The base class for a request that always does the same thing with themoviedb.org.
    /// </summary>
    /// <typeparam name="T">The type the requests will return.</typeparam>
    public abstract class RequestBaseJob<T>
    {
        /// <summary>
        /// The <see cref="VidereMedia"/> to perform the request for.
        /// </summary>
        protected readonly VidereMedia media;

        /// <summary>
        /// The client to use for the request.
        /// </summary>
        protected readonly TMDbClient client;

        /// <summary>
        /// Called whenever the request are suspended due to too many requests having been sent.
        /// </summary>
        public abstract event EventHandler OnRequestLimitReached;

        /// <summary>
        /// The cancellation token.
        /// </summary>
        protected readonly CancellationTokenSource token;

        /// <summary>
        /// The request to use in case a FindContainer is used.
        /// </summary>
        protected TheMovieDBRequest<FindContainer> request;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="media">The media to find information for.</param>
        protected RequestBaseJob( VidereMedia media )
        {
            client = new TMDbClient( TheMovieDBComponent.APIKey, true );
            this.media = media;
            token = new CancellationTokenSource( );
        }

        /// <summary>
        /// Cancels the request.
        /// </summary>
        public void Cancel( )
        {
            token.Cancel( );
            request?.Cancel( );
            this.OnCancel( );
        }

        /// <summary>
        /// Gets called whenever a request is cancelled.
        /// </summary>
        protected virtual void OnCancel( )
        {

        }

        /// <summary>
        /// Initiates the request.
        /// </summary>
        public abstract Task<T> Request( );
    }
}
