using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Videre.Properties;
using VidereLib;
using VidereLib.Components;
using VidereLib.EventArgs;

namespace Videre
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private ViderePlayer player;
        private DispatcherTimer SliderTimer;
        private bool ChangedExternally;

        /// <summary>
        /// True if the player is playing, false otherwise.
        /// </summary>
        public bool IsPlaying { private set; get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindow( )
        {
            InitializeComponent( );
        }

        /// <summary>
        /// Initializes player.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInitialized( EventArgs e )
        {
            SliderTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds( 250 ) };
            SliderTimer.Tick += ( sender, args ) => PerformTimeSlide( );

            player = new ViderePlayer( new WindowData { Window = this, ControlsGrid = ControlsGrid, MediaPlayer = MediaPlayer, MediaArea = MediaArea } );
            player.GetComponent<TimeComponent>( ).OnPositionChanged += PlayerOnOnPositionChanged;
            SubtitlesComponent subtitlesComponent = player.GetComponent<SubtitlesComponent>( );
            subtitlesComponent.OnSubtitlesChanged += PlayerOnOnSubtitlesChanged;
            subtitlesComponent.OnSubtitlesFailedToLoad += SubtitlesComponentOnOnSubtitlesFailedToLoad;

            MediaComponent mediaComponent = player.GetComponent<MediaComponent>( );
            mediaComponent.OnMediaLoaded += OnOnMediaLoaded;
            mediaComponent.OnMediaUnloaded += OnOnMediaUnloaded;
            mediaComponent.OnMediaFailedToLoad += MediaComponentOnOnMediaFailedToLoad;

            StateComponent stateComponent = player.GetComponent<StateComponent>( );
            stateComponent.OnStateChanged += OnOnStateChanged;

            InputComponent inputComponent = player.GetComponent<InputComponent>( );
            inputComponent.OnShowControls += ( Sender, Args ) => ControlsGrid.Visibility = Visibility.Visible;
            inputComponent.OnHideControls += ( Sender, Args ) => ControlsGrid.Visibility = Visibility.Collapsed;

            WindowButtonCommandsOverlayBehavior = WindowCommandsOverlayBehavior.Never;
            RightWindowCommandsOverlayBehavior = WindowCommandsOverlayBehavior.Never;
            LeftWindowCommandsOverlayBehavior = WindowCommandsOverlayBehavior.Never;

            Application.Current.DispatcherUnhandledException += ( Sender, Args ) =>
            {
                using ( FileStream FS = File.Create( "exception.txt" ) )
                    using ( TextWriter Writer = new StreamWriter( FS ) )
                        WriteExceptionDetails( Args.Exception, Writer );

                MessageBox.Show( "An exception has been encountered. The exact details have been saved in exception.txt. Please contact the developer and hand them this file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error );
                Args.Handled = true;
                Application.Current.Shutdown( );
            };

            base.OnInitialized( e );
        }

        private async void MediaComponentOnOnMediaFailedToLoad( object Sender, OnMediaFailedToLoadEventArgs MediaFailedToLoadEventArgs )
        {
            await this.ShowMessageAsync( "Failed to load media", $"Unable to load {MediaFailedToLoadEventArgs.MediaFile.Name}. Reason: {MediaFailedToLoadEventArgs.Exception.Message}" );
        }

        private void OnOnMediaUnloaded( object Sender, OnMediaUnloadedEventArgs MediaUnloadedEventArgs )
        {
            ControlsGrid.IsEnabled = false;
            SubtitlesButton.IsEnabled = false;
            TimeLabel_Total.Content = "--:--:--";
            TimeLabel_Current.Content = TimeLabel_Total.Content;
        }

        private void OnOnMediaLoaded( object Sender, OnMediaLoadedEventArgs MediaLoadedEventArgs )
        {
            ControlsGrid.IsEnabled = true;
            SubtitlesButton.IsEnabled = true;
            FileFlyout.IsOpen = false;
            SubtitlesOffset.Value = 0;
            
            TimeLabel_Total.Content = player.GetComponent<MediaComponent>( ).GetMediaLength( ).ToString( @"hh\:mm\:ss" );
            player.GetComponent<StateComponent>( ).Play( );
        }

        private static void WriteExceptionDetails( Exception exception, TextWriter writer )
        {
            writer.WriteLine( $"Source: {exception.Source}" );
            writer.WriteLine( exception.ToString( ) );
            writer.WriteLine( );
            writer.WriteLine( "Data:" );
            foreach ( object key in exception.Data.Keys )
                writer.WriteLine( $"{key}: {exception.Data[ key ]}" );
        }

        private void OnOnStateChanged( object Sender, OnStateChangedEventArgs StateChangedEventArgs )
        {
            IsPlaying = StateChangedEventArgs.State == StateComponent.PlayerState.Playing;
            this.OnPropertyChanged( nameof( IsPlaying ) );

            switch ( StateChangedEventArgs.State )
            {
                case StateComponent.PlayerState.Stopped:
                    TimeSlider.Value = 0;
                    break;
            }
        }

        #region Subtitles
        
        private void SubtitlesPosition_OnValueChanged( object Sender, RoutedPropertyChangedEventArgs<double?> E )
        {
            if ( !E.NewValue.HasValue )
                return;

            subtitleLabel.Margin = new Thickness( 0, 0, 0, E.NewValue.Value );
        }

        private void NumericUpDown_OnValueChanged( object Sender, RoutedPropertyChangedEventArgs<double?> E )
        {
            if ( !E.NewValue.HasValue )
                return;

            player.GetComponent<SubtitlesComponent>( ).SetSubtitlesOffset( TimeSpan.FromMilliseconds( E.NewValue.Value ) );
        }

        private void SubtitlesSize_OnValueChanged( object Sender, RoutedPropertyChangedEventArgs<double?> E )
        {
            if ( !E.NewValue.HasValue )
                return;

            subtitleLabel.FontSize = E.NewValue.Value;
        }

        private void PlayerOnOnSubtitlesChanged( object Sender, OnSubtitlesChangedEventArgs SubtitlesChangedEventArgs )
        {
            subtitleLabel.Inlines.Clear( );
            if ( SubtitlesChangedEventArgs.Subtitles.Lines.Count <= 0 )
                return;

            for ( int X = 0; X < SubtitlesChangedEventArgs.Subtitles.Lines.Count; X++ )
            {
                subtitleLabel.Inlines.Add( SubtitlesChangedEventArgs.Subtitles.Lines[ X ] );

                if ( X < SubtitlesChangedEventArgs.Subtitles.Lines.Count - 1 )
                    subtitleLabel.Inlines.Add( Environment.NewLine );
            }
        }

        private async void SubtitlesComponentOnOnSubtitlesFailedToLoad( object Sender, OnSubtitlesFailedToLoadEventArgs SubtitlesFailedToLoadEventArgs )
        {
            await this.ShowMessageAsync( "Failed to load subtitles", $"Unable to load {SubtitlesFailedToLoadEventArgs.Subtitles.Name}." );
        }

        #endregion

        private void PlayerOnOnPositionChanged( object Sender, OnPositionChangedEventArgs OnPositionChangedEventArgs )
        {
            ChangedExternally = true;
            TimeSlider.Value = OnPositionChangedEventArgs.Progress * TimeSlider.Maximum;
            TimeLabel_Current.Content = OnPositionChangedEventArgs.Position.ToString( @"hh\:mm\:ss" );
        }

        private void PerformTimeSlide( )
        {
            TimeComponent timeHandler = player.GetComponent<TimeComponent>( );
            double progress = TimeSlider.Value / TimeSlider.Maximum;
            timeHandler.SetPosition( progress );

            SliderTimer.Stop( );
            timeHandler.StopChangingPosition( );
        }

        private void OnPlayPauseButtonClick( object Sender, RoutedEventArgs E )
        {
            if ( !player.GetComponent<MediaComponent>( ).HasMediaBeenLoaded )
                return;

            player.GetComponent<StateComponent>( ).ResumeOrPause( );
        }

        private void OnForwardButtonClick( object Sender, RoutedEventArgs E )
        {
        }

        private void OnBackButtonClick( object Sender, RoutedEventArgs E )
        {
            TimeSlider.Value = 0;
        }

        private void OnTimeSliderValueChanged( object Sender, RoutedPropertyChangedEventArgs<double> E )
        {
            if ( !player.GetComponent<MediaComponent>( ).HasMediaBeenLoaded )
                return;

            if ( ChangedExternally )
            {
                ChangedExternally = false;
                return;
            }

            if ( !SliderTimer.IsEnabled )
                player.GetComponent<TimeComponent>( ).StartChangingPosition( );

            // Reset the timer.
            SliderTimer.Start( );
        }

        private void OnTimeSliderDragCompleted( object Sender, DragCompletedEventArgs E )
        {
            if ( !SliderTimer.IsEnabled ) return;

            SliderTimer.Stop( );

            PerformTimeSlide( );
        }

        private void OnSettingsButtonClicked( object Sender, RoutedEventArgs E )
        {
            SettingsFlyout.IsOpen = true;
        }

        private void OnFileButtonClicked( object Sender, RoutedEventArgs E )
        {
            FileFlyout.IsOpen = true;
        }

        private void OnLoadMediaButtonClick( object Sender, RoutedEventArgs E )
        {
            OpenFileDialog fileDialog = new OpenFileDialog( );
            bool? res = fileDialog.ShowDialog( this );

            if ( !res.Value )
                return;

            player.GetComponent<StateComponent>( ).Stop( );
            player.GetComponent<MediaComponent>( ).LoadMedia( fileDialog.FileName );
        }

        private void OnLoadSubtitlesButtonClick( object Sender, RoutedEventArgs E )
        {
            OpenFileDialog fileDialog = new OpenFileDialog { Filter = "SubRip (*.srt)|*.srt" };
            bool? res = fileDialog.ShowDialog( this );

            if ( !res.Value )
                return;

            player.GetComponent<SubtitlesComponent>( ).LoadSubtitles( fileDialog.FileName );
            FileFlyout.IsOpen = false;
        }

        #region Property Change
        /// <summary>
        /// Gets called whenever a property is changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Virtual method called on a change in properties.
        /// </summary>
        /// <param name="PropertyName">The name of the property.</param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged( [CallerMemberName] string PropertyName = null )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( PropertyName ) );
        }
        #endregion
    }
}
