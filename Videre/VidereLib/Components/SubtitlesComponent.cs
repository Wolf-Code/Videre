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

        internal readonly DispatcherTimer subtitlesTimer;

        public SubtitlesComponent( ViderePlayer player ) : base( player )
        {
            subtitlesTimer = new DispatcherTimer( );
            subtitlesTimer.Tick += SubtitlesTimerOnTick;
        }


        internal void SubtitlesTimerOnTick( object Sender, System.EventArgs Args )
        {
            CheckForSubtitles( );
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
            if ( !Player.HasMediaBeenLoaded )
                throw new Exception( "Unable to load subtitles before any media has been loaded." );

            Subtitles = new Subtitles( filePath );
        }

        internal void CheckForSubtitles( )
        {
            TimeSpan currentPosition = Player.windowData.MediaPlayer.Position + subtitlesOffset;
            SubtitleData nextSubs = Subtitles.GetData( currentPosition );
            SubtitleData subs = nextSubs != null ? Subtitles.GetData( nextSubs.Index - 1 ) : Subtitles.GetData( Subtitles.Count - 1 );

            if ( currentPosition < subs.To && currentPosition >= subs.From )
            {
                this.OnSubtitlesChanged?.Invoke( this, new OnSubtitlesChangedEventArgs( subs ) );

                subtitlesTimer.Interval = subs.To - currentPosition;
            }
            else
            {
                this.OnSubtitlesChanged?.Invoke( this, new OnSubtitlesChangedEventArgs( SubtitleData.Empty ) );
                if ( nextSubs != null )
                    subtitlesTimer.Interval = nextSubs.From - currentPosition;
                else
                {
                    subtitlesTimer.Stop( );
                    return;
                }
            }

            subtitlesTimer.Start( );
        }
    }
}
