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

namespace Planck
{
    /// <summary>
    /// Interaction logic for WindowNewTask.xaml
    /// </summary>
    public partial class WindowNewTask : Window
    {
        public WindowNewTask()
        {
            InitializeComponent();

            this.PreviewKeyDown += WindowNewTask_PreviewKeyDown;
        }

        void WindowNewTask_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.DialogResult = false;
                this.Close();
            }
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
