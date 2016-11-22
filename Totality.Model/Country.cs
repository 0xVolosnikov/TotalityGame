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

        [DataMember]
        public string Alliance;
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public long Money;
        [DataMember]
        public double Ecology;
        [DataMember]
        public double Mood = 100;

        [DataMember]
        public int[] MinsBlocks = new int[Constants.CountOfMinisters];

        [DataMember]
        public double ResAgricultural;
        [DataMember]
        public double ResWood;
        [DataMember]
        public double ResSteel;
        [DataMember]
        public double ResOil;
        [DataMember]
        public double ProdUranus;
        [DataMember]
        public double ResUranus;
        [DataMember]
        public double PowerLightIndustry;
        [DataMember]
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

        [DataMember]
        public double UsedSteel;
        [DataMember]
        public double UsedOil;
        [DataMember]
        public double UsedWood;
        [DataMember]
        public double UsedAgricultural;
        [DataMember]
        public double UsedLIpower;
        [DataMember]
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
        public Dictionary<string, double> CurrencyRatios = new Dictionary<string, double>();

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
            Alliance = name;
            Money = Constants.InitialMoney;
            PremierLvlUpCost = Constants.InitialPremierLvlUpCost;
            IndustryUpgradeCost = Constants.InitialIndustryUpgradeCost;
            ProductionUpgradeCost = Constants.InitialProductionUpgradeCost;
            InnerLvlUpCost = Constants.InitialInnerLvlUpCost;
            CounterSpyLvlUpCost = Constants.InitialCounterSpyLvlUpCost;
            ShadowingLvlUpCost = Constants.InitialShadowingLvlUpCost;
            IntelligenceLvlUpCost = Constants.InitialIntelligenceLvlUpCost;

            ExtractScLvlUpExp = (int)Constants.InitialExtractScLvlUpExp;
            ExtractScLvlUpCost = (int)Constants.InitialExtractScLvlUpCost;
            HeavyScLvlUpExp = (int)Constants.InitialHeavyScLvlUpExp;
            HeavyScLvlUpCost = (int)Constants.InitialHeavyScLvlUpCost;
            LightScLvlUpExp = (int)Constants.InitialLightScLvlUpExp;
            LightScLvlUpCost = (int)Constants.InitialLightScLvlUpCost;
            MilitaryScLvlUpExp = (int)Constants.InitialMilitaryScLvlUpExp;
            MilitaryScLvlUpCost = (int)Constants.InitialMilitaryScLvlUpCost;

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
