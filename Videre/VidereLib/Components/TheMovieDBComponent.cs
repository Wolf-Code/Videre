using System.Net.TMDb;

namespace VidereLib.Components
{
    /// <summary>
    /// The <see cref="ComponentBase"/> for accessing information from themoviedb.org.
    /// </summary>
    public class TheMovieDBComponent : ComponentBase
    {
        private const string APIKey = "51f2d94f857dd380ee6ae5f52e3c782f";
        private readonly ServiceClient client;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="player">The <see cref="ViderePlayer"/>.</param>
        public TheMovieDBComponent( ViderePlayer player ) : base( player )
        {
            client = new ServiceClient( APIKey );
        }
    }
}
