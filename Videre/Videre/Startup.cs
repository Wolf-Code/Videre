using System;

namespace Videre
{
    class Startup
    {
        /// <summary>
        /// Application Entry Point.
        /// </summary>
        [STAThread]
        public static void Main( )
        {
            App app = new App( );
            app.InitializeComponent( );
            app.Run( );
        }
    }
}