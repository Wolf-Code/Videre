using System.Windows.Controls;
using VidereLib;

namespace Videre.Controls
{
    /// <summary>
    /// The base for all control elements.
    /// </summary>
    public class VidereControl : UserControl
    {
        /// <summary>
        /// The videre player.
        /// </summary>
        protected ViderePlayer Player { private set; get; }

        /// <summary>
        /// Whether or not the player has been initialized yet.
        /// </summary>
        public bool IsPlayerInitialized => Player != null;

        /// <summary>
        /// The initialization method, which saves the player.
        /// </summary>
        /// <param name="player">The videre player.</param>
        public void Initialize( ViderePlayer player )
        {
            this.Player = player;

            this.OnInitializedPlayer( );
        }

        /// <summary>
        /// Gets called after the element is initialized with a <see cref="ViderePlayer"/>.
        /// </summary>
        protected virtual void OnInitializedPlayer( )
        {

        }
    }
}
