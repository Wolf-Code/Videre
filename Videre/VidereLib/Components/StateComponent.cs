using System;
using System.Windows.Controls;
using VidereLib.EventArgs;

namespace VidereLib.Components
{
    public class StateComponent : ComponentBase
    {
        private MediaElement mediaElement;

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
                Player.StateChanged( value );
            }
            get { return m_CurrentState; }
        }

        private PlayerState m_CurrentState = PlayerState.Stopped;

        public StateComponent( ViderePlayer player, MediaElement element ) : base( player )
        {
            this.mediaElement = element;
        }


        /// <summary>
        /// Returns whether the player can be paused or not.
        /// </summary>
        /// <returns>True if the player can be paused, false otherwise.</returns>
        internal bool CanPause( )
        {
            return Player.mediaPlayer.CanPause;
        }

        /// <summary>
        /// Stops the player and unloads the media.
        /// </summary>
        internal void Stop( )
        {
            if ( this.CurrentState == StateComponent.PlayerState.Stopped )
                return;

            this.Pause( );
            this.Player.HasMediaBeenLoaded = false;
            this.mediaElement.Source = null;
        }

        /// <summary>
        /// Players the currently loaded media.
        /// </summary>
        internal void Play( )
        {
            if ( this.CurrentState == StateComponent.PlayerState.Playing )
                return;

            if ( !this.Player.HasMediaBeenLoaded )
                throw new Exception( "No media loaded." );

            this.mediaElement.Play( );
            this.CurrentState = StateComponent.PlayerState.Playing;

            if ( Player.subtitles.AnySubtitlesLeft( this.mediaElement.Position ) )
                Player.CheckForSubtitles( );
        }

        /// <summary>
        /// Pauses the currently loaded media.
        /// </summary>
        internal void Pause( )
        {
            if ( this.CurrentState == StateComponent.PlayerState.Paused )
                return;

            if ( !this.Player.HasMediaBeenLoaded )
                throw new Exception( "No media loaded." );

            if ( !this.CanPause( ) )
                throw new Exception( "Player can't be paused at this time." );

            switch ( this.CurrentState )
            {
                case StateComponent.PlayerState.Playing:
                    Player.mediaPlayer.Pause( );
                    break;

                default:
                    throw new Exception( $"Undefined actions for player state {this.CurrentState})" );
            }

            this.CurrentState = StateComponent.PlayerState.Paused;
            Player.subtitlesTimer.Stop( );
        }

        /// <summary>
        /// Resumes the player if it has been paused, or pauses the player if it's currently playing.
        /// </summary>
        internal void ResumeOrPause( )
        {
            if ( !this.Player.HasMediaBeenLoaded )
                throw new Exception( "No media loaded." );

            switch ( this.CurrentState )
            {
                case StateComponent.PlayerState.Playing:
                    this.Pause( );
                    return;

                case StateComponent.PlayerState.Stopped:
                case StateComponent.PlayerState.Paused:
                    this.Play( );
                    return;

                default:
                    throw new Exception( $"Undefined actions for player state {this.CurrentState})" );
            }
        }
    }
}
