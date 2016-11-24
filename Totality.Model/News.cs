using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Totality.Model
{
    [DataContract]
    public class News
    {
        [DataMember]
        public bool IsOur;
        [DataMember]
        public string text;
        [DataMember]
        public string color;
        [DataMember]
        public string picture;

        public News(bool isOur)
        {
            IsOur = isOur;
        }
    }
}
