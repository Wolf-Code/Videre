using System.Windows.Controls;
using Videre.Windows;
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
        protected ViderePlayer Player => MainWindow.Player;

        /// <summary>
        /// Whether or not the player has been initialized yet.
        /// </summary>
        public bool IsPlayerInitialized => Player != null;

        /// <summary>
        /// Gets called whenever the player has been initialized.
        /// </summary>
        public virtual void OnPlayerInitialized( )
        {
            
        }
    }
}
