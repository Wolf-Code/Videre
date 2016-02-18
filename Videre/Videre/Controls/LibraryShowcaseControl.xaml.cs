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
        /// A container class for the misc items.
        /// </summary>
        private class MiscContainer
        {
            /// <summary>
            /// The name of the misc item.
            /// </summary>
            public string Name => media.Name;

            private readonly VidereMedia media;

            /// <summary>
            /// The directory in the library this misc item is associated with.
            /// </summary>
            public readonly string LibraryDirectory;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="media">The media this item represents.</param>
            /// <param name="libDir">The library directory this misc item is part of.</param>
            public MiscContainer( VidereMedia media, string libDir )
            {
                this.media = media;
                this.LibraryDirectory = libDir;
            }
        }

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
                        SeriesList.Items.Add( new LibraryEpisodeControl( item ) { LibraryDirectory = directory } );

                        if ( SeriesTab.Visibility != Visibility.Visible )
                            SeriesTab.Visibility = Visibility.Visible;

                        break;

                    case MovieData.MovieKind.Movie:
                        MoviesList.Items.Add( new LibraryMovieControl( item ) { LibraryDirectory = directory } );

                        if ( MoviesTab.Visibility != Visibility.Visible )
                            MoviesTab.Visibility = Visibility.Visible;
                        break;
                        
                    default:
                        MiscList.Items.Add( new MiscContainer( item, directory ) );

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

        /// <summary>
        /// Unloads media from a given directory.
        /// </summary>
        /// <param name="directory">The directory to unload media from.</param>
        public void UnloadDirectory( string directory )
        {
            for ( int index = SeriesList.Items.Count - 1; index >= 0; index-- )
            {
                LibraryMediaControl child = ( LibraryMediaControl ) SeriesList.Items[ index ];
                if ( child.LibraryDirectory == directory )
                    SeriesList.Items.RemoveAt( index );
            }

            for ( int index = MoviesList.Items.Count - 1; index >= 0; index-- )
            {
                LibraryMediaControl child = ( LibraryMediaControl )MoviesList.Items[ index ];
                if ( child.LibraryDirectory == directory )
                    MoviesList.Items.RemoveAt( index );
            }

            for ( int index = MiscList.Items.Count - 1; index >= 0; index-- )
            {
                MiscContainer child = ( MiscContainer )MiscList.Items[ index ];
                if ( child.LibraryDirectory == directory )
                    MiscList.Items.RemoveAt( index );
            }
        }
    }
}
