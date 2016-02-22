using TMDbLib.Objects.TvShows;

namespace VidereLib.EventArgs
{
    /// <summary>
    /// The <see cref="System.EventArgs"/> for when season information has been received.
    /// </summary>
    public class OnTvSeasonInformationReceivedEventArgs
    {
        /// <summary>
        /// The ID of the show.
        /// </summary>
        public int ShowID { private set; get; }

        /// <summary>
        /// The season's number.
        /// </summary>
        public int SeasonNumber { private set; get; }

        /// <summary>
        /// The season information.
        /// </summary>
        public TvSeason Season { private set; get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="showID">The ID of the show.</param>
        /// <param name="seasonNumber">The season's number.</param>
        /// <param name="season">The season information.</param>
        public OnTvSeasonInformationReceivedEventArgs( int showID, int seasonNumber, TvSeason season )
        {
            this.ShowID = showID;
            this.SeasonNumber = seasonNumber;
            this.Season = season;
        }
    }
}
