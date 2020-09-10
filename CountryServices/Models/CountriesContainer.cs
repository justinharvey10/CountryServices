using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CountryServices.Models
{
    public class CountriesContainer
    {
        public PageData PageData { get; set; }
        public List<CountryCodeSummary> CountryCodeSummaries { get; set; }
    }

    public class PageData
    {

    }
}
