using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro;
using VidereLib;

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

        private void SetFileAssociations( )
        {
            Process process = new Process( );

            Dictionary<string, string> procArgs = new Dictionary<string, string>
            {
                { "-executable", '\"' + System.Windows.Forms.Application.ExecutablePath + '\"' },
                { "-icon", '\"' + System.Windows.Forms.Application.StartupPath + "\\Videre.ico\"" },
                { "-progID", Settings.Default.ProgID },
                { "-videoExtensions", '\"' + string.Join( " ", ViderePlayer.MediaPlayer.VideoFileExtensions ) + '\"' },
            };
            string arguments = procArgs.Aggregate( string.Empty, ( Current, pair ) => Current + pair.Key + " " + pair.Value + " " ).Trim( );

            process.StartInfo.FileName = "VidereFileAssociator.exe";
            process.StartInfo.Arguments = arguments;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            process.Start( );
        }

        private void ButtonBase_OnClick( object Sender, RoutedEventArgs E )
        {
            SetFileAssociations( );
        }
    }
}
