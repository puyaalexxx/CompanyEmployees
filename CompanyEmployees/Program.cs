using CompanyEmployees;
using CompanyEmployees.Core.Services.Abstractions;
using CompanyEmployees.Core.Services.Hateoas;
using CompanyEmployees.Infrastructure.Persistence;
using CompanyEmployees.Infrastructure.Presentation.ActionFilters;
using CompanyEmployees.Infrastructure.Presentation.Validators;
using CompanyEmployees.ServiceExtensions;
using FluentValidation;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Shared.DataTransferObjects;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Host.UseSerilog((hostContext, configuration) =>
{
    configuration.ReadFrom.Configuration(hostContext.Configuration);
});

//builder.Services.AddKeyedScoped<IPlayerGenerator, PlayerGenerator>("player");

builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureCors();
builder.Services.ConfigureIISIintegration();
builder.Services.ConfigureLoggerService();
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddValidatorsFromAssemblyContaining(typeof(EmployeeForUpdateDtoValidator));
builder.Services.AddScoped<IDataShaper<EmployeeDto>, CompanyEmployees.Core.Services.DataShaping.DataShaper<EmployeeDto>>();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    //suppress the default model state validation from ApiController attribute
    options.SuppressModelStateInvalidFilter = true;
});

//register custom filters
builder.Services.AddScoped<ValidationFilterAttribute>();
builder.Services.AddScoped<ValidateMediaAttribute>();
builder.Services.AddScoped<IEmployeeLinks, EmployeeLinks>();

//route the controllers to this class library
builder.Services.AddControllers(config =>
    {
        //content negotiation
        config.RespectBrowserAcceptHeader = true;
        //if requesting another media type that is not supported to return 406 Not Acceptable
        config.ReturnHttpNotAcceptable = true;

        //config.InputFormatters.Insert(0, GetJsonPatchInputFormatters()); // if we want to use JsonPatchDocument
        config.CacheProfiles.Add("120SecondsDuration", new CacheProfile
        {
            Duration = 120
        });
    }).AddXmlDataContractSerializerFormatters()
    .AddCustomCSVFormatter()
    .AddApplicationPart(typeof(CompanyEmployees.Infrastructure.Presentation.AssemblyReference).Assembly);

//add custom media types for hateoas implementation
builder.Services.AddCustomMediaTypes();

//add api versioning
builder.Services.ConfigureVersioning();

//using Response Caching
builder.Services.ConfigureResponseCaching();

//using Output Caching
builder.Services.ConfigureOutputCaching();

builder.Services.ConfigureRateLimitingOptions();

//authentication
builder.Services.AddAuthentication();
builder.Services.ConfigureIdentity();
builder.Services.ConfigureJWT(builder.Configuration);
builder.Services.AddJwtConfiguration(builder.Configuration);

//health checks
builder.Services.ConfigureHealthChecks(builder.Configuration);

//added swagger support
builder.Services.ConfigureSwagger();

var app = builder.Build();

app.UseExceptionHandler(opts => { });

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    app.MapOpenApi();
else
    app.UseHsts();

app.UseHttpsRedirection();
app.ConfigureHealthChecksEndpoints();
app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CompanyEmployees API V1");
    c.SwaggerEndpoint("/swagger/v2/swagger.json", "CompanyEmployees API V2");
    c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
});

//will forward proxy headers to the current request
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});

app.UseRateLimiter();

app.UseCors("CorsPolicy");

//app.UseResponseCaching();
app.UseOutputCache();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//for docker to run migrations automatically, uncomment the line below
app.MigrateDatabase().Run();


/*app.Use(async (context, next) =>
{
    Console.WriteLine($"Logic before executing the next delegate in the Use method");
    await next.Invoke();
    Console.WriteLine($"Logic after executing the next delegate in the Use method");
});
app.Run(async context =>
{
    Console.WriteLine($"Writing the response to the client in the Run method");
    await context.Response.WriteAsync("Hello from the middleware component.");
});*/

await app.RunAsync();

public partial class Program { }