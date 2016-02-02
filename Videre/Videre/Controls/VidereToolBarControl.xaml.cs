using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Videre.Windows;
using VidereLib.Components;
using VidereLib.EventArgs;
using VidereSubs.OpenSubtitles.Data;
using VidereSubs.OpenSubtitles.Outputs;

namespace Videre.Controls
{
    /// <summary>
    /// Interaction logic for VidereToolBarControl.xaml
    /// </summary>
    public partial class VidereToolBarControl
    {
        private ProgressDialogController controller;

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
            MediaComponent comp = MainWindow.Player.GetComponent<MediaComponent>( );
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

            InputComponent inputComponent = Player.GetComponent<InputComponent>( );
            inputComponent.OnShowControls += ( Sender, Args ) => this.Visibility = Visibility.Visible;
            inputComponent.OnHideControls += ( Sender, Args ) => this.Visibility = Visibility.Collapsed;

            SubtitlesComponent subs = MainWindow.Player.GetComponent<SubtitlesComponent>( );
            subs.OnSubtitlesFailedToLoad += SubsOnOnSubtitlesFailedToLoad;
            subs.OnSubtitlesLoaded += SubsOnOnSubtitlesLoaded;
        }

        private void SubsOnOnSubtitlesLoaded( object Sender, OnSubtitlesLoadedEventArgs SubtitlesLoadedEventArgs )
        {
            EnableSubs.IsEnabled = true;
            EnableSubs.IsChecked = true;
        }

        private void SubsOnOnSubtitlesFailedToLoad( object Sender, OnSubtitlesFailedToLoadEventArgs OnSubtitlesFailedToLoadEventArgs )
        {
            EnableSubs.IsChecked = false;
            EnableSubs.IsEnabled = false;
        }

        private void OnOpenMediaClick( object Sender, RoutedEventArgs E )
        {
            OpenFileDialog fileDialog = new OpenFileDialog( );

            string VideoFilter = "Video Files ";
            string VideoCombinedCommaSeparated = "*." + string.Join( ", *.", MainWindow.Player.MediaPlayer.VideoFileExtensions );
            string VideoCombinedSemiColonSeparated = "*." + string.Join( ";*.", MainWindow.Player.MediaPlayer.VideoFileExtensions );
            VideoFilter += $"({VideoCombinedCommaSeparated})|{VideoCombinedSemiColonSeparated}";

            string AudioFilter = "Audio Files ";
            string AudioCombinedCommaSeparated = "*." + string.Join( ", *.", MainWindow.Player.MediaPlayer.AudioFileExtensions );
            string AudioCombinedSemiColonSeparated = "*." + string.Join( ";*.", MainWindow.Player.MediaPlayer.AudioFileExtensions );
            AudioFilter += $"({AudioCombinedCommaSeparated})|{AudioCombinedSemiColonSeparated}";

            string Filter = $"Media Files ({VideoCombinedCommaSeparated}, {AudioCombinedCommaSeparated})|{VideoCombinedSemiColonSeparated};{AudioCombinedSemiColonSeparated}|{VideoFilter}|{AudioFilter}|All Files (*.*)|*.*";
            fileDialog.Filter = Filter;

            if ( !fileDialog.ShowDialog( Window.GetWindow( this ) ).GetValueOrDefault( ) )
                return;

            MainWindow.Player.GetComponent<StateComponent>( ).Stop( );
            MainWindow.Player.GetComponent<MediaComponent>( ).LoadMedia( fileDialog.FileName );
        }

        private void OnOpenLocalSubsClick( object Sender, RoutedEventArgs E )
        {
            OpenFileDialog fileDialog = new OpenFileDialog { Filter = "SubRip (*.srt)|*.srt" };
            bool? res = fileDialog.ShowDialog( Window.GetWindow( this ) );

            if ( !res.Value )
                return;

            MainWindow.Player.GetComponent<SubtitlesComponent>( ).LoadSubtitles( fileDialog.FileName );
        }

        private async void OnOSClick( object Sender, RoutedEventArgs E )
        {
            BackgroundWorker worker = new BackgroundWorker( );
            if ( !MainWindow.Client.IsLoggedIn )
            {
                controller = await ( ( MainWindow ) Window.GetWindow( this ) ).ShowProgressAsync( "Signing in.", "Signing into opensubtitles.org." );
                controller.SetIndeterminate( );
                worker.DoWork += ( s, e ) => e.Result = MainWindow.Client.LogIn( string.Empty, string.Empty, false );
                worker.RunWorkerCompleted += async ( s, e ) =>
                {
                    await controller.CloseAsync( );

                    LogInOutput result = e.Result as LogInOutput;
                    if ( MainWindow.Client.IsLoggedIn )
                        OnOSClick( Sender, E );
                    else
                        await ( ( MainWindow ) Window.GetWindow( this ) ).ShowMessageAsync( "Signing in failed", $"Unable to sign in to opensubtitles.org. Please try again later. (Status: {result.Status}, {result.StatusStringWithoutCode})" );
                };
                worker.RunWorkerAsync( );

                return;
            }

            controller = await ( ( MainWindow ) Window.GetWindow( this ) ).ShowProgressAsync( "Retrieving subtitle languages", "Downloading subtitle languages from opensubtitles.org..." );
            controller.SetIndeterminate( );

            worker.DoWork += ( O, Args ) =>
            {
                SubtitleLanguage[ ] langs = MainWindow.Client.GetSubLanguages( );
                Args.Result = langs;
            };
            worker.RunWorkerCompleted += WorkerOnRunWorkerCompleted;
            worker.RunWorkerAsync( );
        }

        private async void WorkerOnRunWorkerCompleted( object Sender, RunWorkerCompletedEventArgs WorkerCompletedEventArgs )
        {
            SubtitleSelectionWindow subselect = new SubtitleSelectionWindow( ( SubtitleLanguage[ ] ) WorkerCompletedEventArgs.Result );
            Task waitOnClose = controller.CloseAsync( );

            if ( !subselect.ShowDialog( ).GetValueOrDefault( ) ) return;
            if ( !subselect.HasDownloadedSubtitleFile ) return;

            await waitOnClose;
            MainWindow.Player.GetComponent<SubtitlesComponent>( ).LoadSubtitles( subselect.DownloadedFile.FullName );
        }

        private void OnEnableSubtitlesChecked( object Sender, RoutedEventArgs E ) => MainWindow.Player.GetComponent<SubtitlesComponent>( ).Enable( );

        private void OnEnableSubtitlesUnchecked( object Sender, RoutedEventArgs E ) => MainWindow.Player.GetComponent<SubtitlesComponent>( ).Disable( );

        private void OnSettingsClick( object Sender, RoutedEventArgs E ) => new SettingsWindow( ).ShowDialog( );
    }
}