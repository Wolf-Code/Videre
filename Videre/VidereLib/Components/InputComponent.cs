using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using VidereLib.EventArgs;

namespace VidereLib.Components
{
    /// <summary>
    /// The input component.
    /// </summary>
    public class InputComponent : ComponentBase
    {
        private DispatcherTimer hideControlsTimer;

        /// <summary>
        /// True if controls are currently hidden, false otherwise.
        /// </summary>
        public bool AreControlsHidden { private set; get; }

        /// <summary>
        /// Called whenever the events should be shown.
        /// </summary>
        public event EventHandler<OnShowControlsEventArgs> OnShowControls;

        /// <summary>
        /// Called whenever the events should be hidden.
        /// </summary>
        public event EventHandler<OnHideControlsEventArgs> OnHideControls;

        private Point lastCursorPos;

        /// <summary>
        /// Gets called after all components have been added to the player.
        /// </summary>
        protected override void OnInitialize( )
        {
            hideControlsTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds( 1500 ) };
            hideControlsTimer.Tick += HideControlsTimerOnTick;

            ViderePlayer.windowData.MediaArea.MouseLeave += MediaPlayerOnMouseLeave;
            ViderePlayer.windowData.MediaArea.MouseEnter += MediaAreaOnMouseEnter;

            ViderePlayer.windowData.MediaArea.MouseDown += MediaPlayerOnMouseDown;
            ViderePlayer.windowData.MediaArea.MouseMove += MediaPlayerOnMouseMove;
        }

        private void MediaAreaOnMouseEnter( object Sender, MouseEventArgs Args )
        {
            this.ShowCursorAndResetTimer( Args );
        }

        private void HideControlsTimerOnTick( object Sender, System.EventArgs Args )
        {
            hideControlsTimer.Stop( );

            if ( !ViderePlayer.windowData.MediaArea.IsMouseOver )
                return;

            if ( !ViderePlayer.GetComponent<ScreenComponent>( ).IsFullScreen )
                return;

            Mouse.OverrideCursor = Cursors.None;
            OnHideControls?.Invoke( this, new OnHideControlsEventArgs( ) );
            AreControlsHidden = true;
        }

        private void MediaPlayerOnMouseLeave( object Sender, MouseEventArgs Args )
        {
            this.ShowCursorAndResetTimer( Args );
        }

        private void MediaPlayerOnMouseMove( object Sender, MouseEventArgs Args )
        {
            if ( Args.GetPosition( ViderePlayer.windowData.Window ) == lastCursorPos )
                return;

            this.ShowCursorAndResetTimer( Args );
        }

        private void ShowCursorAndResetTimer( MouseEventArgs Args )
        {
            if ( AreControlsHidden )
            {
                OnShowControls?.Invoke( this, new OnShowControlsEventArgs( ) );
                Mouse.OverrideCursor = Cursors.Arrow;
                AreControlsHidden = false;
            }

            // Resets the timer.
            hideControlsTimer.Stop( );
            hideControlsTimer.Start( );
            lastCursorPos = Args.GetPosition( ViderePlayer.windowData.Window );
        }

        private void MediaPlayerOnMouseDown( object Sender, MouseButtonEventArgs MouseButtonEventArgs )
        {
            if ( MouseButtonEventArgs.ClickCount == 2 && MouseButtonEventArgs.ChangedButton == MouseButton.Left )
                ViderePlayer.GetComponent<ScreenComponent>( ).ToggleFullScreen( );
        }
    }
}