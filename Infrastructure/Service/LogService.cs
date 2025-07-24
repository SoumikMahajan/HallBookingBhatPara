using System.Security.Claims;

namespace HallBookingBhatPara.Infrastructure.Repository
{
    public class LogService
    {
        private readonly ILogger<LogService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _logDirectory;
        public LogService(ILogger<LogService> logger, IHttpContextAccessor httpContextAccessor, IHostEnvironment environment)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _logDirectory = Path.Combine(environment.ContentRootPath, "Logs");
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
        }

        public async Task LogExceptionErrorAsync(Exception ex)
        {
            try
            {

                var userId = GetUserIdFromClaims(_httpContextAccessor.HttpContext?.User);

                var logFile = GetLogFilePath();

                if (!File.Exists(logFile))
                {
                    await using var _ = File.Create(logFile);
                }

                await using var txtWriter = new StreamWriter(logFile, append: true);
                await LogAsync(ex, txtWriter, userId);

            }
            catch (Exception loggingEx)
            {
                _logger.LogError(loggingEx, "Failed to log exception.");
            }
        }

        private async Task LogAsync(Exception ex, TextWriter txtWriter, long userId)
        {
            await txtWriter.WriteLineAsync($"\r\nLog Entry For Web Sln: {DateTime.Now:D} {DateTime.Now:T}");
            await txtWriter.WriteLineAsync($"  :[LoggedInUser]: {userId}");
            await txtWriter.WriteLineAsync($"  :[Error]: {ex.Message}");

            var stackTrace = ex.StackTrace?.Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                                   .Take(1);

            await txtWriter.WriteLineAsync($"  :[StackTrace]: {string.Join(Environment.NewLine, stackTrace)}");
            await txtWriter.WriteLineAsync("---------------------------------------------------------------------------------------------");
        }

        public async Task LogCustomAsync(string message)
        {
            try
            {

                var userId = GetUserIdFromClaims(_httpContextAccessor.HttpContext?.User);

                var logFile = GetLogFilePath();

                if (!File.Exists(logFile))
                {
                    await using var _ = File.Create(logFile);
                }

                await using var txtWriter = new StreamWriter(logFile, append: true);
                await txtWriter.WriteLineAsync($"\r\nLog Entry For Web Sln: {DateTime.Now:D} {DateTime.Now:T}");
                await txtWriter.WriteLineAsync($" :[LoggedInUser]: {userId}");
                await txtWriter.WriteLineAsync("  :");
                await txtWriter.WriteLineAsync($" :{message}");
                await txtWriter.WriteLineAsync("---------------------------------------------------------------------------------------------");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log custom message.");
            }

        }

        private string GetLogFilePath()
        {
            return Path.Combine(_logDirectory, $"Log-{DateTime.Now:MM-dd-yyyy}.txt");
        }

        private long GetUserIdFromClaims(ClaimsPrincipal user)
        {
            var userIdClaim = user?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            return long.TryParse(userIdClaim, out var id) ? id : 0;
        }
    }
}
