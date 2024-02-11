using Asp.Versioning.Builder;
using BillOMat.Api.Data.Repositories;
using BillOMat.Api.Data.Specifications;
using BillOMat.Api.Entities;
using Carter;
using Carter.ModelBinding;
using Carter.OpenApi;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace BillOMat.Api.Features.Patients;

public static class ListPatients
{
    public class Query : IRequest<OneOf<Patient[], List<ValidationFailure>>>
    {
    }

    internal sealed class Handler(
        IPatientRepository repository,
        IValidator<Query> validator)
      : IRequestHandler<Query, OneOf<Patient[], List<ValidationFailure>>>
    {
        public async Task<OneOf<Patient[], List<ValidationFailure>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return validationResult.Errors;
            }

            return await repository.GetEntitiesAsync(new AllPatientsSpecification());
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator() { }

    }
}

public class ListPatientsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new Asp.Versioning.ApiVersion(1))
            .Build();

        app.MapGet(
            "patients",
            async ([FromServices] ISender sender) =>
            {
                OneOf<Patient[], List<ValidationFailure>> listPatientResult
                        = await sender.Send(new ListPatients.Query());

                return listPatientResult.Match(
                       patients => Results.Ok(patients),
                       errors => Results.BadRequest(string.Join("\n\r", errors))
                    );
            })
                .Produces(200)
                .Produces(429)
                .Produces<IEnumerable<ModelError>>(422)
                .WithTags("Patient")
                .WithName("ListPatients")
                .IncludeInOpenApi()
                .AddFluentValidationAutoValidation()
                .RequireRateLimiting("fixed-window")
                .WithApiVersionSet(versionSet)
                .MapToApiVersion(1);
    }
}

