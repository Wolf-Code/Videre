using System.Windows.Input;

namespace Videre.Controls
{
    /// <summary>
    /// Interaction logic for MediaControl.xaml
    /// </summary>
    public partial class MediaControl
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MediaControl( )
        {
            InitializeComponent( );
        }

        private void MediaPlayer_OnMouseRightButtonDown( object Sender, MouseButtonEventArgs E )
        {
            MediaPlayer.ContextMenu.IsOpen = true;
        }
    }
}
