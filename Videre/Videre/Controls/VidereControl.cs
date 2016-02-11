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
        /// Whether or not the player has been initialized yet.
        /// </summary>
        public bool IsPlayerInitialized => ViderePlayer.IsInitialized;

        /// <summary>
        /// Gets called whenever the player has been initialized.
        /// </summary>
        public virtual void OnPlayerInitialized( )
        {
            
        }
    }
}
