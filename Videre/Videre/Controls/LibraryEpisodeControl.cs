using System;
using System.Net.TMDb;
using System.Threading;
using System.Windows;
using VidereLib;
using VidereLib.Components;
using VidereLib.Data;
using VidereLib.NetworkingRequests;

namespace Videre.Controls
{
    /// <summary>
    /// Interaction logic for LibraryMediaControl.xaml
    /// </summary>
    public class LibraryEpisodeControl : LibraryMediaControl
    {
        /// <summary>
        /// The show request.
        /// </summary>
        protected RequestEpisodeInfoJob showRequest;

        /// <summary>
        /// Constructor.
        /// </summary>
        public LibraryEpisodeControl( VidereMedia media ) : base( media )
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

            Title.Text = media.Name;
            ToolTip = media.File.Name;

            this.LoadingRing.IsActive = true;

            if ( media.MovieInfo?.IMDBID != null && media.Type == VidereMedia.MediaType.Video )
            {
                MovieInformation movieInfo;
                if ( MediaInformationManager.ContainsMovieInformationForHash( media.MovieInfo.Hash, out movieInfo ) )
                    this.FinishLoadingVideo( );
                else
                {
                    if ( media.MovieInfo != null )
                        MediaInformationManager.SetMovieInformation( media.MovieInfo );


                    ThreadPool.QueueUserWorkItem( async obj =>
                    {
                        showRequest = new RequestEpisodeInfoJob( media );
                        Episode episode = await showRequest.Request( );
                        if ( episode == null )
                        {
                            ViderePlayer.MainDispatcher.Invoke( ( ) => this.LoadingRing.IsActive = false );
                            return;
                        }

                        ViderePlayer.MainDispatcher.Invoke( ( ) =>
                        {
                            MovieInformation info = MediaInformationManager.GetMovieInformationByHash( media.MovieInfo.Hash );
                            info.Poster = ViderePlayer.GetComponent<TheMovieDBComponent>( ).GetPosterURL( episode.Backdrop );
                            info.Rating = episode.VoteAverage;

                            this.FinishLoadingVideo( );
                        } );
                    } );
                }
            }
            else
                this.LoadingRing.IsActive = false;
        }

        /// <summary>
        /// Called whenever the loading of a video has finished.
        /// </summary>
        protected override void OnFinishLoadingVideo( )
        {
            MovieInformation info = MediaInformationManager.GetMovieInformationByHash( media.MovieInfo.Hash );
            this.SubTitle.Visibility = Visibility.Visible;
            this.SubTitle.Text = $"Season {info.Season}, Episode {info.Episode}";
        }

        /// <summary>
        /// Gets called whenever the control is unloaded.
        /// </summary>
        protected override void OnControlUnload( )
        {
            this.showRequest?.Cancel( );
        }
    }
}