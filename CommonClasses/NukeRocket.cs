using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CommonClasses
{
    [DataContract]
    public class NukeRocket
    {
        public string from { get; }
        public string to { get; }
        public int lifeTime;
        public int count;
        public int id;
        static int idCount;

        public NukeRocket(string from, string to)
        {
            this.id = idCount;
            idCount++;
            if (idCount > 1000) idCount = 0;

            this.from = from;
            this.to = to;
            lifeTime = Constants.nukeRocketLifetime;
        }
    }
}
