using System;
using System.IO;
using System.Windows;
using VidereLib.EventArgs;

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
        public bool HasMediaBeenLoaded => Media != null;

        /// <summary>
        /// The currently loaded media.
        /// </summary>
        public FileInfo Media => lastLoadedFile;

        private FileInfo lastLoadedFile;

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
        /// Constructor.
        /// </summary>
        /// <param name="player">The <see cref="ViderePlayer"/>.</param>
        public MediaComponent( ViderePlayer player ) : base( player )
        {
        }

        /// <summary>
        /// Initializes the <see cref="MediaComponent"/>.
        /// </summary>
        protected override void OnInitialize( )
        {
            Player.windowData.MediaPlayer.MediaFailed += MediaPlayerOnMediaFailed;
            Player.windowData.MediaPlayer.MediaOpened += MediaPlayerOnMediaOpened;
        }

        private void MediaPlayerOnMediaOpened( object Sender, RoutedEventArgs Args )
        {
            OnMediaLoaded?.Invoke( this, new OnMediaLoadedEventArgs( lastLoadedFile ) );
        }

        private void MediaPlayerOnMediaFailed( object Sender, ExceptionRoutedEventArgs ExceptionRoutedEventArgs )
        {
            OnMediaFailedToLoad?.Invoke( this, new OnMediaFailedToLoadEventArgs( ExceptionRoutedEventArgs.ErrorException, lastLoadedFile ) );
            lastLoadedFile = null;
        }

        /// <summary>
        /// Gets the length of the media as a <see cref="TimeSpan"/>.
        /// </summary>
        /// <returns>The length of the media.</returns>
        public TimeSpan GetMediaLength( )
        {
            if ( !HasMediaBeenLoaded )
                throw new Exception( "Attempted to get media length while there is no media loaded." );

            return Player.windowData.MediaPlayer.NaturalDuration.TimeSpan;
        }

        /// <summary>
        /// Loads media from a path.
        /// </summary>
        /// <param name="Path">The path of the media.</param>
        public void LoadMedia( string Path )
        {
            if ( Player.GetComponent<StateComponent>( ).CurrentState != StateComponent.PlayerState.Stopped )
                throw new Exception( "Attempting to load media while playing." );

            FileInfo info = new FileInfo( Path );

            lastLoadedFile = info;

            Player.windowData.MediaPlayer.Source = new Uri( info.FullName );
            Player.windowData.MediaPlayer.Play( );
            Player.windowData.MediaPlayer.Stop( );
        }

        /// <summary>
        /// Unloads the currently loaded media.
        /// </summary>
        public void UnloadMedia( )
        {
            lastLoadedFile = null;
            OnMediaUnloaded?.Invoke( this, new OnMediaUnloadedEventArgs( ) );
        }
    }
}
