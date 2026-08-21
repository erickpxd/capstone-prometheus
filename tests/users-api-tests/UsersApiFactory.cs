using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Prometheus.Users.Api.Tests;

internal sealed class UsersApiFactory : WebApplicationFactory<Program>
{
    public UsersApiFactory()
    {
        DatabasePath = Path.Combine(
            Path.GetTempPath(),
            $"prometheus-users-api-tests-{Guid.NewGuid():N}.db");
    }

    public string DatabasePath { get; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = $"Data Source={DatabasePath}",
                ["Jwt:Issuer"] = "Prometheus.Users.Api.Tests",
                ["Jwt:Audience"] = "Prometheus.Users.Api.Tests.Client",
                ["Jwt:SigningKey"] = "TEST-ONLY-PROMETHEUS-SIGNING-KEY-NEVER-USE-IN-PRODUCTION",
                ["Jwt:ExpirationMinutes"] = "10"
            });
        });
    }
}
