﻿using MinimalApi.Domain.Examples;

namespace MinimalApi.Features.CustomExamples.CustomPut
{
    public interface ICustomPutMapper : IRadMapper<CustomPutRequest, CustomPutResponse, Example> { }
    public class CustomPutMapper: ICustomPutMapper
    {
        public CustomPutResponse FromEntity(Example entity) => new()
        {
            Data = new()
            {
                Id = entity.Id,
                FirstName = entity.FirstName,
                LastName = entity.LastName
            }
        };

        public Example ToEntity(CustomPutRequest request) => new(request.Data.FirstName, request.Data.LastName, request.Id);
    }
}
