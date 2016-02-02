using System;
using System.Windows.Controls;
using MahApps.Metro;

namespace Videre.Controls
{
    /// <summary>
    /// Interaction logic for VidereSettingsControl.xaml
    /// </summary>
    public partial class VidereSettingsControl
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        public VidereSettingsControl( )
        {
            InitializeComponent( );
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized( EventArgs e )
        {
            base.OnInitialized( e );

            foreach ( AppTheme theme in ThemeManager.AppThemes )
                ThemeComboBox.Items.Add( theme.Name );

            ThemeComboBox.SelectedItem = Settings.Default.VidereTheme;

            foreach ( Accent accent in ThemeManager.Accents )
                AccentComboBox.Items.Add( accent.Name );

            AccentComboBox.SelectedItem = Settings.Default.VidereAccent;
        }

        private void AccentComboBox_OnSelectionChanged( object Sender, SelectionChangedEventArgs E )
        {
            Settings.Default.VidereAccent = E.AddedItems[ 0 ].ToString( );
            Settings.Default.Save( );
        }

        private void ThemeComboBox_OnSelectionChanged( object Sender, SelectionChangedEventArgs E )
        {
            Settings.Default.VidereTheme = E.AddedItems[ 0 ].ToString( );
            Settings.Default.Save( );
        }
    }
}
