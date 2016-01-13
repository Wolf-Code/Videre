using System;
using VidereLib.EventArgs;

namespace VidereLib.Components
{
    public class StateComponent : ComponentBase
    {
        public event EventHandler<OnStateChangedEventArgs> OnStateChanged;

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

        /// <summary>
        /// The current <see cref="PlayerState"/> of the <see cref="ViderePlayer"/>.
        /// </summary>
        public PlayerState CurrentState
        {
            internal set
            {
                if ( m_CurrentState == value )
                    return;

                m_CurrentState = value;
                this.OnStateChanged?.Invoke( this, new OnStateChangedEventArgs( value ) );
            }
            get { return m_CurrentState; }
        }

        private PlayerState m_CurrentState = PlayerState.Stopped;

        public StateComponent( ViderePlayer player ) : base( player )
        {

        }


        /// <summary>
        /// Returns whether the player can be paused or not.
        /// </summary>
        /// <returns>True if the player can be paused, false otherwise.</returns>
        public bool CanPause( )
        {
            return Player.MediaPlayer.CanPause;
        }

        /// <summary>
        /// Stops the player and unloads the media.
        /// </summary>
        public void Stop( )
        {
            if ( CurrentState == PlayerState.Stopped )
                return;

            Pause( );
            Player.HasMediaBeenLoaded = false;
            Player.windowData.MediaPlayer.Source = null;
        }

        /// <summary>
        /// Players the currently loaded media.
        /// </summary>
        public void Play( )
        {
            if ( CurrentState == PlayerState.Playing )
                return;

            if ( !Player.HasMediaBeenLoaded )
                throw new Exception( "No media loaded." );

            Player.windowData.MediaPlayer.Play( );
            CurrentState = PlayerState.Playing;

            if ( Player.SubtitlesHandler.Subtitles.AnySubtitlesLeft( Player.windowData.MediaPlayer.Position ) )
                Player.SubtitlesHandler.CheckForSubtitles( );
        }

        /// <summary>
        /// Pauses the currently loaded media.
        /// </summary>
        public void Pause( )
        {
            if ( CurrentState == PlayerState.Paused )
                return;

            if ( !Player.HasMediaBeenLoaded )
                throw new Exception( "No media loaded." );

            if ( !CanPause( ) )
                throw new Exception( "Player can't be paused at this time." );

            switch ( CurrentState )
            {
                case PlayerState.Playing:
                    Player.MediaPlayer.Pause( );
                    break;

                default:
                    throw new Exception( $"Undefined actions for player state {CurrentState})" );
            }

            CurrentState = PlayerState.Paused;
            Player.SubtitlesHandler.subtitlesTimer.Stop( );
        }

        /// <summary>
        /// Resumes the player if it has been paused, or pauses the player if it's currently playing.
        /// </summary>
        public void ResumeOrPause( )
        {
            if ( !Player.HasMediaBeenLoaded )
                throw new Exception( "No media loaded." );

            switch ( CurrentState )
            {
                case PlayerState.Playing:
                    Pause( );
                    return;

                case PlayerState.Stopped:
                case PlayerState.Paused:
                    Play( );
                    return;

                default:
                    throw new Exception( $"Undefined actions for player state {CurrentState})" );
            }
        }
    }
}
