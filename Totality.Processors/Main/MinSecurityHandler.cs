using System;
using System.Collections.Generic;
using Totality.CommonClasses;
using Totality.Handlers.News;
using Totality.Model;
using Totality.Model.Interfaces;
using static Totality.Model.Country;

namespace Totality.Handlers.Main
{
    public class MinSecurityHandler : AbstractHandler, IMinisteryHandler
    {
        private enum Orders { ImproveNetwork , AddAgents , OrderToAgent , Purge , CounterSpyLvlUp , ShadowingUp, IntelligenceUp , Sabotage }
        public delegate void SecretOrder(Order order);
        public event SecretOrder SecretOrderProcessed;

        public MinSecurityHandler(NewsHandler newsHandler, IDataLayer dataLayer, ILogger logger) : base(newsHandler, dataLayer, logger)
        {
        }

        public bool ProcessOrder(Order order)
        {
            switch (order.OrderNum)
            {
                case (int)Orders.ImproveNetwork: return ImproveNetwork(order);

                case (int)Orders.AddAgents: return AddAgents(order);

                case (int)Orders.OrderToAgent: return OrderToAgent(order);

                case (int)Orders.Purge: return Purge(order);

                case (int)Orders.CounterSpyLvlUp: return CounterSpyLvlUp(order);

                case (int)Orders.ShadowingUp: return ShadowingLvlUp(order);

                case (int)Orders.IntelligenceUp: return IntelligenceUp(order);

                case (int)Orders.Sabotage: return Sabotage(order);

                default: throw new ArgumentException("Order " + order + " not found in " + typeof(MinSecurityHandler));
            }
        }

        private bool ImproveNetwork(Order order)
        {
            var money = (long)_dataLayer.GetProperty(order.CountryName, "Money");
            var networkLvlUpCost = Constants.InitialNetworkLvlUpCost;

            var spyNetworks = (Dictionary<string, SpyNetwork>)_dataLayer.GetProperty(order.CountryName, "SpyNetworks");
            int count = 0;
            foreach (KeyValuePair<string, SpyNetwork> net in spyNetworks)
            {
                count += net.Value.NetLvl;
            }

            networkLvlUpCost = (long)(networkLvlUpCost*Math.Pow(Constants.NetworkLvlUpCostRatio, count));

            var intLvl = (int)_dataLayer.GetProperty(order.CountryName, "IntelligenceLvl");
            var counterSpyLvl = (int)_dataLayer.GetProperty(order.TargetCountryName, "CounterSpyLvl");

            if (money < networkLvlUpCost || !WinnerChoosingSystems.Tsop(intLvl, counterSpyLvl))
                return false;

            money -= networkLvlUpCost;
            _dataLayer.SetProperty(order.CountryName, "Money", money);

            if (!spyNetworks.ContainsKey(order.TargetCountryName))
            {
                spyNetworks.Add(order.TargetCountryName, new SpyNetwork() { NetLvl = 1 });
                _dataLayer.SetProperty(order.CountryName, "SpyNetworks", spyNetworks);
            }
            else
            {
                if (spyNetworks[order.TargetCountryName].NetLvl < 3)
                    spyNetworks[order.TargetCountryName].NetLvl++;
                _dataLayer.SetProperty(order.CountryName, "SpyNetworks", spyNetworks);
            }

            _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Улучшена разведсеть в стране " + order.TargetCountryName + "." });
            return true;
        }

