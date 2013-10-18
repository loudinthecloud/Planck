using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Shell;

namespace Planck
{
    /// <summary>
    /// Interaction logic for RealBreakWindow.xaml
    /// </summary>
    public partial class RealBreakWindow : Window
    {
        public Int32 BreakMinutes { get; set; }
        private Timer mTimer;
        private Stopwatch mStopWatch;

        public RealBreakWindow()
        {
            InitializeComponent();

            mStopWatch = new Stopwatch();
            mStopWatch.Start();

            mTimer = new Timer(50.0);
            mTimer.Elapsed += mTimer_Elapsed;
            mTimer.Start();
        }

        void mTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke((Action)(() => {
                Int32 TotalSeconds = BreakMinutes * 60;
                Int32 ElapsedSeconds = mStopWatch.Elapsed.Minutes * 60 + mStopWatch.Elapsed.Seconds;
                Int32 TimeLeft = TotalSeconds - ElapsedSeconds;

                if (TimeLeft <= 0)
                {
                    tbii.ProgressState = TaskbarItemProgressState.None;
                    this.DialogResult = true;
                    this.Close();
                    return;
                }

                // Update TaskBarItem
                double progress = (double)(TotalSeconds - TimeLeft) / (double)TotalSeconds;
                tbii.ProgressValue = progress;

                // Update Timer View
                Int32 Mn = TimeLeft / 60;
                Int32 Sc = TimeLeft - Mn * 60;
                lblTimeLeft.Content = String.Format("{0:00}:{1:00}:{2:00}", 0, Mn, Sc);
            }));
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            lblTimeLeft.Content = String.Format("{0:00}:{1:00}:{2:00}", 0, BreakMinutes, 0);
            tbii.ProgressState = TaskbarItemProgressState.Error;
            tbii.ProgressValue = 0;
        }
    }
}
