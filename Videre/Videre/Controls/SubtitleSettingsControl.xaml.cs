using System;
using System.Windows;
using Videre.Windows;
using VidereLib.Components;

namespace Videre.Controls
{
    /// <summary>
    /// Interaction logic for SubtitleSettingsControl.xaml
    /// </summary>
    public partial class SubtitleSettingsControl
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public SubtitleSettingsControl( )
        {
            InitializeComponent( );
        }

        private void TimeOffset_OnValueChanged( object Sender, RoutedPropertyChangedEventArgs<double?> E )
        {
            if ( !this.IsPlayerInitialized )
                return;

            if ( !E.NewValue.HasValue )
                return;

            MainWindow.Player.GetComponent<SubtitlesComponent>( ).SetSubtitlesOffset( TimeSpan.FromMilliseconds( E.NewValue.Value ) );
        }

        private void PositionOffset_OnValueChanged( object Sender, RoutedPropertyChangedEventArgs<double?> E )
        {
            if ( !this.IsPlayerInitialized )
                return;

            if ( !E.NewValue.HasValue )
                return;

            Settings.Default.FontPosition = ( short ) E.NewValue.Value;
            Settings.Default.Save( );
        }

        private void SubSize_OnValueChanged( object Sender, RoutedPropertyChangedEventArgs<double?> E )
        {
            if ( !this.IsPlayerInitialized )
                return;
            
            if ( !E.NewValue.HasValue )
                return;
            
            Settings.Default.FontSize = ( byte ) E.NewValue.Value;
            Settings.Default.Save( );
        }
    }
}