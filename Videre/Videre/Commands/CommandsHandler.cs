using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Videre.Windows;
using VidereLib;
using VidereLib.Components;

namespace Videre.Commands
{
    /// <summary>
    /// The static class which handles command calls.
    /// </summary>
    public static class CommandsHandler
    {
        /// <summary>
        /// The command for showing the library.
        /// </summary>
        public static RoutedUICommand ShowLibrary = new RoutedUICommand( "Show the media library", "ShowLibrary", typeof ( CommandsHandler ) );

        /// <summary>
        /// The command for opening a media file.
        /// </summary>
        /// <param name="window">The window to open from.</param>
        public static void OpenFileCommand( Window window )
        {
            OpenFileDialog fileDialog = new OpenFileDialog( );

            MediaComponent mediaComp = ViderePlayer.GetComponent<MediaComponent>( );

            string VideoFilter = "Video Files ";
            string VideoCombinedCommaSeparated = "*." + string.Join( ", *.", mediaComp.VideoFileExtensions );
            string VideoCombinedSemiColonSeparated = "*." + string.Join( ";*.", mediaComp.VideoFileExtensions );
            VideoFilter += $"({VideoCombinedCommaSeparated})|{VideoCombinedSemiColonSeparated}";

            string Filter = $"{VideoFilter}|All Files (*.*)|*.*";
            fileDialog.Filter = Filter;

            if ( !fileDialog.ShowDialog( window ).GetValueOrDefault( ) )
                return;

            ViderePlayer.GetComponent<StateComponent>( ).Stop( );
            mediaComp.LoadMedia( fileDialog.FileName );
        }

        /// <summary>
        /// The command for showing the media library.
        /// </summary>
        public static void ShowLibraryCommand( )
        {
            new LibraryWindow( ).ShowDialog( );
        }
    }
}
