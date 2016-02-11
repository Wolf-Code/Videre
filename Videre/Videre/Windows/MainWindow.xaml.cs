using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Videre.Controls;
using Videre.Players;
using VidereLib;
using VidereLib.Components;
using VidereLib.Data;
using VidereLib.EventArgs;
using VidereSubs.OpenSubtitles;

namespace Videre.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// The videre user agent.
        /// </summary>
        public const string UserAgent = "Videre v0.1";

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindow( )
        {
            InitializeComponent( );

            this.Loaded += async ( Sender, Args ) =>
            {
                ProgressDialogController controller = await this.ShowProgressAsync( "Signing in.", "Signing in to opensubtitles.org." );
                Interface.Client.LogIn( "", "", false );
                await controller.CloseAsync( );
            };
        }

        /// <summary>
        /// Initializes Player.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInitialized( EventArgs e )
        {
            Application.Current.DispatcherUnhandledException += ( Sender, Args ) =>
            {
                using ( FileStream FS = File.Create( "exception.txt" ) )
                    using ( TextWriter Writer = new StreamWriter( FS ) )
                        WriteExceptionDetails( Args.Exception, Writer );

                MessageBox.Show( "An exception has been encountered. The exact details have been saved in exception.txt. Please contact the developer and hand them this file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error );
#if !DEBUG
                Args.Handled = true;
#endif
                Application.Current.Shutdown( );
            };
            
            ViderePlayer.Initialize( new WindowData { Window = this, MediaControlsContainer = MediaControlsContainer, MediaPlayer = new VLCPlayer( MediaArea.MediaPlayer ), MediaArea = MediaArea } );
            Settings.Default.MediaFolders = new List<string> { @"D:\Folders\Videos" };
            Settings.Default.Save( );

            MediaComponent mediaComponent = ViderePlayer.GetComponent<MediaComponent>( );
            mediaComponent.OnMediaLoaded += OnOnMediaLoaded;
            mediaComponent.OnMediaUnloaded += OnOnMediaUnloaded;
            mediaComponent.OnMediaFailedToLoad += MediaComponentOnOnMediaFailedToLoad;

            Interface.Initialize( UserAgent );

            WindowButtonCommandsOverlayBehavior = WindowCommandsOverlayBehavior.Never;
            RightWindowCommandsOverlayBehavior = WindowCommandsOverlayBehavior.Never;
            LeftWindowCommandsOverlayBehavior = WindowCommandsOverlayBehavior.Never;

            Application.Current.Exit += ( sender, args ) =>
            {
                Settings.Default.SubtitleTimeOffset = 0;
                Settings.Default.Save( );
            };

            KeyDown += OnKeyDown;

            StateComponent stateComponent = ViderePlayer.GetComponent<StateComponent>( );
            ScreenComponent screenComponent = ViderePlayer.GetComponent<ScreenComponent>( );

            stateComponent.OnStateChanged += ( Sender, Args ) =>
            {
                switch ( Args.State )
                {
                    case StateComponent.PlayerState.Playing:
                        screenComponent.DisableSleeping( );
                        break;

                    case StateComponent.PlayerState.Paused:
                    case StateComponent.PlayerState.Stopped:
                        screenComponent.EnableSleeping( );
                        break;
                }
            };

            string[ ] cmdArgs = Environment.GetCommandLineArgs( );
            if ( cmdArgs.Length > 1 )
                ViderePlayer.GetComponent<MediaComponent>( ).LoadMedia( cmdArgs[ 1 ] );

            base.OnInitialized( e );

            ( ( OpenSubtitlesControl ) this.OSFlyout.Content ).InitWindow( this );
            MediaInformationManager.LoadMediaData( );
        }

        private static void OnKeyDown( object Sender, KeyEventArgs KeyEventArgs )
        {
            if ( KeyEventArgs.IsRepeat )
                return;

            switch ( KeyEventArgs.Key )
            {
                case Key.Escape:
                    ScreenComponent screenComp = ViderePlayer.GetComponent<ScreenComponent>( );
                    if ( screenComp.IsFullScreen )
                        screenComp.SetFullScreen( false );
                    break;

                case Key.Space:
                    if ( ViderePlayer.GetComponent<MediaComponent>( ).HasMediaBeenLoaded )
                        ViderePlayer.GetComponent<StateComponent>( ).ResumeOrPause( );
                    break;
            }
        }

        private async void MediaComponentOnOnMediaFailedToLoad( object Sender, OnMediaFailedToLoadEventArgs MediaFailedToLoadEventArgs )
        {
            await this.ShowMessageAsync( "Failed to load media", $"Unable to load {MediaFailedToLoadEventArgs.MediaFile.Name}. Reason: {MediaFailedToLoadEventArgs.Exception.Message}" );
        }

        private void OnOnMediaUnloaded( object Sender, OnMediaUnloadedEventArgs MediaUnloadedEventArgs )
        {
            MediaControlsContainer.IsEnabled = false;
        }

        private void OnOnMediaLoaded( object Sender, OnMediaLoadedEventArgs MediaLoadedEventArgs )
        {
            MediaControlsContainer.IsEnabled = true;

            ViderePlayer.GetComponent<StateComponent>( ).Play( );
        }

        private static void WriteExceptionDetails( Exception exception, TextWriter writer )
        {
            writer.WriteLine( $"Source: {exception.Source}" );
            writer.WriteLine( exception.ToString( ) );
            writer.WriteLine( );
            writer.WriteLine( "Data:" );
            foreach ( object key in exception.Data.Keys )
                writer.WriteLine( $"{key}: {exception.Data[ key ]}" );
        }

        private void Cmd_TogglePlayPause( object Sender, ExecutedRoutedEventArgs E )
        {
            ViderePlayer.GetComponent<StateComponent>( ).ResumeOrPause( );
        }

        private void CanCmd_TogglePlayPause( object Sender, CanExecuteRoutedEventArgs E )
        {
            E.CanExecute = ViderePlayer.GetComponent<MediaComponent>( ).HasMediaBeenLoaded;
        }
    }
}
