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
        /// The command for closing fullscreen.
        /// </summary>
        public static RoutedUICommand CloseFullscreen = new RoutedUICommand( "Closes fullscreen", "CloseFullscreen", typeof ( CommandsHandler ) );

        /// <summary>
        /// The command for opening the local subtitles dialog.
        /// </summary>
        public static RoutedUICommand OpenLocalSubtitles = new RoutedUICommand( "Open the local subtitles dialog", "OpenLocalSubtitles", typeof ( CommandsHandler ) );

        /// <summary>
        /// The command for opening the opensubtitles.org dialog.
        /// </summary>
        public static RoutedUICommand OpenSubtitles = new RoutedUICommand( "Opens the opensubtitles.org dialog", "OpenSubtitles", typeof ( CommandsHandler ) );

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
        /// Shows the dialog for opening local subtitles.
        /// </summary>
        public static void ShowLocalSubtitlesDialog( Window window )
        {
            OpenFileDialog fileDialog = new OpenFileDialog { Filter = "SubRip (*.srt)|*.srt" };
            MediaComponent mediaComp = ViderePlayer.GetComponent<MediaComponent>( );

            if ( mediaComp.HasMediaBeenLoaded )
                fileDialog.InitialDirectory = mediaComp.Media.File.DirectoryName;

            bool? res = fileDialog.ShowDialog( window );

            if ( !res.Value )
                return;

            ViderePlayer.GetComponent<SubtitlesComponent>( ).LoadSubtitles( fileDialog.FileName );
        }

        /// <summary>
        /// The command for showing the media library.
        /// </summary>
        public static void ShowLibraryCommand( )
        {
            new LibraryWindow( ).ShowDialog( );
        }

        /// <summary>
        /// Opens the opensubtitles.org flyout.
        /// </summary>
        /// <param name="window">The <see cref="MainWindow"/> containing the flyout.</param>
        public static void ShowOpenSubtitlesDialog( MainWindow window )
        {
            window.OSFlyout.IsOpen = true;
        }
    }
}
