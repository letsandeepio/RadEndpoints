﻿using MinimalApi.Domain.Examples;

namespace MinimalApi.Features.Examples.UpdateExample
{
    public class UpdateExampleMapper : RadMapper<UpdateExampleRequest, UpdateExampleResponse, Example>
    {
        public override UpdateExampleResponse FromEntity(Example entity) => new()
        {
            Data = new()
            {
                Id = entity.Id,
                FirstName = entity.FirstName,
                LastName = entity.LastName
            }
        };

        public override Example ToEntity(UpdateExampleRequest request) => new(request.Example.FirstName, request.Example.LastName, request.Id);
    }
}