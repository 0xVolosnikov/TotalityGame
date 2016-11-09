using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Totality.CommonClasses;

namespace Totality.Model
{
    [DataContract]
    public class NukeRocket
    {
        [DataMember]
        public string From { get; }
        [DataMember]
        public string To { get; }
        [DataMember]
        public int LifeTime { get; set; }
        [DataMember]
        public int Count { get; set; }
        [DataMember]
        public Guid Id { get; }

        public NukeRocket( string from, string to, int count)
        {
            Id = Guid.NewGuid();
            From = from;
            To = to;
            LifeTime = Constants.NukeRocketLifetime;
        }
    }
}
