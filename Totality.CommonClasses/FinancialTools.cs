
using System;
using System.Net;
using System.Security.Cryptography;

namespace Totality.CommonClasses
{
    public class FinancialTools
    {
        public static long GetExchangeCost(long count, long ourDemand, long theirDemand, long ourQuontityOnStock, long theirQuontityOnStock, double ourSumIndPower, double theirSumIndPower)
        {
            return
                (long)
                (Math.Round(((((double) theirDemand)*(theirSumIndPower/150.0 + 0.01))/
                             ((ourDemand)*(ourSumIndPower/150.0 + 0.01)))*
                            (1 - count +
                             (ourQuontityOnStock + theirQuontityOnStock + 2)*
                             Math.Log((double) (theirQuontityOnStock + 1)/(double) (theirQuontityOnStock + 2 - count)))));
        }

        public static long GetExchangeCostHighAcc(long count, long ourDemand, long theirDemand, long ourQuontityOnStock, long theirQuontityOnStock, double ourSumIndPower, double theirSumIndPower)
        {
            var steps = count/(long) 1000000;
            double cost = 0;
            for (int i = 0; i < steps; i++)
                {
                    var itCost = ((((double)theirDemand) * (theirSumIndPower / 150.0 + 0.01)) /
                             ((ourDemand) * (ourSumIndPower / 150.0 + 0.01))) *
                            (1 - 1000000 +
                             (ourQuontityOnStock + theirQuontityOnStock + 2) *
                             Math.Log((double)(theirQuontityOnStock + 1) / (double)(theirQuontityOnStock + 2 - 1000000)));

                    ourQuontityOnStock += (long)Math.Round(itCost);
                    theirQuontityOnStock -= (long)Math.Round(count / (double)steps);
                    cost += itCost;
                }

            var aCost = 
                ((((double)theirDemand) * (theirSumIndPower / 150.0 + 0.01)) /
                             ((ourDemand) * (ourSumIndPower / 150.0 + 0.01))) *
                            (1 - count%1000000 +
                             (ourQuontityOnStock + theirQuontityOnStock + 2) *
                             Math.Log((double)(theirQuontityOnStock + 1) / (double)(theirQuontityOnStock + 2 - count%1000000)));
            return (long)Math.Round(cost + aCost);
        }

        public static double GetCurrencyRation(long ourDemand, long theirDemand, long ourQuontityOnStock, long theirQuontityOnStock, double ourSumIndPower, double theirSumIndPower)
        {
            return (((theirDemand)*(theirSumIndPower/150.0 + 0.01)) / (((double)ourDemand) * (ourSumIndPower / 150.0 + 0.01))) * (((double)ourQuontityOnStock+1) / (theirQuontityOnStock+1));
        }

        public static double GetMaximumPurchase(long money, long ourDemand, long theirDemand, long ourQuontityOnStock,
            long theirQuontityOnStock, double ourSumIndPower, double theirSumIndPower)
        {
            var B = (((double)theirDemand) * (theirSumIndPower / 150.0 + 0.01)) / ((ourDemand) * (ourSumIndPower / 150.0 + 0.01));
            money -= 1;
            int n = 0;
            double x0 = 15000000;
            double x = 15000000;
            double eps = 0.001;

            do
            {
                x = x0;
                x0 = x - eq(x, B, (ulong)ourQuontityOnStock, (ulong)theirQuontityOnStock, (ulong)money) / (B*der(x, (ulong)ourQuontityOnStock, (ulong)theirQuontityOnStock, (ulong)money));
                n++;

            } while (Math.Abs(x - x0) >= eps && n < 10000000);

            return x;
        }

        public static double GetMaximumPurchaseHighAcc(long money, long ourDemand, long theirDemand, long ourQuontityOnStock,
            long theirQuontityOnStock, double ourSumIndPower, double theirSumIndPower)
        {
            var B = (((double)theirDemand) * (theirSumIndPower / 150.0 + 0.01)) / ((ourDemand) * (ourSumIndPower / 150.0 + 0.01));
            money -= 1;
            double sum = 0;

            var steps = money/(long) 1000000;
            for (int i = 0; i < steps; i++)
            {
                int n = 0;
                double x0 = 1000000;
                double x = 1000000;
                double eps = 0.001;

                do
                {
                    x = x0;
                    x0 = x -
                         eq(x, B, (ulong) ourQuontityOnStock, (ulong) theirQuontityOnStock, (ulong) 1000000)/
                         (B*der(x, (ulong) ourQuontityOnStock, (ulong) theirQuontityOnStock, (ulong) 1000000));
                    n++;

                } while (Math.Abs(x - x0) >= eps && n < 100000);
                sum += x;
                ourQuontityOnStock += 1000000;
                theirQuontityOnStock -= (long)Math.Round(x);
            }

            int an = 0;
            double ax0 = 500000;
            double ax = 500000;
            double aeps = 0.0001;

            do
            {
                ax = ax0;
                ax0 = ax -
                     eq(ax, B, (ulong)ourQuontityOnStock, (ulong)theirQuontityOnStock, (ulong)money%1000000) /
                     (B * der(ax, (ulong)ourQuontityOnStock, (ulong)theirQuontityOnStock, (ulong)money%1000000));
                an++;

            } while (Math.Abs(ax - ax0) >= aeps && an < 100000);
            sum += ax;

            return sum;
        }

        private static double eq(double x, double B, ulong oQ, ulong tQ, ulong money)
        {
            return B*((oQ + tQ + 2)*Math.Log((tQ + 1)/(tQ - x + 2)) - x + 1) - money;
        }

        private static double der(double x, ulong oQ, ulong tQ, ulong money)
        {
            return (oQ + x)/(double)(tQ - x + 2);
        }
    }
}
