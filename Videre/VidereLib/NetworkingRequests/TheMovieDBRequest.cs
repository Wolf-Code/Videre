using System;
using System.Net.TMDb;
using System.Threading;
using System.Threading.Tasks;
using VidereLib.Components;

namespace VidereLib.NetworkingRequests
{
    class TheMovieDBRequest<T>
    {
        private readonly Func<Task<T>> request;
        private readonly CancellationTokenSource token;

        public event EventHandler OnRequestLimitReached;  

        public TheMovieDBRequest( Func<Task<T>> request )
        {
            this.request = request;
            this.token = new CancellationTokenSource( );
        }

        public void Cancel( )
        {
            this.token.Cancel( );
        }

        public async Task<T> Request( )
        {
            bool Retry = true;
            while ( Retry )
            {
                try
                {
                    return await request( );
                }
                catch ( ServiceRequestException e )
                {
                    if ( e.StatusCode == 429 )
                    {
                        this.OnRequestLimitReached?.Invoke( this, null );
                        await Task.Delay( TimeSpan.FromSeconds( TheMovieDBComponent.TheMovieDBRequestLimitPeriod ), token.Token );
                    }
                    else
                        Retry = false;
                }
            }

            return default ( T );
        }
    }
}
