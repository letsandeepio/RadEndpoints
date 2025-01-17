﻿using MinimalApi.Features.Examples.SearchExamples;

namespace MinimalApi.Tests.Integration.Tests.Example
{
    [Collection("Endpoint")]
    public class SearchExampleEndpointTests(RadEndpointFixture f)
    {

        [Theory]
        [InlineData("Luke", "Skywalker")]
        [InlineData("", "Skywalker")]
        [InlineData("Luke", "")]
        public async Task Given_ExampleExists_ReturnsSuccess(string firstName, string lastName) 
        {
            //Arrange
            var request = new SearchExamplesRequest
            {
                FirstName = firstName,
                LastName = lastName
            };

            //Act
            var r = await f.Client.GetAsync<SearchExamplesEndpoint, SearchExamplesRequest, SearchExamplesResponse>(request); 

            //Assert
            r.Should().BeSuccessful<SearchExamplesResponse>()
                .WithStatusCode(HttpStatusCode.OK)
                .WithMessage("Examples found successfully");

            r.Content.Data.Should().Contain(e => e.FirstName == "Luke" && e.LastName == "Skywalker");
            r.Content.Data!.First().LastName.Should().Be("Skywalker");
        }

        [Fact]
        public async Task When_SearchEmpty_ReturnsProblem()
        {
            //Arrange
            var request = new SearchExamplesRequest
            {
                FirstName = string.Empty,
                LastName = string.Empty
            };

            //Act
            var r = await f.Client.GetAsync<SearchExamplesEndpoint, SearchExamplesRequest, ProblemDetails>(request); 

            //Assert
            r.Should().BeProblem()
                .WithStatusCode(HttpStatusCode.BadRequest)
                .WithMessage("Validation Error")
                .WithKey("FirstName")
                .WithKey("LastName");
        }
    }
}