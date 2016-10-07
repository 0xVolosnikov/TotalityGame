using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonClasses.Diplomatical
{
    public class DipContract
    {
        public int id;
        public string from;
        public string to;
        public DipContract(int id,string from, string to)
        {
            this.id = id;
            this.from = from;
            this.to = to;
        }
    }

    public static class ContractFactory
    {
        public static int counterId = 0;

        public static DipContract createContract(string from, string to, string offer)
        {
            return new DipContract(counterId++, from, to);
        }

    }
}
