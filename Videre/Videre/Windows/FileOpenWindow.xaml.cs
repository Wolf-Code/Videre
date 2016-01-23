using System;
using System.ComponentModel;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Videre.Controls;
using VidereLib.Components;
using VidereLib.EventArgs;
using VidereSubs.OpenSubtitles.Data;
using VidereSubs.OpenSubtitles.Outputs;

namespace Videre.Windows
{
    /// <summary>
    /// Interaction logic for FileOpenWindow.xaml
    /// </summary>
    public partial class FileOpenWindow
    {
        private ProgressDialogController controller;

        /// <summary>
        /// Constructor.
        /// </summary>
        public FileOpenWindow( )
        {
            InitializeComponent( );
        }

        /// <summary>
        /// Called whenever the <see cref="VidereControl"/> children have been intialized with a player.
        /// </summary>
        protected override void OnVidereControlsInitialized( )
        {
            SetButtonStates( );

            MediaComponent comp = MainWindow.Player.GetComponent<MediaComponent>( );
            comp.OnMediaLoaded += OnOnMediaLoaded;
            comp.OnMediaUnloaded += CompOnOnMediaUnloaded;
        }

        private void CompOnOnMediaUnloaded( object Sender, OnMediaUnloadedEventArgs MediaUnloadedEventArgs )
        {
            SetButtonStates( );
        }

        private void OnOnMediaLoaded( object Sender, OnMediaLoadedEventArgs OnMediaLoadedEventArgs )
        {
            SetButtonStates( );
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.Closed"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnClosed( EventArgs e )
        {
            MainWindow.Player.GetComponent<MediaComponent>( ).OnMediaLoaded -= OnOnMediaLoaded;
            MainWindow.Player.GetComponent<MediaComponent>( ).OnMediaUnloaded -= CompOnOnMediaUnloaded;

            base.OnClosed( e );
        }

        private void SetButtonStates( )
        {
            bool loaded = MainWindow.Player.GetComponent<MediaComponent>( ).HasMediaBeenLoaded;

            OSSubsButton.IsEnabled = loaded;
            LocalSubsButton.IsEnabled = loaded;
        }

        private async void OSSubsButton_OnClick( object Sender, RoutedEventArgs E )
        {
            BackgroundWorker worker = new BackgroundWorker( );
            if ( !MainWindow.Client.IsLoggedIn )
            {
                controller = await this.ShowProgressAsync( "Signing in.", "Signing into opensubtitles.org." );
                controller.SetIndeterminate( );
                worker.DoWork += ( s, e ) => e.Result = MainWindow.Client.LogIn( string.Empty, string.Empty, false );
                worker.RunWorkerCompleted += async ( s, e ) =>
                {
                    await controller.CloseAsync( );

                    LogInOutput result = e.Result as LogInOutput;
                    if ( MainWindow.Client.IsLoggedIn )
                        OSSubsButton_OnClick( Sender, E );
                    else
                        await this.ShowMessageAsync( "Signing in failed", $"Unable to sign in to opensubtitles.org. Please try again later. (Message: {result.StatusStringWithoutCode})" );
                };
                worker.RunWorkerAsync( );

                return;
            }

            controller = await this.ShowProgressAsync( "Retrieving subtitle languages", "Downloading subtitle languages from opensubtitles.org..." );
            controller.SetIndeterminate( );

            worker.DoWork += ( O, Args ) =>
            {
                SubtitleLanguage[ ] langs = MainWindow.Client.GetSubLanguages( );
                Args.Result = langs;
            };
            worker.RunWorkerCompleted += WorkerOnRunWorkerCompleted;
            worker.RunWorkerAsync( );
        }

        private async void WorkerOnRunWorkerCompleted( object Sender, RunWorkerCompletedEventArgs WorkerCompletedEventArgs )
        {
            SubtitleSelectionWindow subselect = new SubtitleSelectionWindow( ( SubtitleLanguage[ ] )WorkerCompletedEventArgs.Result );
            await controller.CloseAsync( );

            if ( !subselect.ShowDialog( ).GetValueOrDefault( ) ) return;
            if ( !subselect.HasDownloadedSubtitleFile ) return;

            MainWindow.Player.GetComponent<SubtitlesComponent>( ).LoadSubtitles( subselect.DownloadedFile.FullName );
        }

        private void LocalSubsButton_OnClick( object Sender, RoutedEventArgs E )
        {
            OpenFileDialog fileDialog = new OpenFileDialog { Filter = "SubRip (*.srt)|*.srt" };
            bool? res = fileDialog.ShowDialog( this );

            if ( !res.Value )
                return;

            MainWindow.Player.GetComponent<SubtitlesComponent>( ).LoadSubtitles( fileDialog.FileName );
        }

        private void MediaButton_OnClick( object Sender, RoutedEventArgs E )
        {
            OpenFileDialog fileDialog = new OpenFileDialog( );
            if ( !fileDialog.ShowDialog( this ).GetValueOrDefault( ) )
                return;

            MainWindow.Player.GetComponent<StateComponent>( ).Stop( );
            MainWindow.Player.GetComponent<MediaComponent>( ).LoadMedia( fileDialog.FileName );
        }
    }
}
