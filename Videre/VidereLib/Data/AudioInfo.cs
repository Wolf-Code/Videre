
namespace VidereLib.Data
{
    /// <summary>
    /// Info about audio of a <see cref="VidereMedia"/>.
    /// </summary>
    public class AudioInfo
    {
        /// <summary>
        /// The amount of channels.
        /// </summary>
        public uint Channels { set; get; }

        /// <summary>
        /// The rate of the audio.
        /// </summary>
        public uint Rate { set; get; }
    }
}
