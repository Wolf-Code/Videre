using System;

namespace Videre
{
    internal class Startup
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