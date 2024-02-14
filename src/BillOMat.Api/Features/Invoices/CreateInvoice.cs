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

namespace BillOMat.Api.Features.Invoices;

public static class CreateInvoice
{
    public class Command :
        IRequest<OneOf<int, List<ValidationFailure>>>
    {
        public required string InvoiceNumber { get; set; }
        public int InstituteId { get; set; }
        public int PatientId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal Amount { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.InvoiceNumber).NotEmpty();
            RuleFor(x => x.InstituteId).NotEmpty();
            RuleFor(x => x.PatientId).NotEmpty();
            RuleFor(x => x.InvoiceDate).NotEmpty().InclusiveBetween(DateTime.Now.AddYears(-1), DateTime.Now);
            RuleFor(x => x.Amount).NotEmpty();
        }
    }

    internal sealed class Handler(
        IUnitOfWork unitOfWork,
        IValidator<Command> validator)
        : IRequestHandler<Command, OneOf<int, List<ValidationFailure>>>
    {
        public async Task<OneOf<int, List<ValidationFailure>>> Handle(
                Command request,
                CancellationToken cancellationToken = default)
        {
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return validationResult.Errors;
            }

            if (!await unitOfWork.InvoiceRepository.IsInvoiceNumberUniqueAsync(
                request.InvoiceNumber,
                cancellationToken))
            {
                return new List<ValidationFailure>() {
                    new(
                        "InvoiceNumber",
                        "InvoiceNumber is not unique!") };
            }

            var invoice = new Invoice()
            {
                InvoiceNumber = request.InvoiceNumber,
                InstituteId = request.InstituteId,
                PatientId = request.PatientId,
                InvoiceDate = request.InvoiceDate,
                Amount = request.Amount,
                Status = Enumerations.Invoice.Status.Neu
            };

            unitOfWork.InvoiceRepository.Add(invoice);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return invoice.Id;
        }
    }
}

public class CreateInvoiceEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new Asp.Versioning.ApiVersion(1))
            .Build();


        app.MapPost(
        "invoices",
        async (
                    [FromBody] CreateInvoice.Command command,
                    [FromServices] ISender sender) =>
        {

            OneOf<int, List<ValidationFailure>> createInvoiceResult
                    = await sender.Send(command);


            return createInvoiceResult.Match(
                   invoiceId => Results.Created($"invoices/{invoiceId}", invoiceId),
                   errors => Results.BadRequest(errors));
        })
                .Accepts<CreateInvoice.Command>("application/json")
                .Produces(201)
                .Produces(429)
                .Produces<IEnumerable<ModelError>>(422)
                .WithTags("Invoice")
                .WithName("AddInvoice")
                .IncludeInOpenApi()
                .AddFluentValidationAutoValidation()
                .RequireRateLimiting("fixed-window")
                .WithApiVersionSet(versionSet)
                .MapToApiVersion(1);
    }
}

