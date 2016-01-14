using System;

namespace VidereLib.Components
{
    /// <summary>
    /// The base class for all components.
    /// </summary>
    public abstract class ComponentBase
    {
        /// <summary>
        /// The <see cref="ViderePlayer"/> this component is attached to.
        /// </summary>
        protected ViderePlayer Player { set; get; }

        private bool isInitialized;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="player">The <see cref="ViderePlayer"/> this component is attached to.</param>
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

        /// <summary>
        /// Gets called after all components have been added to the player.
        /// </summary>
        protected virtual void OnInitialize( )
        {

        }
    }
}
