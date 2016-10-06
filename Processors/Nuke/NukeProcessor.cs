using CommonClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Processors.Nuke
{
    class NukeProcessor
    {
        const int delay = 500;
        private BackgroundWorker timer = new BackgroundWorker();
        private List<NukeRocket> rockets = new List<NukeRocket>();

        public NukeProcessor()
        {
            timer.DoWork += timer_work;
            timer.WorkerReportsProgress = true;
            timer.ProgressChanged += timer_tick;
        }

        private void timer_tick(object sender, ProgressChangedEventArgs e)
        {

        }

        private void timer_work(object sender, DoWorkEventArgs e)
        {
            while (rockets.Any())
            {
                foreach(NukeRocket rckt in rockets)
                {
                    rckt.lifeTime -= delay;
                }

                timer.ReportProgress(0);            
                Thread.Sleep(delay);
            } 
        }
    }
}
