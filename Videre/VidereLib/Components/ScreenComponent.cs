
using System.Drawing;
using System.Windows;

namespace VidereLib.Components
{
    public class ScreenComponent : ComponentBase
    {        /// <summary>
             /// Whether or not the media is currently fullscreen.
             /// </summary>
        public bool IsFullScreen { internal set; get; }

        private Rectangle oldBounds;

        public ScreenComponent( ViderePlayer player ) : base( player )
        {
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
            if ( fullScreen && !IsFullScreen )
                oldBounds = new Rectangle( ( int ) Player.windowData.Window.Left, ( int ) Player.windowData.Window.Top, ( int ) Player.windowData.Window.Width, ( int ) Player.windowData.Window.Height );

            Player.windowData.Window.Height = fullScreen ? SystemParameters.PrimaryScreenHeight : oldBounds.Height;
            Player.windowData.Window.Width = fullScreen ? SystemParameters.PrimaryScreenWidth : oldBounds.Width;
            Player.windowData.Window.Topmost = fullScreen;
            Player.windowData.Window.Left = fullScreen ? 0 : oldBounds.X;
            Player.windowData.Window.Top = fullScreen ? 0 : oldBounds.Y;
            Player.windowData.Window.ResizeMode = fullScreen ? ResizeMode.NoResize : ResizeMode.CanResize;
            Player.windowData.ControlsGrid.Visibility = fullScreen ? Visibility.Collapsed : Visibility.Visible;
            Player.windowData.Window.ShowTitleBar = !fullScreen;
            Player.windowData.Window.ShowCloseButton = !fullScreen;

            IsFullScreen = fullScreen;
        }
    }
}
