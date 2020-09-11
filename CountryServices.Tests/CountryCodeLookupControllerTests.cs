using CountryServices.Controllers;
using CountryServices.Models;
using CountryServices.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CountryServices.Tests
{
    public class CountryCodeLookupControllerTests
    {
        private Mock<ILogger<CountryCodeLookupController>> _mockLogger = null;
        private Mock<ICountryCodeLookupService> _mockCountryCodeLookupService = null;

        const string ISO_CODE_GB = "GB";
        private readonly List<string> _countryCodes = new List<string>() { "GB", "GBR" };

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<CountryCodeLookupController>>();
            _mockCountryCodeLookupService = new Mock<ICountryCodeLookupService>();
        }

        [Test]
        public async Task GivenServiceReturnsValidCodes_WhenGetCountryCodesCalled_ThenCodesAreReturned()
        {
            CountryCodeLookupController countryCodeLookupController =
                new CountryCodeLookupController(_mockLogger.Object, _mockCountryCodeLookupService.Object);

            Task<IEnumerable<string>> task = new Task<IEnumerable<string>>(() => _countryCodes);
            task.Start();

            _mockCountryCodeLookupService.Setup(service => service.GetValidCountryCodes()).Returns(task);

            var codes = await countryCodeLookupController.GetValidCountryCodes();

            Assert.IsInstanceOf(typeof(OkObjectResult), codes.Result);
            Assert.AreEqual(_countryCodes, (codes.Result as OkObjectResult).Value);

            _mockCountryCodeLookupService.Verify(s => s.GetValidCountryCodes(), Times.Once);
        }

        [Test]
        public void GivenServiceThrows_WhenGetCountryCodesCalled_ThenExceptionLoggedAndThrown()
        {
            CountryCodeLookupController countryCodeLookupController =
                new CountryCodeLookupController(_mockLogger.Object, _mockCountryCodeLookupService.Object);

            var testException = new ApplicationException("Test Exception");

            _mockCountryCodeLookupService.Setup(service => service.GetValidCountryCodes()).Throws(testException);

            var ex = Assert.Throws<AggregateException>(() =>
            {
                var ret = countryCodeLookupController.GetValidCountryCodes().Result;
            });

            Assert.AreEqual(testException, ex.InnerException);

            _mockCountryCodeLookupService.Verify(s => s.GetValidCountryCodes(), Times.Once);

            //TBD verify logging (overcome issues with extension methods)
        }

        [Test]
        public async Task GivenServiceReturnsValidCodes_WhenGetCountryDetails_ThenDetailsAreReturned()
        {
            CountryCodeLookupController countryCodeLookupController =
                new CountryCodeLookupController(_mockLogger.Object, _mockCountryCodeLookupService.Object);

            CountryDetails countryDetails = new CountryDetails();

            Task<CountryDetails> task = new Task<CountryDetails>(() => countryDetails);
            task.Start();

            _mockCountryCodeLookupService.Setup(service => service.GetCountryDetails(ISO_CODE_GB)).Returns(task);

            var codes = await countryCodeLookupController.GetCountryDetails(ISO_CODE_GB);

            Assert.IsInstanceOf(typeof(OkObjectResult), codes.Result);
            Assert.AreEqual(countryDetails.name, ((CountrySummary)(codes.Result as OkObjectResult).Value).CountryName);

            _mockCountryCodeLookupService.Verify(s => s.GetCountryDetails(ISO_CODE_GB), Times.Once);
        }

        [Test]
        public void GivenServiceThrows_WhenGetCountryDetails_ThenExceptionLoggedAndThrown()
        {
            CountryCodeLookupController countryCodeLookupController =
                new CountryCodeLookupController(_mockLogger.Object, _mockCountryCodeLookupService.Object);

            var testException = new ApplicationException("Test Exception");

            _mockCountryCodeLookupService.Setup(service => service.GetCountryDetails(ISO_CODE_GB)).Throws(testException);

            var ex = Assert.Throws<AggregateException>(() =>
            {
                var ret = countryCodeLookupController.GetCountryDetails(ISO_CODE_GB).Result;
            });

            Assert.AreEqual(testException, ex.InnerException);

            _mockCountryCodeLookupService.Verify(s => s.GetCountryDetails(ISO_CODE_GB), Times.Once);

            //TBD verify logging (overcome issues with extension methods)
        }
    }
}