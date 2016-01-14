using System;

namespace VidereLib.Components
{
    public abstract class ComponentBase
    {
        protected ViderePlayer Player { set; get; }

        private bool isInitialized;

        protected ComponentBase( ViderePlayer player )
        {
            Player = player;
        }

        internal void Initialize( )
        {
            if ( isInitialized )
                throw new Exception( $"Attempting to initialize {this} twice." );

            this.OnInitialize( );
            isInitialized = true;
        }

        protected virtual void OnInitialize( )
        {

        }
    }
}
