using System;

namespace VidereLib.Components
{
    /// <summary>
    /// The base class for all components.
    /// </summary>
    public abstract class ComponentBase
    {
        private bool isInitialized;

        internal void Initialize( )
        {
            if ( isInitialized )
                throw new Exception( $"Attempting to initialize {this} twice." );

            this.OnInitialize( );
            isInitialized = true;
        }

        /// <summary>
        /// Gets called after all components have been added to the player.
        /// </summary>
        protected virtual void OnInitialize( )
        {

        }
    }
}
