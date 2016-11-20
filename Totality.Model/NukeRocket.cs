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
        public string From { get; set; }
        [DataMember]
        public string To { get; set; }
        [DataMember]
        public int LifeTime { get; set; }
        [DataMember]
        public int Count { get; set; }
        [DataMember]
        public Guid Id { get; set; }

        public NukeRocket( string from, string to, int count)
        {
            Id = Guid.NewGuid();
            From = from;
            To = to;
            Count = count;
            LifeTime = Constants.NukeRocketLifetime;
        }
    }
}
