using System;
using System.Globalization;
using System.Windows;
using TMDbLib.Objects.Search;
using VidereLib;
using VidereLib.Components;
using VidereLib.Data;
using VidereLib.Data.MediaData;
using VidereLib.EventArgs;
using VidereLib.NetworkingRequests;

namespace Videre.Controls
{
    /// <summary>
    /// Interaction logic for LibraryMediaControl.xaml
    /// </summary>
    public class LibraryEpisodeControl : LibraryMediaControl
    {
        /// <summary>
        /// The episode request.
        /// </summary>
        protected TheMovieDBRequestBase episodeRequest;

        private SearchTvEpisode episode;

        /// <summary>
        /// The season request.
        /// </summary>
        protected TheMovieDBRequestBase seasonRequest;

        /// <summary>
        /// Constructor.
        /// </summary>
        public LibraryEpisodeControl( VidereMedia media ) : base( media )
        {

        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized( EventArgs e )
        {
            base.OnInitialized( e );

            VideoPlaceholder.Visibility = Visibility.Visible;

            Title.Text = media.Name;
            ToolTip = media.File.Name;

            LoadingRing.IsActive = true;

            if ( media.HasImdbID )
            {
                VidereEpisodeInformation episodeInformation;
                if ( MediaInformationManager.ContainsEpisodeInformationForHash( media.MediaInformation.Hash, out episodeInformation ) )
                {
                    FinishLoadingVideo( );
                }
                else
                {
                    TheMovieDBComponent movieDBComp = ViderePlayer.GetComponent<TheMovieDBComponent>( );
                    movieDBComp.OnEpisodeInformationReceived += OnEpisodeInfoReceived;
                    episodeRequest = ViderePlayer.GetComponent<TheMovieDBComponent>( ).RequestEpisodeInformation( media );
                }
            }
            else
                LoadingRing.IsActive = false;
        }

        private void OnEpisodeInfoReceived( object Sender, Tuple<VidereMedia, SearchTvEpisode> Tuple1 )
        {
            if ( Tuple1.Item1?.OpenSubtitlesHash != media.OpenSubtitlesHash )
                return;

            TheMovieDBComponent comp = ViderePlayer.GetComponent<TheMovieDBComponent>( );

            comp.OnEpisodeInformationReceived -= OnEpisodeInfoReceived;
            episodeRequest = null;

            if ( Tuple1.Item2 == null )
            {
                ViderePlayer.MainDispatcher.Invoke( ( ) =>
                {
                    MediaInformationManager.GetOrSaveEpisodeInformation( media.MediaInformation as VidereEpisodeInformation );
                    this.FinishLoadingVideo( );
                } );
                return;
            }

            episode = Tuple1.Item2;
            ViderePlayer.MainDispatcher.Invoke( ( ) =>
            {
                VidereEpisodeInformation epi = MediaInformationManager.GetOrSaveEpisodeInformation( media.MediaInformation as VidereEpisodeInformation );
                epi.Rating = ( decimal ) episode.Rating;
                epi.Poster = comp.GetPosterURL( episode.StillPath );

                this.Rating.Text = epi.Rating.ToString( CultureInfo.InvariantCulture );
            } );

            comp.OnSeasonInformationReceived += OnSeasonInfoReceived;
            seasonRequest = comp.RequestSeasonInformation( episode.ShowId, episode.SeasonNumber );
        }

        private void OnSeasonInfoReceived( object Sender, OnTvSeasonInformationReceivedEventArgs OnTvSeasonInformationReceivedEventArgs )
        {
            if ( OnTvSeasonInformationReceivedEventArgs.SeasonNumber != episode?.SeasonNumber || OnTvSeasonInformationReceivedEventArgs.ShowID != episode.ShowId )
                return;

            TheMovieDBComponent movieDBComp = ViderePlayer.GetComponent<TheMovieDBComponent>( );

            movieDBComp.OnSeasonInformationReceived -= OnSeasonInfoReceived;
            seasonRequest = null;

            if ( OnTvSeasonInformationReceivedEventArgs.Season == null )
            {
                ViderePlayer.MainDispatcher.Invoke( ( ) =>
                {
                    VidereEpisodeInformation epi = MediaInformationManager.GetEpisodeInformationByHash( media.OpenSubtitlesHash );
                    epi.Poster = movieDBComp.GetPosterURL( epi.Poster );

                    this.FinishLoadingVideo( );
                } );

                return;
            }

            ViderePlayer.MainDispatcher.Invoke( ( ) =>
            {
                VidereEpisodeInformation epi = MediaInformationManager.GetEpisodeInformationByHash( media.OpenSubtitlesHash );
                epi.Poster = movieDBComp.GetPosterURL( OnTvSeasonInformationReceivedEventArgs.Season.PosterPath );

                this.FinishLoadingVideo( );
            } );
        }

        /// <summary>
        /// Called whenever the loading of a video has finished.
        /// </summary>
        protected override void OnFinishLoadingVideo( )
        {
            VidereEpisodeInformation info = MediaInformationManager.GetEpisodeInformationByHash( media.MediaInformation.Hash );
            if ( info == null )
                return;

            SubTitle.Visibility = Visibility.Visible;
            SubTitle.Text = $"Season {info.Season}, Episode {info.Episode}";
        }

        /// <summary>
        /// Gets called whenever the control is unloaded.
        /// </summary>
        protected override void OnControlUnload( )
        {
            TheMovieDBComponent movieDBComp = ViderePlayer.GetComponent<TheMovieDBComponent>( );

            if ( episodeRequest != null )
            {
                episodeRequest.Cancel( );
                movieDBComp.OnEpisodeInformationReceived -= OnEpisodeInfoReceived;
            }

            if ( seasonRequest != null )
            {
                seasonRequest.Cancel( );
                movieDBComp.OnSeasonInformationReceived -= OnSeasonInfoReceived;
            }
        }
    }
}