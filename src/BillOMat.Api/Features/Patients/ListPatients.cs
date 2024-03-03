using BillOMat.Api.Data;
using BillOMat.Api.Data.Specifications.Patients;
using BillOMat.Api.Entities;
using BillOMat.Api.Mappers;
using Carter;
using Carter.ModelBinding;
using Carter.OpenApi;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using System.Net;

namespace BillOMat.Api.Features.Patients;

public static class ListPatients
{
    public class Query : IRequest<OneOf<Patient[], List<ValidationFailure>>> { }

    internal sealed class Handler(
        IUnitOfWork unitOfWork,
        IValidator<Query> validator)
      : IRequestHandler<Query, OneOf<Patient[], List<ValidationFailure>>>
    {
        public async Task<OneOf<Patient[], List<ValidationFailure>>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return validationResult.Errors;
            }

            return await unitOfWork.PatientRepository.GetEntitiesAsync(new AllPatientsSpecification(), cancellationToken);
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator() { }

    }
}

public record ListPatientsDto(int Id, string FirstName, string LastName, string Nickname, string Email);

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
                       patients =>
                       {
                           PatientMapper mapper = new();
                           var resultDtos = patients
                                               .Select(p =>
                                                    mapper.PatientToListPatientsDto(p));

                           return Results.Ok(resultDtos);
                       },
                       errors => Results.BadRequest(string.Join("\n\r", errors))
                    );
            })
                .Produces<IEnumerable<ListPatientsDto>>((int)HttpStatusCode.OK)
                .Produces((int)HttpStatusCode.TooManyRequests)
                .Produces<IEnumerable<ModelError>>((int)HttpStatusCode.UnprocessableContent)
                .WithTags("Patient")
                .WithName(nameof(ListPatients))
                .IncludeInOpenApi()
                .AddFluentValidationAutoValidation()
                .RequireRateLimiting("fixed-window")
                .WithApiVersionSet(versionSet)
                .MapToApiVersion(1);
    }
}

