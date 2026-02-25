using Microsoft.Extensions.Logging;

namespace CustomerFluent.Extensions
{
    public static class LoggingExtensions
    {
        public static void LogUserAction(this ILogger logger, string action, string userId, object? data = null)
        {
            logger.LogInformation("User Action: {Action} by User {UserId} with Data {Data}", action, userId, data);
        }

        public static void LogPerformance(this ILogger logger, string operation, TimeSpan duration, object? context = null)
        {
            logger.LogInformation("Performance: {Operation} completed in {Duration}ms {Context}", 
                operation, duration.TotalMilliseconds, context);
        }

        public static void LogBusinessEvent(this ILogger logger, string eventName, object? data = null)
        {
            logger.LogInformation("Business Event: {EventName} {Data}", eventName, data);
        }
    }
}