using Asp.Versioning.Builder;
using BillOMat.Api.Data;
using BillOMat.Api.Data.Repositories;
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

public static class CreatePatient
{
    public class Command(string firstname, string lastname, string nickname, string email) 
        : IRequest<OneOf<int, List<ValidationFailure>>>
    {
        public required string Firstname { get; init; } = firstname;
        public required string Lastname { get; init; } = lastname;
        public required string Nickname { get; init; } = nickname;
        public required string Email { get; init; } = email;
    }

    internal sealed class Handler(
        ApplicationDbContext dbContext,
        IPatientRepository patientRepository,
        IValidator<Command> validator)
      : IRequestHandler<Command, OneOf<int, List<ValidationFailure>>>
    {
        public async Task<OneOf<int, List<ValidationFailure>>> Handle(Command request, CancellationToken cancellationToken)
        {
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return validationResult.Errors;
            }

            if(!await patientRepository.IsEmailUniqueAsync(
                request.Email, 
                cancellationToken))
            {
                return new List<ValidationFailure>() { 
                    new(
                        "Email", 
                        "Email is already in use") };
            }

            var patient = new Patient
            {
                Firstname = request.Firstname,
                Lastname = request.Lastname,
                Nickname = request.Nickname,
                Email = request.Email
            };

            dbContext.Patients.Add(patient);

            await dbContext.SaveChangesAsync(cancellationToken);

            return patient.Id;
        }
    }


    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Firstname).NotEmpty();
            RuleFor(x => x.Lastname).NotEmpty();
            RuleFor(x => x.Nickname).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }
}

public class CreatePatientEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new Asp.Versioning.ApiVersion(1))
            .Build();


        app.MapPost(
        "patients",
        async (
                    [FromBody] CreatePatient.Command command,
                    [FromServices] ISender sender) =>
            {

                OneOf<int, List<ValidationFailure>> createPatientResult
                        = await sender.Send(command);


                return createPatientResult.Match(
                       patientId => Results.Created($"patients/{patientId}", patientId),
                       errors => Results.BadRequest(errors));
            })
                .Accepts<CreatePatient.Command>("application/json")
                .Produces(201)
                .Produces(429)
                .Produces<IEnumerable<ModelError>>(422)
                .WithTags("Patient")
                .WithName("AddPatient")
                .IncludeInOpenApi()
                .AddFluentValidationAutoValidation()
                .RequireRateLimiting("fixed-window")
                .WithApiVersionSet(versionSet)
                .MapToApiVersion(1);
    }
}

