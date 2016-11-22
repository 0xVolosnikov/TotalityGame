using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Totality.Model;

namespace Totality.OrderHandlers
{
    public class Battle
    {
        public Dictionary<string, List<string>> Alliances = new Dictionary<string, List<string>>();
        public int step = 0;

        public Battle(string firstAlliance, string secondAlliance)
        {
            Alliances.Add(firstAlliance, new List<string>());
            Alliances.Add(secondAlliance, new List<string>());
        }
    }
}
