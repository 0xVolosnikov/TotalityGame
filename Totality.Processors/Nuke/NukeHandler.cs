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
using Totality.Handlers.News;

namespace Totality.Handlers.Nuke
{
    public class NukeHandler : AbstractHandler
    {
        private const int _delay = 100;
        private BackgroundWorker _timer = new BackgroundWorker();
        private SynchronizedCollection<NukeRocket> _rockets = new SynchronizedCollection<NukeRocket>();
        private List<NukeRocket> _rockets2 = new List<NukeRocket>();
        private ITransmitter _transmitter;
        private Random rand = new Random();

        public delegate void AttackEnd();
        public event AttackEnd AttackEnded;

        public NukeHandler(NewsHandler newsHandler, ITransmitter transmitter, IDataLayer dataLayer, ILogger logger) : base(newsHandler, dataLayer, logger)
        {
            _transmitter = transmitter;

            _timer.DoWork += timer_work;
            _timer.WorkerReportsProgress = true;
            _timer.ProgressChanged += timer_tick;
            _timer.RunWorkerCompleted += _timer_RunWorkerCompleted;
        }

        private void _timer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _rockets.Clear();
            _rockets2.Clear();
            Thread.Sleep(2000);
            AttackEnded?.Invoke();
        }

        public void AddRocket(NukeRocket newRocket)
        {
            _rockets.Add(newRocket);
        }

        public void StartAttack()
        {
            if (_rockets.Count != 0)
            {
                _rockets2.Add(_rockets[0]);

                _timer.RunWorkerAsync();

                _transmitter.InitializeNukeDialogs();
            }
            else
            {
                AttackEnded?.Invoke();
            }
            
        }

        public void TryToShootdown(string defender, Guid rocketId)
        {
            NukeRocket rckt = _rockets.First(x => x.Id == rocketId);
            if (rckt != null)
            {
                var missilesCount = (int)_dataLayer.GetProperty(defender, "MissilesCount");
                var lvlMilitary = (int)_dataLayer.GetProperty(defender, "LvlMilitary");
                int loosed;
                int result = WinnerChoosingSystems.NukeMassiveTsop(missilesCount, out loosed, rckt.Count, _dataLayer.GetCountry(rckt.From).LvlMilitary, lvlMilitary);
                missilesCount -= loosed;
                _dataLayer.SetProperty(defender, "MissilesCount", missilesCount);
                rckt.Count -= result;
                //_rockets2.First(x => x.Id == rocketId).Count -= result;
            }
        }

        private void timer_tick(object sender, ProgressChangedEventArgs e)
        {
            _transmitter.UpdateNukeDialogs(_rockets2);
        }

        private void timer_work(object sender, DoWorkEventArgs e)
        {
            int time = 0;
            while (_rockets.Any(x => x.LifeTime > 0 && x.Count > 0))
            {

                for (int i = 0; i < _rockets2.Count; i++)
                {
                    if (_rockets[i].LifeTime <= 0 || _rockets[i].Count == 0) continue;
                    
                        _rockets[i].LifeTime -= _delay;
                        _rockets2[i].LifeTime -= _delay;
                    
                    if (_rockets2[i].LifeTime <= 0 && _rockets2[i].Count > 0)
                    {
                        Country curCountry = _dataLayer.GetCountry(_rockets2[i].To);
                        curCountry.ResOil *= (getNukesDamage(_rockets2[i].Count, curCountry.IsAlerted) );
                        curCountry.ResSteel *= (getNukesDamage(_rockets2[i].Count, curCountry.IsAlerted));
                        curCountry.ResWood *= (getNukesDamage(_rockets2[i].Count, curCountry.IsAlerted));
                        curCountry.ResAgricultural *= (getNukesDamage(_rockets2[i].Count, curCountry.IsAlerted));
                        curCountry.ProdUranus *= (getNukesDamage(_rockets2[i].Count, curCountry.IsAlerted));
                        curCountry.PowerHeavyIndustry *= (getNukesDamage(_rockets2[i].Count, curCountry.IsAlerted));
                        curCountry.PowerLightIndustry*= (getNukesDamage(_rockets2[i].Count, curCountry.IsAlerted));
                        curCountry.Mood *= (getNukesDamage(_rockets2[i].Count, curCountry.IsAlerted));
                        _dataLayer.UpdateCountry(curCountry);
                    }
                }

                time += _delay;
                if (_rockets2.Count < _rockets.Count)
                {
                    if (time >= 3000)
                    {
                        time = 0;
                        _rockets2.Add(_rockets[_rockets2.Count]);
                    }
                }

                _timer.ReportProgress(0);
                Thread.Sleep(_delay);
            }
            _timer.ReportProgress(0);
        }

        private double getNukesDamage(int count, bool isAlerted)
        {
            var min = Math.Pow(0.95, count);
            var max = Math.Pow(0.85, count);
            var dif = max - min;
            if (isAlerted) dif /= 3;

            return min + (dif/rand.Next(1,1000));
        }

    }
}
