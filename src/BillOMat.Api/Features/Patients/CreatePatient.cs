using BillOMat.Api.Data;
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

    public class Command : IRequest<OneOf<int, List<ValidationFailure>>>
    {
        public required string Firstname { get; set; }
        public required string Lastname { get; set; }
        public required string Nickname { get; set; }
        public required string Email { get; set; }
    }

    internal sealed class Handler(
        ApplicationDbContext dbContext,
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

            var patient = new Patient
            {
                Firstname = request.Firstname,
                Lastname = request.Lastname,
                Nickname = request.Nickname
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
        app.MapPost(
            "api/patients",
            async (
                        [FromBody] CreatePatient.Command command,
                        [FromServices] ISender sender) =>
        {

            OneOf<int, List<ValidationFailure>> createPatientResult
                    = await sender.Send(command);


            return createPatientResult.Match(
                   patientId => Results.Created($"/api/patients/{patientId}", patientId),
                   errors => Results.BadRequest(errors));
        })
            .Accepts<CreatePatient.Command>("application/json")
            .Produces(201)
            .Produces<IEnumerable<ModelError>>(422)
            .WithTags("Patient")
            .WithName("AddPatient")
            .IncludeInOpenApi()
            .AddFluentValidationAutoValidation();
    }
}

