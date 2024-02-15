using Asp.Versioning;
using BillOMat.Api.Data;
using BillOMat.Api.Data.Exceptions;
using BillOMat.Api.Data.Repositories;
using BillOMat.ServiceDefaults;
using Carter;
using FluentValidation;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

builder.Services
    .AddApiVersioning(options =>
    {
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.DefaultApiVersion = new ApiVersion(1);
        options.ReportApiVersions = true;
        options.ApiVersionReader = ApiVersionReader.Combine(
            new QueryStringApiVersionReader("api-version"),
            new HeaderApiVersionReader("x-version"),
            new MediaTypeApiVersionReader("ver")
         );
    })
    .AddApiExplorer(options =>
    {
        // Add the versioned API explorer, which also adds IApiVersionDescriptionProvider service
        // note: the specified format code will format the version as "'v'major[.minor][-status]"
        options.GroupNameFormat = "'v'V";

        options.DefaultApiVersion = new ApiVersion(1);
    });

builder.Services.AddRateLimiter(o =>
{
    o.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    o.AddFixedWindowLimiter(
        "fixed-window",
        f =>
        {
            f.Window = TimeSpan.FromSeconds(10);
            f.PermitLimit = 3;
        });
});

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.CustomSchemaIds(schemaId =>
        schemaId
            .ToString()
            .Replace("+", "."));
});

builder.Services.AddFluentValidationAutoValidation();

var connectionString =
    builder.Configuration.GetConnectionString("ApplicationDbContext")
    ?? throw new ConnectionStringNotFoundException("ApplicationDbContext");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
                           options.UseSqlServer(connectionString));

builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var currentAssembly = typeof(Program).Assembly;

builder.Services.AddValidatorsFromAssembly(currentAssembly);
builder.Services.AddCarter();
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(currentAssembly));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseDeveloperExceptionPage();

    await IntializeDb(app);
}

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.UseHttpsRedirection();
app.UseRateLimiter();

app.MapDefaultEndpoints();
app.MapCarter();

app.MapGet("startup", () =>
{
    string? grafnaUrl = app.Configuration["GRAFANA_URL"];

    return new
    {
        GrafanaUrl = grafnaUrl
    };
});

app.Run();

static async Task IntializeDb(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    await context.Database.EnsureDeletedAsync();
    await context.Database.MigrateAsync();
}