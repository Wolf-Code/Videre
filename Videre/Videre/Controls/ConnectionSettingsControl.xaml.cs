using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;
using Videre.Windows;
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

        protected override void OnInitialized( EventArgs e )
        {
            SetQRCodeImage( );
        }

        private void SetQRCodeImage( )
        {
            using ( Bitmap bmp = MainWindow.Player.GetComponent<NetworkComponent>( ).GetQRCode( ) )
            {
                bmp.Save( qrStream, ImageFormat.Png );

                BitmapImage img = new BitmapImage( );
                img.BeginInit( );
                img.StreamSource = qrStream;
                img.EndInit( );

                QRImage.Source = img;
            }
        }
    }
}