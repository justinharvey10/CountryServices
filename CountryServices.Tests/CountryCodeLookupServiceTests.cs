using CountryServices.Controllers;
using CountryServices.Models;
using CountryServices.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CountryServices.Tests
{
    public class CountryCodeLookupServiceTests
    {
        private Mock<ILogger<CountryCodeLookupService>> _mockLogger = null;
        private Mock<IConfiguration> _mockConfiguration = null;
        const string ISO_CODE_GB = "GB";
        private readonly List<string> _countryCodes = new List<string>() { "GB", "GBR" };
        const string COUNTRY_CODES = "[{\"page\":1,\"pages\":152,\"per_page\":\"2\",\"total\":304},[{\"id\":\"ABW\",\"iso2Code\":\"AW\",\"name\":\"Aruba\",\"region\":{\"id\":\"LCN\",\"iso2code\":\"ZJ\",\"value\":\"Latin America & Caribbean \"},\"adminregion\":{\"id\":\"\",\"iso2code\":\"\",\"value\":\"\"},\"incomeLevel\":{\"id\":\"HIC\",\"iso2code\":\"XD\",\"value\":\"High income\"},\"lendingType\":{\"id\":\"LNX\",\"iso2code\":\"XX\",\"value\":\"Not classified\"},\"capitalCity\":\"Oranjestad\",\"longitude\":\"-70.0167\",\"latitude\":\"12.5167\"},{\"id\":\"AFG\",\"iso2Code\":\"AF\",\"name\":\"Afghanistan\",\"region\":{\"id\":\"SAS\",\"iso2code\":\"8S\",\"value\":\"South Asia\"},\"adminregion\":{\"id\":\"SAS\",\"iso2code\":\"8S\",\"value\":\"South Asia\"},\"incomeLevel\":{\"id\":\"LIC\",\"iso2code\":\"XM\",\"value\":\"Low income\"},\"lendingType\":{\"id\":\"IDX\",\"iso2code\":\"XI\",\"value\":\"IDA\"},\"capitalCity\":\"Kabul\",\"longitude\":\"69.1761\",\"latitude\":\"34.5228\"}]]";
        const string COUNTRY_DETAILS = "[{\"page\":1,\"pages\":1,\"per_page\":\"50\",\"total\":1},[{\"id\":\"GBR\",\"iso2Code\":\"GB\",\"name\":\"United Kingdom\",\"region\":{\"id\":\"ECS\",\"iso2code\":\"Z7\",\"value\":\"Europe & Central Asia\"},\"adminregion\":{\"id\":\"\",\"iso2code\":\"\",\"value\":\"\"},\"incomeLevel\":{\"id\":\"HIC\",\"iso2code\":\"XD\",\"value\":\"High income\"},\"lendingType\":{\"id\":\"LNX\",\"iso2code\":\"XX\",\"value\":\"Not classified\"},\"capitalCity\":\"London\",\"longitude\":\"-0.126236\",\"latitude\":\"51.5002\"}]]";
        const string COUNTRY_DETAILS_INVALID_CODE = "[{\"message\":[{\"id\":\"120\",\"key\":\"Invalid value\",\"value\":\"The provided parameter value is not valid\"}]}]";

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<CountryCodeLookupService>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(c => c["CountryCodeLookup:Url"]).Returns("http://api.worldbank.org");
        }

        [Test]
        public async Task GivenApiReturnsCodes_WhenGetValidCountryCodesCalled_ThenExpectedListOfCodesIsReturned()
        {
            HttpClient httpClient = GetHttpClientForSend(HttpStatusCode.OK, COUNTRY_CODES);

            CountryCodeLookupService countryCodeLookupService =
                new CountryCodeLookupService(httpClient, _mockConfiguration.Object, _mockLogger.Object);

            var ret = await countryCodeLookupService.GetValidCountryCodes();

            _mockConfiguration.Verify(c => c["CountryCodeLookup:Url"], Times.Once);

            Assert.AreEqual("ABW", ret.First());
            Assert.AreEqual(4, ret.Count());
        }

        [Test]
        public void GivenApiNotAvaiable_WhenGetValidCountryCodesCalled_ThenExceptionIsLoggedAndThrown()
        {
            HttpClient httpClient = GetHttpClientForSend(HttpStatusCode.NotFound, null);

            CountryCodeLookupService countryCodeLookupService =
                new CountryCodeLookupService(httpClient, _mockConfiguration.Object, _mockLogger.Object);

            var ex = Assert.Throws<AggregateException>(() =>
            {
                var ret = countryCodeLookupService.GetValidCountryCodes().Result;
            });

            Assert.IsAssignableFrom(typeof(HttpRequestException), ex.InnerException);
        }

        [Test]
        public async Task GivenApiReturnsCountryDetails_WhenGetCountryDetailsCalled_ThenCountryDetailsIsReturned()
        {
            HttpClient httpClient = GetHttpClientForSend(HttpStatusCode.OK, COUNTRY_DETAILS);

            CountryCodeLookupService countryCodeLookupService =
                new CountryCodeLookupService(httpClient, _mockConfiguration.Object, _mockLogger.Object);

            var countryDetails = await countryCodeLookupService.GetCountryDetails(ISO_CODE_GB);

            Assert.AreEqual("United Kingdom", countryDetails.name);
        }

        [Test]
        public void GivenNullCountryCodeUsed_WhenGetCountryDetailsCalled_ThenExceptionLoggedAndThrown()
        {
            HttpClient httpClient = GetHttpClientForSend(HttpStatusCode.OK, COUNTRY_DETAILS);

            CountryCodeLookupService countryCodeLookupService =
                new CountryCodeLookupService(httpClient, _mockConfiguration.Object, _mockLogger.Object);

            var ex = Assert.Throws<AggregateException>(() =>
            {
                var countryDetails = countryCodeLookupService.GetCountryDetails(null).Result;
            });
            Mock.Verify();
            Assert.IsInstanceOf(typeof(ArgumentException), ex.InnerException);
        }

        [Test]
        public void GivenApiDoesNotRecogniseCode_WhenGetCountryDetailsCalled_ThenExceptionLoggedAndThrown()
        {
            HttpClient httpClient = GetHttpClientForSend(HttpStatusCode.OK, COUNTRY_DETAILS_INVALID_CODE);

            CountryCodeLookupService countryCodeLookupService =
                new CountryCodeLookupService(httpClient, _mockConfiguration.Object, _mockLogger.Object);

            var ex = Assert.Throws<AggregateException>(() =>
            {
                var countryDetails = countryCodeLookupService.GetCountryDetails("ZZ").Result;
            });

            Assert.IsInstanceOf(typeof(ApplicationException), ex.InnerException);
            Assert.AreEqual($"Unexpected data in response to get country details, response: {COUNTRY_DETAILS_INVALID_CODE}", ex.InnerException.Message);
        }

        private static HttpClient GetHttpClientForSend(HttpStatusCode statusCode, string response)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               // Setup the PROTECTED method to mock
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               // prepare the expected response of the mocked http call
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = statusCode,
                   Content = statusCode == HttpStatusCode.OK ? new StringContent(response) : new StringContent(string.Empty),
               })
               .Verifiable();

            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object);
            return httpClient;
        }
    }
}