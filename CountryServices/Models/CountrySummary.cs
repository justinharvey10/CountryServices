using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CountryServices.Models
{
    public class CountrySummary
    {
        public CountrySummary(CountryDetails countryDetails)
        {
            CountryName = countryDetails.name;
            Region = countryDetails.region.value;
            CapitalCity = countryDetails.capitalCity;
            Longitude = countryDetails.longitude;
            Latitude = countryDetails.latitude;
        }
        public string CountryName { get; set; }
        public string Region { get; set; }
        public string CapitalCity { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }

    }
}
