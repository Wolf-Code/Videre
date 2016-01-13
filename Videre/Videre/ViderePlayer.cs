using System;
using System.Windows.Controls;
using System.Windows.Threading;
using Videre.EventArgs;

namespace Videre
{
    internal class ViderePlayer
    {
        /// <summary>
        /// The different states the player can have.
        /// </summary>
        public enum PlayerState
        {
            /// <summary>
            /// The state when the player has been paused.
            /// </summary>
            Paused,

            /// <summary>
            /// The state when the player is currently playing.
            /// </summary>
            Playing,

            /// <summary>
            /// The state when the player has been stopped.
            /// </summary>
            Stopped,

            /// <summary>
            /// The state when the position in the media is currently being changed.
            /// </summary>
            ChangingPosition
        }
        private readonly MediaElement mediaPlayer;

        /// <summary>
        /// The current <see cref="PlayerState"/> of the <see cref="ViderePlayer"/>.
        /// </summary>
        public PlayerState CurrentState { protected set; get; } = PlayerState.Stopped;

        /// <summary>
        /// True if media has been loaded, false otherwise.
        /// </summary>
        public bool HasMediaBeenLoaded { protected set; get; }

        private bool pausedWhenChangingPosition;

        private DispatcherTimer timeTimer;

        public event EventHandler<OnPositionChangedEventArgs> OnPositionChanged;
        private TimeSpan previousTimeSpan;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="element">The <see cref="MediaElement"/> to use with this <see cref="ViderePlayer"/>.</param>
        public ViderePlayer( MediaElement element )
        {
            this.mediaPlayer = element;
            this.timeTimer = new DispatcherTimer( DispatcherPriority.Normal ) { Interval = TimeSpan.FromMilliseconds( 100 ) };
            this.timeTimer.Tick += TimeTimerOnTick;
            this.timeTimer.Start( );
        }

        private void TimeTimerOnTick( object Sender, System.EventArgs Args )
        {
            if ( !this.HasMediaBeenLoaded )
                return;

            if ( mediaPlayer.Position == previousTimeSpan ) return;

            double progress = ( double ) mediaPlayer.Position.Ticks / mediaPlayer.NaturalDuration.TimeSpan.Ticks;
            this.OnPositionChanged?.Invoke( this, new OnPositionChangedEventArgs( mediaPlayer.Position, progress ) );
            this.previousTimeSpan = mediaPlayer.Position;
        }

        #region Media

        /// <summary>
        /// Loads media from a path.
        /// </summary>
        /// <param name="Path">The path of the media.</param>
        public void LoadMedia( string Path )
        {
            if ( this.CurrentState != PlayerState.Stopped )
                throw new Exception( "Attempting to load media while playing." );

            mediaPlayer.Source = new Uri( Path );
            this.HasMediaBeenLoaded = true;
        }

        #endregion

        #region Timing

        /// <summary>
        /// Sets the position in the media to play at.
        /// </summary>
        /// <param name="Progress">The float containing the progress of the media, 0 being the start and 1 being the end.</param>
        public void SetPosition( double Progress )
        {
            if ( !this.HasMediaBeenLoaded )
                throw new Exception( "No media loaded." );

            if ( Progress < 0 )
                Progress = 0;

            if ( Progress > 1 )
                Progress = 1;

            TimeSpan duration = mediaPlayer.NaturalDuration.TimeSpan;
            TimeSpan newPositon = new TimeSpan( ( long ) ( duration.Ticks * Progress ) );

            this.SetPosition( newPositon );
        }

        /// <summary>
        /// Sets the position in the media to play at.
        /// </summary>
        /// <param name="Span">The <see cref="TimeSpan"/> to set the player to.</param>
        public void SetPosition( TimeSpan Span )
        {
            if ( !this.HasMediaBeenLoaded )
                throw new Exception( "No media loaded." );

            mediaPlayer.Position = Span;
        }

        /// <summary>
        /// Starts the changing of the position.
        /// </summary>
        public void StartChangingPosition( )
        {
            pausedWhenChangingPosition = this.CurrentState == PlayerState.Paused;
            this.Pause( );
            this.CurrentState = PlayerState.ChangingPosition;
        }

        /// <summary>
        /// Stops the changing of the position.
        /// </summary>
        public void StopChangingPosition( )
        {
            if ( this.CurrentState != PlayerState.ChangingPosition )
                throw new Exception( "Can't stop changing the position if not currently changing the position." );

            if ( !pausedWhenChangingPosition )
                this.Play( );
            else
                this.CurrentState = PlayerState.Paused;
        }

        #endregion

        #region States

        /// <summary>
        /// Returns whether the player can be paused or not.
        /// </summary>
        /// <returns>True if the player can be paused, false otherwise.</returns>
        public bool CanPause( )
        {
            return mediaPlayer.CanPause;
        }

        /// <summary>
        /// Stops the player and unloads the media.
        /// </summary>
        public void Stop( )
        {
            if ( this.CurrentState == PlayerState.Stopped )
                return;

            this.Pause( );
            this.HasMediaBeenLoaded = false;
            mediaPlayer.Source = null;
        }

        /// <summary>
        /// Players the currently loaded media.
        /// </summary>
        public void Play( )
        {
            if ( this.CurrentState == PlayerState.Playing )
                return;

            if ( !this.HasMediaBeenLoaded )
                throw new Exception( "No media loaded." );

            mediaPlayer.Play( );
            this.CurrentState = PlayerState.Playing;
        }

        /// <summary>
        /// Pauses the currently loaded media.
        /// </summary>
        public void Pause( )
        {
            if ( this.CurrentState == PlayerState.Paused )
                return;

            if ( !this.HasMediaBeenLoaded )
                throw new Exception( "No media loaded." );

            if ( !this.CanPause( ) )
                throw new Exception( "Player can't be paused at this time." );

            switch ( this.CurrentState )
            {
                case PlayerState.Playing:
                    mediaPlayer.Pause( );
                    break;

                default:
                    throw new Exception( $"Undefined actions for player state {this.CurrentState})" );
            }

            this.CurrentState = PlayerState.Paused;
        }

        /// <summary>
        /// Resumes the player if it has been paused, or pauses the player if it's currently playing.
        /// </summary>
        public void ResumeOrPause( )
        {
            if ( !this.HasMediaBeenLoaded )
                throw new Exception( "No media loaded." );

            switch ( this.CurrentState )
            {
                case PlayerState.Playing:
                    this.Pause( );
                    return;

                case PlayerState.Stopped:
                case PlayerState.Paused:
                    this.Play( );
                    return;

                default:
                    throw new Exception( $"Undefined actions for player state {this.CurrentState})" );
            }
        }

        #endregion
    }
}
