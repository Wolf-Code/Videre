using System;

namespace VidereLib.NetworkingRequests
{
    /// <summary>
    /// The base for all themoviedb.org requests.
    /// </summary>
    public abstract class TheMovieDBRequestBase
    {
        /// <summary>
        /// The event which is called whenever too many requests have been made in a small period, causing the server to deny new requests which means we will have to wait for a certain period.
        /// </summary>
        public abstract event EventHandler OnRequestLimitReached;

        /// <summary>
        /// Cancels the request.
        /// </summary>
        public abstract void Cancel( );
    }
}
