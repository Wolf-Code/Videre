using System;

namespace VidereLib.EventArgs
{
    /// <summary>
    /// The <see cref="System.EventArgs"/> for when the position in the media has been changed.
    /// </summary>
    public class OnPositionChangedEventArgs : System.EventArgs
    {
        /// <summary>
        /// The new position.
        /// </summary>
        public TimeSpan Position { private set; get; }

        /// <summary>
        /// The progress in the media, 0 being the start and 1 being the end.
        /// </summary>
        public double Progress { private set; get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="position">The new position.</param>
        /// <param name="progress">The progress.</param>
        public OnPositionChangedEventArgs( TimeSpan position, double progress )
        {
            Position = position;
            Progress = progress;
        }
    }
}
