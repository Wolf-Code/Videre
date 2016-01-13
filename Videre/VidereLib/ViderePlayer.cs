using System;
using System.Windows.Controls;
using System.Windows.Threading;
using VidereLib.Components;
using VidereLib.EventArgs;

namespace VidereLib
{
    public class ViderePlayer
    {
        internal readonly MediaElement mediaPlayer;

        /// <summary>
        /// True if media has been loaded, false otherwise.
        /// </summary>
        public bool HasMediaBeenLoaded { internal set; get; }

        private bool pausedWhenChangingPosition;

        private DispatcherTimer timeTimer;

        public event EventHandler<OnPositionChangedEventArgs> OnPositionChanged;
        private TimeSpan previousTimeSpan;

        internal readonly WindowData windowData;

        public InputComponent InputHandler { private set; get; }
        public StateComponent StateHandler { private set; get; }
        public SubtitlesComponent SubtitlesHandler { private set; get; }
        public ScreenComponent ScreenHandler { private set; get; }

        internal MediaElement MediaPlayer => windowData.MediaPlayer;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ViderePlayer( WindowData data )
        {
            windowData = data;

            mediaPlayer = data.MediaPlayer;
            timeTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds( 100 ) };
            timeTimer.Tick += TimeTimerOnTick;
            timeTimer.Start( );

            InputHandler = new InputComponent( this );
            StateHandler = new StateComponent( this );
            SubtitlesHandler = new SubtitlesComponent( this );
            ScreenHandler = new ScreenComponent( this );
        }

        #region Media

        /// <summary>
        /// Loads media from a path.
        /// </summary>
        /// <param name="Path">The path of the media.</param>
        public void LoadMedia( string Path )
        {
            if ( this.StateHandler.CurrentState != StateComponent.PlayerState.Stopped )
                throw new Exception( "Attempting to load media while playing." );

            mediaPlayer.Source = new Uri( Path );
            HasMediaBeenLoaded = true;
        }

        #endregion

        #region Timing

        private void TimeTimerOnTick( object Sender, System.EventArgs Args )
        {
            if ( !HasMediaBeenLoaded )
                return;

            if ( mediaPlayer.Position == previousTimeSpan ) return;

            double progress = ( double )mediaPlayer.Position.Ticks / mediaPlayer.NaturalDuration.TimeSpan.Ticks;
            OnPositionChanged?.Invoke( this, new OnPositionChangedEventArgs( mediaPlayer.Position, progress ) );
            previousTimeSpan = mediaPlayer.Position;
        }

        /// <summary>
        /// Sets the position in the media to play at.
        /// </summary>
        /// <param name="Progress">The float containing the progress of the media, 0 being the start and 1 being the end.</param>
        public void SetPosition( double Progress )
        {
            if ( !HasMediaBeenLoaded )
                throw new Exception( "No media loaded." );

            if ( Progress < 0 )
                Progress = 0;

            if ( Progress > 1 )
                Progress = 1;

            TimeSpan duration = mediaPlayer.NaturalDuration.TimeSpan;
            TimeSpan newPositon = new TimeSpan( ( long ) ( duration.Ticks * Progress ) );

            SetPosition( newPositon );
            this.SubtitlesHandler.CheckForSubtitles( );
        }

        /// <summary>
        /// Sets the position in the media to play at.
        /// </summary>
        /// <param name="Span">The <see cref="TimeSpan"/> to set the player to.</param>
        public void SetPosition( TimeSpan Span )
        {
            if ( !HasMediaBeenLoaded )
                throw new Exception( "No media loaded." );

            mediaPlayer.Position = Span;
        }

        /// <summary>
        /// Starts the changing of the position.
        /// </summary>
        public void StartChangingPosition( )
        {
            pausedWhenChangingPosition = this.StateHandler.CurrentState == StateComponent.PlayerState.Paused;
            this.StateHandler.Pause( );
            this.StateHandler.CurrentState = StateComponent.PlayerState.ChangingPosition;
        }

        /// <summary>
        /// Stops the changing of the position.
        /// </summary>
        public void StopChangingPosition( )
        {
            if ( this.StateHandler.CurrentState != StateComponent.PlayerState.ChangingPosition )
                throw new Exception( "Can't stop changing the position if not currently changing the position." );

            if ( !pausedWhenChangingPosition )
                this.StateHandler.Play( );
            else
                this.StateHandler.CurrentState = StateComponent.PlayerState.Paused;
        }

        #endregion
    }
}
