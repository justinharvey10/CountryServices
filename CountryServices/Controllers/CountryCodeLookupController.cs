using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CountryServices.Models;
using CountryServices.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CountryServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryCodeLookupController : ControllerBase
    {
        private readonly ILogger<CountryCodeLookupController> _logger;
        private readonly ICountryCodeLookupService _countryCodeLookupService;

        public CountryCodeLookupController(ILogger<CountryCodeLookupController> logger, ICountryCodeLookupService countryCodeLookupService)
        {
            _logger = logger;
            _countryCodeLookupService = countryCodeLookupService;
        }

        /// <summary>
        /// Call service to get all country codes
        /// </summary>
        /// <returns>Code list</returns>
        [HttpGet]
        [Route("GetValidCountryCodes")]
        public async Task<ActionResult<IEnumerable<string>>> GetValidCountryCodes()
        {
            try
            {
                var ret = await _countryCodeLookupService.GetValidCountryCodes();

                return Ok(ret);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Exception caught in GetValidCountryCodes");
                throw;
            }
        }

        /// <summary>
        /// Call service to get country details
        /// </summary>
        /// <param name="code">Country id or iso code</param>
        /// <returns>Country details</returns>
        [HttpGet]
        [Route("GetCountryDetails/{code}")]
        public async Task<ActionResult<CountrySummary>> GetCountryDetails(string code)
        {
            try
            {
                var countryDetails = await _countryCodeLookupService.GetCountryDetails(code);

                return Ok(new CountrySummary(countryDetails));  //Map to view model
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Exception caught in GetCountryDetails");
                throw;
            }
        }          
    }
}
