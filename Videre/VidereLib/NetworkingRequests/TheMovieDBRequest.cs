using System;
using System.Threading;
using System.Threading.Tasks;
using TMDbLib.Objects.Exceptions;
using VidereLib.Components;

namespace VidereLib.NetworkingRequests
{
    /// <summary>
    /// A request with built-in retrying after 10 seconds if there were too many requests.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TheMovieDBRequest<T> : TheMovieDBRequestBase
    {
        /// <summary>
        /// The event which is called whenever too many requests have been made in a small period, causing the server to deny new requests which means we will have to wait for a certain period.
        /// </summary>
        public override event EventHandler OnRequestLimitReached;

        /// <summary>
        /// Called whenever the request completes.
        /// </summary>
        public event EventHandler<T> OnRequestCompleted;

        private bool isStarted;

        private readonly Func<T> RequestFunc;

        private Thread thread;

        /// <summary>
        /// A request used for interfacing with TheMovieDB.org, as it takes the request limit into account.
        /// </summary>
        /// <param name="request"></param>
        public TheMovieDBRequest( Func<T> request )
        {
            this.RequestFunc = request;
        }

        private void PerformRequest( object obj )
        {
            try
            {
                T result = RequestFunc( );

                this.OnRequestCompleted?.Invoke( this, result );
            }
            catch ( AggregateException exception )
            {
                exception.Handle( x =>
                {
                    if ( x is RequestLimitExceededException )
                    {
                        OnRequestLimitReached?.Invoke( this, null );
                        Task.Delay( TimeSpan.FromSeconds( TheMovieDBComponent.TheMovieDBRequestLimitPeriod ) ).Wait( );
                        PerformRequest( obj );

                        return true;
                    }
                    
                    throw x;
                } );
            }
        }

        /// <summary>
        /// Cancels the request.
        /// </summary>
        public override void Cancel( )
        {
            thread?.Abort( );
        }

        /// <summary>
        /// Performs the request.
        /// </summary>
        /// <returns>An awaitable task which will return the <typeparamref name="T"/>.</returns>
        public void Request( )
        {
            if ( isStarted )
                throw new Exception( "Attempted to start a request twice." );

            isStarted = true;
            thread = new Thread( PerformRequest );
            thread.Start( );
        }
    }
}
