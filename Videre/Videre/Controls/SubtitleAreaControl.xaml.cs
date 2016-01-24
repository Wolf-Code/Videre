using System;
using System.Windows;
using VidereLib.Components;
using VidereLib.EventArgs;

namespace Videre.Controls
{
    /// <summary>
    /// Interaction logic for SubtitleAreaControl.xaml
    /// </summary>
    public partial class SubtitleAreaControl
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public SubtitleAreaControl( )
        {
            InitializeComponent( );
        }

        /// <summary>
        /// Gets called whenever the player has been initialized.
        /// </summary>
        public override void OnPlayerInitialized( )
        {
            Settings.Default.PropertyChanged += ( Sender, Args ) => SubsTextBlock.Margin = new Thickness( 0, 0, 0, Settings.Default.FontPosition );
            SubsTextBlock.Margin = new Thickness( 0, 0, 0, Settings.Default.FontPosition );

            Player.GetComponent<SubtitlesComponent>( ).OnSubtitlesChanged += OnOnSubtitlesChanged;
        }

        private void OnOnSubtitlesChanged( object Sender, OnSubtitlesChangedEventArgs OnSubtitlesChangedEventArgs )
        {
            SubsTextBlock.Inlines.Clear( );
            if ( OnSubtitlesChangedEventArgs.Subtitles.Lines.Count <= 0 )
                return;

            for ( int X = 0; X < OnSubtitlesChangedEventArgs.Subtitles.Lines.Count; X++ )
            {
                SubsTextBlock.Inlines.Add( OnSubtitlesChangedEventArgs.Subtitles.Lines[ X ] );

                if ( X < OnSubtitlesChangedEventArgs.Subtitles.Lines.Count - 1 )
                    SubsTextBlock.Inlines.Add( Environment.NewLine );
            }
        }
    }
}