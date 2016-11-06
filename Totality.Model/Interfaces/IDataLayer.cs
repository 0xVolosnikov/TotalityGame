

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

        object GetProperty(string countryName, string propertyName);
        void SetProperty(string countryName, string propertyName, object value);

        long GetCurrencyOnStock(string countryName);
        void SetCurrencyOnStock(string countryName, long count);


    }
}
