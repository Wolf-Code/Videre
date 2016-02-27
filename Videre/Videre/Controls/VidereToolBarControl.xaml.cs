using System.Windows;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Videre.Commands;
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

        private void OnEnableSubtitlesChecked( object Sender, RoutedEventArgs E ) => ViderePlayer.GetComponent<SubtitlesComponent>( ).Enable( );

        private void OnEnableSubtitlesUnchecked( object Sender, RoutedEventArgs E ) => ViderePlayer.GetComponent<SubtitlesComponent>( ).Disable( );

        private void OnSettingsClick( object Sender, RoutedEventArgs E ) => new SettingsWindow( ).ShowDialog( );
    }
}