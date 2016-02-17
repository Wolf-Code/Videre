using System;
using System.Net.TMDb;
using System.Threading.Tasks;
using VidereLib.Components;
using VidereLib.Networking;

namespace VidereLib.NetworkingRequests
{
    class TheMovieDBRequest<T> : NetworkingRequest<T>
    {
        public override event EventHandler OnRequestLimitReached;
        public override event EventHandler<Exception> OnExceptionThrown;

        private const int MaxRetries = 30;

        public TheMovieDBRequest( Func<Task<T>> request ) : base( request )
        {
        }

        public override async Task<T> Request( )
        {
            int tries = 0;
            while ( true )
            {
                if ( this.TokenSource.IsCancellationRequested )
                    break;

                try
                {
                    return await this.RequestFunc( );
                }
                catch ( ServiceRequestException e )
                {
                    if ( e.StatusCode == 429 )
                    {
                        this.OnRequestLimitReached?.Invoke( this, null );
                        await Task.Delay( TimeSpan.FromSeconds( TheMovieDBComponent.TheMovieDBRequestLimitPeriod ), this.TokenSource.Token );
                    }
                    else
                    {
                        if ( OnExceptionThrown != null )
                        {
                            OnExceptionThrown.Invoke( this, e );
                            await Task.Delay( TimeSpan.FromSeconds( TheMovieDBComponent.TheMovieDBRequestLimitPeriod ), this.TokenSource.Token );
                        }
                        else
                            throw;
                    }
                }

                if ( ++tries >= MaxRetries )
                    throw new Exception( "Retries exceeded the maximum retry amount." );
            }

            return default ( T );
        }
    }
}
