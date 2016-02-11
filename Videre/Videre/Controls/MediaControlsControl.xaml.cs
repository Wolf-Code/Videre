using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
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
        private const string TimeFormat = @"hh\:mm\:ss";
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

        private readonly Thumb thumb;

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

            TimeShower.Visibility = Visibility.Hidden;

            TimeSlider.ApplyTemplate( );

            thumb = ( ( Track ) TimeSlider.Template.FindName( "PART_Track", TimeSlider ) ).Thumb;
            thumb.MouseEnter += ( sender, e ) =>
            {
                if ( e.LeftButton != MouseButtonState.Pressed || e.MouseDevice.Captured != null ) return;

                MouseButtonEventArgs args = new MouseButtonEventArgs( e.MouseDevice, e.Timestamp, MouseButton.Left ) { RoutedEvent = MouseLeftButtonDownEvent };
                ( ( Thumb ) sender ).RaiseEvent( args );
            };
        }

        /// <summary>
        /// Gets called whenever the player has been initialized.
        /// </summary>
        public override void OnPlayerInitialized( )
        {
            ViderePlayer.GetComponent<TimeComponent>( ).OnPositionChanged += OnOnPositionChanged;
            ViderePlayer.GetComponent<StateComponent>( ).OnStateChanged += OnOnStateChanged;

            MediaComponent media = ViderePlayer.GetComponent<MediaComponent>( );
            media.OnMediaLoaded += MediaOnOnMediaLoaded;
            media.OnMediaUnloaded += MediaOnOnMediaUnloaded;

            InputComponent inputComponent = ViderePlayer.GetComponent<InputComponent>( );
            inputComponent.OnShowControls += ( Sender, Args ) => this.Visibility = Visibility.Visible;
            inputComponent.OnHideControls += ( Sender, Args ) => this.Visibility = Visibility.Collapsed;
        }

        private void MediaOnOnMediaUnloaded( object Sender, OnMediaUnloadedEventArgs MediaUnloadedEventArgs )
        {
            this.TimeLabel_Total.Content = "--:--:--";
            this.TimeLabel_Current.Content = this.TimeLabel_Total.Content;
        }

        private void MediaOnOnMediaLoaded( object Sender, OnMediaLoadedEventArgs MediaLoadedEventArgs )
        {
            this.TimeLabel_Total.Content = MediaLoadedEventArgs.MediaFile.Duration.ToString( TimeFormat );
        }

        private void OnOnStateChanged( object Sender, OnStateChangedEventArgs StateChangedEventArgs )
        {
            this.IsPlaying = StateChangedEventArgs.State == StateComponent.PlayerState.Playing;

            if ( StateChangedEventArgs.State == StateComponent.PlayerState.Stopped )
                this.TimeSlider.Value = 0;
        }

        private void OnOnPositionChanged( object Sender, OnPositionChangedEventArgs PositionChangedEventArgs )
        {
            if ( double.IsNaN( PositionChangedEventArgs.Progress ) )
                return;

            changedExternally = true;
            this.TimeSlider.Value = PositionChangedEventArgs.Progress * this.TimeSlider.Maximum;
            this.TimeLabel_Current.Content = PositionChangedEventArgs.Position.ToString( TimeFormat );
        }

        private void PerformTimeSlide( )
        {
            TimeComponent timeHandler = ViderePlayer.GetComponent<TimeComponent>( );
            double progress = this.TimeSlider.Value / this.TimeSlider.Maximum;
            timeHandler.SetPosition( progress );

            SliderTimer.Stop( );
            timeHandler.StopChangingPosition( );
        }

        private void m_OnTimeSliderValueChanged( object Sender, RoutedPropertyChangedEventArgs<double> E )
        {
            if ( !ViderePlayer.GetComponent<MediaComponent>( ).HasMediaBeenLoaded )
                return;

            if ( changedExternally )
            {
                changedExternally = false;
                return;
            }

            if ( !SliderTimer.IsEnabled )
                ViderePlayer.GetComponent<TimeComponent>( ).StartChangingPosition( );

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
            if ( !ViderePlayer.GetComponent<MediaComponent>( ).HasMediaBeenLoaded )
                return;

            ViderePlayer.GetComponent<StateComponent>( ).ResumeOrPause( );

            OnPlayPauseButtonClick?.Invoke( this, E );
        }

        private void m_VolumeSlider_OnValueChanged( object Sender, RoutedPropertyChangedEventArgs<double> E )
        {
            if ( !IsPlayerInitialized )
                return;

            ViderePlayer.MediaPlayer.SetVolume( ( float ) E.NewValue );

            OnVolumeChanged?.Invoke( this, E );
        }

        private void ResizeTimeShower( )
        {
            TimeShower.Width = this.ActualWidth / 7;
            TimeShower.SetPointerWidth( TimeShower.ActualWidth / 10 );
        }

        private void TimeSlider_OnMouseMove( object Sender, MouseEventArgs E )
        {
            ResizeTimeShower( );

            // Getting the progress and finding the offset from the top left corner of the slider, relative to the canvas containing the notifier.
            double mouseFromLeftEdge = E.GetPosition( TimeSlider ).X;

            double barWidth = TimeSlider.ActualWidth - thumb.ActualWidth;
            double mousePos = Math.Min( Math.Max( E.GetPosition( TimeSlider ).X - thumb.ActualWidth / 2, 0 ), barWidth );
            double Progress = mousePos / barWidth;

            Point translated = TimeSlider.TranslatePoint( new Point( 0, 0 ), PopupContainer );

            // The half width of the popup.
            double halfWidth = TimeShower.ActualWidth / 2;
            double OffsetFromBorder = translated.X + mouseFromLeftEdge - halfWidth;

            double MaxRight = this.ActualWidth - TimeShower.ActualWidth + thumb.ActualWidth / 2;
            double PointerOffset = 0;
            if ( OffsetFromBorder < 0 )
                PointerOffset = OffsetFromBorder;
            else if ( OffsetFromBorder > MaxRight )
                PointerOffset = OffsetFromBorder - MaxRight;

            TimeSpan hoverTime = TimeSpan.FromTicks( ( long ) ( ViderePlayer.MediaPlayer.GetMediaLength( ).Ticks * Progress ) );
            TimeShower.TimeLabel.Content = hoverTime.ToString( TimeFormat );

            Canvas.SetLeft( TimeShower.Pointer, halfWidth + PointerOffset );
            Canvas.SetLeft( TimeShower, Math.Min( Math.Max( 0, OffsetFromBorder ), MaxRight ) );
            Canvas.SetTop( TimeShower, translated.Y - TimeShower.ActualHeight - TimeShower.Pointer.ActualHeight );
        }

        private void TimeSlider_OnMouseEnter( object Sender, MouseEventArgs E )
        {
            TimeShower.Visibility = Visibility.Visible;
            ResizeTimeShower( );
        }

        private void TimeSlider_OnMouseLeave( object Sender, MouseEventArgs E )
        {
            TimeShower.Visibility = Visibility.Hidden;
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