using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace Planck
{
    public class PomoTask
    {
        // Stopwatch object
        private Stopwatch mStopWatch;
        private Stopwatch mStopWatchBreaks;

        // DB Attributes
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime StartedDate { get; set; }
        public long Breaks { get; set; }
        public long DurationS { get; set; }
        public long BreaksDurationS { get; set; }

        // Useful Properties
        public String StartedTimeFmt { get { return StartedDate.ToString("H:mm:ss"); } }
        public String StartedDateFmt { get { return StartedDate.ToString("d"); } }
        public TimeSpan Elapsed { get { return mStopWatch.Elapsed; } }

        ////////////////////
        // Public Actions //
        ////////////////////

        public void init(string name)
        {
            Name = name;
            mStopWatch = new Stopwatch();
            mStopWatchBreaks = new Stopwatch();
            Breaks = 0;    
        }

        public void start()
        {
            StartedDate = DateTime.Now;            
            mStopWatch.Start();
        }

        public void resume()
        {
            mStopWatch.Start();
            mStopWatchBreaks.Stop();
        }

        public void pause()
        {
            Breaks++;
            mStopWatch.Stop();
            mStopWatchBreaks.Start();
        }

        public void stop()
        {
            mStopWatch.Stop();
            mStopWatchBreaks.Stop();
            DurationS = Util.TStoSEC(mStopWatch.Elapsed);
            BreaksDurationS = Util.TStoSEC(mStopWatchBreaks.Elapsed);
        }
    }

    class PomoContext : DbContext
    {
        public DbSet<PomoTask> PomoTasks { get; set; }
    }

    static public class Store
    {
        private static PomoContext db = new PomoContext();

        public static IQueryable<PomoTask> getTasksOfToday()
        {
            return (from t in db.PomoTasks where t.StartedDate >= DateTime.Today select t).OrderByDescending(t=>t.StartedDate);
        }

        public static IQueryable<PomoTask> getTasksOfYesterday()
        {
            DateTime Yesterday = DateTime.Today;
            Yesterday = Yesterday.AddDays(-1);            
            return (from t in db.PomoTasks where ((t.StartedDate >= Yesterday) && (t.StartedDate <= DateTime.Today)) select t).OrderByDescending(t => t.StartedDate);
        }

        public static IQueryable<PomoTask> getTasksOfLastWeek()
        {
            DateTime LastWeek = DateTime.Today;
            LastWeek = LastWeek.AddDays(-7);
            return (from t in db.PomoTasks where t.StartedDate >= LastWeek select t).OrderByDescending(t => t.StartedDate);
        }

        public static IQueryable<PomoTask> getTasksOfLastMonth()
        {
            DateTime LastMonth = DateTime.Today;
            LastMonth = LastMonth.AddDays(-30);
            return (from t in db.PomoTasks where t.StartedDate >= LastMonth select t).OrderByDescending(t => t.StartedDate);
        }

        public static IQueryable<PomoTask> getTasksOfAllTime()
        {
            return (from t in db.PomoTasks select t).OrderByDescending(t => t.StartedDate);
        }

        public static void addTask(PomoTask pt)
        {
            db.PomoTasks.Add(pt);
            db.SaveChanges();
        }

        public static void removeTask(PomoTask pt)
        {
            db.PomoTasks.Remove(pt);
            db.SaveChanges();
        }
    }
}
