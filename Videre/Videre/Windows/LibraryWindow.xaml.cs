using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using MahApps.Metro.Controls.Dialogs;
using VidereLib;
using VidereLib.Components;

namespace Videre.Windows
{
    /// <summary>
    /// Interaction logic for LibraryWindow.xaml
    /// </summary>
    public partial class LibraryWindow
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public LibraryWindow( )
        {
            InitializeComponent( );

            Loaded += async ( Sender, Args ) =>
            {
                ProgressDialogController controller;
                TheMovieDBComponent dbComp = ViderePlayer.GetComponent<TheMovieDBComponent>( );
                if ( !dbComp.HasConfig )
                {
                    controller = await this.ShowProgressAsync( "Retrieving TheMovieDB.org configuration", "Retrieving server configuration from TheMovieDB.org." );
                    controller.SetIndeterminate( );

                    await dbComp.RetrieveConfiguration( );

                    await controller.CloseAsync( );
                }

                controller = await this.ShowProgressAsync( "Loading directories", "Loading new media directories" );
                controller.SetIndeterminate( );
                if ( Settings.Default.MediaFolders == null )
                {
                    Settings.Default.MediaFolders = new List<string>( );
                    Settings.Default.Save( );
                }

                DirectorySelector.SetDirectories( Settings.Default.MediaFolders.ToArray( ) );

                await controller.CloseAsync( );
            };

            Closed += ( Sender, Args ) =>
            {
                MediaInformationManager.SaveMediaData( );

                List<string> directoryList = DirectorySelector.DirectoryList.Items.Cast<string>( ).ToList( );
                Settings.Default.MediaFolders = directoryList;
                Settings.Default.Save( );
            };
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized( EventArgs e )
        {
            base.OnInitialized( e );

            DirectorySelector.DirectoryList.SelectionChanged += DirectoryListOnSelectionChanged;
        }

        private async void DirectoryListOnSelectionChanged( object Sender, SelectionChangedEventArgs SelectionChangedEventArgs )
        {
            List<Task> loadTasks = new List<Task>( SelectionChangedEventArgs.AddedItems.Count );
            foreach ( string dir in SelectionChangedEventArgs.AddedItems )
                loadTasks.Add( MediaShowcase.LoadDirectory( dir ) );

            foreach ( Task task in loadTasks )
                await task;

            foreach ( string dir in SelectionChangedEventArgs.RemovedItems )
                MediaShowcase.UnloadDirectory( dir );
        }
    }
}
