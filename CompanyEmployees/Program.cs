using CompanyEmployees;
using CompanyEmployees.Core.Services.Abstractions;
using CompanyEmployees.Core.Services.Hateoas;
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
    }).AddXmlDataContractSerializerFormatters()
    .AddCustomCSVFormatter()
    .AddApplicationPart(typeof(CompanyEmployees.Infrastructure.Presentation.AssemblyReference).Assembly);

//add custom media types for hateoas implementation
builder.Services.AddCustomMediaTypes();


var app = builder.Build();

app.UseExceptionHandler(opts => { });

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    app.MapOpenApi();
else
    app.UseHsts();

app.UseHttpsRedirection();
app.UseStaticFiles();
//will forward proxy headers to the current request
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});

app.UseCors("CorsPolicy");

app.UseAuthentication();

app.MapControllers();

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