        private bool AddAgents(Order order)
        {
            var money = (long)_dataLayer.GetProperty(order.CountryName, "Money");
            var agentCost = Constants.InitialAgentCost;

            var spyNetworks = (Dictionary<string, SpyNetwork>)_dataLayer.GetProperty(order.CountryName, "SpyNetworks");
            int count = 0;
            foreach (KeyValuePair<string, SpyNetwork> net in spyNetworks)
            {
                count += net.Value.Recruit.FindAll(x => x == true).Count;
            }

            agentCost = (long)(agentCost * Math.Pow(Constants.AgentCostRatio, count));

            if (money < agentCost)
                return false;

            if (!spyNetworks.ContainsKey(order.TargetCountryName) || spyNetworks[order.TargetCountryName].NetLvl < Constants.MinAgentLvl)
            {
                return false;
            }

            money -= agentCost;
            _dataLayer.SetProperty(order.CountryName, "Money", money);

            var intLvl = (int)_dataLayer.GetProperty(order.CountryName, "IntelligenceLvl");
            var shadowingLvl = (int)_dataLayer.GetProperty(order.TargetCountryName, "ShadowingLvl");

            if (!WinnerChoosingSystems.Tsop(intLvl, shadowingLvl))
                return false;

            spyNetworks[order.TargetCountryName].Recruit[order.TargetMinistery] = true;

            if (order.TargetMinistery == (short)Mins.Security) // В случае вербовки МГБ, получаем его агентов и режем шпионов у себя
            {
                var foreignSpyNetworks = (Dictionary<string, SpyNetwork>)_dataLayer.GetProperty(order.TargetCountryName, "SpyNetworks");
                foreach (KeyValuePair<string, SpyNetwork> net in foreignSpyNetworks)
                {
                    if (net.Key == order.CountryName)
                    { 
                        var foreignSpyes = (List<List<string>>)_dataLayer.GetProperty(order.CountryName, "ForeignSpyes");
                        for (int i = 0; i < net.Value.Recruit.Count; i++)
                            if (net.Value.Recruit[i])
                            {
                                foreignSpyes[i].Remove(order.TargetCountryName);
                            }
                        _dataLayer.SetProperty(order.CountryName, "ForeignSpyes", foreignSpyes);
                        continue;
                    }

                    if (!spyNetworks.ContainsKey(net.Key))
                    {
                        spyNetworks.Add(net.Key, net.Value);
                        continue;
                    }

                    if (net.Value.NetLvl > spyNetworks[net.Key].NetLvl)
                        spyNetworks[net.Key].NetLvl = net.Value.NetLvl;

                    var foreignSpyesInTarget = (List<List<string>>)_dataLayer.GetProperty(net.Key, "ForeignSpyes");
                    for (int i = 0; i < net.Value.Recruit.Count; i++)
                        if (net.Value.Recruit[i] && !spyNetworks[net.Key].Recruit[i])
                        {
                            spyNetworks[net.Key].Recruit[i] = true;
                            foreignSpyesInTarget[i].Add(order.CountryName);
                        }
                    _dataLayer.SetProperty(net.Key, "ForeignSpyes", foreignSpyesInTarget);
                }
                foreignSpyNetworks.Remove(order.CountryName);
                _dataLayer.SetProperty(order.TargetCountryName, "SpyNetworks", foreignSpyNetworks);
            }

           _dataLayer.SetProperty(order.CountryName, "SpyNetworks", spyNetworks);

            var spyesInTarget = (List<List<string>>)_dataLayer.GetProperty(order.TargetCountryName, "ForeignSpyes");
            spyesInTarget[order.TargetMinistery].Add(order.CountryName);
            _dataLayer.SetProperty(order.TargetCountryName, "ForeignSpyes", spyesInTarget);

            _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Внедрены агенты в страну " + order.TargetCountryName + ", " + order.TargetMinistery + "."});
            return true;
        }

        private bool OrderToAgent(Order order)
        {
            if (order.Value == -1)
            {
                var foreignSpyes = (List<List<string>>)_dataLayer.GetProperty(order.TargetCountryName, "ForeignSpyes");

                var spyNetwork = (Dictionary<string, SpyNetwork>)_dataLayer.GetProperty(order.CountryName, "SpyNetworks");
                spyNetwork[order.TargetCountryName].Recruit[order.TargetMinistery] = false;
                _dataLayer.SetProperty(order.CountryName, "SpyNetworks", spyNetwork);

                foreignSpyes[order.TargetMinistery].Remove(order.CountryName);
                _dataLayer.SetProperty(order.CountryName, "ForeignSpyes", foreignSpyes);

                return true;
            }

            else SecretOrderProcessed.Invoke(new Order(order.TargetCountryName, order.TargetCountryName2)
            {
                Count = order.Count,
                Ministery = order.TargetMinistery,
                OrderNum = order.Value,
                Value = order.Value2
            });
            _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Отдан приказ агентам в стране " + order.TargetCountryName + ", " + order.TargetMinistery + "."});
            return true;
        }

        private bool Purge(Order order)
        {
            var shadowingLvl = (int)_dataLayer.GetProperty(order.CountryName, "ShadowingLvl");

            var money = (long)_dataLayer.GetProperty(order.CountryName, "Money");
            var purgeCost = Constants.PurgeCost;

            if (money < purgeCost )
                return false;

            money -= purgeCost;
            _dataLayer.SetProperty(order.CountryName, "Money", money);

            var foreignSpyes = (List<List<string>>)_dataLayer.GetProperty(order.CountryName, "ForeignSpyes");
            for (int i = 0; i < foreignSpyes[order.TargetMinistery].Count; i++)
            {
                var intLvl = (int)_dataLayer.GetProperty(foreignSpyes[order.TargetMinistery][i], "IntelligenceLvl");
                if (WinnerChoosingSystems.Tsop(shadowingLvl, intLvl))
                {
                    var foreignSpyNetwork = (Dictionary<string, SpyNetwork>)_dataLayer.GetProperty(foreignSpyes[order.TargetMinistery][i], "SpyNetworks");
                    foreignSpyNetwork[order.CountryName].Recruit[order.TargetMinistery] = false;
                    _dataLayer.SetProperty(foreignSpyes[order.TargetMinistery][i], "SpyNetworks", foreignSpyNetwork);
                    foreignSpyes[order.TargetMinistery].Remove(foreignSpyes[order.TargetMinistery][i]);
                }
            }
            _dataLayer.SetProperty(order.CountryName, "ForeignSpyes", foreignSpyes);

            var minsBlocks = (int[])_dataLayer.GetProperty(order.CountryName, "MinsBlocks");
            minsBlocks[order.TargetMinistery] += Constants.PurgeTime;
            _dataLayer.SetProperty(order.CountryName, "MinsBlocks", minsBlocks);

            _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Произведены чистки: " + order.TargetMinistery + "."});
            return true;
        }

