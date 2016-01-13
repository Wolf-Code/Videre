using System;
using System.Windows.Threading;
using VidereLib.EventArgs;

namespace VidereLib.Components
{
    public class TimeComponent : ComponentBase
    {

        private bool pausedWhenChangingPosition;

        private DispatcherTimer timeTimer;

        public event EventHandler<OnPositionChangedEventArgs> OnPositionChanged;
        private TimeSpan previousTimeSpan;

        public TimeComponent( ViderePlayer player ) : base( player )
        {
            timeTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds( 100 ) };
            timeTimer.Tick += TimeTimerOnTick;
            timeTimer.Start( );
        }


        private void TimeTimerOnTick( object Sender, System.EventArgs Args )
        {
            if ( !Player.HasMediaBeenLoaded )
                return;

            if ( Player.MediaPlayer.Position == previousTimeSpan ) return;

            double progress = ( double )Player.MediaPlayer.Position.Ticks / Player.MediaPlayer.NaturalDuration.TimeSpan.Ticks;
            OnPositionChanged?.Invoke( this, new OnPositionChangedEventArgs( Player.MediaPlayer.Position, progress ) );
            previousTimeSpan = Player.MediaPlayer.Position;
        }

        /// <summary>
        /// Sets the position in the media to play at.
        /// </summary>
        /// <param name="Progress">The float containing the progress of the media, 0 being the start and 1 being the end.</param>
        public void SetPosition( double Progress )
        {
            if ( !Player.HasMediaBeenLoaded )
                throw new Exception( "No media loaded." );

            if ( Progress < 0 )
                Progress = 0;

            if ( Progress > 1 )
                Progress = 1;

            TimeSpan duration = Player.MediaPlayer.NaturalDuration.TimeSpan;
            TimeSpan newPositon = new TimeSpan( ( long )( duration.Ticks * Progress ) );

            SetPosition( newPositon );
            this.Player.SubtitlesHandler.CheckForSubtitles( );
        }

        /// <summary>
        /// Sets the position in the media to play at.
        /// </summary>
        /// <param name="Span">The <see cref="TimeSpan"/> to set the player to.</param>
        public void SetPosition( TimeSpan Span )
        {
            if ( !Player.HasMediaBeenLoaded )
                throw new Exception( "No media loaded." );

            Player.MediaPlayer.Position = Span;
        }

        /// <summary>
        /// Starts the changing of the position.
        /// </summary>
        public void StartChangingPosition( )
        {
            pausedWhenChangingPosition = this.Player.StateHandler.CurrentState == StateComponent.PlayerState.Paused;
            this.Player.StateHandler.Pause( );
            this.Player.StateHandler.CurrentState = StateComponent.PlayerState.ChangingPosition;
        }

        /// <summary>
        /// Stops the changing of the position.
        /// </summary>
        public void StopChangingPosition( )
        {
            if ( this.Player.StateHandler.CurrentState != StateComponent.PlayerState.ChangingPosition )
                throw new Exception( "Can't stop changing the position if not currently changing the position." );

            if ( !pausedWhenChangingPosition )
                this.Player.StateHandler.Play( );
            else
                this.Player.StateHandler.CurrentState = StateComponent.PlayerState.Paused;
        }

    }
}
