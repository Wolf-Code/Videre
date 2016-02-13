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

        public TheMovieDBRequest( Func<Task<T>> request ) : base( request )
        {
        }

        public override async Task<T> Request( )
        {
            bool Retry = true;
            while ( Retry )
            {
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
                        Retry = false;
                }
            }

            return default ( T );
        }
    }
}
