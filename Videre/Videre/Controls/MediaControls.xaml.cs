using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Videre.Properties;
using VidereLib;
using VidereLib.Components;
using VidereLib.EventArgs;

namespace Videre.Controls
{
    /// <summary>
    /// Interaction logic for MediaControls.xaml
    /// </summary>
    public partial class MediaControls : INotifyPropertyChanged
    {
        private readonly DispatcherTimer SliderTimer;
        private bool changedExternally;

        private bool m_IsPlaying;

        /// <summary>
        /// Gets called whenever the time slider has changed value.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double> OnTimeSliderChanged;

        /// <summary>
        /// Gets called whenever the time slider has stopped being dragged.
        /// </summary>
        public event DragCompletedEventHandler OnTimeSliderDragCompleted;

        /// <summary>
        /// Gets called whenever the back button is clicked.
        /// </summary>
        public event RoutedEventHandler OnBackButtonClick;

        /// <summary>
        /// Gets called whenever the forward button is clicked.
        /// </summary>
        public event RoutedEventHandler OnForwardButtonClick;

        /// <summary>
        /// Gets called whenever the play/pause button is clicked.
        /// </summary>
        public event RoutedEventHandler OnPlayPauseButtonClick;

        /// <summary>
        /// Gets called whenever the volume is changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double> OnVolumeChanged;

        /// <summary>
        /// Whether or not the player is actually playing a video.
        /// </summary>
        public bool IsPlaying
        {
            set
            {
                m_IsPlaying = value;
                this.OnPropertyChanged( nameof( IsPlaying ) );
            }
            get { return m_IsPlaying; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MediaControls( )
        {
            InitializeComponent( );

            SliderTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds( 250 ) };
            SliderTimer.Tick += ( sender, args ) => PerformTimeSlide( );
        }

        /// <summary>
        /// Gets called whenever the player has been initialized.
        /// </summary>
        public override void OnPlayerInitialized( )
        {
            Player.GetComponent<TimeComponent>( ).OnPositionChanged += OnOnPositionChanged;
            Player.GetComponent<StateComponent>( ).OnStateChanged += OnOnStateChanged;

            InputComponent inputComponent = Player.GetComponent<InputComponent>( );
            inputComponent.OnShowControls += ( Sender, Args ) => this.Visibility = Visibility.Visible;
            inputComponent.OnHideControls += ( Sender, Args ) => this.Visibility = Visibility.Collapsed;
            Console.WriteLine("player init");
        }

        private void OnOnStateChanged( object Sender, OnStateChangedEventArgs StateChangedEventArgs )
        {
            this.IsPlaying = StateChangedEventArgs.State == StateComponent.PlayerState.Playing;

            if ( StateChangedEventArgs.State == StateComponent.PlayerState.Stopped )
                this.TimeSlider.Value = 0;
        }

        private void OnOnPositionChanged( object Sender, OnPositionChangedEventArgs PositionChangedEventArgs )
        {
            changedExternally = true;
            this.TimeSlider.Value = PositionChangedEventArgs.Progress * this.TimeSlider.Maximum;
            this.TimeLabel_Current.Content = PositionChangedEventArgs.Position.ToString( @"hh\:mm\:ss" );
        }

        private void PerformTimeSlide( )
        {
            TimeComponent timeHandler = Player.GetComponent<TimeComponent>( );
            double progress = this.TimeSlider.Value / this.TimeSlider.Maximum;
            timeHandler.SetPosition( progress );

            SliderTimer.Stop( );
            timeHandler.StopChangingPosition( );
        }

        private void m_OnTimeSliderValueChanged( object Sender, RoutedPropertyChangedEventArgs<double> E )
        {
            if ( !Player.GetComponent<MediaComponent>( ).HasMediaBeenLoaded )
                return;

            if ( changedExternally )
            {
                changedExternally = false;
                return;
            }

            if ( !SliderTimer.IsEnabled )
                Player.GetComponent<TimeComponent>( ).StartChangingPosition( );

            // Reset the timer.
            SliderTimer.Start( );

            OnTimeSliderChanged?.Invoke( this, E );
        }

        private void m_OnTimeSliderDragCompleted( object Sender, DragCompletedEventArgs E )
        {
            if ( !SliderTimer.IsEnabled ) return;

            SliderTimer.Stop( );

            PerformTimeSlide( );

            OnTimeSliderDragCompleted?.Invoke( this, E );
        }

        private void m_OnBackButtonClick( object Sender, RoutedEventArgs E )
        {
            this.TimeSlider.Value = 0;

            OnBackButtonClick?.Invoke( this, E );
        }

        private void m_OnForwardButtonClick( object Sender, RoutedEventArgs E )
        {
            OnForwardButtonClick?.Invoke( this, E );
        }

        private void m_OnPlayPauseButtonClick( object Sender, RoutedEventArgs E )
        {
            if ( !Player.GetComponent<MediaComponent>( ).HasMediaBeenLoaded )
                return;

            Player.GetComponent<StateComponent>( ).ResumeOrPause( );

            OnPlayPauseButtonClick?.Invoke( this, E );
        }

        private void m_VolumeSlider_OnValueChanged( object Sender, RoutedPropertyChangedEventArgs<double> E )
        {
            if ( !IsPlayerInitialized )
                return;

            Player.MediaPlayer.SetVolume( ( float ) E.NewValue );

            OnVolumeChanged?.Invoke( this, E );
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
