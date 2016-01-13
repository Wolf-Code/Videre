
using VidereLib.Components;

namespace VidereLib.EventArgs
{
    public class OnStateChangedEventArgs : System.EventArgs
    {
        public StateComponent.PlayerState State { private set; get; }

        public OnStateChangedEventArgs( StateComponent.PlayerState state )
        {
            this.State = state;
        }
    }
}
