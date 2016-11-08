using System.Collections.Generic;
using System.Runtime.Serialization;
using Totality.CommonClasses;

namespace Totality.Model
{
    [DataContract]
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

        public double ResAgricultural;
        public double ResWood;
        public double ResSteel;
        public double ResOil;
        public double ProdUranus;
        public double ResUranus;
        public double PowerLightIndustry;
        public double PowerHeavyIndustry;

        public double FinalAgricultural;
        public double FinalWood;
        public double FinalSteel;
        public double FinalOil;
        public double FinalLightIndustry;
        public double FinalHeavyIndustry;

        public double UsedSteel;
        public double UsedOil;
        public double UsedWood;
        public double UsedAgricultural;
        public double UsedLIpower;
        public double UsedHIpower;

        public double MilitaryPower;

        public long ProductionUpgradeCost;
        public long IndustryUpgradeCost;

        public short TaxesLvl;


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

        public long NationalCurrencyDemand;
        public Dictionary<string, long> CurrencyAccounts = new Dictionary<string, long>();

        public int LvlResIndustry;
        public int LvlLightIndustry;
        public int LvlHeavyIndustry;
        public int LvlMilitary;
        public int PremierLvl = 1;
        public int InnerLvl = 1;
        public int IntelligenceLvl = 0;
        public int CounterSpyLvl = 0;
        public int ShadowingLvl = 0;
        public int ExtractScienceLvl = 1;
        public int HeavyScienceLvl = 1;
        public int LightScienceLvl = 1;
        public int MilitaryScienceLvl = 1;

        public int ExtractExperience = 0;
        public int HeavyExperience = 0;
        public int LightExperience = 0;
        public int MilitaryExperience = 0;

        public long CounterSpyLvlUpCost;
        public long PremierLvlUpCost;
        public long InnerLvlUpCost;
        public long ShadowingLvlUpCost;
        public long IntelligenceLvlUpCost;
        public long ExtractScLvlUpCost;
        public long HeavyScLvlUpCost;
        public long LightScLvlUpCost;
        public long MilitaryScLvlUpCost;
        public int ExtractScLvlUpExp;
        public int HeavyScLvlUpExp;
        public int LightScLvlUpExp;
        public int MilitaryScLvlUpExp;

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

            NationalCurrencyDemand = Constants.InitialNationalCurrencyDemand;

            Name = name;
            ForeignSpyes = new List<List<string>>();
            for (int i = 0; i < Constants.CountOfMinisters; i++)
            {
                ForeignSpyes.Add(new List<string>());
            }
        }
    }
}
