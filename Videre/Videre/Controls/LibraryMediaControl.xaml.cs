using System;
using System.Globalization;
using System.Net.TMDb;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using VidereLib;
using VidereLib.Components;
using VidereLib.Data;
using VidereLib.NetworkingRequests;

namespace Videre.Controls
{
    /// <summary>
    /// Interaction logic for LibraryMediaControl.xaml
    /// </summary>
    public partial class LibraryMediaControl
    {
        private readonly VidereMedia media;
        private RequestMovieInfoJob movieRequest;

        /// <summary>
        /// Constructor.
        /// </summary>
        public LibraryMediaControl( VidereMedia media )
        {
            this.media = media;
            InitializeComponent( );
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized( EventArgs e )
        {
            base.OnInitialized( e );

            Title.Text = media.Name;
            ToolTip = media.File.Name;

            switch ( media.Type )
            {
                    case VidereMedia.MediaType.Video:
                    VideoPlaceholder.Visibility = Visibility.Visible;
                    break;

                    case VidereMedia.MediaType.Audio:
                    AudioPlaceholder.Visibility = Visibility.Visible;
                    break;
            }
            
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

                    movieRequest = new RequestMovieInfoJob( media );
                    ThreadPool.QueueUserWorkItem( async obj =>
                    {
                        Movie movie = await movieRequest.Request( );
                        if ( movie == null )
                            return;

                        ViderePlayer.MainDispatcher.Invoke( ( ) =>
                        {
                            MovieInformation info = MediaInformationManager.GetMovieInformationByHash( media.MovieInfo.Hash );
                            info.Poster = ViderePlayer.GetComponent<TheMovieDBComponent>( ).GetPosterURL( movie.Poster );
                            info.Rating = movie.VoteAverage;

                            this.FinishLoadingVideo( );
                        } );
                    } );
                }
            }
            else
                this.LoadingRing.IsActive = false;
        }

        private void FinishLoadingVideo( )
        {
            this.LoadingRing.IsActive = false;

            if ( media.MovieInfo?.IMDBID == null || media.Type == VidereMedia.MediaType.Audio )
                return;

            this.VideoPlaceholder.Visibility = Visibility.Hidden;

            TheMovieDBComponent movieComp = ViderePlayer.GetComponent<TheMovieDBComponent>( );

            MovieInformation info = MediaInformationManager.GetMovieInformationByHash( media.MovieInfo.Hash );
            this.Title.Text = info.Name;

            BitmapImage img = new BitmapImage( new Uri( movieComp.GetPosterURL( info.Poster ) ) );
            this.Image.Source = img;

            this.Rating.Text = info.Rating.ToString( CultureInfo.InvariantCulture );
        }

        private void OnControlUnloaded( object Sender, RoutedEventArgs E )
        {
            movieRequest?.Cancel( );
        }
    }
}