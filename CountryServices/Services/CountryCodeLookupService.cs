using CountryServices.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CountryServices.Services
{
    public class CountryCodeLookupService : ICountryCodeLookupService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;

        public CountryCodeLookupService(HttpClient httpClient, IConfiguration configuration)
        {
            _configuration = configuration;
            _client = httpClient;
            _client.BaseAddress = new Uri(_configuration["CountryCodeLookup:Url"]);
            _client.Timeout = new TimeSpan(0, 0, 30);
            _client.DefaultRequestHeaders.Clear();
        }

        public async Task<IEnumerable<string>> GetValidCountryCodes()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/v2/country?format=json&per_page=500"); //TBD configurable per_page

            using (var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
            {
                var stream = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();

                //TBD improve de-serialisation 
                var outerList = JsonConvert.DeserializeObject<ArrayList>(stream);
                JArray outerArray = (outerList[1] as JArray);

                return outerArray.Select(jo => jo.ToObject<CountryCodeSummary>().Id).
                    Concat(outerArray.Select(jo => jo.ToObject<CountryCodeSummary>().Iso2Code));
            }
        }

        public async Task<CountryDetails> GetCountryDetails(string code)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/v2/country/{code}?format=json"); 

            using (var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
            {
                var stream = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();

                //TBD improve de-serialisation 
                var outerList = JsonConvert.DeserializeObject<ArrayList>(stream);
                JArray outerArray = (outerList[1] as JArray);
                var countryDetails = outerArray[0].ToObject<CountryDetails>();

                return countryDetails;
            }
        }
    }
}
