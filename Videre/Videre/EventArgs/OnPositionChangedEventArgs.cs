using System;

namespace Videre.EventArgs
{
    class OnPositionChangedEventArgs : System.EventArgs
    {
        public TimeSpan Position { private set; get; }

        public double Progress { private set; get; }

        public OnPositionChangedEventArgs( TimeSpan position, double progress )
        {
            this.Position = position;
            this.Progress = progress;
        }
    }
}
