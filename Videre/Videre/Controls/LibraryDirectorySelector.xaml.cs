using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace Videre.Controls
{
    /// <summary>
    /// Interaction logic for LibraryDirectorySelector.xaml
    /// </summary>
    public partial class LibraryDirectorySelector
    {
        private readonly HashSet<string> directories = new HashSet<string>( ); 
         
        /// <summary>
        /// Constructor.
        /// </summary>
        public LibraryDirectorySelector( )
        {
            InitializeComponent( );
        }

        /// <summary>
        /// Sets the list of directories.
        /// </summary>
        /// <param name="dirs">The directories that can be chosen.</param>
        public void SetDirectories( params string[ ] dirs )
        {
            DirectoryList.Items.Clear( );
            directories.Clear( );

            foreach ( string dir in dirs )
                AddDirectory( dir );
        }

        private void AddDirectory( string dir )
        {
            DirectoryInfo info = new DirectoryInfo( dir );
            if ( !info.Exists )
                return;

            if ( directories.Contains( info.FullName ) )
                return;

            while ( info.Parent != null )
            {
                if ( directories.Contains( info.Parent.FullName ) )
                    return;

                info = info.Parent;
            }

            DirectoryList.Items.Add( dir );
            DirectoryList.SelectedItems.Add( dir );
            directories.Add( dir );
        }

        private void OnAddNewDirectory( object Sender, RoutedEventArgs E )
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog( );
            if ( dialog.ShowDialog( ) != DialogResult.OK )
                return;

            AddDirectory( dialog.SelectedPath );
        }

        private void OnRemoveExistingDirectory( object Sender, RoutedEventArgs E )
        {
            for ( int index = DirectoryList.SelectedItems.Count - 1; index >= 0; index-- )
            {
                string dir = ( string ) DirectoryList.SelectedItems[ index ];
                RemoveDirectory( dir );
            }
        }

        private void RemoveDirectory( string dir )
        {
            DirectoryList.Items.Remove( dir );
            directories.Remove( dir );
        }

        private void OnSelectionChange( object Sender, SelectionChangedEventArgs E )
        {
            RemoveButton.IsEnabled = DirectoryList.SelectedItems.Count > 0;
        }
    }
}
