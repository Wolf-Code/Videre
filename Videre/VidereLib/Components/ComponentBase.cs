namespace VidereLib.Components
{
    public abstract class ComponentBase
    {
        protected ViderePlayer Player { set; get; }

        protected ComponentBase( ViderePlayer player )
        {
            this.Player = player;
        }
    }
}
