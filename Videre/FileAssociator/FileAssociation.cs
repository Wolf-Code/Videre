using System;
using Microsoft.Win32;

namespace VidereFileAssociator
{
    /// <summary>
    /// A utility class to associate file extensions with a program.
    /// </summary>
    public static class FileAssociation
    {
        /// <summary>
        /// Associates file extensions with a program ID, description, icon and an application.
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="progID"></param>
        /// <param name="description"></param>
        /// <param name="icon"></param>
        public static void Associate( string extension, string progID, string executablePath, string description, string icon )
        {
            Registry.ClassesRoot.CreateSubKey( extension ).SetValue( "", progID );
            if ( string.IsNullOrEmpty( progID ) ) return;

            Console.WriteLine( $"Associating {extension} with {progID}" );

            using ( RegistryKey key = Registry.ClassesRoot.CreateSubKey( progID ) )
            {
                if ( description != null )
                    key.SetValue( "", description );

                if ( icon != null )
                    key.CreateSubKey( "DefaultIcon" ).SetValue( "", icon );

                key.CreateSubKey( @"Shell\Open\Command" ).SetValue( "", executablePath + " \"%1\"" );
            }
        }

        [System.Runtime.InteropServices.DllImport( "Shell32.dll" )]
        private static extern int SHChangeNotify( int eventId, int flags, IntPtr item1, IntPtr item2 );

        public static void NotifyFileExplorer( )
        {
            SHChangeNotify( 0x8000000, 0x1000, IntPtr.Zero, IntPtr.Zero );
        }

        // Return true if extension already associated in registry
        /// <summary>
        /// Returns whether an extension has already been associated in the registry.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <param name="progID">The program ID.</param>
        /// <returns>True if it has already been associated, false otherwise.</returns>
        public static bool IsAssociated( string extension, string progID )
        {
            using ( RegistryKey key = Registry.ClassesRoot.OpenSubKey( extension, false ) )
                return key != null;
        }
    }
}
