using System.Threading.RateLimiting;

namespace Backend.Backend.Configuration
{
    public static class RateLimitingExtensions
    {
        public const string AuthPolicy = "auth";
        public const string ApiPolicy = "api";

        public static IServiceCollection AddAppRateLimiting(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var options = configuration
                .GetSection(RateLimitingOptions.SectionName)
                .Get<RateLimitingOptions>() ?? new RateLimitingOptions();

            services.Configure<RateLimitingOptions>(
                configuration.GetSection(RateLimitingOptions.SectionName));

            services.AddRateLimiter(limiter =>
            {
                limiter.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                limiter.OnRejected = async (context, cancellationToken) =>
                {
                    var retryAfterSeconds = 60;
                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    {
                        retryAfterSeconds = (int)Math.Ceiling(retryAfter.TotalSeconds);
                        context.HttpContext.Response.Headers.RetryAfter =
                            retryAfterSeconds.ToString();
                    }

                    context.HttpContext.Response.ContentType = "application/json";
                    await context.HttpContext.Response.WriteAsJsonAsync(new
                    {
                        message = "Too many requests. Please try again later.",
                        retryAfterSeconds
                    }, cancellationToken);
                };

                limiter.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                {
                    var partitionKey = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    var isLogin = httpContext.Request.Path.Equals("/LogIn", StringComparison.OrdinalIgnoreCase)
                        && HttpMethods.IsPost(httpContext.Request.Method);

                    if (isLogin)
                    {
                        return RateLimitPartition.GetFixedWindowLimiter(
                            $"auth:{partitionKey}",
                            _ => new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = options.AuthPermitLimit,
                                Window = TimeSpan.FromSeconds(options.AuthWindowSeconds),
                                QueueLimit = 0
                            });
                    }

                    return RateLimitPartition.GetFixedWindowLimiter(
                        $"api:{partitionKey}",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = options.ApiPermitLimit,
                            Window = TimeSpan.FromSeconds(options.ApiWindowSeconds),
                            QueueLimit = 0
                        });
                });
            });

            return services;
        }
    }
}
