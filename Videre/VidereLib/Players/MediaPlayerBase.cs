using System;
using System.Collections.Generic;
using System.IO;
using VidereLib.Data;
using VidereLib.EventArgs;

namespace VidereLib.Players
{
    /// <summary>
    /// The interface for a media player.
    /// </summary>
    public abstract class MediaPlayerBase : IDisposable
    {
        private string[ ] m_VideoFileExtensions;
        private string[ ] m_AudioFileExtensions;

        private HashSet<string> videoFileHashSet = new HashSet<string>( );
        private HashSet<string> audioFileHashSet = new HashSet<string>( );  

        /// <summary>
        /// The event that is called whenever media failed to load.
        /// </summary>
        public event EventHandler<OnMediaFailedToLoadEventArgs> MediaFailedToLoad;

        /// <summary>
        /// The event that is called whenever media has loaded.
        /// </summary>
        public event EventHandler<OnMediaLoadedEventArgs> MediaLoaded;

        /// <summary>
        /// The event that is called whenever media has been unloaded.
        /// </summary>
        public event EventHandler<OnMediaUnloadedEventArgs> MediaUnloaded;

        /// <summary>
        /// The supported video file extensions.
        /// </summary>
        public string[ ] VideoFileExtensions
        {
            protected set
            {
                m_VideoFileExtensions = value;
                videoFileHashSet.Clear( );

                foreach ( string extension in value )
                    videoFileHashSet.Add( extension );
            }
            get { return m_VideoFileExtensions; }
        }

        /// <summary>
        /// The supported audio file extensions.
        /// </summary>
        public string[ ] AudioFileExtensions
        {
            protected set
            {
                m_AudioFileExtensions = value;

                audioFileHashSet.Clear( );

                foreach ( string extension in value )
                    audioFileHashSet.Add( extension );
            }
            get { return m_AudioFileExtensions; }
        }
        
        /// <summary>
        /// Indicates if any media has been loaded.
        /// </summary>
        public bool IsMediaLoaded => Media != null && Media.File.Exists;

        /// <summary>
        /// The loaded media.
        /// </summary>
        public VidereMedia Media { private set; get; }

        /// <summary>
        /// Calls the <see cref="MediaLoaded"/> event.
        /// </summary>
        /// <param name="args">The event args.</param>
        protected virtual void OnMediaLoaded( OnMediaLoadedEventArgs args )
        {
            Media = args.MediaFile;
            MediaLoaded?.Invoke( this, args );
        }

        /// <summary>
        /// Calls the <see cref="MediaUnloaded"/> event.
        /// </summary>
        /// <param name="args">The event args.</param>
        protected virtual void OnMediaUnloaded( OnMediaUnloadedEventArgs args )
        {
            Media = null;
            MediaUnloaded?.Invoke( this, args );
        }

        /// <summary>
        /// Calls the <see cref="MediaFailedToLoad"/> event.
        /// </summary>
        /// <param name="args">The event args.</param>
        protected virtual void OnMediaFailedToLoad( OnMediaFailedToLoadEventArgs args )
        {
            Media = null;
            MediaFailedToLoad?.Invoke( this, args );
        }

        /// <summary>
        /// Checks if the <see cref="MediaPlayerBase"/> can play a given video file extension using a hashset for speed.
        /// </summary>
        /// <param name="extension">The extension to check for.</param>
        /// <returns>True if it can be played, false otherwise.</returns>
        public bool CanPlayVideoExtension( string extension )
        {
            return videoFileHashSet.Contains( extension );
        }

        /// <summary>
        /// Checks if the <see cref="MediaPlayerBase"/> can play a given audio file extension using a hashset for speed.
        /// </summary>
        /// <param name="extension">The extension to check for.</param>
        /// <returns>True if it can be played, false otherwise.</returns>
        public bool CanPlayAudioExtension( string extension )
        {
            return audioFileHashSet.Contains( extension );
        }

        /// <summary>
        /// Checks if the <see cref="MediaPlayerBase"/> can play the media file extension.
        /// </summary>
        /// <param name="extension">The file extension to check for.</param>
        /// <returns>True if it can be played, false otherwise.</returns>
        public bool CanPlayMediaExtension( string extension )
        {
            return CanPlayVideoExtension( extension ) || CanPlayAudioExtension( extension );
        }

        /// <summary>
        /// Plays the currently loaded media.
        /// </summary>
        public abstract void Play( );

        /// <summary>
        /// Pauses the currently playing media.
        /// </summary>
        public abstract void Pause( );

        /// <summary>
        /// Stops the currently loaded media and unloads it.
        /// </summary>
        public void Stop( )
        {
            if ( !IsMediaLoaded )
                return;

            OnStop( );
            Media = null;
        }
        
        /// <summary>
        /// Called whenever the player is stopped.
        /// </summary>
        protected abstract void OnStop( );

        /// <summary>
        /// Sets the media player's volume.
        /// </summary>
        /// <param name="volume">0 means no volume, 1 means full volume.</param>
        public abstract void SetVolume( float volume );

        /// <summary>
        /// Loads media into the media player.
        /// </summary>
        /// <param name="file">The media file.</param>
        public abstract void LoadMedia( FileInfo file );

        /// <summary>
        /// Unloads the currently loaded media.
        /// </summary>
        public abstract void UnloadMedia( );

        /// <summary>
        /// Gets the length of the loaded media.
        /// </summary>
        /// <returns>The length of the loaded media.</returns>
        public abstract TimeSpan GetMediaLength( );

        /// <summary>
        /// Gets the position in the loaded media.
        /// </summary>
        /// <returns>The position in the loaded media.</returns>
        public abstract TimeSpan GetPosition( );

        /// <summary>
        /// Sets the position in the loaded media.
        /// </summary>
        /// <param name="time">The time to move to.</param>
        public abstract void SetPosition( TimeSpan time );

        /// <summary>
        /// Gets the currently loaded media's aspect ratio.
        /// </summary>
        /// <returns>The aspect ratio of the currently loaded media.</returns>
        public abstract float GetAspectRatio( );

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public abstract void Dispose( );
    }
}
