
using System;

namespace Totality.CommonClasses
{
    public class FinancialTools
    {
        public static long GetExchangeCost(long count, long ourDemand, long theirDemand, long ourQuontityOnStock, long theirQuontityOnStock)
        {
            return (long)( Math.Round( ( ((double)theirDemand + 1)/(ourDemand + 1) )*(1-count + (ourQuontityOnStock + theirQuontityOnStock + 2) * Math.Log( (double)(theirQuontityOnStock + 1) / (double)(theirQuontityOnStock + 2 - count))) ) );
        }

        public static double GetCurrencyRation(long ourDemand, long theirDemand, long ourQuontityOnStock, long theirQuontityOnStock)
        {
            return ((theirDemand+1) / ((double)ourDemand+1)) * (((double)ourQuontityOnStock+1) / (theirQuontityOnStock+1));
        }
    }
}
