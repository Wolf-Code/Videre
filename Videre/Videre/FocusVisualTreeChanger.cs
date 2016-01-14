using System.Windows;

namespace Videre
{
    /// <summary>
    /// Used to prevent the focus borders from being shown.
    /// </summary>
    public class FocusVisualTreeChanger
    {
        /// <summary>
        /// Setter for the XAML.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetIsChanged( DependencyObject obj )
        {
            return ( bool )obj.GetValue( IsChangedProperty );
        }

        /// <summary>
        /// Getter for the XAML.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetIsChanged( DependencyObject obj, bool value )
        {
            obj.SetValue( IsChangedProperty, value );
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for the XAML.
        /// </summary>
        public static readonly DependencyProperty IsChangedProperty =
            DependencyProperty.RegisterAttached( "IsChanged", typeof( bool ), typeof( FocusVisualTreeChanger ), new FrameworkPropertyMetadata( false, FrameworkPropertyMetadataOptions.Inherits, IsChangedCallback ) );

        private static void IsChangedCallback( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            if ( false.Equals( e.NewValue ) ) return;

            FrameworkContentElement contentElement = d as FrameworkContentElement;
            if ( contentElement != null )
            {
                contentElement.FocusVisualStyle = null;
                return;
            }

            FrameworkElement element = d as FrameworkElement;
            if ( element != null )
                element.FocusVisualStyle = null;
        }
    }
}
