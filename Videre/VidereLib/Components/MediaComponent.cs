using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VidereLib.Data;
using VidereLib.EventArgs;
using VidereSubs.OpenSubtitles;
using VidereSubs.OpenSubtitles.Data;
using VidereSubs.OpenSubtitles.Outputs;

namespace VidereLib.Components
{
    /// <summary>
    /// The media componenet.
    /// </summary>
    public class MediaComponent : ComponentBase
    {
        /// <summary>
        /// Returns whether or not any media has been loaded.
        /// </summary>
        public bool HasMediaBeenLoaded => ViderePlayer.MediaPlayer.IsMediaLoaded;

        /// <summary>
        /// The currently loaded media.
        /// </summary>
        public VidereMedia Media => ViderePlayer.MediaPlayer.Media;

        /// <summary>
        /// Gets called whenever media has been loaded.
        /// </summary>
        public event EventHandler<OnMediaLoadedEventArgs> OnMediaLoaded;

        /// <summary>
        /// Gets called whenever media has been unloaded.
        /// </summary>
        public event EventHandler<OnMediaUnloadedEventArgs> OnMediaUnloaded;

        /// <summary>
        /// Gets called whenever media failed to load.
        /// </summary>
        public event EventHandler<OnMediaFailedToLoadEventArgs> OnMediaFailedToLoad;

        /// <summary>
        /// Initializes the <see cref="MediaComponent"/>.
        /// </summary>
        protected override void OnInitialize( )
        {
            ViderePlayer.MediaPlayer.MediaFailedToLoad += ( sender, args ) => OnMediaFailedToLoad?.Invoke( this, args );
            ViderePlayer.MediaPlayer.MediaLoaded += ( sender, args ) => OnMediaLoaded?.Invoke( this, args );
            ViderePlayer.MediaPlayer.MediaUnloaded += ( sender, args ) => OnMediaUnloaded?.Invoke( this, args );
        }

        /// <summary>
        /// Gets the length of the media as a <see cref="TimeSpan"/>.
        /// </summary>
        /// <returns>The length of the media.</returns>
        public TimeSpan GetMediaLength( )
        {
            if ( !HasMediaBeenLoaded )
                throw new Exception( "Attempted to get media length while there is no media loaded." );

            return ViderePlayer.MediaPlayer.GetMediaLength( );
        }

        /// <summary>
        /// Loads media from a path.
        /// </summary>
        /// <param name="Path">The path of the media.</param>
        public void LoadMedia( string Path )
        {
            if ( ViderePlayer.GetComponent<StateComponent>( ).CurrentState != StateComponent.PlayerState.Stopped )
                throw new Exception( "Attempting to load media while playing." );

            FileInfo info = new FileInfo( Path );

            ViderePlayer.MediaPlayer.LoadMedia( info );
        }

        /// <summary>
        /// Looks for all media files which the <see cref="ViderePlayer"/> can play.
        /// </summary>
        /// <param name="directory">The directory to look inside for all media files.</param>
        /// <param name="recursive">Whether or not to look into subdirectories recursively.</param>
        /// <returns>A list of all media files found in the directory.</returns>
        public List<VidereMedia> FindMediaInDirectory( string directory, bool recursive = true )
        {
            List<VidereMedia> media = new List<VidereMedia>( );

            DirectoryInfo info = new DirectoryInfo( directory );

            if ( !info.Exists )
                return media;

            FileInfo[ ] files = info.GetFiles( );
            foreach ( FileInfo file in files )
            {
                if ( ViderePlayer.MediaPlayer.CanPlayMediaExtension( file.Extension.Substring( 1 ) ) )
                    media.Add( new VidereMedia( file ) );
            }

            if ( !recursive ) return media;

            DirectoryInfo[ ] subDirs = info.GetDirectories( );
            foreach ( DirectoryInfo subDir in subDirs )
                media.AddRange( FindMediaInDirectory( subDir.FullName ) );

            return media;
        }

        /// <summary>
        /// Gets information about a for <see cref="VidereMedia"/>.
        /// </summary>
        /// <param name="medias">The <see cref="VidereMedia"/> for which we want the information.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public async Task RetrieveMediaInformation( params VidereMedia[ ] medias )
        {
            string[ ] hashes = new string[ medias.Length ];
            Dictionary<string, VidereMedia> hashMedias = new Dictionary<string, VidereMedia>( );
            for ( int x = 0; x < medias.Length; x++ )
            {
                if ( medias[ x ].Type != VidereMedia.MediaType.Video )
                    return;

                string hash = Hasher.ComputeMovieHash( medias[ x ].File.FullName );
                hashes[ x ] = hash;
                hashMedias.Add( hash, medias[ x ] );
            }

            CheckMovieHashOutput output = await Interface.CheckMovieHashBestGuessOnly( hashes );
            foreach ( var pair in output.MovieData )
            {
                if ( pair.Value.Length <= 0 ) continue;
                MovieData data = pair.Value[ 0 ];

                VidereMedia media = hashMedias[ pair.Key ];
                MovieInformation movieInfo = new MovieInformation
                {
                    Hash = data.MovieHash,
                    IMDBID = data.MovieImbdID,
                    Name = data.MovieName,
                    Year = data.MovieYear
                };

                media.MovieInfo = movieInfo;
            }
        }

        /// <summary>
        /// Unloads the currently loaded media.
        /// </summary>
        public void UnloadMedia( )
        {
            ViderePlayer.MediaPlayer.UnloadMedia( );
        }
    }
}
