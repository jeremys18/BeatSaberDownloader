using Cronos;
using System.Text.RegularExpressions;

namespace BeatSaberDownloader.Synchronizer
{
    public static class CronRunner
    {
        // Accepts either a human time "HH:mm" (daily) or a cron expression (supports seconds with 6-field cron).
        public static async Task ScheduleAsync(Func<CancellationToken, Task> job, string timeOrCron, CancellationToken stoppingToken)
        {
            if (job is null) throw new ArgumentNullException(nameof(job));
            if (string.IsNullOrWhiteSpace(timeOrCron)) throw new ArgumentNullException(nameof(timeOrCron));

            // If user passed "HH:mm" or "H:mm", convert to cron with seconds: "0 mm HH * * *"
            string cronExpression = ConvertToCronIfTimeString(timeOrCron.Trim());

            var cron = CronExpression.Parse(cronExpression, CronFormat.IncludeSeconds);

            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTimeOffset.Now;
                var next = cron.GetNextOccurrence(now, TimeZoneInfo.Local);

                if (!next.HasValue)
                {
                    // No next occurrence — exit loop.
                    continue;
                }

                var delay = next.Value - now;
                if (delay > TimeSpan.Zero)
                {
                    try
                    {
                        await Task.Delay(delay, stoppingToken).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                    {
                        break;
                    }
                }

                if (stoppingToken.IsCancellationRequested) break;

                try
                {
                    await job(stoppingToken).ConfigureAwait(false);
                }
                catch
                {
                    // Swallow here; let worker continue to next schedule. Let caller log inside job.
                }
            }
        }

        private static string ConvertToCronIfTimeString(string input)
        {
            // Matches "HH:mm" or "H:mm" or "HH:mm:ss"
            var hhmm = Regex.Match(input, @"^(\d{1,2}):(\d{2})(?::(\d{2}))?$");
            if (hhmm.Success)
            {
                var hour = int.Parse(hhmm.Groups[1].Value);
                var minute = int.Parse(hhmm.Groups[2].Value);
                var second = hhmm.Groups[3].Success ? int.Parse(hhmm.Groups[3].Value) : 0;

                // Cronos with seconds uses: "s m h dom mon dow"
                return $"{second} {minute} {hour} * * *";
            }

            // otherwise assume it's a cron expression already (user responsible for correct format)
            return input;
        }
    }
}
