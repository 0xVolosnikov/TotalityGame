using System.Collections.Generic;
using Totality.CommonClasses;

namespace Totality.Model
{
    public class Country
    {
        public class SpyNetwork
        {
            public int NetLvl;
            public List<bool> Recruit;

            public SpyNetwork()
            {
                NetLvl = 0;
                bool[] rc = new bool[Constants.CountOfMinisters];
                Recruit = new List<bool>(rc);
            }
        }

        public string Name { get; }
        public long Money;
        public long ProductionUpgradeCost;
        public double Ecology;
        public Property Mood; //настроение

        public int[] MinsBlocks = new int[Constants.CountOfMinisters];

        public Property PowerResIndustry;
        public Property ResLand;
        public Property ResWood;
        public Property ResSteel;
        public Property ResOil;
        public double ProdUranus;
        public Property ResUranus;
        public Property PowerLightIndustry;
        public Property PowerHeavyIndustry;

        public Property TaxesLvl;

        public Property PowerMilitary;
        public int NukesCount;
        public int MissilesCount;
        public List<string> WarList = new List<string>();

        public Property PowerMassMedia;
        public Dictionary<string, double> MassMedia = new Dictionary<string, double>();

        public bool IsRiot = false;
        public bool IsMobilized = false;
        public bool IsAlerted = false;

        public int LvlFSB { get; set; }
        public Dictionary<string, SpyNetwork> SpyNetworks = new Dictionary<string, SpyNetwork>();
        public List<List<string>> ForeignSpyes;

        public Property PowerScience;
        public int LvlResIndustry;
        public int LvlLightIndustry;
        public int LvlHeavyIndustry;
        public int LvlMilitary;
        public int PremierLvl = 1;
        public long PremierLvlUpCost;

        public Country(string name)
        {
            Money = Constants.InitialMoney;
            PremierLvlUpCost = Constants.InitialPremierLvlUpCost;
            Name = name;
            ForeignSpyes = new List<List<string>>();
            for (int i = 0; i < Constants.CountOfMinisters; i++)
            {
                ForeignSpyes.Add(new List<string>());
            }
        }
    }
}
