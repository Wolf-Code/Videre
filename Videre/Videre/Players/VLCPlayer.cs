using System;
using System.IO;
using System.Linq;
using System.Reflection;
using VidereLib;
using VidereLib.Data;
using VidereLib.EventArgs;
using VidereLib.Players;
using xZune.Vlc;
using xZune.Vlc.Interop.Core.Events;
using xZune.Vlc.Interop.Media;
using xZune.Vlc.Wpf;

namespace Videre.Players
{
    /// <summary>
    /// The <see cref="MediaPlayerBase"/> implementation for a VLC-based player.
    /// </summary>
    public class VLCPlayer : MediaPlayerBase
    {
        private readonly VlcPlayer player;
        private VlcMedia lastMedia;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="control">The <see cref="VlcPlayer"/> with which media is played.</param>
        public VLCPlayer( VlcPlayer control )
        {
            player = control;
            player.Initialize( GetLibDirectory( ).FullName, "--no-osd", "--no-sub-autodetect-file" );
            player.VlcMediaPlayer.MediaChanged += PlayerOnMediaChanged;

            VideoFileExtensions = new[ ]
            {
                "mp4", "mkv", "avi", "divx", "f4v", "flv", "mov", "ogg", "ogm"
            };

            AudioFileExtensions = new[ ]
            {
                "mp3", "wav", "flac"
            };
        }

        private void PlayerOnMediaChanged( object sender, MediaPlayerMediaChangedEventArgs e )
        {
            lastMedia = e.NewMedia;
            lastMedia.ParsedChanged += NewMediaOnParsedChanged;
        }

        private async void NewMediaOnParsedChanged( object sender, ObjectEventArgs<MediaParsedChangedArgs> e )
        {
            if ( lastMedia == null )
                throw new Exception( "No media loaded" );

            FileInfo mediaFile = new FileInfo( new Uri( lastMedia.Mrl ).LocalPath );
            VidereMedia media = new VidereMedia( mediaFile )
            {
                Duration = lastMedia.Duration,
                Name = lastMedia.GetMeta( MetaDataType.Title )
            };

            xZune.Vlc.MediaTrack vid = lastMedia.GetTracks( ).FirstOrDefault( O => O.Type == TrackType.Video );
            if ( vid?.VideoTrack != null )
            {
                VideoTrack vidTrack = vid.VideoTrack.Value;
                media.Video = new VideoInfo
                {
                    Width = vidTrack.Width,
                    Height = vidTrack.Height
                };
            }

            xZune.Vlc.MediaTrack aud = lastMedia.GetTracks( ).FirstOrDefault( O => O.Type == TrackType.Audio );
            if ( aud?.AudioTrack != null )
            {
                AudioTrack audTrack = aud.AudioTrack.Value;
                media.Audio = new AudioInfo
                {
                    Channels = audTrack.Channels,
                    Rate = audTrack.Rate
                };
            }

            await ViderePlayer.MainDispatcher.InvokeAsync( ( ) => OnMediaLoaded( new OnMediaLoadedEventArgs( media ) ) );
        }

        private DirectoryInfo GetLibDirectory( )
        {
            var currentAssembly = Assembly.GetEntryAssembly( );
            var currentDirectory = new FileInfo( currentAssembly.Location ).DirectoryName;
            if ( currentDirectory == null )
                return null;

            return AssemblyName.GetAssemblyName( currentAssembly.Location ).ProcessorArchitecture == ProcessorArchitecture.X86 ?
                       new DirectoryInfo( Path.Combine( currentDirectory, @"Content\lib\x86" ) ) :
                       new DirectoryInfo( Path.Combine( currentDirectory, @"Content\lib\x64" ) );
        }

        /// <summary>
        /// Plays the currently loaded media.
        /// </summary>
        public override void Play( )
        {
            player.Play( );
        }

        /// <summary>
        /// Pauses the currently playing media.
        /// </summary>
        public override void Pause( )
        {
            player.VlcMediaPlayer.Pause( );
        }

        /// <summary>
        /// Stops the currently loaded media and unloads it.
        /// </summary>
        protected override void OnStop( )
        {
            player.BeginStop( );
        }

        /// <summary>
        /// Sets the media player's volume.
        /// </summary>
        /// <param name="volume">0 means no volume, 1 means full volume.</param>
        public override void SetVolume( float volume )
        {
            player.Volume = ( int ) ( volume * 100 );
        }

        /// <summary>
        /// Loads media into the media player.
        /// </summary>
        /// <param name="file">The media file.</param>
        public override void LoadMedia( FileInfo file )
        {
            player.BeginStop( ( ) =>
            {
                player.LoadMedia( file.FullName );
            } );
        }

        /// <summary>
        /// Unloads the currently loaded media.
        /// </summary>
        public override void UnloadMedia( )
        {
            OnMediaUnloaded( new OnMediaUnloadedEventArgs( ) );
        }

        /// <summary>
        /// Gets the length of the loaded media.
        /// </summary>
        /// <returns>The length of the loaded media.</returns>
        public override TimeSpan GetMediaLength( )
        {
            return player.Length;
        }

        /// <summary>
        /// Gets the position in the loaded media.
        /// </summary>
        /// <returns>The position in the loaded media.</returns>
        public override TimeSpan GetPosition( )
        {
            return TimeSpan.FromSeconds( player.Position * player.Length.TotalSeconds );
        }

        /// <summary>
        /// Sets the position in the loaded media.
        /// </summary>
        /// <param name="time">The time to move to.</param>
        public override void SetPosition( TimeSpan time )
        {
            player.Position = ( float ) ( time.TotalSeconds / GetMediaLength( ).TotalSeconds );
        }

        /// <summary>
        /// Gets the currently loaded media's aspect ratio.
        /// </summary>
        /// <returns>The aspect ratio of the currently loaded media.</returns>
        public override float GetAspectRatio( )
        {
            if ( !IsMediaLoaded )
                return 1f;

            return Media.Video.Width / ( float ) Media.Video.Height;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose( )
        {
            player.BeginStop( ( ) => player.Dispose( ) );
        }
    }
}