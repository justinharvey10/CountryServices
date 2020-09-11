using CountryServices.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<CountryCodeLookupService> _logger;

        public CountryCodeLookupService(
            HttpClient httpClient, IConfiguration configuration, ILogger<CountryCodeLookupService> logger)
        {
            _configuration = configuration;
            _client = httpClient;
            _client.BaseAddress = new Uri(_configuration["CountryCodeLookup:Url"]);
            _client.Timeout = new TimeSpan(0, 0, 30);
            _client.DefaultRequestHeaders.Clear();
            _logger = logger;
        }

        /// <summary>
        /// Call bank API to get list of all country details and summarise
        /// </summary>
        /// <returns>List of codes</returns>
        public async Task<IEnumerable<string>> GetValidCountryCodes()
        {
            IEnumerable<string> ret = new List<string>();
            var request = new HttpRequestMessage(HttpMethod.Get, $"/v2/country?format=json&per_page=500"); //TBD configurable per_page

            using (var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
            {
                var stream = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();

                var outerList = JsonConvert.DeserializeObject<ArrayList>(stream);

                try
                {
                    //First item in array is page data so we take the second
                    JArray outerArray = (outerList[1] as JArray);

                    ret = outerArray.Select(jo => jo.ToObject<CountryCodeSummary>().Id).
                        Concat(outerArray.Select(jo => jo.ToObject<CountryCodeSummary>().Iso2Code));
                }
                catch (Exception e)
                {
                    string message = $"Unexpected data in response to get country codes, response: {stream}";
                    _logger.LogError(e, message);
                    throw new ApplicationException(message, e);
                }
            }

            return ret;
        }

        /// <summary>
        /// Rerieve detailed information for country and return it
        /// </summary>
        /// <param name="code">Country id or iso code</param>
        /// <returns>Country detailed information</returns>
        public async Task<CountryDetails> GetCountryDetails(string code)
        {
            if (code == null)
            {
                string message = $"Null argument value passed for code";
                _logger.LogError(message);
                throw new ArgumentException("Null argument value passed for code");
            }

            CountryDetails countryDetails;

            var request = new HttpRequestMessage(HttpMethod.Get, $"/v2/country/{code}?format=json"); 

            using (var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
            {
                var stream = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();

                try
                {
                    //TBD - handle case where invalid code makes it into this method, deserialise response and return
                    //as part of the return value which would need extending for this

                    var outerList = JsonConvert.DeserializeObject<ArrayList>(stream);

                    //First item in array is page data so we take the second
                    JArray outerArray = (outerList[1] as JArray);

                    //Next take first (and only) item in array
                    countryDetails = outerArray[0].ToObject<CountryDetails>();
                }
                catch(Exception e)
                {
                    string message = $"Unexpected data in response to get country details, response: {stream}";
                    _logger.LogError(e, message);
                    throw new ApplicationException(message, e);
                }

                return countryDetails;
            }
        }
    }
}
