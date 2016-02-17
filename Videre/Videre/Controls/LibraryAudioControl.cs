using System;
using System.Windows;
using VidereLib.Data;

namespace Videre.Controls
{
    /// <summary>
    /// Interaction logic for LibraryMediaControl.xaml
    /// </summary>
    public class LibraryAudioControl : LibraryMediaControl
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public LibraryAudioControl( VidereMedia media ) : base( media )
        {

        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized( EventArgs e )
        {
            base.OnInitialized( e );
            AudioPlaceholder.Visibility = Visibility.Visible;

            Title.Text = media.Name;
            ToolTip = media.File.Name;

            this.FinishLoadingAudio( );
        }
    }
}