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
    public partial class SettingsWindow : Window
    {
        public Int32 PomodrovDuration = 25;
        public Int32 BreakDuration = 5;

        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Int32 BDuration;
            Int32 PDuration;
            if ((Int32.TryParse(txtBDuration.Text, out BDuration) == false) ||
                (Int32.TryParse(txtPDuration.Text, out PDuration) == false)) {
                MessageBox.Show("Duration should be numeric", "Error");
                return;
            }

            if ((BDuration > 60) || (PDuration > 60))
            {
                MessageBox.Show("Duration must no exceed 60 minutes", "Error");
                return;
            }

            PomodrovDuration = PDuration;
            BreakDuration = BDuration;
            this.DialogResult = true;
            this.Close();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            txtBDuration.Text = BreakDuration.ToString();
            txtPDuration.Text = PomodrovDuration.ToString();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
