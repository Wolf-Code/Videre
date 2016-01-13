using System;
using System.Windows.Controls;
using VidereLib.Components;

namespace VidereLib
{
    public class ViderePlayer
    {
        /// <summary>
        /// True if media has been loaded, false otherwise.
        /// </summary>
        public bool HasMediaBeenLoaded { internal set; get; }

        internal readonly WindowData windowData;

        public InputComponent InputHandler { private set; get; }
        public StateComponent StateHandler { private set; get; }
        public SubtitlesComponent SubtitlesHandler { private set; get; }
        public ScreenComponent ScreenHandler { private set; get; }
        public TimeComponent TimeHandler { private set; get; }

        internal MediaElement MediaPlayer => windowData.MediaPlayer;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ViderePlayer( WindowData data )
        {
            windowData = data;

            InputHandler = new InputComponent( this );
            StateHandler = new StateComponent( this );
            SubtitlesHandler = new SubtitlesComponent( this );
            ScreenHandler = new ScreenComponent( this );
            TimeHandler = new TimeComponent( this );
        }

        #region Media

        /// <summary>
        /// Loads media from a path.
        /// </summary>
        /// <param name="Path">The path of the media.</param>
        public void LoadMedia( string Path )
        {
            if ( this.StateHandler.CurrentState != StateComponent.PlayerState.Stopped )
                throw new Exception( "Attempting to load media while playing." );

            MediaPlayer.Source = new Uri( Path );
            HasMediaBeenLoaded = true;
        }

        #endregion
    }
}
