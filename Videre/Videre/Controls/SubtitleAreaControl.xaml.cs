using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
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
            
            List<Inline> inlines = ConvertLinesToRuns( OnSubtitlesChangedEventArgs.Subtitles.Lines );
            foreach ( Inline inline in inlines )
                SubsTextBlock.Inlines.Add( inline );
        }

        private Inline GetInlineWithStyling( string line, System.Drawing.FontStyle style )
        {
            Inline styledText = new Run( line );
            if ( style.HasFlag( System.Drawing.FontStyle.Bold ) )
                styledText = new Bold( styledText );

            if ( style.HasFlag( System.Drawing.FontStyle.Italic ) )
                styledText = new Italic( styledText );

            if ( style.HasFlag( System.Drawing.FontStyle.Underline ) )
                styledText = new Underline( styledText );

            return styledText;
        }

        private List<Inline> ConvertLinesToRuns( List<string> lines )
        {
            System.Drawing.FontStyle currentStyle = System.Drawing.FontStyle.Regular;
            List<Inline> inlines = new List<Inline>( );
            for ( int X = 0; X < lines.Count; ++X )
            {
                string line = lines[ X ];

                string currentText = string.Empty;
                for ( int q = 0; q < line.Length; q++ )
                {
                    if ( line[ q ] == '<' )
                    {
                        inlines.Add( GetInlineWithStyling( currentText, currentStyle ) );
                        currentText = string.Empty;
                        string contents = string.Empty;
                        while ( line[ ++q ] != '>' )
                            contents += line[ q ];

                        string[ ] separated = contents.Split( new[ ] { ' ' }, StringSplitOptions.RemoveEmptyEntries );
                        switch ( separated[ 0 ] )
                        {
                            case "i":
                                currentStyle |= System.Drawing.FontStyle.Italic;
                                break;

                            case "/i":
                                currentStyle &= ~System.Drawing.FontStyle.Italic;
                                break;

                            case "b":
                                currentStyle |= System.Drawing.FontStyle.Bold;
                                break;

                            case "/b":
                                currentStyle &= ~System.Drawing.FontStyle.Bold;
                                break;

                            case "u":
                                currentStyle |= System.Drawing.FontStyle.Underline;
                                break;

                            case "/u":
                                currentStyle &= ~System.Drawing.FontStyle.Underline;
                                break;
                        }

                        continue;
                    }
                    currentText += line[ q ];
                }

                if ( X < lines.Count - 1 )
                    currentText += Environment.NewLine;

                inlines.Add( GetInlineWithStyling( currentText, currentStyle ) );
            }

            return inlines;
        }
    }
}