using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
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

        public bool IsPlaying { private set; get; }

        public MainWindow( )
        {
            InitializeComponent( );
        }

        protected override void OnInitialized( EventArgs e )
        {
            SliderTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds( 250 ) };
            SliderTimer.Tick += ( sender, args ) => PerformTimeSlide( );

            player = new ViderePlayer( new WindowData { Window = this, ControlsGrid = controlsGrid, MediaPlayer = mediaElement } );
            player.LoadMedia( @"D:\Folders\Videos\Movies\Minions (2015)\Minions 2015 1080p BluRay x264 AC3-JYK.mkv" );
            player.SubtitlesHandler.LoadSubtitles( @"D:\Folders\Videos\Movies\Minions (2015)\Subs\English.srt" );
            player.TimeHandler.OnPositionChanged += PlayerOnOnPositionChanged;
            player.SubtitlesHandler.OnSubtitlesChanged += PlayerOnOnSubtitlesChanged;
            player.StateHandler.OnStateChanged += ( Sender, Args ) =>
            {
                IsPlaying = Args.State == StateComponent.PlayerState.Playing;
                this.OnPropertyChanged( nameof( IsPlaying ) );
            };

            base.OnInitialized( e );
        }

        private void PlayerOnOnSubtitlesChanged( object Sender, OnSubtitlesChangedEventArgs SubtitlesChangedEventArgs )
        {
            subtitleLabel.Inlines.Clear( );

            foreach ( string S in SubtitlesChangedEventArgs.Subtitles.Lines )
                subtitleLabel.Inlines.Add( S );
        }

        private void PlayerOnOnPositionChanged( object Sender, OnPositionChangedEventArgs OnPositionChangedEventArgs )
        {
            ChangedExternally = true;
            TimeSlider.Value = OnPositionChangedEventArgs.Progress * TimeSlider.Maximum;
        }

        private void PerformTimeSlide( )
        {
            double progress = TimeSlider.Value / TimeSlider.Maximum;
            player.TimeHandler.SetPosition( progress );

            SliderTimer.Stop( );
            player.TimeHandler.StopChangingPosition( );
        }

        private void OnPlayPauseButtonClick( object Sender, RoutedEventArgs E )
        {
            player.StateHandler.ResumeOrPause( );
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
            if ( ChangedExternally )
            {
                ChangedExternally = false;
                return;
            }

            if ( !SliderTimer.IsEnabled )
                player.TimeHandler.StartChangingPosition( );

            // Reset the timer.
            SliderTimer.Start( );
        }

        private void OnTimeSliderDragCompleted( object Sender, DragCompletedEventArgs E )
        {
            if ( !SliderTimer.IsEnabled ) return;

            SliderTimer.Stop( );

            PerformTimeSlide( );
        }

        private void OnVideoMouseDown( object Sender, MouseButtonEventArgs E )
        {
            if ( E.ClickCount == 2 && E.ChangedButton == MouseButton.Left )
                player.ScreenHandler.ToggleFullScreen( );
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged( [CallerMemberName] string PropertyName = null )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( PropertyName ) );
        }
    }
}
