using System;
using System.Collections.Generic;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using VidereLib;
using VidereLib.Components;
using VidereLib.Data;
using VidereSubs.OpenSubtitles.Data;

namespace Videre.Controls
{
    /// <summary>
    /// Interaction logic for LibraryShowcaseControl.xaml
    /// </summary>
    public partial class LibraryShowcaseControl
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public LibraryShowcaseControl( )
        {
            InitializeComponent( );
        }

        /// <summary>
        /// Adds all media inside a given directory to the library showcase.
        /// </summary>
        /// <param name="directory">The directory to add.</param>
        public async void LoadDirectory( string directory )
        {
            List<VidereMedia> media = ViderePlayer.GetComponent<MediaComponent>( ).FindMediaInDirectory( directory );

            ProgressDialogController controller = await ( ( MetroWindow ) Window.GetWindow( this ) ).ShowProgressAsync( "Retrieving media information", "Retrieving media information from OpenSubtitles.org." );
            controller.SetIndeterminate( );

            await ViderePlayer.GetComponent<MediaComponent>( ).RetrieveMediaInformation( media.ToArray( ) );

            media.Sort( ( A, B ) => string.Compare( A.Name, B.Name, StringComparison.Ordinal ) );

            foreach ( VidereMedia item in media )
            {
                LibraryMediaControl control;

                switch ( item.MediaInformation?.MovieType )
                {
                    case MovieData.MovieKind.Episode:
                        control = new LibraryEpisodeControl( item );
                        break;

                    case MovieData.MovieKind.Movie:
                        control = new LibraryMovieControl( item );
                        break;

                    case null:
                    default:
                        control = new LibraryUnknownControl( item );
                        break;
                }

                MediaList.Items.Add( control );
            }

            await controller.CloseAsync( );
        }
    }
}
