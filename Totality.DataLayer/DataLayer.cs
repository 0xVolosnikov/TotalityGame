using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Totality.CommonClasses.Diplomatical;
using Totality.Model;
using Totality.Model.Interfaces;

namespace Totality.DataLayer
{
    public class DataLayer : IDataLayer
    {
        private Dictionary<string, List<DipContract>> _diplomaticalDatabase = new Dictionary<string, List<DipContract>>();
        private Dictionary<string, Country> _countries = new Dictionary<string, Country>();


        public bool AddCountry(Country newCountry)
        {
            try
            {
                _countries.Add(newCountry.Name, newCountry);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool DeleteCountry(string name)
        {
            try
            {
                _countries.Remove(name);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public Country GetCountry(string name)
        {
            try
            {
                return _countries[name];
            }
            catch (Exception e)
            {
                throw new KeyNotFoundException(nameof(GetCountry));
            }
        }

        public bool UpdateCountry(Country newCountry)
        {
            try
            {
                _countries[newCountry.Name] = newCountry;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
