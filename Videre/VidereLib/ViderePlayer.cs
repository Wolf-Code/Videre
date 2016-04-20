
namespace VidereLib
{
    public abstract class ViderePlayer
    {
        /// <summary>
        /// Plays the currently loaded media, if there is any.
        /// </summary>
        public abstract void Play( );

        /// <summary>
        /// Pauses the currently loaded media, if there is any.
        /// </summary>
        public abstract void Pause( );

        /// <summary>
        /// Stops the currently loaded media, if there is any.
        /// </summary>
        public abstract void Stop( );
    }
}
