using Totality.CommonClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Totality.Model;
using Totality.Model.Interfaces;

namespace Totality.Processors.Nuke
{
    public class NukeProcessor : AbstractProcessor
    {
        private const int _delay = 500;
        private BackgroundWorker _timer = new BackgroundWorker();
        private List<NukeRocket> _rockets = new List<NukeRocket>();
        private ITransmitter _transmitter;

        public NukeProcessor( ITransmitter transmitter, IDataLayer dataLayer, ILogger logger) : base(dataLayer, logger)
        {
            _transmitter = transmitter;

            _timer.DoWork += timer_work;
            _timer.WorkerReportsProgress = true;
            _timer.ProgressChanged += timer_tick;
        }

        public void AddRocket(NukeRocket newRocket)
        {
            _rockets.Add(newRocket);
        }

        public void StartAttack()
        {
            if(!_timer.IsBusy)
            _timer.RunWorkerAsync();

            _transmitter.InitializeNukeDialogs();
        }

        public void TryToShootdown(Country defender, Guid rocketId)
        {
            NukeRocket rckt = _rockets.Find(x => x.Id == rocketId);
            if (rckt != null)
            {
                int loosed;
                int result = WinnerChoosingSystems.NukeMassiveTsop(defender.CountMissiles, out loosed, rckt.Count, _dataLayer.GetCountry(rckt.From).LvlMilitary, defender.LvlMilitary);
                defender.CountMissiles -= loosed;
                rckt.Count -= result;
            }
        }

        private void timer_tick(object sender, ProgressChangedEventArgs e)
        {
            _transmitter.UpdateNukeDialogs(_rockets);
        }

        private void timer_work(object sender, DoWorkEventArgs e)
        {
            while (_rockets.Any())
            {
                foreach(NukeRocket rckt in _rockets)
                {
                    rckt.LifeTime -= _delay;
                    if (rckt.LifeTime <= 0)
                    {
                        Country curCountry = _dataLayer.GetCountry(rckt.To);
                        // ToDo: ядерный взрыв
                        _dataLayer.UpdateCountry(curCountry);
                        _rockets.Remove(rckt);
                    }
                }

                _timer.ReportProgress(0);            
                Thread.Sleep(_delay);
            } 
        }


    }
}
