
namespace VidereFileAssociator
{
    class Program
    {
        static void Main( string[ ] args )
        {
            string exec = string.Empty;
            string icon = string.Empty;
            string progID = string.Empty;
            string[ ] videoExtensions = new string[ 0 ];
            int x = 0;
            while ( x < args.Length )
            {
                string key = args[ x++ ];
                string value = args[ x++ ];

                switch ( key )
                {
                    case "-progID":
                        progID = value;
                        break;

                    case "-executable":
                        exec = value;
                        break;

                    case "-icon":
                        icon = value;
                        break;

                    case "-videoExtensions":
                        videoExtensions = value.Split( ' ' );
                        break;
                }
            }

            foreach ( string videoExtension in videoExtensions )
                FileAssociation.Associate( '.' + videoExtension, progID, exec, videoExtension.ToUpper( ) + " File", icon );

            FileAssociation.NotifyFileExplorer( );
        }
    }
}