namespace Videre.Controls
{
    /// <summary>
    /// Interaction logic for LibraryDirectorySelector.xaml
    /// </summary>
    public partial class LibraryDirectorySelector
    {
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
        /// <param name="directories">The directories that can be chosen.</param>
        public void SetDirectories( params string[ ] directories )
        {
            DirectoryList.Items.Clear( );
            foreach ( string dir in directories )
                DirectoryList.Items.Add( dir );
        }
    }
}
