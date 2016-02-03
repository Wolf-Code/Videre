
using System.Windows;
using MahApps.Metro;

namespace Videre
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Startup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs"/> that contains the event data.</param>
        protected override void OnStartup( StartupEventArgs e )
        {
            UpdateTheme( );

            Settings.Default.PropertyChanged += ( Sender, Args ) =>
            {
                if ( Args.PropertyName == "VidereTheme" || Args.PropertyName == "VidereAccent" )
                    UpdateTheme( );
            };

            base.OnStartup( e );
        }

        private static void UpdateTheme( )
        {
            ThemeManager.ChangeAppStyle( Current,
                                                 ThemeManager.GetAccent( Settings.Default.VidereAccent ),
                                                 ThemeManager.GetAppTheme( Settings.Default.VidereTheme ) );
        }
    }
}