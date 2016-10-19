using Newtonsoft.Json;
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

        public bool Save(string savePath)
        {
            try
            {
                System.IO.File.WriteAllText(savePath, JsonConvert.SerializeObject(new DataBaseSave
                {
                    Countries = _countries,
                    DiplomaticalDatabase = _diplomaticalDatabase
                }));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Load(string savePath)
        {
            try
            {
                var loaded = JsonConvert.DeserializeObject<DataBaseSave>(System.IO.File.ReadAllText(savePath));
                _countries = loaded.Countries;
                _diplomaticalDatabase = loaded.DiplomaticalDatabase;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    class DataBaseSave
    {
        public Dictionary<string, Country> Countries { get; set; }
        public Dictionary<string, List<DipContract>> DiplomaticalDatabase { get; set; }
    }
}
