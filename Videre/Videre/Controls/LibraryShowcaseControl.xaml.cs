using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
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
                switch ( item.MediaInformation?.MovieType )
                {
                    case MovieData.MovieKind.Episode:
                        SeriesList.Items.Add( new LibraryEpisodeControl( item ) );

                        if ( SeriesTab.Visibility != Visibility.Visible )
                            SeriesTab.Visibility = Visibility.Visible;

                        break;

                    case MovieData.MovieKind.Movie:
                        MoviesList.Items.Add( new LibraryMovieControl( item ) );

                        if ( MoviesTab.Visibility != Visibility.Visible )
                            MoviesTab.Visibility = Visibility.Visible;
                        break;
                        
                    default:
                        MiscList.Items.Add( item );

                        if ( MiscTab.Visibility != Visibility.Visible )
                            MiscTab.Visibility = Visibility.Visible;
                        break;
                }
            }

            await controller.CloseAsync( );
        }

        private void OnSelectionChange( object Sender, SelectionChangedEventArgs E )
        {
            Window.GetWindow( this ).Close( );

            ViderePlayer.MediaPlayer.Stop( );
            ViderePlayer.MediaPlayer.LoadMedia( ( ( VidereMedia ) MiscList.SelectedItem ).File );
            ViderePlayer.MediaPlayer.Play( );
        }
    }
}
