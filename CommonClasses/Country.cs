using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonClasses;

namespace CommonClasses
{
    public class Country
    {
        public int money;
        public Property ecology;
        public Property mood; //настроение

        public Property powerResIndustry;
        public Property resLand;
        public Property resWood;
        public Property resSteel;
        public Property resOil;
        public Property resUranus;
        public Property powerLightIndustry;
        public Property powerHeavyIndustry;

        public Property taxes;

        public Property powerMilitary;
        public int nuke;
        public int missile;

        public Property powerMassMedia;
        public Dictionary<string, double> massMedia = new Dictionary<string, double>();

        public bool riot;

        public int lvlFSB;
        public Dictionary<string, FSB> FSBStructure = new Dictionary<string, FSB>();

        public Property powerScience;
        public int lvlResIndustry;
        public int lvlLightIndustry;
        public int lvlHeavyIndustry;
        public int lvlMilitary;
        public int university;
        public int scientist;

        public Country()
        {
            money = Constants.initialMoney;
            
        }

        public void NukeExplosion(int count)
        {

        }

        public struct FSB
        {
            public int lvlNet;
            public List<bool> recruit;

            public FSB(int lvl)
            {
                lvlNet = 0;
                recruit = new List<bool>(Constants.countOfMinisters);
            }
        }

        public class Property
        {
            public double value;
            private List<Buff> buffs = new List<Buff>();

            public void addBuff(int time, bool valueType, double buff)
            {
                buffs.Add(new Buff(time, valueType, buff));
            }

            public double getBuffedValue()
            {
                double bufValue = value;
                double mult = 1; //множитель
                //добавление баффов по значению
                for (int i = 0; i < buffs.Count; i++)
                {
                    if (buffs[i].valueType)
                    {
                        bufValue += buffs[i].buff;
                    }
                    else
                    {
                        mult += buffs[i].buff / 100;
                    }
                }

                return bufValue * mult;
            }

            public void nextStep()
            {
                foreach (Buff bff in buffs)
                {
                    bff.time--;
                    if (bff.time <= 0)
                    {
                        buffs.Remove(bff);
                    }
                }
            }

            public class Buff
            {
                public int time;
                public bool valueType; //бафф по значению
                public double buff;
                public Buff(int time, bool valueType, double buff)
                {
                    this.time = time;
                    this.valueType = valueType;
                    this.buff = buff;
                }
            }
        }
    }
}
