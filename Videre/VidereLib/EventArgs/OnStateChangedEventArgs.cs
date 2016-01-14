
using VidereLib.Components;

namespace VidereLib.EventArgs
{
    /// <summary>
    /// The <see cref="System.EventArgs"/> for when the state has been changed.
    /// </summary>
    public class OnStateChangedEventArgs : System.EventArgs
    {
        /// <summary>
        /// The new <see cref="StateComponent.PlayerState"/>.
        /// </summary>
        public StateComponent.PlayerState State { private set; get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="state">The new <see cref="StateComponent.PlayerState"/>.</param>
        public OnStateChangedEventArgs( StateComponent.PlayerState state )
        {
            State = state;
        }
    }
}
