using System;
using System.Diagnostics;
using System.Linq;

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
            Process[ ] processes = Process.GetProcessesByName( Process.GetCurrentProcess( ).ProcessName );
            if ( processes.Length > 1 )
            {
                Process p = processes.FirstOrDefault( O => O != Process.GetCurrentProcess( ) );
                string[ ] args = Environment.GetCommandLineArgs( );
                if ( args.Length > 1 )
                    p.StandardInput.WriteLine( Environment.GetCommandLineArgs( )[ 1 ] );

                return;
            }

            App app = new App( );
            app.InitializeComponent( );
            app.Run( );
        }
    }
}