using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Totality.Model;

namespace Totality.Model.Interfaces
{
    public interface IDataLayer
    {
        bool AddCountry(Country newCountry);
        bool DeleteCountry(string name);
        Country GetCountry(string name);
        bool UpdateCountry(Country newCountry);


    }
}
