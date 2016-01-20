using System;
using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;

namespace Videre.Windows
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow
    {
        /// <summary>
        /// The currently active settings window.
        /// </summary>
        public static SettingsWindow ActiveWindow { private set; get; }
        /// <summary>
        /// Constructor.
        /// </summary>
        public SettingsWindow( )
        {
            InitializeComponent( );

            ActiveWindow = this;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.Closed"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnClosed( EventArgs e )
        {
            ActiveWindow = null;
        }

        /// <summary>
        /// Shows a progress message.
        /// </summary>
        /// <param name="title">The title of the message.</param>
        /// <param name="message">The message.</param>
        /// <returns>A tasking containing the <see cref="ProgressDialogController"/>.</returns>
        public Task<ProgressDialogController> ShowWorkingMessage( string title, string message )
        {
            Task<ProgressDialogController> controller = this.ShowProgressAsync( title, message );

            return controller;
        }
    }
}
