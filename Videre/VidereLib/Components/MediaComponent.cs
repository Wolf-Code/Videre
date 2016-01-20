using System;
using System.IO;
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
        public bool HasMediaBeenLoaded => Player.MediaPlayer.IsMediaLoaded;

        /// <summary>
        /// The currently loaded media.
        /// </summary>
        public FileInfo Media => Player.MediaPlayer.Media;

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
            Player.MediaPlayer.MediaFailedToLoad += ( sender, args ) => OnMediaFailedToLoad?.Invoke( this, args );
            Player.MediaPlayer.MediaLoaded += ( sender, args ) => OnMediaLoaded?.Invoke( this, args );
            Player.MediaPlayer.MediaUnloaded += ( sender, args ) => OnMediaUnloaded?.Invoke( this, args );
        }

        /// <summary>
        /// Gets the length of the media as a <see cref="TimeSpan"/>.
        /// </summary>
        /// <returns>The length of the media.</returns>
        public TimeSpan GetMediaLength( )
        {
            if ( !HasMediaBeenLoaded )
                throw new Exception( "Attempted to get media length while there is no media loaded." );

            return Player.MediaPlayer.GetMediaLength( );
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

            Player.MediaPlayer.LoadMedia( info );
        }

        /// <summary>
        /// Unloads the currently loaded media.
        /// </summary>
        public void UnloadMedia( )
        {
            Player.MediaPlayer.UnloadMedia( );
        }
    }
}
