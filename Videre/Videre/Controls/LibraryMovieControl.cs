﻿using System;
using System.Windows;
using TMDbLib.Objects.General;
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
    public class LibraryMovieControl : LibraryMediaControl
    {
        /// <summary>
        /// The movie request.
        /// </summary>
        protected TheMovieDBRequestBase movieRequest;

        /// <summary>
        /// Constructor.
        /// </summary>
        public LibraryMovieControl( VidereMedia media ) : base( media )
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

            if ( media.HasImdbID )
            {
                VidereMovieInformation movieInfo;
                if ( MediaInformationManager.ContainsMovieInformationForHash( media.MediaInformation.Hash, out movieInfo ) )
                    FinishLoadingVideo( );
                else
                {
                    MediaInformationManager.SetMovieInformation( media.MediaInformation as VidereMovieInformation );

                    TheMovieDBComponent movieDBComp = ViderePlayer.GetComponent<TheMovieDBComponent>( );
                    movieDBComp.OnMovieInformationReceived += OnMovieInfoReceived;
                    movieRequest = ViderePlayer.GetComponent<TheMovieDBComponent>( ).RequestMovieInformation( media );
                }
            }
            else
                LoadingRing.IsActive = false;
        }

        private void OnMovieInfoReceived( object Sender, Tuple<VidereMedia, MovieResult> Tuple1 )
        {
            if ( Tuple1.Item1.OpenSubtitlesHash != media.OpenSubtitlesHash )
                return;

            MovieResult movie = Tuple1.Item2;
            if ( movie == null )
            {
                ViderePlayer.MainDispatcher.Invoke( this.FinishLoadingVideo );
                return;
            }

            TheMovieDBComponent movieDBComp = ViderePlayer.GetComponent<TheMovieDBComponent>( );
            ViderePlayer.MainDispatcher.Invoke( ( ) =>
            {
                VidereMovieInformation info = MediaInformationManager.GetMovieInformationByHash( media.MediaInformation.Hash );
                info.Poster = movieDBComp.GetPosterURL( movie.PosterPath );
                info.Rating = ( decimal ) movie.VoteAverage;

                this.FinishLoadingVideo( );
            } );

            movieDBComp.OnMovieInformationReceived -= OnMovieInfoReceived;
            movieRequest = null;
        }

        /// <summary>
        /// Gets called whenever the control is unloaded.
        /// </summary>
        protected override void OnControlUnload( )
        {
            if ( movieRequest != null )
            {
                movieRequest.Cancel( );
                ViderePlayer.GetComponent<TheMovieDBComponent>( ).OnMovieInformationReceived -= OnMovieInfoReceived;
            }
        }
    }
}