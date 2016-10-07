using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace CommonClasses
{
    [DataContract]
    public class DipMsg
    {
        public string from;
        public string to;
        public string offer; //  программно сгенерированное описание контракта
        public string text;  //  дополнительный текст
        public int id;

        public DipMsg(int id, string from, string to, string offer)
        {
            this.id = id;
        }
    }

    public static class DipMsgFactory
    {
        public static int counterId = 0;

        public static DipMsg createMsg(string from, string to, string offer)
        {
            return new DipMsg(counterId++, from, to, offer);
        }

    }
}
