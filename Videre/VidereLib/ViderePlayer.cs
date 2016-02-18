using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Threading;
using VidereLib.Components;
using VidereLib.Data;
using VidereLib.Players;

namespace VidereLib
{
    /// <summary>
    /// The media player controller.
    /// </summary>
    public static class ViderePlayer
    {
        /// <summary>
        /// The directory in which the program can be found.
        /// </summary>
        public static DirectoryInfo ProgramDirectory => new DirectoryInfo( Path.GetDirectoryName( Assembly.GetEntryAssembly( ).Location ) );

        internal static WindowData windowData { private set; get; }

        private static readonly Dictionary<Type, ComponentBase> components = new Dictionary<Type, ComponentBase>( );

        /// <summary>
        /// The dispatcher to dispatch to the main thread.
        /// </summary>
        public static Dispatcher MainDispatcher { private set; get; }

        /// <summary>
        /// The <see cref="MediaPlayerBase"/> used to play the media.
        /// </summary>
        public static MediaPlayerBase MediaPlayer => windowData.MediaPlayer;

        /// <summary>
        /// Whether or not the player has been initialized yet.
        /// </summary>
        public static bool IsInitialized { private set; get; }

        /// <summary>
        /// Initializes the <see cref="ViderePlayer"/>.
        /// </summary>
        /// <param name="data">The data of the main window.</param>
        public static void Initialize( WindowData data )
        {
            if ( IsInitialized )
                throw new Exception( "Attempting to initialize player twice." );

            windowData = data;
            data.Window.Closing += ( Sender, Args ) => data.MediaPlayer.Dispose( );

            MainDispatcher = data.Window.Dispatcher;

            LoadComponents( );
            InitializeComponents( );
            IsInitialized = true;
        }

        private static void LoadComponents( )
        {
            Assembly A = Assembly.GetExecutingAssembly( );

            foreach ( Type T in A.GetTypes( ).Where( T => T.IsSubclassOf( typeof ( ComponentBase ) ) ) )
                components.Add( T, ( ComponentBase ) Activator.CreateInstance( T ) );
        }

        private static void InitializeComponents( )
        {
            foreach ( KeyValuePair<Type, ComponentBase> pair in components )
                pair.Value.Initialize( );
        }

        /// <summary>
        /// Gets the <typeparamref name="T"/> in the <see cref="ViderePlayer"/>.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="ComponentBase"/>.</typeparam>
        /// <returns>The component <typeparamref name="T"/>.</returns>
        public static T GetComponent<T>( ) where T : ComponentBase
        {
            return components[ typeof ( T ) ] as T;
        }
    }
}