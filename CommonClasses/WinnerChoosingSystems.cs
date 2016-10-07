using System;

namespace CommonClasses
{
    public static class WinnerChoosingSystems
    {
        static Random randomizer = new Random( (DateTime.Today - new DateTime(1995, 1, 1) ).Milliseconds );
        public static bool tsop(int attackerLvl, int defenderLvl)
        {
            double attackerChance = 0.5;

            if (attackerLvl >= defenderLvl)            
                for (int i = 1; i <= defenderLvl - attackerLvl; i++)
                {
                    attackerChance += Math.Pow(0.5, i+1);
                }            
            else
                for (int i = 1; i <=  attackerLvl - defenderLvl; i++)
                {
                    attackerChance -= Math.Pow(0.5, i+1);
                }

            int result = randomizer.Next(1000);

            if (result < (int)(attackerChance*1000))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

       public  static int nukeMassiveTsop(int count, out int loosed, int nukesCount, int attackerLvl, int defenderLvl)
        {
            double attackerChance = 0.5;
            int result = 0;
            loosed = 0;

            if (attackerLvl >= defenderLvl)
                for (int i = 1; i <= defenderLvl - attackerLvl; i++)
                {
                    attackerChance += Math.Pow(0.5, i + 1);
                }
            else
                for (int i = 1; i <= attackerLvl - defenderLvl; i++)
                {
                    attackerChance -= Math.Pow(0.5, i + 1);
                }


            for (int i = 0; i < count && result < nukesCount; i++)
            {
                if (randomizer.Next(1000) < (int)(attackerChance * 1000))
                    result++;
                loosed++;
            }
            return result;
        }
    }
}
