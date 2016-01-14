﻿using System;

namespace VidereLib.EventArgs
{
    public class OnPositionChangedEventArgs : System.EventArgs
    {
        public TimeSpan Position { private set; get; }

        public double Progress { private set; get; }

        public OnPositionChangedEventArgs( TimeSpan position, double progress )
        {
            Position = position;
            Progress = progress;
        }
    }
}