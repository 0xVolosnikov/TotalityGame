using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using Totality.CommonClasses;
using Totality.CommonClasses.Diplomatical;
using Totality.Model;
using Totality.Model.Interfaces;

namespace Totality.DataLayer
{
    public class DataLayer : AbstractLoggable, IDataLayer
    {
        private class DataBaseSave
        {
            public Dictionary<string, Country> Countries { get; set; }
            public Dictionary<string, List<DipContract>> DiplomaticalDatabase { get; set; }
            public Dictionary<string, long> FinancialStock { get; set; }
        }

        private Dictionary<string, List<DipContract>> _diplomaticalDatabase = new Dictionary<string, List<DipContract>>();
        private Dictionary<string, Country> _countries = new Dictionary<string, Country>();
        private Dictionary<string, FieldInfo> countryFields = new Dictionary<string, FieldInfo>();
        private Dictionary<string, long> _financialStock = new Dictionary<string, long>();

        public DataLayer(ILogger logger) : base(logger)
        {
            Type t = typeof(Country);
            FieldInfo[] fields = t.GetFields();
            foreach (FieldInfo field in fields)           
                countryFields.Add(field.Name, field);            
        }

        public bool AddCountry(Country newCountry)
        {
            try
            {
                Country country = newCountry;
                _countries.Add(newCountry.Name, country);
                _financialStock.Add(newCountry.Name, Constants.InitialCurrencyOnStock);
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

        public object GetProperty(string countryName, string propertyName)
        {
            return countryFields[propertyName].GetValue(_countries[countryName]);
        }

        public void SetProperty(string countryName, string propertyName, object value)
        {
            countryFields[propertyName].SetValue(_countries[countryName], value);
        }

        public long GetCurrencyOnStock(string countryName)
        {
            return _financialStock[countryName];
        }

        public void SetCurrencyOnStock(string countryName, long count)
        {
            _financialStock[countryName] = count;
        }

        public bool Save(string savePath)
        {
            try
            {
                System.IO.File.WriteAllText(savePath, JsonConvert.SerializeObject(new DataBaseSave
                {
                    Countries = _countries,
                    DiplomaticalDatabase = _diplomaticalDatabase,
                    FinancialStock = _financialStock
                }));
                return true;
            }
            catch (Exception e)
            {
                _log.Error("Can't save database! " + e.Message);
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
                _financialStock = loaded.FinancialStock;
                return true;
            }
            catch (Exception e)
            {
                _log.Error("Can't load database! " + e.Message);
                return false;
            }
        }
    }
}
