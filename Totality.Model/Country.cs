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
        public double Ecology;
        public double Mood = 100; 

        public int[] MinsBlocks = new int[Constants.CountOfMinisters];

        public double PowerResIndustry;
        public double ResAgricultural;
        public double ResWood;
        public double ResSteel;
        public double ResOil;
        public double ProdUranus;
        public double ResUranus;
        public double PowerLightIndustry;
        public double PowerHeavyIndustry;
        public long ProductionUpgradeCost;
        public long IndustryUpgradeCost;

        public Property TaxesLvl;

        public Property PowerMilitary;
        public int NukesCount;
        public int MissilesCount;
        public List<string> WarList = new List<string>();

        public Property PowerMassMedia;
        public Dictionary<string, short> MassMedia = new Dictionary<string, short>();

        public bool IsRiot = false;
        public bool IsMobilized = false;
        public bool IsAlerted = false;
        public bool IsRepressed = false;

        public int LvlFSB { get; set; }
        public Dictionary<string, SpyNetwork> SpyNetworks = new Dictionary<string, SpyNetwork>();
        public List<List<string>> ForeignSpyes;


        public Property PowerScience;
        public int LvlResIndustry;
        public int LvlLightIndustry;
        public int LvlHeavyIndustry;
        public int LvlMilitary;
        public int PremierLvl = 1;
        public int InnerLvl = 1;
        public int IntelligenceLvl = 0;
        public int CounterSpyLvl = 0;
        public int ShadowingLvl = 0;

        public long CounterSpyLvlUpCost;
        public long PremierLvlUpCost;
        public long InnerLvlUpCost;
        public long ShadowingLvlUpCost;
        public long IntelligenceLvlUpCost;

        public Country(string name)
        {
            Money = Constants.InitialMoney;
            PremierLvlUpCost = Constants.InitialPremierLvlUpCost;
            IndustryUpgradeCost = Constants.InitialIndustryUpgradeCost;
            ProductionUpgradeCost = Constants.InitialProductionUpgradeCost;
            InnerLvlUpCost = Constants.InitialInnerLvlUpCost;
            CounterSpyLvlUpCost = Constants.InitialCounterSpyLvlUpCost;
            ShadowingLvlUpCost = Constants.InitialShadowingLvlUpCost;
            IntelligenceLvlUpCost = Constants.InitialIntelligenceLvlUpCost;

            Name = name;
            ForeignSpyes = new List<List<string>>();
            for (int i = 0; i < Constants.CountOfMinisters; i++)
            {
                ForeignSpyes.Add(new List<string>());
            }
        }
    }
}
