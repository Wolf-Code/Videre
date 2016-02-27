using VidereLib.Players;

namespace VidereLib.Components
{
    /// <summary>
    /// The <see cref="ComponentBase"/> for audio.
    /// </summary>
    public class AudioComponent : ComponentBase
    {
        /// <summary>
        /// Sets the loaded <see cref="MediaPlayerBase"/>'s volume.
        /// </summary>
        /// <param name="volume"></param>
        public void SetVolume( float volume )
        {
            ViderePlayer.MediaPlayer.SetVolume( volume );
        }
    }
}
