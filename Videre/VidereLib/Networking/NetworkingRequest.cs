using System;
using System.Threading;
using System.Threading.Tasks;

namespace VidereLib.Networking
{
    /// <summary>
    /// A base class for a networking request return a <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type that will be returned.</typeparam>
    public abstract class NetworkingRequest<T>
    {
        /// <summary>
        /// The function which will be called in <see cref="Request"/>.
        /// </summary>
        protected readonly Func<Task<T>> RequestFunc;

        /// <summary>
        /// The request's token source.
        /// </summary>
        protected readonly CancellationTokenSource TokenSource;

        /// <summary>
        /// The event which is called whenever too many requests have been made in a small period, causing the server to deny new requests which means we will have to wait for a certain period.
        /// </summary>
        public abstract event EventHandler OnRequestLimitReached;

        /// <summary>
        /// Called whenever an exception has been thrown.
        /// </summary>
        public abstract event EventHandler<Exception> OnExceptionThrown;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="request">The request which will be called in <see cref="Request"/>.</param>
        protected NetworkingRequest( Func<Task<T>> request )
        {
            RequestFunc = request;
            TokenSource = new CancellationTokenSource( );
        }

        /// <summary>
        /// Cancels the request.
        /// </summary>
        public void Cancel( )
        {
            TokenSource.Cancel( );
        }

        /// <summary>
        /// Performs the request.
        /// </summary>
        /// <returns>An awaitable task which will return the <typeparamref name="T"/>.</returns>
        public abstract Task<T> Request( );
    }
}
