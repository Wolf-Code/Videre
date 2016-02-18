using System;
using System.Threading;
using System.Windows;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.TvShows;
using VidereLib;
using VidereLib.Components;
using VidereLib.Data;
using VidereLib.Data.MediaData;
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
        protected RequestEpisodeInfoJob episodeRequest;

        /// <summary>
        /// The season request.
        /// </summary>
        protected RequestSeasonInfoJob seasonRequest;

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
                    FinishLoadingVideo( );
                else
                {
                    ThreadPool.QueueUserWorkItem( async obj =>
                    {
                        episodeRequest = new RequestEpisodeInfoJob( media );
                        SearchTvEpisode episode = await episodeRequest.Request( );
                        if ( episode == null )
                        {
                            ViderePlayer.MainDispatcher.Invoke( ( ) => LoadingRing.IsActive = false );
                            return;
                        }

                        seasonRequest = new RequestSeasonInfoJob( media, episode.ShowId, episode.SeasonNumber );
                        TvSeason season = await seasonRequest.Request( );

                        ViderePlayer.MainDispatcher.Invoke( ( ) =>
                        {
                            MediaInformationManager.SetEpisodeInformation( media.MediaInformation as VidereEpisodeInformation );

                            VidereEpisodeInformation info = MediaInformationManager.GetEpisodeInformationByHash( media.MediaInformation.Hash );
                            info.Poster = ViderePlayer.GetComponent<TheMovieDBComponent>( ).GetPosterURL( season.PosterPath );
                            info.Rating = ( decimal ) episode.VoteAverage;

                            FinishLoadingVideo( );
                        } );
                    } );
                }
            }
            else
                LoadingRing.IsActive = false;
        }

        /// <summary>
        /// Called whenever the loading of a video has finished.
        /// </summary>
        protected override void OnFinishLoadingVideo( )
        {
            VidereEpisodeInformation info = MediaInformationManager.GetEpisodeInformationByHash( media.MediaInformation.Hash );
            SubTitle.Visibility = Visibility.Visible;
            SubTitle.Text = $"Season {info.Season}, Episode {info.Episode}";
        }

        /// <summary>
        /// Gets called whenever the control is unloaded.
        /// </summary>
        protected override void OnControlUnload( )
        {
            episodeRequest?.Cancel( );
        }
    }
}