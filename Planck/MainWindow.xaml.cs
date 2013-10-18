using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Media;
using System.Reflection;
using System.Windows.Shell;

namespace Planck
{

    public partial class MainWindow : Window
    {
        private enum State { Play, Paused, Stop };

        private State mCurrentState;
        private PomoTask mCurrentTask;
        private Timer mTimer;
        
        // Configuration
        private Int32 Minutes = 25;
        private Int32 ShortBreakMinutes = 5;

        public ObservableCollection<PomoTask> mHistory;    
        
        public MainWindow()
        {
            InitializeComponent();

            this.PreviewKeyDown += MainWindow_PreviewKeyDown;
            
            mCurrentState = State.Stop;
            updateControls();
        }

        void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.F5) || (e.Key == Key.Insert))
            {
                CmdStartPause();
            }
            else if ((e.Key == Key.F6) || (e.Key == Key.Escape))
            {
                CmdStop();
            }
        }

        public void updateControls()
        {
            if (mCurrentState == State.Stop)
            {
                lblStatus.Content = "Idle";
                lblTime.Content = String.Format("00:{0:00}:00", Minutes);
                btnStartPause.Content = "Start";
                tbii.ProgressValue = 0;
                tbii.ProgressState = TaskbarItemProgressState.None;
            }
            else if (mCurrentState == State.Play)
            {
                lblStatus.Content = "In Task: [" + mCurrentTask.Name + "]";
                btnStartPause.Content = "Pause";
                tbii.ProgressValue = 0;
                tbii.ProgressState = TaskbarItemProgressState.Normal;
            }
            else if (mCurrentState == State.Paused)
            {
                btnStartPause.Content = "Resume";
                tbii.ProgressState = TaskbarItemProgressState.Paused;
            }

            updateListView();
        }

        private void updateListView()
        {
            switch (cbPeriod.SelectedIndex)
            {
                // Yesterday
                case 0:
                    mHistory = new ObservableCollection<PomoTask>(Store.getTasksOfYesterday());
                    break;
                // Today
                case 1:
                    mHistory = new ObservableCollection<PomoTask>(Store.getTasksOfToday());
                    break;
                // Last 7 Days
                case 2:
                    mHistory = new ObservableCollection<PomoTask>(Store.getTasksOfLastWeek());
                    break;
                // Last 30 Days
                case 3:
                    mHistory = new ObservableCollection<PomoTask>(Store.getTasksOfLastMonth());
                    break;
                // All Time
                case 4:
                    mHistory = new ObservableCollection<PomoTask>(Store.getTasksOfAllTime());
                    break;
            }
            
            lvHistory.ItemsSource = mHistory;
            updateSummary();
        }

        private void updateSummary()
        {
            long breaksTime = 0;
            long tasksTime = 0;
            long numBreaks = 0;
            long numTasks = 0;
            
            foreach (PomoTask t in mHistory)
            {
                breaksTime += t.BreaksDurationS;
                tasksTime += t.DurationS;
                numBreaks += t.Breaks;
                numTasks++;
            }

            // Number of tasks
            if (numTasks == 0)
            {
                tbSummaryTasks.Text = "No Tasks Performed";
            }
            else
            {
                tbSummaryTasks.Text = String.Format("Performed {0} Tasks", numTasks);
            }

            // Work and Breaks Duration
            tbSummaryWork.Text = String.Format("Tasks Time: {0:hh\\:mm\\:ss}", Util.SECtoTS(tasksTime));
            tbSummaryBreaks.Text = String.Format("Breaks Time: {0:hh\\:mm\\:ss}", Util.SECtoTS(breaksTime));

            // Number of breaks
            if (numBreaks == 0)
            {
                tbSummaryBreaksNum.Text = "No Breaks";
            }
            else
            {
                tbSummaryBreaksNum.Text = String.Format("{0} Breaks", numBreaks);
            }            
        }

        private void CmdStartPause()
        {
            //////////////////////
            // Start a new Task //
            //////////////////////

            if (mCurrentState == State.Stop)
            {
                var windowCreate = new WindowNewTask();
                Nullable<bool> rc = windowCreate.ShowDialog();

                if (rc == true)
                {
                    // Start Timer
                    mTimer = new Timer(100.0);
                    mTimer.Start();
                    mTimer.Elapsed += mTimer_Elapsed;

                    // Create Task
                    mCurrentTask = new PomoTask();
                    mCurrentTask.init(windowCreate.txtName.Text);
                    mCurrentTask.start();

                    mCurrentState = State.Play;
                    playFX("start.wav");
                }
            }

            ///////////
            // Pause //
            ///////////

            else if (mCurrentState == State.Play)
            {
                mTimer.Stop();
                mCurrentTask.pause();
                btnStartPause.Content = "Resume";
                mCurrentState = State.Paused;
            }

            ////////////
            // Resume //
            ////////////

            else if (mCurrentState == State.Paused)
            {
                mTimer.Start();
                mCurrentTask.resume();
                btnStartPause.Content = "Pause";
                mCurrentState = State.Play;
            }

            // Update view on control
            updateControls();
        }

        private void CmdStop()
        {
            if (mCurrentState != State.Stop)
            {
                MessageBoxResult rc = MessageBox.Show("Are you sure you want to stop?", "Task in progress", MessageBoxButton.YesNo);
                if (rc == MessageBoxResult.Yes)
                {
                    CmdNoAskStop();
                    playFX("xstop.wav");
                }
            }
        }

        private void CmdNoAskStop()
        {
            if (mCurrentState != State.Stop)
            {
                mCurrentState = State.Stop;
                mTimer.Stop();
                mCurrentTask.stop();
                Store.addTask(mCurrentTask);
                updateControls();
            }
        }

        private void btnStartPause_Click(object sender, RoutedEventArgs e)
        {
            CmdStartPause();
        }

        // Timer Function
        void mTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (mCurrentState == State.Play)
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    if (mCurrentState == State.Play)
                    {
                        TimeSpan ts = mCurrentTask.Elapsed;
                        Int32 TotalSeconds = ts.Hours * 3600 + ts.Minutes * 60 + ts.Seconds;
                        Int32 TotalTime = Minutes * 60;
                        Int32 TimeLeft = TotalTime - TotalSeconds;
                        if (TimeLeft <= 0)
                        {
                            CmdNoAskStop();
                            playFX("yay.wav");
                            RealBreakWindow rbw = new RealBreakWindow();
                            rbw.BreakMinutes = ShortBreakMinutes;
                            rbw.ShowDialog();
                            return;
                        }

                        // Update the task bar
                        tbii.ProgressValue = (double)(TotalTime - TimeLeft) / (double)TotalTime;

                        // Update the timer section
                        Int32 Hr = TimeLeft / 3600;
                        Int32 Mn = (TimeLeft - (Hr * 3600)) / 60;
                        Int32 Sc = TimeLeft - (Hr * 3600) - (Mn * 60);

                        lblTime.Content = String.Format("{0:00}:{1:00}:{2:00}", Hr, Mn, Sc);
                    }
                }));
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            CmdStop();
        }

        private void playFX(string filename)
        {
            SoundPlayer snd = new SoundPlayer("res/" + filename);
            snd.Play();
        }

        private void menuExit_Click_1(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void menuSettings_Click_1(object sender, RoutedEventArgs e)
        {
            SettingsWindow sw = new SettingsWindow();
            sw.BreakDuration = ShortBreakMinutes;
            sw.PomodrovDuration = Minutes;

            Nullable<bool> rc = sw.ShowDialog();
            if (rc == true)
            {
                ShortBreakMinutes = sw.BreakDuration;
                Minutes = sw.PomodrovDuration;
                updateControls();
            }
        }

        private void SortClick(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = sender as GridViewColumnHeader;
            String tag = column.Tag as String;
        }

        private void cbPeriod_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            updateListView();
        }

        private void MenuItem_LV_Delete(object sender, RoutedEventArgs e)
        {
            if (lvHistory.SelectedItems.Count == 0)
            {
                return;
            }

            MessageBoxResult rc = MessageBox.Show(String.Format("Delete {0} Tasks?", lvHistory.SelectedItems.Count), "Delete", MessageBoxButton.YesNo);
            if (rc == MessageBoxResult.No)
            {
                return;
            }

            foreach (PomoTask pt in lvHistory.SelectedItems)
            {
                Store.removeTask(pt);
            }

            updateListView();
        }

    }
}
