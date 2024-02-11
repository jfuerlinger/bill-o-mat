using BillOMat.Api.Data;
using BillOMat.Api.Data.Repositories;
using BillOMat.ServiceDefaults;
using Carter;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddFluentValidationAutoValidation();

var connectionString =
    builder.Configuration.GetConnectionString("ApplicationDbContext")
    ?? throw new Exception($"ConnectionString 'ApplicationDbContext' wasn't found!");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
                           options.UseSqlServer(connectionString));

builder.Services.AddScoped<IPatientRepository, PatientRepository>();

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
    IntializeDb(app);
}

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.UseHttpsRedirection();

app.MapDefaultEndpoints();

app.MapCarter();

app.Run();

static void IntializeDb(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
}