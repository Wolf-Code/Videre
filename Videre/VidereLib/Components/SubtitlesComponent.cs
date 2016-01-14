using System;
using System.Windows.Threading;
using VidereLib.EventArgs;

namespace VidereLib.Components
{
    public class SubtitlesComponent : ComponentBase
    {
        public event EventHandler<OnSubtitlesChangedEventArgs> OnSubtitlesChanged;

        public Subtitles Subtitles { private set; get; }
        private TimeSpan subtitlesOffset;

        private DispatcherTimer subtitlesTimer;

        /// <summary>
        /// Returns whether or not subtitles have been loaded.
        /// </summary>
        public bool HaveSubtitlesBeenLoaded => Subtitles != null;

        public SubtitlesComponent( ViderePlayer player ) : base( player )
        {

        }

        protected override void OnInitialize( )
        {
            subtitlesTimer = new DispatcherTimer( );
            subtitlesTimer.Tick += SubtitlesTimerOnTick;

            Player.GetComponent<StateComponent>( ).OnStateChanged += StateHandlerOnOnStateChanged;
        }

        private void StateHandlerOnOnStateChanged( object Sender, OnStateChangedEventArgs StateChangedEventArgs )
        {
            switch ( StateChangedEventArgs.State )
            {
                case StateComponent.PlayerState.Paused:
                    this.subtitlesTimer.Stop( );
                    break;

                case StateComponent.PlayerState.Playing:
                    this.CheckForSubtitles( );
                    break;

                case StateComponent.PlayerState.Stopped:
                    this.StopSubtitles( );
                    break;
            }
        }

        internal void SubtitlesTimerOnTick( object Sender, System.EventArgs Args )
        {
            CheckForSubtitles( );
        }

        private void StopSubtitles( )
        {
            this.OnSubtitlesChanged?.Invoke( this, new OnSubtitlesChangedEventArgs( SubtitleData.Empty ) );
            this.subtitlesTimer.Stop( );
        }

        /// <summary>
        /// The subtitles offset with which it is played.
        /// </summary>
        /// <param name="offset">The offset.</param>
        public void SetSubtitlesOffset( TimeSpan offset )
        {
            subtitlesOffset = offset;
        }

        public void LoadSubtitles( string filePath )
        {
            if ( !Player.GetComponent<MediaComponent>( ).HasMediaBeenLoaded )
                throw new Exception( "Unable to load subtitles before any media has been loaded." );

            Subtitles = new Subtitles( filePath );
            this.CheckForSubtitles( );
        }

        internal void CheckForSubtitles( )
        {
            if ( !HaveSubtitlesBeenLoaded )
                return;

            TimeSpan position = Player.windowData.MediaPlayer.Position;
            TimeSpan currentPosition = position + subtitlesOffset;
            if ( !Subtitles.AnySubtitlesLeft( currentPosition ) )
                return;

            SubtitleData nextSubs = Subtitles.GetData( currentPosition );
            SubtitleData subs = nextSubs != null ? Subtitles.GetData( nextSubs.Index - 1 ) : Subtitles.GetData( Subtitles.Count - 1 );

            // If we're in the sub interval.
            if ( currentPosition < subs.To && currentPosition >= subs.From )
            {
                this.OnSubtitlesChanged?.Invoke( this, new OnSubtitlesChangedEventArgs( subs ) );

                subtitlesTimer.Interval = subs.To - currentPosition;
            }
            // If we're not in a sub interval
            else
            {
                // Stop the showing of subtitles.
                this.StopSubtitles( );

                // If there are still subs left, change the interval to run when they need to be shown.
                if ( nextSubs != null )
                    subtitlesTimer.Interval = nextSubs.From - currentPosition;
                // Otherwise, don't bother starting the timer again.
                else
                    return;
            }

            subtitlesTimer.Start( );
        }
    }
}
