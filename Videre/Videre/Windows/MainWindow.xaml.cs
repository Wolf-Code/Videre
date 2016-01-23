using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using VidereLib;
using VidereLib.Components;
using VidereLib.EventArgs;
using VidereLib.Implementations;
using VidereSubs.OpenSubtitles;
using VidereSubs.OpenSubtitles.Data;
using VidereSubs.OpenSubtitles.Outputs;

namespace Videre.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// The active <see cref="ViderePlayer"/>.
        /// </summary>
        public static ViderePlayer Player { private set; get; }

        /// <summary>
        /// The videre user agent.
        /// </summary>
        public const string UserAgent = "Videre v0.1";

        /// <summary>
        /// The client which connects with opensubtitles.org.
        /// </summary>
        public static Client Client { private set; get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindow( )
        {
            InitializeComponent( );
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
                Args.Handled = true;

                Application.Current.Shutdown( );
            };

            Player = new ViderePlayer( new WindowData { Window = this, MediaControlsContainer = MediaControlsContainer, MediaPlayer = new MediaElementPlayer( MediaPlayer ), MediaArea = MediaArea } );

            Player.GetComponent<NetworkComponent>( ).SetUpNetworkReceiver( Settings.Default.ListenPort );

            SubtitlesComponent subtitlesComponent = Player.GetComponent<SubtitlesComponent>( );
            subtitlesComponent.OnSubtitlesChanged += PlayerOnOnSubtitlesChanged;
            subtitlesComponent.OnSubtitlesFailedToLoad += SubtitlesComponentOnOnSubtitlesFailedToLoad;

            MediaComponent mediaComponent = Player.GetComponent<MediaComponent>( );
            mediaComponent.OnMediaLoaded += OnOnMediaLoaded;
            mediaComponent.OnMediaUnloaded += OnOnMediaUnloaded;
            mediaComponent.OnMediaFailedToLoad += MediaComponentOnOnMediaFailedToLoad;

            WindowButtonCommandsOverlayBehavior = WindowCommandsOverlayBehavior.Never;
            RightWindowCommandsOverlayBehavior = WindowCommandsOverlayBehavior.Never;
            LeftWindowCommandsOverlayBehavior = WindowCommandsOverlayBehavior.Never;
            
            Settings.Default.PropertyChanged += ( Sender, Args ) => subtitleLabel.Margin = new Thickness( 0, 0, 0, Settings.Default.FontPosition );
            subtitleLabel.Margin = new Thickness( 0, 0, 0, Settings.Default.FontPosition );

            Client = new Client( UserAgent );

            Application.Current.Exit += ( sender, args ) =>
            {
                Settings.Default.SubtitleTimeOffset = 0;
                Settings.Default.Save( );
            };

            KeyDown += OnKeyDown;

            base.OnInitialized( e );
        }

        private void OnKeyDown( object Sender, KeyEventArgs KeyEventArgs )
        {
            if ( KeyEventArgs.IsRepeat )
                return;

            switch ( KeyEventArgs.Key )
            {
                case Key.Escape:
                    if ( Player.GetComponent<ScreenComponent>( ).IsFullScreen )
                        Player.GetComponent<ScreenComponent>( ).SetFullScreen( false );
                    break;

                case Key.Space:
                    if ( Player.GetComponent<MediaComponent>( ).HasMediaBeenLoaded )
                        Player.GetComponent<StateComponent>( ).ResumeOrPause( );
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

            Player.GetComponent<StateComponent>( ).Play( );
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

        #region Subtitles
       
        private void PlayerOnOnSubtitlesChanged( object Sender, OnSubtitlesChangedEventArgs SubtitlesChangedEventArgs )
        {
            subtitleLabel.Inlines.Clear( );
            if ( SubtitlesChangedEventArgs.Subtitles.Lines.Count <= 0 )
                return;

            for ( int X = 0; X < SubtitlesChangedEventArgs.Subtitles.Lines.Count; X++ )
            {
                subtitleLabel.Inlines.Add( SubtitlesChangedEventArgs.Subtitles.Lines[ X ] );

                if ( X < SubtitlesChangedEventArgs.Subtitles.Lines.Count - 1 )
                    subtitleLabel.Inlines.Add( Environment.NewLine );
            }
        }

        private async void SubtitlesComponentOnOnSubtitlesFailedToLoad( object Sender, OnSubtitlesFailedToLoadEventArgs SubtitlesFailedToLoadEventArgs )
        {
            await this.ShowMessageAsync( "Failed to load subtitles", $"Unable to load {SubtitlesFailedToLoadEventArgs.Subtitles.Name}." );
        }

        #endregion

        private void OnSettingsButtonClicked( object Sender, RoutedEventArgs E )
        {
            SettingsWindow settings = new SettingsWindow( );

            settings.ShowDialog( );
        }

        private void OnFileButtonClicked( object Sender, RoutedEventArgs E )
        {
            FileOpenWindow window = new FileOpenWindow( );
            window.ShowDialog( );
        }
    }
}
