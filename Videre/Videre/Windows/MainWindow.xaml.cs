using System;
using System.IO;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using VidereLib;
using VidereLib.Components;
using VidereLib.EventArgs;
using VidereSubs.OpenSubtitles;
using VidereSubs.OpenSubtitles.Outputs;

namespace Videre.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private ViderePlayer player;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindow( )
        {
            InitializeComponent( );
        }

        /// <summary>
        /// Initializes player.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInitialized( EventArgs e )
        {
            player = new ViderePlayer( new WindowData { Window = this, MediaControlsContainer = MediaControlsContainer, MediaPlayer = MediaPlayer, MediaArea = MediaArea } );
            this.MediaControlsContainer.Initialize( player );

            SubtitlesComponent subtitlesComponent = player.GetComponent<SubtitlesComponent>( );
            subtitlesComponent.OnSubtitlesChanged += PlayerOnOnSubtitlesChanged;
            subtitlesComponent.OnSubtitlesFailedToLoad += SubtitlesComponentOnOnSubtitlesFailedToLoad;

            MediaComponent mediaComponent = player.GetComponent<MediaComponent>( );
            mediaComponent.OnMediaLoaded += OnOnMediaLoaded;
            mediaComponent.OnMediaUnloaded += OnOnMediaUnloaded;
            mediaComponent.OnMediaFailedToLoad += MediaComponentOnOnMediaFailedToLoad;

            WindowButtonCommandsOverlayBehavior = WindowCommandsOverlayBehavior.Never;
            RightWindowCommandsOverlayBehavior = WindowCommandsOverlayBehavior.Never;
            LeftWindowCommandsOverlayBehavior = WindowCommandsOverlayBehavior.Never;

            Application.Current.DispatcherUnhandledException += ( Sender, Args ) =>
            {
                using ( FileStream FS = File.Create( "exception.txt" ) )
                    using ( TextWriter Writer = new StreamWriter( FS ) )
                        WriteExceptionDetails( Args.Exception, Writer );

                MessageBox.Show( "An exception has been encountered. The exact details have been saved in exception.txt. Please contact the developer and hand them this file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error );
                Args.Handled = true;
                Application.Current.Shutdown( );
            };

            base.OnInitialized( e );
        }

        private async void MediaComponentOnOnMediaFailedToLoad( object Sender, OnMediaFailedToLoadEventArgs MediaFailedToLoadEventArgs )
        {
            await this.ShowMessageAsync( "Failed to load media", $"Unable to load {MediaFailedToLoadEventArgs.MediaFile.Name}. Reason: {MediaFailedToLoadEventArgs.Exception.Message}" );
        }

        private void OnOnMediaUnloaded( object Sender, OnMediaUnloadedEventArgs MediaUnloadedEventArgs )
        {
            MediaControlsContainer.IsEnabled = false;
            SubtitlesButton.IsEnabled = false;
            MediaControlsContainer.TimeLabel_Total.Content = "--:--:--";
            MediaControlsContainer.TimeLabel_Current.Content = MediaControlsContainer.TimeLabel_Total.Content;
        }

        private void OnOnMediaLoaded( object Sender, OnMediaLoadedEventArgs MediaLoadedEventArgs )
        {
            MediaControlsContainer.IsEnabled = true;
            SubtitlesButton.IsEnabled = true;
            FileFlyout.IsOpen = false;
            SubtitlesOffset.Value = 0;

            MediaControlsContainer.TimeLabel_Total.Content = player.GetComponent<MediaComponent>( ).GetMediaLength( ).ToString( @"hh\:mm\:ss" );
            player.GetComponent<StateComponent>( ).Play( );
            Client cl = new Client( );
            LogInOutput outp = cl.LogIn( "", "" );
            cl.CheckMovieHash2( Hasher.ComputeMovieHash( MediaLoadedEventArgs.MediaFile.FullName ) );
            cl.LogOut( );
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
        
        private void SubtitlesPosition_OnValueChanged( object Sender, RoutedPropertyChangedEventArgs<double?> E )
        {
            if ( !E.NewValue.HasValue )
                return;

            subtitleLabel.Margin = new Thickness( 0, 0, 0, E.NewValue.Value );
        }

        private void NumericUpDown_OnValueChanged( object Sender, RoutedPropertyChangedEventArgs<double?> E )
        {
            if ( !E.NewValue.HasValue )
                return;

            player.GetComponent<SubtitlesComponent>( ).SetSubtitlesOffset( TimeSpan.FromMilliseconds( E.NewValue.Value ) );
        }

        private void SubtitlesSize_OnValueChanged( object Sender, RoutedPropertyChangedEventArgs<double?> E )
        {
            if ( !E.NewValue.HasValue )
                return;

            subtitleLabel.FontSize = E.NewValue.Value;
        }

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
            SettingsFlyout.IsOpen = true;
        }

        private void OnFileButtonClicked( object Sender, RoutedEventArgs E )
        {
            FileFlyout.IsOpen = true;
        }

        private void OnLoadMediaButtonClick( object Sender, RoutedEventArgs E )
        {
            OpenFileDialog fileDialog = new OpenFileDialog( );
            bool? res = fileDialog.ShowDialog( this );

            if ( !res.Value )
                return;

            player.GetComponent<StateComponent>( ).Stop( );
            player.GetComponent<MediaComponent>( ).LoadMedia( fileDialog.FileName );
            Console.WriteLine( VidereSubs.OpenSubtitles.Hasher.ComputeMovieHash( fileDialog.FileName ) );
        }

        private void OnLoadSubtitlesButtonClick( object Sender, RoutedEventArgs E )
        {
            OpenFileDialog fileDialog = new OpenFileDialog { Filter = "SubRip (*.srt)|*.srt" };
            bool? res = fileDialog.ShowDialog( this );

            if ( !res.Value )
                return;

            player.GetComponent<SubtitlesComponent>( ).LoadSubtitles( fileDialog.FileName );
            FileFlyout.IsOpen = false;
        }
    }
}
