using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using VidereLib;
using VidereLib.Components;
using VidereLib.Data;

namespace Videre.Controls
{
    /// <summary>
    /// Interaction logic for LibraryMediaControl.xaml
    /// </summary>
    public partial class LibraryMediaControl
    {
        /// <summary>
        /// The <see cref="VidereMedia"/>.
        /// </summary>
        protected readonly VidereMedia media;

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

            LoadingRing.IsActive = true;
        }

        /// <summary>
        /// Finishes the loading of the video.
        /// </summary>
        protected void FinishLoadingVideo( )
        {
            LoadingRing.IsActive = false;

            if ( media.MovieInfo?.IMDBID == null || media.Type == VidereMedia.MediaType.Audio )
                return;

            VideoPlaceholder.Visibility = Visibility.Hidden;

            TheMovieDBComponent movieComp = ViderePlayer.GetComponent<TheMovieDBComponent>( );

            MovieInformation info = MediaInformationManager.GetMovieInformationByHash( media.MovieInfo.Hash );
            Title.Text = info.Name;

            BitmapImage img = new BitmapImage( new Uri( movieComp.GetPosterURL( info.Poster ) ) );
            Image.Source = img;

            Rating.Text = Math.Round( info.Rating, 1 ).ToString( CultureInfo.InvariantCulture );

            OnFinishLoadingVideo( );
        }

        /// <summary>
        /// Called whenever the loading of a video is being finished.
        /// </summary>
        protected virtual void OnFinishLoadingVideo( )
        {

        }

        /// <summary>
        /// Finishes the loading of audio.
        /// </summary>
        protected void FinishLoadingAudio( )
        {
            LoadingRing.IsActive = false;
        }

        private void OnControlUnloaded( object Sender, RoutedEventArgs E )
        {
            OnControlUnload( );
        }

        /// <summary>
        /// Gets called whenever the control is unloaded.
        /// </summary>
        protected virtual void OnControlUnload( )
        {
            
        }

        private void OnControlClick( object Sender, MouseButtonEventArgs E )
        {
            ViderePlayer.MediaPlayer.Stop( );
            ViderePlayer.MediaPlayer.LoadMedia( media.File );
            ViderePlayer.MediaPlayer.Play( );

            Window.GetWindow( this ).Close( );
        }
    }
}