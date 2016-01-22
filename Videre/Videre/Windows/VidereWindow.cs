using System;
using System.Windows;
using MahApps.Metro.Controls;
using Videre.Controls;

namespace Videre.Windows
{
    /// <summary>
    /// The base class for all Videre windows.
    /// </summary>
    public class VidereWindow : MetroWindow
    {
        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized( EventArgs e )
        {
            LoopAndInitVidereChildren( this );
            OnVidereControlsInitialized( );

            base.OnInitialized( e );
        }

        /// <summary>
        /// Called whenever the <see cref="VidereControl"/> children have been intialized with a player.
        /// </summary>
        protected virtual void OnVidereControlsInitialized( )
        {

        }

        private static void LoopAndInitVidereChildren( DependencyObject parent )
        {
            foreach ( var child in parent.GetChildObjects( ) )
            {
                if ( child.GetType( ).IsSubclassOf( typeof( VidereControl ) ) )
                    ( ( VidereControl )child ).OnPlayerInitialized( );

                LoopAndInitVidereChildren( child );
            }
        }
    }
}
