﻿using System;
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

        [HttpGet]
        [Route("GetValidCountryCodes")]
        public async Task<ActionResult<IEnumerable<string>>> GetValidCountryCodes()
        {
            var ret = await _countryCodeLookupService.GetValidCountryCodes();
            return Ok(ret);
        }

        [HttpGet]
        [Route("GetCountryDetails/{code}")]
        public async Task<ActionResult<CountrySummary>> GetCountryDetails(string code)
        {
            var countryDetails = await _countryCodeLookupService.GetCountryDetails(code);

            return Ok(new CountrySummary(countryDetails));
        }          
    }
}
