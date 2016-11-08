
using System;

namespace Totality.CommonClasses
{
    public class FinancialTools
    {
        public static long GetExchangeCost(long count, long ourDemand, long theirDemand, long ourQuontityOnStock, long theirQuontityOnStock)
        {
            return (long)( Math.Round( ( ((double)theirDemand + 1)/(ourDemand + 1) )*(-count + (ourQuontityOnStock + theirQuontityOnStock + 2) * Math.Log( (double)(ourQuontityOnStock + 2) / (double)(ourQuontityOnStock + 2 - count))) ) );
        }
    }
}
