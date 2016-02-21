using TMDbLib.Objects.TvShows;

namespace VidereLib.EventArgs
{
    public class OnTvSeasonInformationReceivedEventArgs
    {
        public int ShowID { private set; get; }

        public int SeasonNumber { private set; get; }

        public TvSeason Season { private set; get; }

        public OnTvSeasonInformationReceivedEventArgs( int showID, int seasonNumber, TvSeason season )
        {
            this.ShowID = showID;
            this.SeasonNumber = seasonNumber;
            this.Season = season;
        }
    }
}
