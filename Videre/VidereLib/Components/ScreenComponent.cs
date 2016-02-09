using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;
using VidereLib.EventArgs;

namespace VidereLib.Components
{
    /// <summary>
    /// The screen component.
    /// </summary>
    public class ScreenComponent : ComponentBase
    {
        private readonly DispatcherTimer displayTimer;
        private const int StandbyResetInterval = 60;

        [Flags]
        enum EXECUTION_STATE : uint
        {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001
        }
        [DllImport( "kernel32.dll", CharSet = CharSet.Auto, SetLastError = true )]
        private static extern EXECUTION_STATE SetThreadExecutionState( EXECUTION_STATE esFlags );

        /// <summary>
        /// Whether or not the media is currently fullscreen.
        /// </summary>
        public bool IsFullScreen { private set; get; }

        private WindowState oldState;

        private WindowStyle oldStyle;

        /// <summary>
        /// Gets called whenever the screen switches between fullscreen or non-fullscreen.
        /// </summary>
        public event EventHandler<OnFullscreenChangedEventArgs> OnFullscreenChanged; 

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="player">the <see cref="ViderePlayer"/>.</param>
        public ScreenComponent( ViderePlayer player ) : base( player )
        {
            displayTimer = new DispatcherTimer( TimeSpan.FromSeconds( StandbyResetInterval ), DispatcherPriority.Input, ( Sender, Args ) => ResetStandbyTimers( ), Dispatcher.CurrentDispatcher );
            displayTimer.Stop( );
        }
        
        /// <summary>
        /// Toggles the fullscreen state.
        /// </summary>
        public void ToggleFullScreen( )
        {
            SetFullScreen( !IsFullScreen );
        }

        /// <summary>
        /// Sets the window's fullscreen state.
        /// </summary>
        /// <param name="fullScreen">True for fullscreen, false otherwise.</param>
        public void SetFullScreen( bool fullScreen = true )
        {
            if ( fullScreen == IsFullScreen )
                return;

            if ( fullScreen && !IsFullScreen )
            {
                oldState = Player.windowData.Window.WindowState;
                oldStyle = Player.windowData.Window.WindowStyle;
            }

            Player.windowData.Window.ResizeMode = fullScreen ? ResizeMode.NoResize : ResizeMode.CanResize;
            Player.windowData.Window.ShowTitleBar = !fullScreen;
            Player.windowData.Window.ShowCloseButton = !fullScreen;
            Player.windowData.Window.WindowStyle = fullScreen ? WindowStyle.None : oldStyle;
            Player.windowData.Window.WindowState = fullScreen ? WindowState.Maximized : oldState;

            IsFullScreen = fullScreen;
            OnFullscreenChanged?.Invoke( this, new OnFullscreenChangedEventArgs( IsFullScreen ) );
        }

        /// <summary>
        /// Disables the computer from entering sleep mode and turning the display off.
        /// </summary>
        public void DisableSleeping( )
        {
            if ( displayTimer.IsEnabled ) return;

            ResetStandbyTimers( );

            displayTimer.Start( );
        }

        /// <summary>
        /// Enables the computer to enter sleep mode and turn the display off.
        /// </summary>
        public void EnableSleeping( )
        {
            if ( !displayTimer.IsEnabled ) return;

            displayTimer.Stop( );
            if ( SetThreadExecutionState( EXECUTION_STATE.ES_CONTINUOUS ) == 0 )
                throw new Win32Exception( );
        }

        private void ResetStandbyTimers( )
        {
            if ( SetThreadExecutionState( EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_SYSTEM_REQUIRED ) == 0 )
                throw new Win32Exception( );
        }
    }
}
