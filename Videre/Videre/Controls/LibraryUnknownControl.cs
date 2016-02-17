using System;
using System.Windows;
using VidereLib.Data;

namespace Videre.Controls
{
    /// <summary>
    /// Interaction logic for LibraryMediaControl.xaml
    /// </summary>
    public class LibraryUnknownControl : LibraryMediaControl
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public LibraryUnknownControl( VidereMedia media ) : base( media )
        {

        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized( EventArgs e )
        {
            base.OnInitialized( e );

            UnknownPlaceHolder.Visibility = Visibility.Visible;
            LoadingRing.IsActive = false;
        }
    }
}