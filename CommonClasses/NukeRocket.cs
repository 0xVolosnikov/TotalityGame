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

        public NukeRocket(int id, string from, string to)
        {
            this.id = id;

            this.from = from;
            this.to = to;
            lifeTime = Constants.nukeRocketLifetime;
        }
    }

    public static class NukeRocketFactory
    {
        public static int counterId = 0;

        public static NukeRocket createRocket(string from, string to)
        {
            return new NukeRocket(counterId++, from, to);
        }

    }
}
