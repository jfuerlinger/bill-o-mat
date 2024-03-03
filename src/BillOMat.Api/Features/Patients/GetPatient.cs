using BillOMat.Api.Data;
using BillOMat.Api.Data.Specifications.Patients;
using BillOMat.Api.Entities;
using Carter;
using Carter.ModelBinding;
using Carter.OpenApi;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using OneOf.Types;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using System.Net;

namespace BillOMat.Api.Features.Patients;

public static class GetPatient
{
    public class Query(int id) : IRequest<OneOf<Patient, List<ValidationFailure>, NotFound>>
    {
        public int Id { get; init; } = id;
    }

    internal sealed class Handler(
        IUnitOfWork unitOfWork,
        IValidator<Query> validator)
      : IRequestHandler<Query, OneOf<Patient, List<ValidationFailure>, NotFound>>
    {
        public async Task<OneOf<Patient, List<ValidationFailure>, NotFound>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return validationResult.Errors;
            }

            var patients = await unitOfWork.PatientRepository.GetEntitiesAsync(new PatientByIdSpecification(request.Id), cancellationToken);

            if (patients.Length == 1)
            {
                return patients[0];
            }
            else
            {
                return new NotFound();
            }
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                   .NotEmpty()
                   .GreaterThan(0);
        }

    }
}

public class GetPatientEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new Asp.Versioning.ApiVersion(1))
            .Build();

        app.MapGet(
            "patients/{id}",
            async ([FromServices] ISender sender, int id) =>
            {
                OneOf<Patient, List<ValidationFailure>, NotFound> getPatientResult
                        = await sender.Send(new GetPatient.Query(id));

                return getPatientResult.Match(
                       patient => Results.Ok(patient),
                       errors => Results.BadRequest(string.Join("\n\r", errors)),
                       notfound => Results.NotFound()
                    );
            })
                .Produces((int)HttpStatusCode.OK)
                .Produces((int)HttpStatusCode.TooManyRequests)
                .Produces((int)HttpStatusCode.NotFound)
                .Produces<IEnumerable<ModelError>>((int)HttpStatusCode.UnprocessableContent)
                .WithTags("Patient")
                .WithName(nameof(GetPatient))
                .IncludeInOpenApi()
                .AddFluentValidationAutoValidation()
                .RequireRateLimiting("fixed-window")
                .WithApiVersionSet(versionSet)
                .MapToApiVersion(1);
    }
}

