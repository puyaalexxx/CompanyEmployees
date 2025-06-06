using Asp.Versioning;
using CompanyEmployees.Core.Domain.ConfigurationModels;
using CompanyEmployees.Core.Domain.Entities;
using CompanyEmployees.Core.Domain.Repositories;
using CompanyEmployees.Core.Services;
using CompanyEmployees.Core.Services.Abstractions;
using CompanyEmployees.Formatters;
using CompanyEmployees.Infrastructure.Persistence;
using HealthChecks.UI.Client;
using LoggingService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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

    public static void ConfigureHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddSqlServer(configuration.GetConnectionString("sqlConnection")!)
            .AddCheck<CustomHealthCheck>("CustomHealthCheck", tags: ["custom"]);

        //services.AddHealthChecksUI().AddInMemoryStorage(); // Use in-memory storage for health checks UI
    }

    public static void ConfigureHealthChecksEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.MapHealthChecks("/health/custom", new HealthCheckOptions
        {
            Predicate = reg => reg.Tags.Contains("custom"),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        // app.MapHealthChecksUI(); // Uncomment if you want to use Health Checks UI
    }

    public static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Company Employees API",
                Version = "v1",
                Description = "API for managing company employees.",
                TermsOfService = new Uri("https://example.com/terms"),
                Contact = new OpenApiContact
                {
                    Name = "Support Team",
                    Email = "support@mail.com",
                    Url = new Uri("https://example.com/support")
                },
                License = new OpenApiLicense
                {
                    Name = "Use under LICX",
                    Url = new Uri("https://example.com/license")
                }
            });

            options.SwaggerDoc("v2", new OpenApiInfo
            {
                Title = "Company Employees API",
                Version = "v2",
                Description = "API for managing company employees v2.",
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Place to add JWT with Bearer.",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                   new List<string>()
                }
            });

            //add comments support (check .cproj file for suppressing some warning related to XML comments on methods and controllers and xml file generation)
            var xmlFile = $"{typeof(Infrastructure.Presentation.AssemblyReference).Assembly.GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);

        });
    }
}