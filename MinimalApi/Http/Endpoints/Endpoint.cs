﻿using Microsoft.AspNetCore.Http.HttpResults;

namespace MinimalApi.Http.Endpoints
{

    public abstract class Endpoint
    {
        protected ILogger Logger { get; private set; } = null!;
        protected IEndpointRouteBuilder RouteBuilder { get; private set; } = null!;
        protected HttpContext HttpContext => _httpContextAccessor.HttpContext!;
        private IHttpContextAccessor _httpContextAccessor = null!;
        protected bool HasValidator;

        protected static Ok Ok() => TypedResults.Ok();
        protected static ProblemHttpResult ServerError(string title) => TypedResults.Problem(title: title, statusCode: StatusCodes.Status500InternalServerError);
        protected static ProblemHttpResult ValidationError(string title) => TypedResults.Problem(title: title, statusCode: StatusCodes.Status400BadRequest);
        protected static ProblemHttpResult Conflict(string title) => TypedResults.Problem(title: title, statusCode: StatusCodes.Status409Conflict);
        protected static ProblemHttpResult NotFound(string title) => TypedResults.Problem(title: title, statusCode: StatusCodes.Status404NotFound);
        protected static ProblemHttpResult Forbidden(string title) => TypedResults.Problem(title: title, statusCode: StatusCodes.Status403Forbidden);

        public abstract void Configure();

        public void EnableValidation()
        {
            HasValidator = true;
        }
        public void SetLogger(ILogger logger)
        {
            if (Logger is not null) throw new InvalidOperationException("Logger already set.");
            Logger = logger;
        }
        public void SetBuilder(IEndpointRouteBuilder routeBuilder)
        {
            if (RouteBuilder is not null) throw new InvalidOperationException("Route builder already set.");
            RouteBuilder = routeBuilder;
        }
        public void SetContext(IHttpContextAccessor contextAccessor)
        {
            if (_httpContextAccessor is not null) throw new InvalidOperationException("Context accessor already set.");
            _httpContextAccessor = contextAccessor;
        }        
    }
    public abstract class Endpoint<TRequest, TResponse> : Endpoint where TResponse : EndpointResponse, new() where TRequest : class, new()
    {
        public TResponse Response { get; set; } = new();
        public abstract Task<IResult> Handle(TRequest r, CancellationToken ct);
        public RouteHandlerBuilder Get(string route)
        {
            var builder = RouteBuilder!
                .MapGet(route, async ([AsParameters] TRequest request, CancellationToken ct) => await Handle(request, ct));

            return TryAddEndpointFilter(builder);
        }

        public RouteHandlerBuilder Post(string route)
        {
            var builder = RouteBuilder!
                .MapPost(route, async ([AsParameters] TRequest request, CancellationToken ct) => await Handle(request, ct));

            return TryAddEndpointFilter(builder);
        }

        public RouteHandlerBuilder Put(string route)
        {
            var builder = RouteBuilder!
                .MapPut(route, async ([AsParameters] TRequest request, CancellationToken ct) => await Handle(request, ct));

            return TryAddEndpointFilter(builder);
        }

        public RouteHandlerBuilder Patch(string route)
        {
            var builder = RouteBuilder!
                .MapPatch(route, async ([AsParameters] TRequest request, CancellationToken ct) => await Handle(request, ct));

            return TryAddEndpointFilter(builder);
        }

        public RouteHandlerBuilder Delete(string route)
        {
            var builder = RouteBuilder!
                .MapDelete(route, async ([AsParameters] TRequest request, CancellationToken ct) => await Handle(request, ct));

            return TryAddEndpointFilter(builder);
        }

        public static Ok<TResponse> Ok(TResponse response) => TypedResults.Ok(response);

        private RouteHandlerBuilder TryAddEndpointFilter(RouteHandlerBuilder builder)
        {
            if (HasValidator) builder.AddEndpointFilter<ValidationFilter<TRequest>>();
            return builder;
        }
    }

    public abstract class EndpointWithoutRequest<TResponse> : Endpoint where TResponse : EndpointResponse, new()
    {
        public TResponse Response { get; set; } = new();
        public abstract Task<IResult> Handle(CancellationToken ct);
        public RouteHandlerBuilder Get(string route) => RouteBuilder!.MapGet(route, async (CancellationToken ct) => await Handle(ct));
        public RouteHandlerBuilder Post(string route) => RouteBuilder!.MapPost(route, async (CancellationToken ct) => await Handle(ct));
        public RouteHandlerBuilder Put(string route) => RouteBuilder!.MapPut(route, async (CancellationToken ct) => await Handle(ct));
        public RouteHandlerBuilder Patch(string route) => RouteBuilder!.MapPatch(route, async (CancellationToken ct) => await Handle(ct));
        public RouteHandlerBuilder Delete(string route) => RouteBuilder!.MapDelete(route, async (CancellationToken ct) => await Handle(ct));
        public static Ok<TResponse> Ok(TResponse response) => TypedResults.Ok(response);
    }

    public abstract class EndpointWithQuery<TResponse> : Endpoint where TResponse : EndpointResponse, new()
    {
        public TResponse Response { get; set; } = new();

        public static Ok<TResponse> Ok(TResponse response) => TypedResults.Ok(response);
    }
}
