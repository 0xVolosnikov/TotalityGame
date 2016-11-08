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
        public string newsCode;
        public List<string> args = new List<string>();
        
        public News(string code)
        {
            newsCode = code;
        }
    }
}
