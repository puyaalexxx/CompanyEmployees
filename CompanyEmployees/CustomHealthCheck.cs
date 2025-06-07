using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CompanyEmployees
{
    public class CustomHealthCheck : IHealthCheck
    {
        private Random _random = new Random();

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var responseTime = _random.Next(1, 300); // Simulate a response time between 50ms and 200ms

            return await Task.FromResult(responseTime switch
            {
                < 100 => HealthCheckResult.Healthy("Health result from CustomHealthCheck."),
                (> 100) and (< 200) => HealthCheckResult.Degraded("Degraded result from CustomHealthCheck."),
                _ => HealthCheckResult.Unhealthy("Unhealthy result from CustomHealthCheck.")
            });
        }
    }
}
