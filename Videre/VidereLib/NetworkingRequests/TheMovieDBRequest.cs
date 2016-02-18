using System;
using System.Threading.Tasks;
using VidereLib.Components;
using VidereLib.Networking;

namespace VidereLib.NetworkingRequests
{
    /// <summary>
    /// A request with built-in retrying after 10 seconds if there were too many requests.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TheMovieDBRequest<T> : NetworkingRequest<T>
    {
        /// <summary>
        /// The event which is called whenever too many requests have been made in a small period, causing the server to deny new requests which means we will have to wait for a certain period.
        /// </summary>
        public override event EventHandler OnRequestLimitReached;

        /// <summary>
        /// Called whenever an exception has been thrown.
        /// </summary>
        public override event EventHandler<Exception> OnExceptionThrown;

        private const int MaxRetries = 30;

        /// <summary>
        /// A request used for interfacing with TheMovieDB.org, as it takes the request limit into account.
        /// </summary>
        /// <param name="request"></param>
        public TheMovieDBRequest( Func<Task<T>> request ) : base( request )
        {
        }

        /// <summary>
        /// Performs the request.
        /// </summary>
        /// <returns>An awaitable task which will return the <typeparamref name="T"/>.</returns>
        public override async Task<T> Request( )
        {
            int tries = 0;
            while ( true )
            {
                if ( TokenSource.IsCancellationRequested )
                    break;

                try
                {
                    return await Task.Run( ( ) => RequestFunc( ) );
                }
                catch ( TMDbLib.Objects.Exceptions.RequestLimitExceededException )
                {
                    OnRequestLimitReached?.Invoke( this, null );
                    if ( TokenSource.IsCancellationRequested )
                        break;

                    await Task.Delay( TimeSpan.FromSeconds( TheMovieDBComponent.TheMovieDBRequestLimitPeriod ), TokenSource.Token );
                }
                catch ( TaskCanceledException )
                {
                    break;
                }
                catch ( Exception e )
                {
                    if ( OnExceptionThrown != null )
                    {
                        OnExceptionThrown.Invoke( this, e );
                        try
                        {
                            if ( TokenSource.IsCancellationRequested )
                                break;

                            await Task.Delay( TimeSpan.FromSeconds( TheMovieDBComponent.TheMovieDBRequestLimitPeriod ), TokenSource.Token );
                        }
                        catch ( TaskCanceledException )
                        {
                            break;
                        }
                    }
                    else
                        throw;
                }

                if ( ++tries >= MaxRetries )
                    throw new Exception( "Retries exceeded the maximum retry amount." );
            }

            return default ( T );
        }
    }
}
