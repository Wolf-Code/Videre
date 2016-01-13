using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using VidereLib.Components;
using VidereLib.EventArgs;

namespace VidereLib
{
    public class ViderePlayer
    {
        internal readonly MediaElement mediaPlayer;

        /// <summary>
        /// Whether or not the media is currently fullscreen.
        /// </summary>
        public bool IsFullScreen { internal set; get; }

        /// <summary>
        /// True if media has been loaded, false otherwise.
        /// </summary>
        public bool HasMediaBeenLoaded { internal set; get; }

        private bool pausedWhenChangingPosition;

        private DispatcherTimer timeTimer;
        internal readonly DispatcherTimer subtitlesTimer;

        public event EventHandler<OnPositionChangedEventArgs> OnPositionChanged;
        public event EventHandler<OnSubtitlesChangedEventArgs> OnSubtitlesChanged;
        public event EventHandler<OnStateChangedEventArgs> OnStateChanged; 
        private TimeSpan previousTimeSpan;

        private TimeSpan subtitlesOffset;
        internal Subtitles subtitles;

        private readonly MetroWindow window;
        private Rectangle oldBounds;

        internal readonly WindowData windowData;

        internal readonly InputComponent inputComponent;
        internal readonly StateComponent stateComponent;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="window">The <see cref="MetroWindow"/> the controls are in.</param>
        /// <param name="element">The <see cref="MediaElement"/> to use with this <see cref="ViderePlayer"/>.</param>
        public ViderePlayer( WindowData data )
        {
            this.windowData = data;

            this.mediaPlayer = data.MediaPlayer;
            this.timeTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds( 100 ) };
            this.timeTimer.Tick += TimeTimerOnTick;
            this.timeTimer.Start( );

            this.subtitlesTimer = new DispatcherTimer( );
            this.subtitlesTimer.Tick += SubtitlesTimerOnTick;

            this.window = data.Window;

            this.inputComponent = new InputComponent( this, window );
            this.stateComponent = new StateComponent( this, data.MediaPlayer );
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

                this.window.Height = SystemParameters.PrimaryScreenHeight;
                this.window.Width = SystemParameters.PrimaryScreenWidth;
                this.window.Topmost = true;
                this.window.Left = 0;
                this.window.Top = 0;
                this.window.ResizeMode = ResizeMode.NoResize;
                this.windowData.ControlsGrid.Visibility = Visibility.Collapsed;
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
                this.windowData.ControlsGrid.Visibility = Visibility.Visible;
                this.window.ShowTitleBar = true;
                this.window.ShowCloseButton = true;
            }

            IsFullScreen = fullScreen;
        }

        #endregion

        #region Subtitles

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
            this.subtitlesOffset = offset;
        }

        public void LoadSubtitles( string filePath )
        {
            if ( !this.HasMediaBeenLoaded )
                throw new Exception( "Unable to load subtitles before any media has been loaded." );

            this.subtitles = new Subtitles( filePath );
        }

        internal void CheckForSubtitles( )
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
            if ( this.stateComponent.CurrentState != StateComponent.PlayerState.Stopped )
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
            pausedWhenChangingPosition = this.stateComponent.CurrentState == StateComponent.PlayerState.Paused;
            this.Pause( );
            this.stateComponent.CurrentState = StateComponent.PlayerState.ChangingPosition;
        }

        /// <summary>
        /// Stops the changing of the position.
        /// </summary>
        public void StopChangingPosition( )
        {
            if ( this.stateComponent.CurrentState != StateComponent.PlayerState.ChangingPosition )
                throw new Exception( "Can't stop changing the position if not currently changing the position." );

            if ( !pausedWhenChangingPosition )
                this.Play( );
            else
                this.stateComponent.CurrentState = StateComponent.PlayerState.Paused;
        }

        #endregion

        #region States

        internal void StateChanged( StateComponent.PlayerState newState )
        {
            this.OnStateChanged?.Invoke( this, new OnStateChangedEventArgs( newState ) );
        }

        /// <summary>
        /// Returns whether the player can be paused or not.
        /// </summary>
        /// <returns>True if the player can be paused, false otherwise.</returns>
        public bool CanPause( )
        {
            return stateComponent.CanPause( );
        }

        /// <summary>
        /// Stops the player and unloads the media.
        /// </summary>
        public void Stop( )
        {
            this.stateComponent.Stop( );
        }

        /// <summary>
        /// Players the currently loaded media.
        /// </summary>
        public void Play( )
        {
            this.stateComponent.Play( );
        }

        /// <summary>
        /// Pauses the currently loaded media.
        /// </summary>
        public void Pause( )
        {
            this.stateComponent.Pause( );
        }

        /// <summary>
        /// Resumes the player if it has been paused, or pauses the player if it's currently playing.
        /// </summary>
        public void ResumeOrPause( )
        {
            this.stateComponent.ResumeOrPause( );
        }

        #endregion
    }
}
