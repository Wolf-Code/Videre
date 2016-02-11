using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Videre.Windows;
using VidereLib;
using VidereLib.Components;
using VidereSubs.OpenSubtitles;
using VidereSubs.OpenSubtitles.Data;
using VidereSubs.OpenSubtitles.Outputs;

namespace Videre.Controls
{
    /// <summary>
    /// Interaction logic for OpenSubtitlesControl.xaml
    /// </summary>
    public partial class OpenSubtitlesControl
    {
        private ProgressDialogController controller;

        private Flyout flyout;
        private SubtitleLanguage[ ] languages;
        private bool firstOpen = true;

        private WebClient downloader;

        private MainWindow window;

        private string downloadPath;
        private readonly string tempFile = Path.GetTempFileName( );

        /// <summary>
        /// Constructor.
        /// </summary>
        public OpenSubtitlesControl( )
        {
            InitializeComponent( );
        }

        /// <summary>
        /// Initializes the control with a <see cref="MainWindow"/>. Necessary, as the <see cref="Window.GetWindow"/> method is unable to find a window for flyout controls.
        /// </summary>
        /// <param name="mainWindow">The <see cref="MainWindow"/>.</param>
        public void InitWindow( MainWindow mainWindow )
        {
            this.window = mainWindow;
            flyout = window.OSFlyout;
            flyout.IsOpenChanged += OsFlyoutOnIsOpenChanged;

            downloader = new WebClient( );
            downloader.DownloadProgressChanged += ( Sender, Args ) => controller.SetProgress( Args.BytesReceived );;
            downloader.DownloadFileCompleted += DownloaderOnDownloadFileCompleted;

            LanguageList.SelectionChanged += ( Sender, Args ) =>
            {
                bool HasSelection = LanguageList.SelectedItems.Count > 0;

                DownloadSubsLanguagesButton.IsEnabled = HasSelection;
            };

            window.SizeChanged += ( Sender, Args ) =>
            {
                if ( !Args.WidthChanged )
                    return;

                this.Width = Args.NewSize.Width * 0.8;
            };
        }

        private async void DownloaderOnDownloadFileCompleted( object Sender, AsyncCompletedEventArgs E )
        {
            using ( FileStream originalFileStream = File.OpenRead( tempFile ) )
                using ( FileStream decompressedFileStream = File.Create( downloadPath ) )
                    using ( GZipStream decompressionStream = new GZipStream( originalFileStream, CompressionMode.Decompress ) )
                        decompressionStream.CopyTo( decompressedFileStream );

            File.Delete( tempFile );

            ViderePlayer.GetComponent<SubtitlesComponent>( ).LoadSubtitles( downloadPath );

            await controller.CloseAsync( );

            flyout.IsOpen = false;
        }

        private void OsFlyoutOnIsOpenChanged( object Sender, RoutedEventArgs Args )
        {
            if ( flyout.IsOpen )
            {
                if ( firstOpen )
                {
                    firstOpen = false;
                    OnFlyoutFirstOpen(  );
                    return;
                }
                OnFlyoutOpen( );
            }
            else
                OnFlyoutClose( );
        }

        private async void OnFlyoutFirstOpen( )
        {
            if ( Interface.Client.IsLoggedIn ) return;

            controller = await window.ShowProgressAsync( "Signing in.", "Signing into opensubtitles.org." );
            controller.SetIndeterminate( );
            LogInOutput output = await SignInClient( );

            if ( output.LogInSuccesful && Interface.Client.IsLoggedIn )
            {
                controller.SetTitle( "Downloading subtitle languages." );
                controller.SetMessage( "Downloading the available subtitle languages from opensubtitles.org." );
                this.languages = await GetSubtitleLanguages( );
                this.LanguageList.ItemsSource = this.languages;

                CollectionView languagesView = ( CollectionView )CollectionViewSource.GetDefaultView( LanguageList.ItemsSource );
                languagesView.SortDescriptions.Add( new SortDescription( "LanguageName", ListSortDirection.Ascending ) );
            }
            else
                await window.ShowMessageAsync( "Signing in failed", $"Unable to sign in to opensubtitles.org. Please try again later. (Status: {output.Status}, {output.StatusStringWithoutCode})" );

            await controller.CloseAsync( );

            OnFlyoutOpen( );
        }

        private void OnFlyoutOpen( )
        {

        }

        private void OnFlyoutClose( )
        {

        }

        private async Task<LogInOutput> SignInClient( )
        {
            return await Task.Run( ( ) => Interface.Client.LogIn( string.Empty, string.Empty, false ) );
        }

        private async Task<SubtitleLanguage[ ]> GetSubtitleLanguages( )
        {
            return await Task.Run( ( ) => Interface.Client.GetSubLanguages( ) );
        }

        private async void DownloadSelectedLanguageSubtitles( )
        {
            if ( LanguageList.SelectedItems.Count <= 0 )
                return;

            controller = await window.ShowProgressAsync( "Downloading subtitles", "Downloading subtitles for the selected languages from opensubtitles.org." );
            controller.SetIndeterminate( );

            string[ ] languages = new string[ LanguageList.SelectedItems.Count ];
            for ( int x = 0; x < languages.Length; ++x )
                languages[ x ] = ( ( SubtitleLanguage ) LanguageList.SelectedItems[ x ] ).ISO639_3;

            SubtitleData[ ] data = await Task.Run( ( ) => Interface.Client.SearchSubtitles( languages, ViderePlayer.GetComponent<MediaComponent>( ).Media.File ) );

            SubsGroupBox.Visibility = Visibility.Visible;

            this.SubtitlesList.ItemsSource = data;

            CollectionView subsView = ( CollectionView ) CollectionViewSource.GetDefaultView( SubtitlesList.ItemsSource );
            subsView.SortDescriptions.Add( new SortDescription( "DownloadsCount", ListSortDirection.Descending ) );

            Scroller.ScrollToBottom( );

            await controller.CloseAsync( );
        }

        #region UI

        private void DownloadSubsLanguagesButton_OnClick( object Sender, RoutedEventArgs E )
        {
            DownloadSelectedLanguageSubtitles( );
        }

        private void SubtitlesList_OnSelectionChanged( object Sender, SelectionChangedEventArgs E )
        {
            bool hasSelected = SubtitlesList.SelectedItems.Count > 0;

            DownloadSubFileButton.IsEnabled = hasSelected;
        }

        private async void DownloadSubFileButton_OnClick( object Sender, RoutedEventArgs E )
        {
            SubtitleData data = SubtitlesList.SelectedValue as SubtitleData;

            if ( data == null )
                await window.ShowMessageAsync( "No subtitles selected", "Please select a subtitles file to download." );
            else
            {
                SaveFileDialog dialog = new SaveFileDialog { FileName = data.SubFileName, Filter = "SubRip (*.srt)|*.srt" };
                if ( !dialog.ShowDialog( ).GetValueOrDefault( ) ) return;

                controller = await window.ShowProgressAsync( "Downloading subtitles", $"Downloading {data.SubFileName} from opensubtitles.org." );
                controller.Maximum = data.SubFileSize;
                downloadPath = dialog.FileName;

                downloader.DownloadFileAsync( new Uri( data.SubDownloadLink ), tempFile );
            }
        }

        #endregion
    }
}
