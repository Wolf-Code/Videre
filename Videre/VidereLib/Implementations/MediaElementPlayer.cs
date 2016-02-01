using System;
using System.IO;
using System.Windows.Controls;
using VidereLib.Data;
using VidereLib.EventArgs;
using VidereLib.Players;

namespace VidereLib.Implementations
{
    /// <summary>
    /// An implementation of the <see cref="MediaPlayerBase"/> for a <see cref="MediaElement"/>
    /// </summary>
    public class MediaElementPlayer : MediaPlayerBase
    {
        private readonly MediaElement player;
        private FileInfo lastLoaded;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="element">The <see cref="MediaElement"/> for the player.</param>
        public MediaElementPlayer( MediaElement element )
        {
            this.player = element;
            this.player.MediaOpened += ( Sender, Args ) =>
            {
                VidereMedia media = new VidereMedia( lastLoaded )
                {
                    Duration = this.player.NaturalDuration.TimeSpan,
                    Name = lastLoaded.Name
                };
                OnMediaLoaded( new OnMediaLoadedEventArgs( media ) );
            };
            this.player.MediaFailed += ( Sender, Args ) => OnMediaFailedToLoad( new OnMediaFailedToLoadEventArgs( Args.ErrorException, Media ) );

            VideoFileExtensions = new[ ]
            {
                "mp4", "mkv", "avi"
            };

            AudioFileExtensions = new[ ]
            {
                "mp3", "wav"
            };
        }

        /// <summary>
        /// Plays the currently loaded media.
        /// </summary>
        public override void Play( )
        {
            if ( !this.IsMediaLoaded )
                return;

            player.Play( );
        }

        /// <summary>
        /// Pauses the currently playing media.
        /// </summary>
        public override void Pause( )
        {
            if ( !this.IsMediaLoaded )
                return;
            
            player.Pause( );
        }

        /// <summary>
        /// Stops the currently loaded media and unloads it.
        /// </summary>
        public override void Stop( )
        {
            if ( !this.IsMediaLoaded )
                return;

            player.Stop( );
        }

        /// <summary>
        /// Sets the media player's volume.
        /// </summary>
        /// <param name="volume">0 means no volume, 1 means full volume.</param>
        public override void SetVolume( float volume )
        {
            player.Volume = volume;
        }

        /// <summary>
        /// Loads media into the media player.
        /// </summary>
        /// <param name="file">The media file.</param>
        public override void LoadMedia( FileInfo file )
        {
            lastLoaded = file;
            player.Source = new Uri( file.FullName );
            player.Play( );
            player.Stop( );
        }

        /// <summary>
        /// Unloads the currently loaded media.
        /// </summary>
        public override void UnloadMedia( )
        {
            player.Source = null;
            player.Stop( );

            OnMediaUnloaded( new OnMediaUnloadedEventArgs( ) );
        }

        /// <summary>
        /// Gets the length of the loaded media.
        /// </summary>
        /// <returns>The length of the loaded media.</returns>
        public override TimeSpan GetMediaLength( )
        {
            return player.NaturalDuration.TimeSpan;
        }

        /// <summary>
        /// Gets the position in the loaded media.
        /// </summary>
        /// <returns>The position in the loaded media.</returns>
        public override TimeSpan GetPosition( )
        {
            return player.Position;
        }

        /// <summary>
        /// Sets the position in the loaded media.
        /// </summary>
        /// <param name="time">The time to move to.</param>
        public override void SetPosition( TimeSpan time )
        {
            player.Position = time;
        }

        /// <summary>
        /// Gets the currently loaded media's aspect ratio.
        /// </summary>
        /// <returns>The aspect ratio of the currently loaded media.</returns>
        public override float GetAspectRatio( )
        {
            return player.NaturalVideoWidth / ( float ) player.NaturalVideoWidth;
        }
    }
}
