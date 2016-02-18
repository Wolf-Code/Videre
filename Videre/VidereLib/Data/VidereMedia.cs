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
        /// The name of the media.
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// The opensubtitles.org hash for the media file.
        /// </summary>
        public string OpenSubtitlesHash => !File.Exists ? null : Hasher.ComputeMovieHash( File.FullName );

        /// <summary>
        /// The information for this media file, if any.
        /// </summary>
        public VidereMediaInformation MediaInformation { set; get; }

        /// <summary>
        /// The file information.
        /// </summary>
        public VidereFileInformation FileInformation { set; get; }

        /// <summary>
        /// Checks whether or not this <see cref="VidereMedia"/> has an IMDB ID and has thus been processed by the opensubtitles.org api.
        /// </summary>
        public bool HasImdbID => MediaInformation?.IMDBID != null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="file">The <see cref="FileInfo"/> of the media.</param>
        public VidereMedia( FileInfo file )
        {
            File = file;
            Name = file.Name;
        }
    }
}
