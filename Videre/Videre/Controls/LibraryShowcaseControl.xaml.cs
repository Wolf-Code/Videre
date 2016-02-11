using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using VidereLib;
using VidereLib.Components;
using VidereLib.Data;

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

            await ViderePlayer.GetComponent<MediaComponent>( ).RetrieveMediaInformation( media.ToArray(  ) );

            media.Sort( ( A, B ) => String.Compare( A.Name, B.Name, StringComparison.Ordinal ) );

            controller.SetTitle( "Retrieving movie posters" );
            controller.SetMessage( "Retrieving movie posters from TheMovieDB.org." );
            Dictionary<string, string> imgs = await ViderePlayer.GetComponent<TheMovieDBComponent>( ).GetMoviePosters( media.ToArray( ) );

            foreach ( VidereMedia item in media )
            {
                LibraryMediaControl control = new LibraryMediaControl
                {
                    Title = { Text = item.Name },
                    Rating = { Text = item.IMDBID },
                    ToolTip = item.File.Name,
                };

                if ( item.IMDBID != null && imgs.ContainsKey( item.IMDBID ) )
                {
                    BitmapImage img = new BitmapImage( new Uri( imgs[ item.IMDBID ] ) );
                    control.Image.Source = img;
                }

                MediaList.Items.Add( control );
            }

            await controller.CloseAsync( );
        }
    }
}
