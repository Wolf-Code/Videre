using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using VidereLib.Components;
using VidereSubs.OpenSubtitles.Data;

namespace Videre.Windows
{
    /// <summary>
    /// Interaction logic for SubtitleSelectionWindow.xaml
    /// </summary>
    public partial class SubtitleSelectionWindow
    {
        private const string tempFile = "temp.gz";
        private ProgressDialogController progressController;
        private readonly WebClient downloader;
        private string downloadPath;

        /// <summary>
        /// The downloaded subtitles file.
        /// </summary>
        public FileInfo DownloadedFile { private set; get; }

        /// <summary>
        /// Indicates whether a subtitles file has been downloaded.
        /// </summary>
        public bool HasDownloadedSubtitleFile => DownloadedFile != null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="languages">The allowed languages.</param>
        public SubtitleSelectionWindow( SubtitleLanguage[ ] languages )
        {
            InitializeComponent( );

            LanguageList.ItemsSource = languages;

            CollectionView languagesView = ( CollectionView ) CollectionViewSource.GetDefaultView( LanguageList.ItemsSource );
            languagesView.SortDescriptions.Add( new SortDescription( "LanguageName", ListSortDirection.Ascending ) );

            downloader = new WebClient( );
            downloader.DownloadProgressChanged += DownloaderOnDownloadProgressChanged;
            downloader.DownloadFileCompleted += DownloaderOnDownloadFileCompleted;
        }

        private async void DownloaderOnDownloadFileCompleted( object Sender, AsyncCompletedEventArgs CompletedEventArgs )
        {
            using ( FileStream originalFileStream = File.OpenRead( tempFile ) )
                using ( FileStream decompressedFileStream = File.Create( downloadPath ) )
                    using ( GZipStream decompressionStream = new GZipStream( originalFileStream, CompressionMode.Decompress ) )
                        decompressionStream.CopyTo( decompressedFileStream );

            File.Delete( tempFile );

            DownloadedFile = new FileInfo( downloadPath );

            Task t = progressController.CloseAsync( );
            await t;
            t.Wait( );
            this.DialogResult = true;
            this.Close( );
        }

        private void DownloaderOnDownloadProgressChanged( object Sender, DownloadProgressChangedEventArgs ChangedEventArgs )
        {
            progressController.SetProgress( ChangedEventArgs.BytesReceived );
        }

        private async void OnSubtitlesDownloadButtonClick( object Sender, RoutedEventArgs E )
        {
            SubtitleData data = SubsList.SelectedValue as SubtitleData;

            if ( data == null )
                await this.ShowMessageAsync( "No subtitles selected", "Please select a subtitles file to download." );
            else
            {
                SaveFileDialog dialog = new SaveFileDialog { FileName = data.SubFileName, Filter = "SubRip (*.srt)|*.srt" };
                if ( !dialog.ShowDialog( ).GetValueOrDefault( ) ) return;

                progressController = await this.ShowProgressAsync( "Downloading subtitles", $"Downloading {data.SubFileName} from opensubtitles.org." );
                progressController.Maximum = data.SubFileSize;
                downloadPath = dialog.FileName;

                downloader.DownloadFileAsync( new Uri( data.SubDownloadLink ), tempFile );
            }
        }

        private void OnCancelButtonClick( object Sender, RoutedEventArgs E )
        {
            this.Close( );
        }

        private async void OnSearchButtonClick( object Sender, RoutedEventArgs E )
        {
            if ( LanguageList.SelectedItems.Count <= 0 )
            {
                await this.ShowMessageAsync( "No language selected", "Unable to download subtitles, there were no languages selected." );
                return;
            }

            progressController = await this.ShowProgressAsync( "Downloading subtitles", "Downloading subtitles for the selected languages from opensubtitles.org." );
            progressController.SetIndeterminate( );

            string[ ] languages = new string[ LanguageList.SelectedItems.Count ];
            for ( int x = 0; x < languages.Length; ++x )
                languages[ x ] = ( ( SubtitleLanguage )LanguageList.SelectedItems[ x ] ).ISO639_3;

            BackgroundWorker worker = new BackgroundWorker( );
            worker.DoWork += ( O, Args ) =>
            {
                SubtitleData[ ] subs = MainWindow.Client.SearchSubtitles( Args.Argument as string[ ], MainWindow.Player.GetComponent<MediaComponent>( ).Media );
                Args.Result = subs;
            };
            worker.RunWorkerCompleted += async ( O, Args ) =>
            {
                DownloadButton.Visibility = Visibility.Visible;
                SubsList.Visibility = Visibility.Visible;

                SubtitleData[ ] subs = Args.Result as SubtitleData[ ];
                this.SubsList.ItemsSource = subs;

                CollectionView subsView = ( CollectionView )CollectionViewSource.GetDefaultView( SubsList.ItemsSource );
                subsView.SortDescriptions.Add( new SortDescription( "DownloadsCount", ListSortDirection.Descending ) );

                await progressController.CloseAsync( );
            };
            worker.RunWorkerAsync( languages );
        }
    }
}
