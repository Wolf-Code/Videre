using System;
using System.IO;
using System.Reflection;
using VidereLib;
using VidereLib.Data;
using VidereLib.EventArgs;
using VidereLib.Players;
using xZune.Vlc;
using xZune.Vlc.Interop.Core.Events;
using xZune.Vlc.Wpf;

namespace Videre.Players
{
    class VLCPlayer : MediaPlayerBase
    {
        private readonly VlcPlayer player;
        private VlcMedia lastMedia;

        public VLCPlayer( VlcPlayer control )
        {
            this.player = control;
            this.player.Initialize( GetLibDirectory( ).FullName );
            this.player.VlcMediaPlayer.MediaChanged += PlayerOnMediaChanged;

            VideoFileExtensions = new[ ]
            {
                "mp4", "mkv", "avi", "divx", "f4v", "flv", "mov", "ogg", "ogm"
            };

            AudioFileExtensions = new[ ]
            {
                "mp3", "wav"
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
                Name = mediaFile.Name
            };

            await ViderePlayer.MainDispatcher.InvokeAsync( ( ) => OnMediaLoaded( new OnMediaLoadedEventArgs( media ) ) );
        }

        private DirectoryInfo GetLibDirectory( )
        {
            var currentAssembly = Assembly.GetEntryAssembly( );
            var currentDirectory = new FileInfo( currentAssembly.Location ).DirectoryName;
            if ( currentDirectory == null )
                return null;

            return AssemblyName.GetAssemblyName( currentAssembly.Location ).ProcessorArchitecture == ProcessorArchitecture.X86 ? 
                new DirectoryInfo( Path.Combine( currentDirectory, @"D:\Folders\Documents\Github\Videre\lib\x86" ) ) : 
                new DirectoryInfo( Path.Combine( currentDirectory, @"D:\Folders\Documents\Github\Videre\lib\x64" ) );
        }

        public override void Play( )
        {
            this.player.Play();
        }

        public override void Pause( )
        {
            this.player.VlcMediaPlayer.Pause(  );
        }

        public override void Stop( )
        {
            this.player.Stop(  );
        }

        public override void SetVolume( float volume )
        {
            this.player.Volume = ( int ) ( volume * 100 );
        }

        public override void LoadMedia( FileInfo file )
        {
            this.player.BeginStop( ( ) =>
            {
                this.player.LoadMedia( file.FullName );
            } );
        }

        public override void UnloadMedia( )
        {
            
        }

        public override TimeSpan GetMediaLength( )
        {
            return this.player.Length;
        }

        public override TimeSpan GetPosition( )
        {
            return TimeSpan.FromSeconds( this.player.Position * this.player.Length.TotalSeconds );
        }

        public override void SetPosition( TimeSpan time )
        {
            this.player.Position = ( float ) ( time.TotalSeconds / GetMediaLength( ).TotalSeconds );
        }

        public override float GetAspectRatio( )
        {
            return 1f;
        }

        public override void Dispose( )
        {
            this.player.Dispose( );
        }
    }
}
