using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.ServiceProcess;
using System.Threading;

namespace BeatSaberDownloader.Service
{
    public partial class DownloadService : ServiceBase
    {
        private Timer _timer;
        private HttpClient _httpClient;
        private readonly string _logFilePath = "C:\\Logs\\BeatSaberDownloader.log";

        public DownloadService()
        {
            this.ServiceName = "BeatSaberDownloaderService3";
            InitializeComponent();
            _httpClient = new HttpClient();
        }

        protected override void OnStart(string[] args)
        {
            Log("Service started.");

            ScheduleDailyTask();
        }

        private void ScheduleDailyTask()
        {
            DateTime now = DateTime.Now;
            DateTime nextRun = new DateTime(now.Year, now.Month, now.Day, 8, 0, 0, 0);
            if (now > nextRun)
            {
                nextRun = nextRun.AddMinutes(1);
            }

            TimeSpan timeToGo = nextRun - now;
            _timer = new Timer(x =>
            {
                RunDownloader();
                ScheduleDailyTask(); // Reschedule for the next day
            }, null, timeToGo, Timeout.InfiniteTimeSpan);
        }

        private async void RunDownloader()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://www.google.com");
                if (response.IsSuccessStatusCode)
                {
                    Log("Successfully polled google.com.");
                }
                else
                {
                    Log($"Failed to poll google.com. Status code: {response.StatusCode}", EventLogEntryType.Warning);
                }
            }
            catch (Exception ex)
            {
                Log($"Exception occurred while polling google.com: {ex.Message}", EventLogEntryType.Error);
            }
        }

        protected override void OnStop()
        {
            Log("Service stopped.");
            _timer?.Change(Timeout.Infinite, 0);
            _timer?.Dispose();
            _httpClient.Dispose();
        }

        private void Log(string message, EventLogEntryType type = EventLogEntryType.Information)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(_logFilePath, true))
                {
                    writer.WriteLine($"{DateTime.Now} [{type}] {message}");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur while writing to the log file
                // For example, you could write to the event log as a fallback
                EventLog.WriteEntry("BeatSaberDownloader", $"Failed to write to log file: {ex.Message}", EventLogEntryType.Error);
            }
        }
    }
}
