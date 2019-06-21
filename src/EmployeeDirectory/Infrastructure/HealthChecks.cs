using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EmployeeDirectory.Infrastructure
{
    public class HealthChecks
    {
        public interface ISqlServerHealthCheck : IHealthCheck
        {
        }

        public class SqlServerHealthCheck : ISqlServerHealthCheck
        {
            private readonly string _connectionString;

            public SqlServerHealthCheck(string connectionString)
            {
                _connectionString = connectionString;
            }

            public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    try
                    {
                        await connection.OpenAsync(cancellationToken);
                    }
                    catch (Exception)
                    {
                        return HealthCheckResult.Unhealthy();
                    }

                    return HealthCheckResult.Healthy();
                }
            }
        }
    }
}
