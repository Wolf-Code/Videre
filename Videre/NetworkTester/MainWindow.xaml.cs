using System.Net.Sockets;
using System.Windows;
using VidereLib.Attributes;

namespace NetworkTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TcpClient client;

        public MainWindow( )
        {
            InitializeComponent( );
        }

        private void OnPlayClick( object Sender, RoutedEventArgs E )
        {
            client.GetStream( ).Write( new [ ] { ( byte ) NetworkRequestAttribute.RequestIdentifier.Play }, 0, 1 );
        }

        private void OnPauseClick( object Sender, RoutedEventArgs E )
        {
            client.GetStream( ).Write( new[ ] { ( byte )NetworkRequestAttribute.RequestIdentifier.Pause }, 0, 1 );
        }

        private void ConnectClick( object Sender, RoutedEventArgs E )
        {
            client = new TcpClient( );
            client.Connect( VidereLib.Networking.Server.IP, 13337 );
        }
    }
}
