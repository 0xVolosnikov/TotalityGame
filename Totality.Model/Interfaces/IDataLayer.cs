

using System;
using System.Collections.Generic;
using Totality.Model.Diplomatical;

namespace Totality.Model.Interfaces
{
    public interface IDataLayer
    {
        bool Save(string saveName);
        bool Load(string saveName);

        bool AddCountry(Country newCountry);
        bool DeleteCountry(string name);
        Country GetCountry(string name);
        bool UpdateCountry(Country newCountry);
        Dictionary<string, Country> GetCountries();

        object GetProperty(string countryName, string propertyName);
        void SetProperty(string countryName, string propertyName, object value);

        long GetCurrencyOnStock(string countryName);
        void SetCurrencyOnStock(string countryName, long count);

        List<DipContract> GetContractList();
        void AddContract(DipContract contract);
        void BreakContract(Guid id);
        Dictionary<string, long> GetStock();
    }
}
