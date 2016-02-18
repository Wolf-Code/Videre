using System.Windows;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Videre.Windows;
using VidereLib;
using VidereLib.Components;
using VidereLib.EventArgs;

namespace Videre.Controls
{
    /// <summary>
    /// Interaction logic for VidereToolBarControl.xaml
    /// </summary>
    public partial class VidereToolBarControl
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public VidereToolBarControl( )
        {
            InitializeComponent( );
        }

        /// <summary>
        /// Gets called whenever the player has been initialized.
        /// </summary>
        public override void OnPlayerInitialized( )
        {
            MediaComponent comp = ViderePlayer.GetComponent<MediaComponent>( );
            comp.OnMediaLoaded += ( Sender, Args ) =>
            {
                OpenLocalSubs.IsEnabled = true;
                OpenOSSubs.IsEnabled = true;
            };
            comp.OnMediaFailedToLoad += ( Sender, Args ) =>
            {
                OpenLocalSubs.IsEnabled = false;
                OpenOSSubs.IsEnabled = false;
                EnableSubs.IsEnabled = false;
                EnableSubs.IsChecked = false;
            };

            InputComponent inputComponent = ViderePlayer.GetComponent<InputComponent>( );
            inputComponent.OnShowControls += ( Sender, Args ) => Visibility = Visibility.Visible;
            inputComponent.OnHideControls += ( Sender, Args ) => Visibility = Visibility.Collapsed;

            SubtitlesComponent subs = ViderePlayer.GetComponent<SubtitlesComponent>( );
            subs.OnSubtitlesFailedToLoad += SubsOnOnSubtitlesFailedToLoad;
            subs.OnSubtitlesLoaded += SubsOnOnSubtitlesLoaded;
        }

        private void SubsOnOnSubtitlesLoaded( object Sender, OnSubtitlesLoadedEventArgs SubtitlesLoadedEventArgs )
        {
            EnableSubs.IsEnabled = true;
            EnableSubs.IsChecked = true;
        }

        private async void SubsOnOnSubtitlesFailedToLoad( object Sender, OnSubtitlesFailedToLoadEventArgs OnSubtitlesFailedToLoadEventArgs )
        {
            EnableSubs.IsChecked = false;
            EnableSubs.IsEnabled = false;

            await ( ( MainWindow ) Window.GetWindow( this ) ).ShowMessageAsync( "Failed to load subtitles", $"Subtitles file {OnSubtitlesFailedToLoadEventArgs.Subtitles.Name} could not be loaded." );
        }

        private void OnOpenMediaClick( object Sender, RoutedEventArgs E )
        {
            OpenFileDialog fileDialog = new OpenFileDialog( );

            string VideoFilter = "Video Files ";
            string VideoCombinedCommaSeparated = "*." + string.Join( ", *.", ViderePlayer.MediaPlayer.VideoFileExtensions );
            string VideoCombinedSemiColonSeparated = "*." + string.Join( ";*.", ViderePlayer.MediaPlayer.VideoFileExtensions );
            VideoFilter += $"({VideoCombinedCommaSeparated})|{VideoCombinedSemiColonSeparated}";

            string Filter = $"{VideoFilter}|All Files (*.*)|*.*";
            fileDialog.Filter = Filter;

            if ( !fileDialog.ShowDialog( Window.GetWindow( this ) ).GetValueOrDefault( ) )
                return;

            ViderePlayer.GetComponent<StateComponent>( ).Stop( );
            ViderePlayer.GetComponent<MediaComponent>( ).LoadMedia( fileDialog.FileName );
        }

        private void OnOpenLocalSubsClick( object Sender, RoutedEventArgs E )
        {
            OpenFileDialog fileDialog = new OpenFileDialog { Filter = "SubRip (*.srt)|*.srt" };
            if ( ViderePlayer.MediaPlayer.IsMediaLoaded )
                fileDialog.InitialDirectory = ViderePlayer.MediaPlayer.Media.File.DirectoryName;

            bool? res = fileDialog.ShowDialog( Window.GetWindow( this ) );

            if ( !res.Value )
                return;

            ViderePlayer.GetComponent<SubtitlesComponent>( ).LoadSubtitles( fileDialog.FileName );
        }

        private void OnOSClick( object Sender, RoutedEventArgs E )
        {
            MainWindow window = ( MainWindow ) Window.GetWindow( this );
            window.OSFlyout.IsOpen = true;
        }

        private void OnEnableSubtitlesChecked( object Sender, RoutedEventArgs E ) => ViderePlayer.GetComponent<SubtitlesComponent>( ).Enable( );

        private void OnEnableSubtitlesUnchecked( object Sender, RoutedEventArgs E ) => ViderePlayer.GetComponent<SubtitlesComponent>( ).Disable( );

        private void OnSettingsClick( object Sender, RoutedEventArgs E ) => new SettingsWindow( ).ShowDialog( );

        private void OnShowLibraryClick( object Sender, RoutedEventArgs E )
        {
            new LibraryWindow( ).ShowDialog( );
        }
    }
}