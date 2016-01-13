
namespace Videre.EventArgs
{
    class OnStateChangedEventArgs : System.EventArgs
    {
        public ViderePlayer.PlayerState State { private set; get; }

        public OnStateChangedEventArgs( ViderePlayer.PlayerState state )
        {
            this.State = state;
        }
    }
}
