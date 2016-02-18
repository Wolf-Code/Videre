using System.Threading.Tasks;
using VidereSubs.OpenSubtitles.Outputs;

namespace VidereSubs.OpenSubtitles
{
    /// <summary>
    /// The interface with opensubtitles.org.
    /// </summary>
    public static class Interface
    {
        /// <summary>
        /// The OpenSubtitles.Org client.
        /// </summary>
        public static Client Client { private set; get; }

        /// <summary>
        /// Initializes the client with a user agent.
        /// </summary>
        /// <param name="userAgent">The user agent.</param>
        public static void Initialize( string userAgent )
        {
            Client = new Client( userAgent );
        }

        /// <summary>
        /// Performs the CheckMovieHash2 method.
        /// </summary>
        /// <param name="movieHashes">The movie hashes to check for.</param>
        /// <returns>The output.</returns>
        public static async Task<CheckMovieHashOutput> CheckMovieHashAllGuesses( params string[ ] movieHashes )
        {
            try
            {
                return await Task.Run( ( ) => Client.CheckMovieHashAllGuesses( movieHashes ) );
            }
            catch ( TaskCanceledException )
            {
                return null;
            }
        }

        /// <summary>
        /// Performs the CheckMovieHash method.
        /// </summary>
        /// <param name="movieHashes">The movie hashes to check for.</param>
        /// <returns>The output.</returns>
        public static async Task<CheckMovieHashOutput> CheckMovieHashBestGuessOnly( params string[ ] movieHashes )
        {
            try
            {
                return await Task.Run( ( ) => Client.CheckMovieHashBestGuessOnly( movieHashes ) );
            }
            catch ( TaskCanceledException )
            {
                return null;
            }
        }
    }
}
