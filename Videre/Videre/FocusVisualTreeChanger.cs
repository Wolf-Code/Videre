using System.Windows;

namespace Videre
{
    public class FocusVisualTreeChanger
    {
        public static bool GetIsChanged( DependencyObject obj )
        {
            return ( bool )obj.GetValue( IsChangedProperty );
        }

        public static void SetIsChanged( DependencyObject obj, bool value )
        {
            obj.SetValue( IsChangedProperty, value );
        }

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
