using CountryServices.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CountryServices.Services
{
    public interface ICountryCodeLookupService
    {
        Task<IEnumerable<string>> GetValidCountryCodes();
        Task<CountryDetails> GetCountryDetails(string code);
    }
}