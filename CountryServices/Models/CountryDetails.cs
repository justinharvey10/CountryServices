using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CountryServices.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Region
    {
        public string id { get; set; }
        public string iso2code { get; set; }
        public string value { get; set; }
    }

    public class Adminregion
    {
        public string id { get; set; }
        public string iso2code { get; set; }
        public string value { get; set; }
    }

    public class IncomeLevel
    {
        public string id { get; set; }
        public string iso2code { get; set; }
        public string value { get; set; }
    }

    public class LendingType
    {
        public string id { get; set; }
        public string iso2code { get; set; }
        public string value { get; set; }
    }

    public class CountryDetails
    {
        public string id { get; set; }
        public string iso2Code { get; set; }
        public string name { get; set; }
        public Region region { get; set; }
        public Adminregion adminregion { get; set; }
        public IncomeLevel incomeLevel { get; set; }
        public LendingType lendingType { get; set; }
        public string capitalCity { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
    }


}
