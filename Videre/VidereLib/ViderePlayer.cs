using System;
using System.Collections.Generic;
using System.Reflection;
using VidereLib.Components;
using VidereLib.Players;

namespace VidereLib
{
    /// <summary>
    /// The media player controller.
    /// </summary>
    public class ViderePlayer
    {
        internal readonly WindowData windowData;

        private readonly Dictionary<Type, ComponentBase> components = new Dictionary<Type, ComponentBase>( );

        /// <summary>
        /// The <see cref="MediaPlayerBase"/> used to play the media.
        /// </summary>
        public MediaPlayerBase MediaPlayer => windowData.MediaPlayer;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ViderePlayer( WindowData data )
        {
            windowData = data;

            this.LoadComponents( );
            this.InitializeComponents( );
        }

        private void LoadComponents( )
        {
            Assembly A = Assembly.GetExecutingAssembly( );

            foreach ( Type T in A.GetTypes( ) )
                if ( T.IsSubclassOf( typeof ( ComponentBase ) ) )
                    components.Add( T, ( ComponentBase ) Activator.CreateInstance( T, this ) );
        }

        private void InitializeComponents( )
        {
            foreach ( KeyValuePair<Type, ComponentBase> pair in components )
                pair.Value.Initialize( );
        }

        /// <summary>
        /// Gets the <typeparamref name="T"/> in the <see cref="ViderePlayer"/>.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="ComponentBase"/>.</typeparam>
        /// <returns>The component <typeparamref name="T"/>.</returns>
        public T GetComponent<T>( ) where T : ComponentBase
        {
            return components[ typeof ( T ) ] as T;
        }
    }
}