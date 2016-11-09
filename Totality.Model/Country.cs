using System.Collections.Generic;
using System.Runtime.Serialization;
using Totality.CommonClasses;

namespace Totality.Model
{
    [DataContract]
    public class Country
    {
        [DataContract]
        public class SpyNetwork
        {
            [DataMember]
            public int NetLvl;
            [DataMember]
            public List<bool> Recruit;

            public SpyNetwork()
            {
                NetLvl = 0;
                bool[] rc = new bool[Constants.CountOfMinisters];
                Recruit = new List<bool>(rc);
            }
        }

        public string Name { get; }
        [DataMember]
        public long Money;
        [DataMember]
        public double Ecology;
        [DataMember]
        public double Mood = 100;

        [DataMember]
        public int[] MinsBlocks = new int[Constants.CountOfMinisters];

        public double ResAgricultural;
        public double ResWood;
        public double ResSteel;
        public double ResOil;
        public double ProdUranus;
        public double ResUranus;
        public double PowerLightIndustry;
        public double PowerHeavyIndustry;

        [DataMember]
        public double FinalAgricultural;
        [DataMember]
        public double FinalWood;
        [DataMember]
        public double FinalSteel;
        [DataMember]
        public double FinalOil;
        [DataMember]
        public double FinalLightIndustry;
        [DataMember]
        public double FinalHeavyIndustry;

        public double UsedSteel;
        public double UsedOil;
        public double UsedWood;
        public double UsedAgricultural;
        public double UsedLIpower;
        public double UsedHIpower;

        [DataMember]
        public double MilitaryPower;

        [DataMember]
        public long ProductionUpgradeCost;
        [DataMember]
        public long IndustryUpgradeCost;

        [DataMember]
        public short TaxesLvl;

        [DataMember]
        public int NukesCount;
        [DataMember]
        public int MissilesCount;
        [DataMember]
        public List<string> WarList = new List<string>();

        [DataMember]
        public Dictionary<string, short> MassMedia = new Dictionary<string, short>();

        [DataMember]
        public bool IsRiot = false;
        [DataMember]
        public bool IsMobilized = false;
        [DataMember]
        public bool IsAlerted = false;
        [DataMember]
        public bool IsRepressed = false;

        [DataMember]
        public Dictionary<string, SpyNetwork> SpyNetworks = new Dictionary<string, SpyNetwork>();
        public List<List<string>> ForeignSpyes;

        public long NationalCurrencyDemand;
        [DataMember]
        public Dictionary<string, long> CurrencyAccounts = new Dictionary<string, long>();

        [DataMember]
        public int LvlResIndustry;
        [DataMember]
        public int LvlLightIndustry;
        [DataMember]
        public int LvlHeavyIndustry;
        [DataMember]
        public int LvlMilitary;
        [DataMember]
        public int PremierLvl = 1;
        [DataMember]
        public int InnerLvl = 1;
        [DataMember]
        public int IntelligenceLvl = 0;
        [DataMember]
        public int CounterSpyLvl = 0;
        [DataMember]
        public int ShadowingLvl = 0;
        [DataMember]
        public int ExtractScienceLvl = 1;
        [DataMember]
        public int HeavyScienceLvl = 1;
        [DataMember]
        public int LightScienceLvl = 1;
        [DataMember]
        public int MilitaryScienceLvl = 1;

        [DataMember]
        public int ExtractExperience = 0;
        [DataMember]
        public int HeavyExperience = 0;
        [DataMember]
        public int LightExperience = 0;
        [DataMember]
        public int MilitaryExperience = 0;

        [DataMember]
        public long CounterSpyLvlUpCost;
        [DataMember]
        public long PremierLvlUpCost;
        [DataMember]
        public long InnerLvlUpCost;
        [DataMember]
        public long ShadowingLvlUpCost;
        [DataMember]
        public long IntelligenceLvlUpCost;
        [DataMember]
        public long ExtractScLvlUpCost;
        [DataMember]
        public long HeavyScLvlUpCost;
        [DataMember]
        public long LightScLvlUpCost;
        [DataMember]
        public long MilitaryScLvlUpCost;
        [DataMember]
        public int ExtractScLvlUpExp;
        [DataMember]
        public int HeavyScLvlUpExp;
        [DataMember]
        public int LightScLvlUpExp;
        [DataMember]
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
