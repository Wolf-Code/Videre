using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using VidereLib;
using VidereLib.Components;

namespace Videre.Controls
{
    /// <summary>
    /// Interaction logic for ConnectionSettingsControl.xaml
    /// </summary>
    public partial class ConnectionSettingsControl
    {
        private MemoryStream qrStream = new MemoryStream( );

        /// <summary>
        /// Constructor.
        /// </summary>
        public ConnectionSettingsControl( )
        {
            InitializeComponent( );
        }

        /// <summary>
        /// Gets called whenever the player has been initialized.
        /// </summary>
        public override void OnPlayerInitialized( )
        {
            SetQRCodeImage( );
            IPLabel.Content = ViderePlayer.GetComponent<NetworkComponent>( ).IP;
        }

        private void SetQRCodeImage( )
        {
            if ( !ViderePlayer.IsInitialized )
                return;

            qrStream = new MemoryStream( );

            using ( Bitmap bmp = ViderePlayer.GetComponent<NetworkComponent>( ).GetQRCode( ) )
            {
                bmp.Save( qrStream, ImageFormat.Png );

                BitmapImage img = new BitmapImage( );
                img.BeginInit( );
                img.StreamSource = qrStream;
                img.EndInit( );

                QRImage.Source = img;
            }
        }

        private void OnPortChanged( object Sender, RoutedPropertyChangedEventArgs<double?> E )
        {
            if ( !ViderePlayer.IsInitialized )
                return;

            if ( !E.NewValue.HasValue )
                return;

            Settings.Default.ListenPort = ( ushort )E.NewValue;
            Settings.Default.Save( );

            NetworkComponent comp = ViderePlayer.GetComponent<NetworkComponent>( );

            comp.ShutdownServer( );
            comp.SetUpNetworkReceiver( Settings.Default.ListenPort );
            SetQRCodeImage( );
        }

        private void ServerStartButton_OnClick( object Sender, RoutedEventArgs E )
        {
            NetworkComponent comp = ViderePlayer.GetComponent<NetworkComponent>( );

            comp.ShutdownServer( );
            comp.SetUpNetworkReceiver( Settings.Default.ListenPort );
        }
    }
}
