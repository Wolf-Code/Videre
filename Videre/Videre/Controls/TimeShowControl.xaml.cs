using System.Windows;
using System.Windows.Media;

namespace Videre.Controls
{
    /// <summary>
    /// Interaction logic for TimeShowControl.xaml
    /// </summary>
    public partial class TimeShowControl
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public TimeShowControl( )
        {
            InitializeComponent( );
        }

        /// <summary>
        /// Sets the width of the pointer.
        /// </summary>
        /// <param name="width">The width of the pointer.</param>
        public void SetPointerWidth( double width )
        {
            Pointer.Points = new PointCollection
            {
                new Point( -width / 2f, 0 ),
                new Point( 0, width / 2f ),
                new Point( width / 2f, 0 )
            };
        }
    }
}
