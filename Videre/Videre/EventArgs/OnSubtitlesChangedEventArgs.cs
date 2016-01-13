
namespace Videre.EventArgs
{
    class OnSubtitlesChangedEventArgs : System.EventArgs
    {
        public SubtitleData Subtitles { private set; get; }

        public OnSubtitlesChangedEventArgs( SubtitleData subtitleData )
        {
            Subtitles = subtitleData;
        }
    }
}
