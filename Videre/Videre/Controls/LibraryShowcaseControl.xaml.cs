using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.TMDb;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using VidereLib;
using VidereLib.Components;
using VidereLib.Data;
using VidereLib.NetworkingRequests;

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

            media.Sort( ( A, B ) => string.Compare( A.Name, B.Name, StringComparison.Ordinal ) );

            TheMovieDBComponent movieComp = ViderePlayer.GetComponent<TheMovieDBComponent>( );

            foreach ( VidereMedia item in media )
            {
                LibraryMediaControl control = new LibraryMediaControl
                {
                    Title = { Text = item.Name },
                    Rating = { Text = string.Empty },
                    ToolTip = item.File.Name,
                };

                if ( item.IMDBID != null )
                {
                    control.LoadingRing.IsActive = true;

                    ThreadPool.QueueUserWorkItem( async obj =>
                    {
                        RequestMovieInfoJob job = new RequestMovieInfoJob( item );
                        Movie movie = await job.Request( );
                        if ( movie == null )
                            return;

                        ViderePlayer.MainDispatcher.Invoke( ( ) =>
                        {
                            BitmapImage img = new BitmapImage( new Uri( movieComp.GetPosterURL( movie.Poster ) ) );
                            control.Image.Source = img;

                            control.Rating.Text = movie.VoteAverage.ToString( CultureInfo.InvariantCulture );
                            control.LoadingRing.IsActive = false;
                        } );
                    } );
                }

                MediaList.Items.Add( control );
            }

            await controller.CloseAsync( );
        }
    }
}
