using System;
using VidereLib.EventArgs;

namespace VidereLib.Components
{
    /// <summary>
    /// The state component.
    /// </summary>
    public class StateComponent : ComponentBase
    {
        /// <summary>
        /// Gets called whenever the player changes state.
        /// </summary>
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

        /// <summary>
        /// Stops the player and unloads the media.
        /// </summary>
        public void Stop( )
        {
            if ( CurrentState == PlayerState.Stopped )
                return;

            ViderePlayer.MediaPlayer.Stop( );
            ViderePlayer.GetComponent<MediaComponent>( ).UnloadMedia( );
            this.CurrentState = PlayerState.Stopped;
        }

        /// <summary>
        /// Players the currently loaded media.
        /// </summary>
        public void Play( )
        {
            if ( CurrentState == PlayerState.Playing )
                return;

            if ( !ViderePlayer.GetComponent<MediaComponent>( ).HasMediaBeenLoaded )
                throw new Exception( "No media loaded." );

            ViderePlayer.windowData.MediaPlayer.Play( );
            CurrentState = PlayerState.Playing;
        }

        /// <summary>
        /// Pauses the currently loaded media.
        /// </summary>
        public void Pause( )
        {
            if ( CurrentState == PlayerState.Paused )
                return;

            if ( !ViderePlayer.GetComponent<MediaComponent>( ).HasMediaBeenLoaded )
                throw new Exception( "No media loaded." );

            switch ( CurrentState )
            {
                case PlayerState.Playing:
                    ViderePlayer.windowData.MediaPlayer.Pause( );
                    break;

                default:
                    throw new Exception( $"Undefined actions for player state {CurrentState})" );
            }

            CurrentState = PlayerState.Paused;
        }

        /// <summary>
        /// Resumes the player if it has been paused, or pauses the player if it's currently playing.
        /// </summary>
        public void ResumeOrPause( )
        {
            if ( !ViderePlayer.GetComponent<MediaComponent>( ).HasMediaBeenLoaded )
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
