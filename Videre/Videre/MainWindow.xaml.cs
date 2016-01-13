using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using Videre.EventArgs;

namespace Videre
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private ViderePlayer player;
        private DispatcherTimer SliderTimer;
        private bool ChangedExternally;

        public MainWindow( )
        {
            InitializeComponent( );
        }

        protected override void OnInitialized( System.EventArgs e )
        {
            SliderTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds( 250 ) };
            SliderTimer.Tick += ( sender, args ) => PerformTimeSlide( );

            player = new ViderePlayer( mediaElement );
            player.LoadMedia( @"D:\Folders\Videos\Movies\Minions (2015)\Minions 2015 1080p BluRay x264 AC3-JYK.mkv" );
            player.OnPositionChanged += PlayerOnOnPositionChanged;

            base.OnInitialized( e );
        }

        private void PlayerOnOnPositionChanged( object Sender, OnPositionChangedEventArgs OnPositionChangedEventArgs )
        {
            ChangedExternally = true;
            TimeSlider.Value = OnPositionChangedEventArgs.Progress * TimeSlider.Maximum;
        }

        private void PerformTimeSlide( )
        {
            double progress = TimeSlider.Value / TimeSlider.Maximum;
            player.SetPosition( progress );

            SliderTimer.Stop( );
            player.StopChangingPosition( );
        }

        private void OnPlayPauseButtonClick( object Sender, RoutedEventArgs E )
        {
            player.ResumeOrPause( );
            switch ( player.CurrentState )
            {
                case ViderePlayer.PlayerState.Paused:
                    PlayPauseButton.Content = "Play";
                    break;

                case ViderePlayer.PlayerState.Playing:
                    PlayPauseButton.Content = "Pause";
                    break;
            }
        }

        private void OnForwardButtonClick( object Sender, RoutedEventArgs E )
        {
        }

        private void GetLength_Click( object Sender, RoutedEventArgs E )
        {
        }

        private void GetCurrentTime_Click( object Sender, RoutedEventArgs E )
        {
        }

        private void SetCurrentTime_Click( object Sender, RoutedEventArgs E )
        {
        }

        private void OnBackButtonClick( object Sender, RoutedEventArgs E )
        {
        }

        private void OnTimeSliderValueChanged( object Sender, RoutedPropertyChangedEventArgs<double> E )
        {
            if ( ChangedExternally )
            {
                ChangedExternally = false;
                return;
            }

            if ( !SliderTimer.IsEnabled )
                player.StartChangingPosition( );

            // Reset the timer.
            SliderTimer.Start( );
        }

        private void OnTimeSliderDragCompleted( object Sender, DragCompletedEventArgs E )
        {
            if ( !SliderTimer.IsEnabled ) return;

            SliderTimer.Stop( );

            PerformTimeSlide( );
        }
    }
}
