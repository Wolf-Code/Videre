using System;
using System.Linq;
using System.Threading.Tasks;
using TMDbLib.Objects.Find;
using TMDbLib.Objects.Search;
using VidereLib.Data;

namespace VidereLib.NetworkingRequests
{
    /// <summary>
    /// A job for requesting movie info.
    /// </summary>
    public class RequestEpisodeInfoJob : RequestBaseJob<SearchTvEpisode>
    {
        /// <summary>
        /// Called whenever the request are suspended due to too many requests having been sent.
        /// </summary>
        public override event EventHandler OnRequestLimitReached;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="media">The media to find information for.</param>
        public RequestEpisodeInfoJob( VidereMedia media ) : base( media )
        {
        }

        /// <summary>
        /// Initiates the request.
        /// </summary>
        public override async Task<SearchTvEpisode> Request( )
        {
            if ( media.MovieInfo?.IMDBID == null ) return null;

            request = new TheMovieDBRequest<FindContainer>( async ( ) => await Task.Run( ( ) => client.Find( FindExternalSource.Imdb, "tt" + media.MovieInfo.IMDBID ) ) );
            request.OnRequestLimitReached += ( Sender, Args ) => OnRequestLimitReached?.Invoke( this, null );
            FindContainer Res = await request.Request( );

            return Res.TvEpisodes.FirstOrDefault( );
        }
    }
}