using Asp.Versioning;
using CompanyEmployees.Core.Domain.ConfigurationModels;
using CompanyEmployees.Core.Domain.Entities;
using CompanyEmployees.Core.Domain.Repositories;
using CompanyEmployees.Core.Services;
using CompanyEmployees.Core.Services.Abstractions;
using CompanyEmployees.Formatters;
using CompanyEmployees.Infrastructure.Persistence;
using LoggingService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;

namespace CompanyEmployees.ServiceExtensions;


public static class ServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services) =>
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("X-Pagination"));
        });

    public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) =>
        services.AddDbContext<RepositoryContext>(opts => opts.UseSqlServer(configuration.GetConnectionString("sqlConnection")));

    public static void ConfigureLoggerService(this IServiceCollection services) =>
        services.AddSingleton<ILoggerManager, LoggerManager>();

    public static void ConfigureRepositoryManager(this IServiceCollection services) =>
        services.AddScoped<IRepositoryManager, RepositoryManager>();

    public static void ConfigureServiceManager(this IServiceCollection services) =>
        services.AddScoped<IServiceManager, ServiceManager>();

    public static IMvcBuilder AddCustomCSVFormatter(this IMvcBuilder builder) =>
        builder.AddMvcOptions(config => config.OutputFormatters.Add(new CsvOutputFormatter()));

    //if we will need to host it on IIS
    public static void ConfigureIISIintegration(this IServiceCollection services) =>
        services.Configure<IISOptions>(options =>
        {

        });

    public static void AddCustomMediaTypes(this IServiceCollection services)
    {
        services.Configure<MvcOptions>(config =>
        {
            var systemTextJsonOutputFormatter = config.OutputFormatters
                    .OfType<SystemTextJsonOutputFormatter>()?
                    .FirstOrDefault();

            if (systemTextJsonOutputFormatter != null)
            {
                systemTextJsonOutputFormatter.SupportedMediaTypes
                .Add("application/vnd.juc.hateoas+json");
                systemTextJsonOutputFormatter.SupportedMediaTypes
                .Add("application/vnd.juc.apiroot+json");
            }

            var xmlOutputFormatter = config.OutputFormatters
                    .OfType<XmlDataContractSerializerOutputFormatter>()?
                    .FirstOrDefault();

            if (xmlOutputFormatter != null)
            {
                xmlOutputFormatter.SupportedMediaTypes
                .Add("application/vnd.juc.hateoas+xml");
                xmlOutputFormatter.SupportedMediaTypes
                .Add("application/vnd.juc.apiroot+xml");
            }
        });
    }

    public static void ConfigureVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ApiVersionReader = new UrlSegmentApiVersionReader(); // use URL versioning
            //http header versioning
            //options.ApiVersionReader = new HeaderApiVersionReader("api-version");
            //query string versioning
            //options.ApiVersionReader = new QueryStringApiVersionReader("api-version");
        }).AddMvc(opts =>
        {
            //if we have a lot of versions of a single controller, we can assign these versions in the configurations instead of attributes
            /*opts.Conventions.Controller<Infrastructure.Presentation.Controllers.v1.CompaniesController>()
                .HasApiVersion(new ApiVersion(1.0));
            opts.Conventions.Controller<Infrastructure.Presentation.Controllers.v2.CompaniesController>()
            .HasDeprecatedApiVersion(new ApiVersion(2.0));*/

        });

    }

    public static void ConfigureResponseCaching(this IServiceCollection services) =>
        services.AddResponseCaching();

    public static void ConfigureOutputCaching(this IServiceCollection services) =>
        services.AddOutputCache(opts =>
        {
            opts.AddBasePolicy(bp => bp.Expire(TimeSpan.FromSeconds(10)));

            //policy applied to specific controller
            opts.AddPolicy("120SecondsDuration", policy => policy.Expire(TimeSpan.FromSeconds(120)));

            //policy applied by query parameter
            opts.AddPolicy("QueryParamDuration", p => p.Expire(TimeSpan.FromSeconds(10)).SetVaryByQuery("firstKey"));
        });

    public static void ConfigureRateLimitingOptions(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                return RateLimitPartition.GetFixedWindowLimiter("GlobalLimiter",
                    partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 30,
                        QueueLimit = 0,

                        //don't reject the exceeded requests but hang them and process them later
                        // QueueLimit = 2,
                        // QueueProcessingOrder = QueueProcessingOrder.OldestFirst,

                        Window = TimeSpan.FromMinutes(1)
                    });
            });

            options.AddPolicy("RateLimitPolicy", context =>
            {
                return RateLimitPartition.GetFixedWindowLimiter("RateLimiter",
                    partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 2,
                        Window = TimeSpan.FromSeconds(10)
                    });
            });

            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        });
    }

    public static void ConfigureIdentity(this IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 10;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<RepositoryContext>()
        .AddDefaultTokenProviders();
    }

    public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtConfiguration = new JwtConfiguration();
        configuration.Bind(jwtConfiguration.Section, jwtConfiguration);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtConfiguration.ValidIssuer,
                ValidAudience = jwtConfiguration.ValidAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.SecretKey!)),
            };
        });
    }

    public static void AddJwtConfiguration(this IServiceCollection services, IConfiguration configuration) => services
            .Configure<JwtConfiguration>(configuration.GetSection("JwtSettings"));
}