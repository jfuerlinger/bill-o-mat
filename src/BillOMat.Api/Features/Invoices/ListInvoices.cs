using BillOMat.Api.Data;
using BillOMat.Api.Data.Specifications.Institutes;
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

public static class ListInvoices
{
    public class Query : IRequest<OneOf<Invoice[], List<ValidationFailure>>>
    {
    }

    internal sealed class Handler(
        IUnitOfWork unitOfWork,
        IValidator<Query> validator)
      : IRequestHandler<Query, OneOf<Invoice[], List<ValidationFailure>>>
    {
        public async Task<OneOf<Invoice[], List<ValidationFailure>>> Handle(
            Query request, 
            CancellationToken cancellationToken)
        {
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return validationResult.Errors;
            }

            return await unitOfWork.InvoiceRepository
                .GetEntitiesAsync(
                    new AllInvoicesSpecification(), 
                    cancellationToken);
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
            "invoices",
            async ([FromServices] ISender sender) =>
            {
                OneOf<Invoice[], List<ValidationFailure>> listPatientResult
                        = await sender.Send(new ListInvoices.Query());

                return listPatientResult.Match(
                       patients => Results.Ok(patients),
                       errors => Results.BadRequest(string.Join("\n\r", errors))
                    );
            })
                .Produces(200)
                .Produces(429)
                .Produces<IEnumerable<ModelError>>(422)
                .WithTags("Invoice")
                .WithName("ListInvoices")
                .IncludeInOpenApi()
                .AddFluentValidationAutoValidation()
                .RequireRateLimiting("fixed-window")
                .WithApiVersionSet(versionSet)
                .MapToApiVersion(1);
    }
}

