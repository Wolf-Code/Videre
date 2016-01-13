using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using Videre.EventArgs;

namespace Videre
{
    public class WinApi
    {
        [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
        public static extern int GetSystemMetrics( int which );

        [DllImport("user32.dll")]
        public static extern void
            SetWindowPos( IntPtr hwnd, IntPtr hwndInsertAfter,
                         int X, int Y, int width, int height, uint flags );

        private const int SM_CXSCREEN = 0;
        private const int SM_CYSCREEN = 1;
        private static IntPtr HWND_TOP = IntPtr.Zero;
        private const int SWP_SHOWWINDOW = 64; // 0×0040

        public static int ScreenX
        {
            get { return GetSystemMetrics( SM_CXSCREEN ); }
        }

        public static int ScreenY
        {
            get { return GetSystemMetrics( SM_CYSCREEN ); }
        }

        public static void SetWinFullScreen( IntPtr hwnd )
        {
            SetWindowPos( hwnd, HWND_TOP, 0, 0, ScreenX, ScreenY, SWP_SHOWWINDOW );
        }
    }
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
        public PlayerState CurrentState
        {
            protected set
            {
                if ( m_CurrentState == value )
                    return;

                m_CurrentState = value;
                this.OnStateChanged?.Invoke( this, new OnStateChangedEventArgs( value ) );
            }
            get { return m_CurrentState; }
        }

        /// <summary>
        /// Whether or not the media is currently fullscreen.
        /// </summary>
        public bool IsFullScreen { private set; get; }

        /// <summary>
        /// True if media has been loaded, false otherwise.
        /// </summary>
        public bool HasMediaBeenLoaded { protected set; get; }

        private bool pausedWhenChangingPosition;

        private DispatcherTimer timeTimer;
        private readonly DispatcherTimer subtitlesTimer;

        public event EventHandler<OnPositionChangedEventArgs> OnPositionChanged;
        public event EventHandler<OnSubtitlesChangedEventArgs> OnSubtitlesChanged;
        public event EventHandler<OnStateChangedEventArgs> OnStateChanged; 
        private TimeSpan previousTimeSpan;

        private TimeSpan subtitlesOffset;
        private Subtitles subtitles;

        private readonly MainWindow window;
        private Rectangle oldBounds;
        private PlayerState m_CurrentState = PlayerState.Stopped;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="element">The <see cref="MediaElement"/> to use with this <see cref="ViderePlayer"/>.</param>
        public ViderePlayer( MainWindow window, MediaElement element )
        {
            this.mediaPlayer = element;
            this.timeTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds( 100 ) };
            this.timeTimer.Tick += TimeTimerOnTick;
            this.timeTimer.Start( );

            this.subtitlesTimer = new DispatcherTimer( );
            this.subtitlesTimer.Tick += SubtitlesTimerOnTick;

            this.window = window;
        }

        #region FullScreen

        /// <summary>
        /// Toggles the fullscreen state.
        /// </summary>
        public void ToggleFullScreen( )
        {
            this.SetFullScreen( !this.IsFullScreen );
        }

        /// <summary>
        /// Sets the window's fullscreen state.
        /// </summary>
        /// <param name="fullScreen">True for fullscreen, false otherwise.</param>
        public void SetFullScreen( bool fullScreen = true )
        {
            if ( fullScreen )
            {
                if ( !IsFullScreen )
                    oldBounds = new Rectangle( ( int ) this.window.Left, ( int ) this.window.Top, ( int ) this.window.Width, ( int ) this.window.Height );

                this.window.Height = Screen.PrimaryScreen.Bounds.Height;
                this.window.Width = Screen.PrimaryScreen.Bounds.Width;
                this.window.Topmost = true;
                this.window.Left = 0;
                this.window.Top = 0;
                this.window.ResizeMode = ResizeMode.NoResize;
                this.window.controlsGrid.Visibility = Visibility.Collapsed;
                this.window.ShowTitleBar = false;
                this.window.ShowCloseButton = false;
            }
            else
            {
                this.window.Topmost = false;
                this.window.ResizeMode = ResizeMode.CanResizeWithGrip;
                this.window.Left = oldBounds.X;
                this.window.Top = oldBounds.Y;
                this.window.Width = oldBounds.Width;
                this.window.Height = oldBounds.Height;
                this.window.controlsGrid.Visibility = Visibility.Visible;
                this.window.ShowTitleBar = true;
                this.window.ShowCloseButton = true;
            }

            IsFullScreen = fullScreen;
        }

        #endregion

        #region Subtitles

        private void SubtitlesTimerOnTick( object Sender, System.EventArgs Args )
        {
            CheckForSubtitles( );
        }

        /// <summary>
        /// The subtitles offset with which it is played.
        /// </summary>
        /// <param name="offset">The offset.</param>
        public void SetSubtitlesOffset( TimeSpan offset )
        {
            this.subtitlesOffset = offset;
        }

        public void LoadSubtitles( string filePath )
        {
            if ( !this.HasMediaBeenLoaded )
                throw new Exception( "Unable to load subtitles before any media has been loaded." );

            this.subtitles = new Subtitles( filePath );
        }

        private void CheckForSubtitles( )
        {
            TimeSpan currentPosition = mediaPlayer.Position + subtitlesOffset;
            SubtitleData nextSubs = subtitles.GetData( currentPosition );
            SubtitleData subs = nextSubs != null ? subtitles.GetData( nextSubs.Index - 1 ) : subtitles.GetData( subtitles.Count - 1 );

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

        #endregion

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

        private void TimeTimerOnTick( object Sender, System.EventArgs Args )
        {
            if ( !this.HasMediaBeenLoaded )
                return;

            if ( mediaPlayer.Position == previousTimeSpan ) return;

            double progress = ( double )mediaPlayer.Position.Ticks / mediaPlayer.NaturalDuration.TimeSpan.Ticks;
            this.OnPositionChanged?.Invoke( this, new OnPositionChangedEventArgs( mediaPlayer.Position, progress ) );
            this.previousTimeSpan = mediaPlayer.Position;
        }

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
            this.CheckForSubtitles( );
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

            if ( subtitles.AnySubtitlesLeft( mediaPlayer.Position ) )
                CheckForSubtitles( );
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
            subtitlesTimer.Stop( );
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
