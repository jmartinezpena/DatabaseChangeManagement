namespace EmployeeDirectory.Infrastructure
{
    using System;
    using System.Data.SqlClient;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using StackExchange.Redis;

    public class HealthChecks
    {
        public interface ISqlServerHealthCheck : IHealthCheck
        {
        }

        public interface IRedisHealthCheck : IHealthCheck
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

        public class RedisHealthCheck : IRedisHealthCheck
        {
            private readonly string _configuration;

            public RedisHealthCheck(string configuration)
            {
                _configuration = configuration;
            }

            public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var redis = ConnectionMultiplexer.Connect(_configuration))
                {
                    try
                    {
                        var db = redis.GetDatabase(0);
                    }
                    catch (Exception)
                    {
                        return await Task.FromResult(HealthCheckResult.Unhealthy());
                    }
                }
                return await Task.FromResult(HealthCheckResult.Healthy());
            }
        }
    }
}
