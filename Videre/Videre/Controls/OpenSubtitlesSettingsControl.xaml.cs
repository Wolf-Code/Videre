using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Videre.Properties;
using Videre.Windows;
using VidereSubs.OpenSubtitles;
using VidereSubs.OpenSubtitles.Outputs;

namespace Videre.Controls
{
    /// <summary>
    /// Interaction logic for OpenSubtitlesSettingsControl.xaml
    /// </summary>
    public partial class OpenSubtitlesSettingsControl : INotifyPropertyChanged
    {
        private bool m_IsSignedIn;

        /// <summary>
        /// Whether or not the user is signed in.
        /// </summary>
        public bool IsSignedIn
        {
            private set
            {
                m_IsSignedIn = value;
                OnPropertyChanged( nameof( IsSignedIn ) );
            }
            get { return m_IsSignedIn; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public OpenSubtitlesSettingsControl( )
        {
            InitializeComponent( );
        }

        private async void OnConnectClick( object Sender, RoutedEventArgs E )
        {
            ProgressDialogController controller = await ( ( MetroWindow ) Window.GetWindow( this ) ).ShowProgressAsync( "Signing in.", "Attempting to sign in with the new credentials." );
            controller.SetIndeterminate( );

            Settings.Default.OSUsername = Username.Text;
            Settings.Default.Save( );

            Client tester = new Client( MainWindow.UserAgent );
            BackgroundWorker worker = new BackgroundWorker(  );
            worker.DoWork += ( sender, args ) =>
            {
                Tuple<string, string> info = args.Argument as Tuple<string, string>;
                LogInOutput output = tester.LogIn( info.Item1, info.Item2, info.Item1.Length != info.Item2.Length || info.Item1.Length > 0 );
                args.Result = output;
            };
            worker.RunWorkerCompleted += async ( O, Args ) =>
            {
                await controller.CloseAsync( );

                LogInOutput output = ( LogInOutput ) Args.Result;
                IsSignedIn = output.LogInSuccesful;
                StatusImage.Visibility = Visibility.Visible;
                StatusLabel.Visibility = Visibility.Visible;

                StatusLabel.Content = !IsSignedIn ? output.StatusString.Substring( 4 ) : string.Empty;
            };
            worker.RunWorkerAsync( new Tuple<string, string>( Username.Text, Password.Password ) );
        }
        
        #region Property Change
        /// <summary>
        /// Gets called whenever a property is changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Virtual method called on a change in properties.
        /// </summary>
        /// <param name="PropertyName">The name of the property.</param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged( [CallerMemberName] string PropertyName = null )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( PropertyName ) );
        }

        #endregion
    }
}