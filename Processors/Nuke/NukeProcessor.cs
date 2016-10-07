using CommonClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using transmitterService;

namespace Processors.Nuke
{
    public class NukeProcessor
    {
        const int delay = 500;
        private BackgroundWorker timer = new BackgroundWorker();
        private List<NukeRocket> rockets = new List<NukeRocket>();
        private Dictionary<string, Country> countries;
        private Transmitter transmitter;

        public NukeProcessor(ref Dictionary<string, Country> countries, ref Transmitter transmitter)
        {
            this.countries = countries;
            this.transmitter = transmitter;

            timer.DoWork += timer_work;
            timer.WorkerReportsProgress = true;
            timer.ProgressChanged += timer_tick;
        }

        public void addRocket(NukeRocket newRocket)
        {
            rockets.Add(newRocket);
        }

        public void startAttack()
        {
            if(!timer.IsBusy)
            timer.RunWorkerAsync();

            foreach (Transmitter.Client cl in transmitter.clients)
            {
                cl.transmitter.InitializeNukeDialog();
            }
        }

        public void tryToShootdown(Country defender, int rocketId)
        {
            NukeRocket rckt = rockets.Find(x => x.id == rocketId);
            if (rckt != null)
            {
                // TSOP
            }
        }

        private void timer_tick(object sender, ProgressChangedEventArgs e)
        {
            foreach(Transmitter.Client cl in transmitter.clients)
            {
                cl.transmitter.updateNukeDialog(rockets);
            }
        }

        private void timer_work(object sender, DoWorkEventArgs e)
        {
            while (rockets.Any())
            {
                foreach(NukeRocket rckt in rockets)
                {
                    rckt.lifeTime -= delay;
                    if (rckt.lifeTime <= 0)
                    {
                        countries[rckt.to].NukeExplosion(rckt.count);
                        rockets.Remove(rckt);
                    }
                }

                timer.ReportProgress(0);            
                Thread.Sleep(delay);
            } 
        }


    }
}
