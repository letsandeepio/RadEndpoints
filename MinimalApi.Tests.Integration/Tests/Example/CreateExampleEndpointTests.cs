﻿using MinimalApi.Features.Examples.CreateExample;

namespace MinimalApi.Tests.Integration.Tests.Example
{
    [Collection("Endpoint")]
    public class CreateExampleEndpointTests(RadEndpointFixture f)
    {
        [Fact]
        public async Task Given_ExampleDoesNotExist_ReturnsSuccess()
        {
            //Arrange
            var createRequest = f.DataGenerator.Create<CreateExampleRequest>();

            //Act
            var r = await f.Client.PostAsync<CreateExampleEndpoint, CreateExampleRequest, CreateExampleResponse>(createRequest);

            //Assert
            r.Should().BeSuccessful<CreateExampleResponse>()
                .WithStatusCode(HttpStatusCode.Created)
                .WithMessage("Example created successfully");

            r.Content.Data!.Id.Should().BeGreaterThan(0);
            r.Content.Data.FirstName.Should().Be(createRequest.FirstName);
            r.Content.Data.LastName.Should().Be(createRequest.LastName);
        }

        [Fact]
        public async Task Given_ExampleExists_ReturnsProblem()
        {
            //Arrange
            var createRequest = f.DataGenerator.Create<CreateExampleRequest>();

            //Act
            var _ = await f.Client.PostAsync<CreateExampleEndpoint, CreateExampleRequest, CreateExampleResponse>(createRequest);
            var r = await f.Client.PostAsync<CreateExampleEndpoint, CreateExampleRequest, ProblemDetails>(createRequest);

            //Assert
            r.Should().BeProblem()
                .WithStatusCode(HttpStatusCode.Conflict)
                .WithMessage("An example with the same first and last name already exists");
        }

        [Fact]
        public async Task When_FirstNameEmpty_ReturnsProblem()
        {
            //Arrange
            var createRequest = f.DataGenerator.Create<CreateExampleRequest>();
            createRequest.FirstName = string.Empty;

            //Act
            var r = await f.Client.PostAsync<CreateExampleEndpoint, CreateExampleRequest, ProblemDetails>(createRequest);

            //Assert
            r.Should().BeProblem()
                .WithStatusCode(HttpStatusCode.BadRequest)
                .WithMessage("Validation Error")
                .WithKey("FirstName");
        }

        [Fact]
        public async Task When_LastNameEmpty_ReturnsProblem()
        {
            //Arrange
            var createRequest = f.DataGenerator.Create<CreateExampleRequest>();
            createRequest.LastName = string.Empty;

            //Act
            var r = await f.Client.PostAsync<CreateExampleEndpoint, CreateExampleRequest, ProblemDetails>(createRequest);

            //Assert
            r.Should().BeProblem()
                .WithStatusCode(HttpStatusCode.BadRequest)
                .WithMessage("Validation Error")
                .WithKey("LastName");
        }
    }
}