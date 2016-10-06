using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonClasses
{
    class Country
    {
        public struct FSB
        {
            public int lvlNet;
            public List<bool> recruit;

            public FSB(int lvl)
            {
                lvlNet = 0;
                recruit = new List<bool>(8);
            }

        }

        private int money;
        private double ecology;
        private double mood; //настроение

        private double powerResIndustry;
        private double resLand;
        private double resWood;
        private double resSteel;
        private double resOil;
        private double resUranus;
        private double powerLightIndustry;
        private double powerHeavyIndustry;

        private double taxes;

        private double powerMilitary;
        private int nuke;
        private double missile;

        private double powerMassMedia;
        private Dictionary<string, double> massMedia = new Dictionary<string, double>();

        private bool riot;

        private int lvlFSB;
        private Dictionary<string, FSB> FSBStructure = new Dictionary<string, FSB>();

        private double powerScience;
        private int lvlResIndustry;
        private int lvlLightIndustry;
        private int lvlHeavyIndustry;
        private int lvlMilitary;
        private int university;
        private int scientist; 
        
        public Country()
        {

        }
    }
}
