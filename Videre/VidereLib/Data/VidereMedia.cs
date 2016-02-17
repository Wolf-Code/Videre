using System;
using System.IO;
using VidereSubs.OpenSubtitles;

namespace VidereLib.Data
{
    /// <summary>
    /// A container class for information about a media file.
    /// </summary>
    public class VidereMedia
    {
        /// <summary>
        /// The file containing the media.
        /// </summary>
        public FileInfo File { set; get; }

        /// <summary>
        /// The opensubtitles.org hash for the media file.
        /// </summary>
        public string OpenSubtitlesHash => !File.Exists ? null : Hasher.ComputeMovieHash( File.FullName );

        /// <summary>
        /// The duration of the media.
        /// </summary>
        public TimeSpan Duration { set; get; }

        /// <summary>
        /// The name of the media.
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// The <see cref="VideoInfo"/> of the media.
        /// </summary>
        public VideoInfo Video { set; get; }

        /// <summary>
        /// Indicates if the media has video.
        /// </summary>
        public bool HasVideo => Video != null;

        /// <summary>
        /// The <see cref="AudioInfo"/> of the media.
        /// </summary>
        public AudioInfo Audio { set; get; }

        /// <summary>
        /// Indicates if the media has audio.
        /// </summary>
        public bool HasAudio => Audio != null;

        /// <summary>
        /// The different kinds of media.
        /// </summary>
        public enum MediaType
        {
            /// <summary>
            /// A video file.
            /// </summary>
            Video,

            /// <summary>
            /// An audio file.
            /// </summary>
            Audio,

            /// <summary>
            /// An unknown media file.
            /// </summary>
            Unknown
        }

        /// <summary>
        /// The type of the media.
        /// </summary>
        public MediaType Type { private set; get; }

        /// <summary>
        /// The movie information for this media file, if any.
        /// </summary>
        public MovieInformation MovieInfo { set; get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="file">The <see cref="FileInfo"/> of the media.</param>
        public VidereMedia( FileInfo file )
        {
            File = file;
            Name = file.Name;
            Type = GetMediaTypeFromFile( file );
        }

        /// <summary>
        /// Changes the automatically assigned media type.
        /// </summary>
        /// <param name="newType">The new media type.</param>
        public void ChangeMediaType( MediaType newType )
        {
            Type = newType;
        }

        /// <summary>
        /// Gets the media type from the file extension, using the currently active media player.
        /// </summary>
        /// <param name="file">The file to check for.</param>
        /// <returns>The type of the media.</returns>
        public static MediaType GetMediaTypeFromFile( FileInfo file )
        {
            string extension = file.Extension.Substring( 1 );
            if ( ViderePlayer.MediaPlayer.CanPlayVideoExtension( extension ) )
                return MediaType.Video;

            if ( ViderePlayer.MediaPlayer.CanPlayAudioExtension( extension ) )
                return MediaType.Audio;

            return MediaType.Unknown;
        }
    }
}
