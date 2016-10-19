using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Totality.CommonClasses;

namespace Totality.Model
{
    public class Country
    {
        public struct FSB
        {
            public int NetLvl { get; set; }
            public List<bool> Recruit { get; set; }

            public FSB(int lvl)
            {
                NetLvl = 0;
                Recruit = new List<bool>(Constants.CountOfMinisters);
            }
        }


        public int Money { get; set; }
        public Property Ecology { get; set; }
        public Property Mood { get; set; } //настроение

        public Property PowerResIndustry { get; set; }
        public Property ResLand { get; set; }
        public Property ResWood { get; set; }
        public Property ResSteel { get; set; }
        public Property ResOil { get; set; }
        public Property ResUranus { get; set; }
        public Property PowerLightIndustry { get; set; }
        public Property PowerHeavyIndustry { get; set; }

        public Property TaxesLvl { get; set; }

        public Property PowerMilitary { get; set; }
        public int CountNukes { get; set; }
        public int CountMissiles { get; set; }

        public Property PowerMassMedia { get; set; }
        public Dictionary<string, double> MassMedia = new Dictionary<string, double>();

        public bool IsRiot { get; set; }

        public int LvlFSB { get; set; }
        public Dictionary<string, FSB> FSBStructure = new Dictionary<string, FSB>();

        public Property PowerScience { get; set; }
        public int LvlResIndustry { get; set; }
        public int LvlLightIndustry { get; set; }
        public int LvlHeavyIndustry { get; set; }
        public int LvlMilitary { get; set; }

        public Country()
        {
            Money = Constants.InitialMoney;           
        }

        public void NukeExplosion(int count)
        {

        }
    }
}