        private bool CounterSpyLvlUp(Order order)
        {
            var money = (long)_dataLayer.GetProperty(order.CountryName, "Money");
            var counterSpyLvlUpCost = (long)_dataLayer.GetProperty(order.CountryName, "CounterSpyLvlUpCost");

            if (money < counterSpyLvlUpCost)
                return false;

            money -= counterSpyLvlUpCost;
            _dataLayer.SetProperty(order.CountryName, "Money", money);
            counterSpyLvlUpCost = (long)(counterSpyLvlUpCost * Constants.CounterSpyLvlUpCostRatio);
            _dataLayer.SetProperty(order.CountryName, "CounterSpyLvlUpCost", counterSpyLvlUpCost);

            var lvl = (int)_dataLayer.GetProperty(order.CountryName, "CounterSpyLvl") + 1;
            _dataLayer.SetProperty(order.CountryName, "CounterSpyLvl", lvl);


            _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Повышен уровень контрразведки. "});
            return true;
        }

        private bool ShadowingLvlUp(Order order)
        {
            var money = (long)_dataLayer.GetProperty(order.CountryName, "Money");
            var shadowingLvlUpCost = (long)_dataLayer.GetProperty(order.CountryName, "ShadowingLvlUpCost");

            if (money < shadowingLvlUpCost)
                return false;

            money -= shadowingLvlUpCost;
            _dataLayer.SetProperty(order.CountryName, "Money", money);
            shadowingLvlUpCost = (long)(shadowingLvlUpCost * Constants.ShadowingLvlUpCostRatio);
            _dataLayer.SetProperty(order.CountryName, "ShadowingLvlUpCost", shadowingLvlUpCost);

            var lvl = (int)_dataLayer.GetProperty(order.CountryName, "ShadowingLvl") + 1;
            _dataLayer.SetProperty(order.CountryName, "ShadowingLvl", lvl);

            _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Повышен уровень слежки. " });
            return true;
        }

        private bool IntelligenceUp(Order order)
        {
            var money = (long)_dataLayer.GetProperty(order.CountryName, "Money");
            var intelligenceLvlUpCost = (long)_dataLayer.GetProperty(order.CountryName, "IntelligenceLvlUpCost");

            if (money < intelligenceLvlUpCost)
                return false;

            money -= intelligenceLvlUpCost;
            _dataLayer.SetProperty(order.CountryName, "Money", money);
            intelligenceLvlUpCost = (long)(intelligenceLvlUpCost * Constants.IntelligenceLvlUpCostRatio);
            _dataLayer.SetProperty(order.CountryName, "IntelligenceLvlUpCost", intelligenceLvlUpCost);

            var lvl = (int)_dataLayer.GetProperty(order.CountryName, "IntelligenceLvl") + 1;
            _dataLayer.SetProperty(order.CountryName, "IntelligenceLvl", lvl);

            _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Повышен уровень разведки. " });
            return true;
        }

        private bool Sabotage(Order order)
        {
            var spyNetwork = (Dictionary<string, SpyNetwork>)_dataLayer.GetProperty(order.CountryName, "SpyNetworks");

            if (!spyNetwork[order.TargetCountryName].Recruit[order.TargetMinistery])
                return false;

            spyNetwork[order.TargetCountryName].Recruit[order.TargetMinistery] = false;
            _dataLayer.SetProperty(order.CountryName, "SpyNetworks", spyNetwork);

            var foreignSpyes = (List<List<string>>)_dataLayer.GetProperty(order.TargetCountryName, "ForeignSpyes");
            foreignSpyes[order.TargetMinistery].Remove(order.CountryName);
            _dataLayer.SetProperty(order.CountryName, "ForeignSpyes", foreignSpyes);

            var minsBlocks = (int[])_dataLayer.GetProperty(order.TargetCountryName, "MinsBlocks");
            minsBlocks[order.TargetMinistery] += Constants.SabotageTime;
            _dataLayer.SetProperty(order.TargetCountryName, "MinsBlocks", minsBlocks);

            _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Произведен саботаж в стране " + order.TargetCountryName +", " + order.TargetMinistery + "."});
            return true;
        }
    }
}
