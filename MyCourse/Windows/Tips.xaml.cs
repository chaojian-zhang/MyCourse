using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyCourse.Windows
{
    /// <summary>
    /// Provides links to useful sites
    /// </summary>
    public partial class Tips : Window
    {
        public Tips()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            e.Cancel = true;
        }

        // Ref: http://www.c-sharpcorner.com/UploadFile/mahesh/using-xaml-frame-in-wpf857/
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Can raise an exception, hanlded in dispatcher
            WebFrame.Navigate(new System.Uri("http://www.totalimagine.com/Service/MyCourse/", UriKind.Absolute));
        }
    }
}
