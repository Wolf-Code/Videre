using System;
using System.Threading;
using System.Windows;
using TMDbLib.Objects.General;
using VidereLib;
using VidereLib.Components;
using VidereLib.Data;
using VidereLib.NetworkingRequests;

namespace Videre.Controls
{
    /// <summary>
    /// Interaction logic for LibraryMediaControl.xaml
    /// </summary>
    public class LibraryMovieControl : LibraryMediaControl
    {
        /// <summary>
        /// The movie request.
        /// </summary>
        protected RequestMovieInfoJob movieRequest;

        /// <summary>
        /// Constructor.
        /// </summary>
        public LibraryMovieControl( VidereMedia media ) : base( media )
        {

        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized( EventArgs e )
        {
            base.OnInitialized( e );

            VideoPlaceholder.Visibility = Visibility.Visible;

            if ( media.MovieInfo?.IMDBID != null )
            {
                MovieInformation movieInfo;
                if ( MediaInformationManager.ContainsMovieInformationForHash( media.MovieInfo.Hash, out movieInfo ) )
                    FinishLoadingVideo( );
                else
                {
                    if ( media.MovieInfo != null )
                        MediaInformationManager.SetMovieInformation( media.MovieInfo );


                    ThreadPool.QueueUserWorkItem( async obj =>
                    {
                        movieRequest = new RequestMovieInfoJob( media );
                        MovieResult movie = await movieRequest.Request( );
                        if ( movie == null )
                        {
                            ViderePlayer.MainDispatcher.Invoke( ( ) => LoadingRing.IsActive = false );
                            return;
                        }

                        ViderePlayer.MainDispatcher.Invoke( ( ) =>
                        {
                            MovieInformation info = MediaInformationManager.GetMovieInformationByHash( media.MovieInfo.Hash );
                            info.Poster = ViderePlayer.GetComponent<TheMovieDBComponent>( ).GetPosterURL( movie.PosterPath );
                            info.Rating = ( decimal ) movie.VoteAverage;

                            FinishLoadingVideo( );
                        } );

                    } );
                }
            }
            else
                LoadingRing.IsActive = false;
        }

        /// <summary>
        /// Gets called whenever the control is unloaded.
        /// </summary>
        protected override void OnControlUnload( )
        {
            movieRequest?.Cancel( );
        }
    }
}