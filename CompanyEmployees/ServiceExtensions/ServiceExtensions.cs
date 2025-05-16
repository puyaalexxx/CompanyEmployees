using System.Xml.Serialization;
using LoggingService;

namespace CompanyEmployees.ServiceExtensions;

public static class ServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });
    }

    public static void ConfigureLoggerService(this IServiceCollection services)
    {
        services.AddScoped<ILoggerManager, LoggerManager>();
    }

    //if we will need to host it on IIS
    public static void ConfigureIISIintegration(this IServiceCollection services) => 
        services.Configure<IISOptions>(options =>
        {

        });
}