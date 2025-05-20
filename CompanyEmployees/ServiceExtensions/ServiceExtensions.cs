using System.Xml.Serialization;
using CompanyEmployees.Core.Domain.Repositories;
using CompanyEmployees.Core.Services;
using CompanyEmployees.Core.Services.Abstractions;
using CompanyEmployees.Infrastructure.Persistence;
using LoggingService;
using Microsoft.EntityFrameworkCore;

namespace CompanyEmployees.ServiceExtensions;

public static class ServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services) =>
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });
    
    public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) =>
        services.AddDbContext<RepositoryContext>(opts => opts.UseSqlServer(configuration.GetConnectionString("sqlConnection")));

    public static void ConfigureLoggerService(this IServiceCollection services) => 
        services.AddSingleton<ILoggerManager, LoggerManager>();

    public static void ConfigureRepositoryManager(this IServiceCollection services) =>
        services.AddScoped<IRepositoryManager, RepositoryManager>();
    
    public static void ConfigureServiceManager(this IServiceCollection services) => 
        services.AddScoped<IServiceManager, ServiceManager>();

    //if we will need to host it on IIS
    public static void ConfigureIISIintegration(this IServiceCollection services) => 
        services.Configure<IISOptions>(options =>
        {

        });
}