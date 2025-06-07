using CompanyEmployees.IntegrationTests.Factories;
using Shared.DataTransferObjects;
using System.Net;
using System.Net.Http.Json;

namespace CompanyEmployees.IntegrationTests.Tests
{
    public class ValidationTests : IClassFixture<CompanyEmployeesTestContainersFactory>
    {
        private readonly HttpClient _client;
        private const string CompaniesUrl = "/api/companies";

        public ValidationTests(CompanyEmployeesTestContainersFactory factory)
        {
            _client = factory.CreateClient();
        }


        [Fact]
        public async Task WhenEnityDoesntExist_ThenResturn404NotFound()
        {
            //Act
            var response = await _client.GetAsync($"{CompaniesUrl}/c9d4c053-49b6-410c-bc78-2d54a9991872");

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task WhenModelStateInvalidOnCreation_ThenReturns422UnprocessableEntity()
        {
            //Act
            var company = new CompanyForCreationDto
            {
                Address = "Test Address",
                Country = "USA"
            };

            //Act
            var response = await _client.PostAsJsonAsync(CompaniesUrl, company);

            //Assert
            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }
    }
}
