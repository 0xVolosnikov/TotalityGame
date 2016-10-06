using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonClasses
{
    public class NukeRocket
    {
        string from;
        string to;
        public int lifeTime;
        public int count;

        public NukeRocket(string from, string to)
        {
            this.from = from;
            this.to = to;
            lifeTime = Constants.nukeRocketLifetime;
        }
    }
}
