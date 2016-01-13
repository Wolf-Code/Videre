using MahApps.Metro.Controls;

namespace VidereLib.Components
{
    public class InputComponent : ComponentBase 
    {
        private MetroWindow window;

        public InputComponent( ViderePlayer Player, MetroWindow Window ) : base( Player )
        {
            this.window = Window;
        }
    }
}
