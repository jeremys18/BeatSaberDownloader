using Hangfire;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.ServiceProcess;
using Hangfire.MemoryStorage;
using System.Threading.Tasks;

namespace BeatSaber.SongDownloadService
{
    public partial class DownloadService: ServiceBase
    {
        private int eventId = 1;
        private BackgroundJobServer _hangfireServer;

        public DownloadService()
        {
            InitializeComponent();
            eventLog1 = new EventLog();
            if (!EventLog.SourceExists("MySource"))
            {
                EventLog.CreateEventSource("MySource", "MyNewLog");
            }
            eventLog1.Source = "BeatSaberSongDownloaderService";
            eventLog1.Log = "";

            GlobalConfiguration.Configuration.UseSqlServerStorage("data source=.;initial catalog=BeatSaberSongDownloader;integrated security=True;MultipleActiveResultSets=True;TrustServerCertificate=True");
        }

        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("Starting the BeatSaber Song Downloading service");

            // Start Hangfire server
            _hangfireServer = new BackgroundJobServer();

            // Schedule the download to run daily at 8 AM
            // Cron expression for 8 AM daily
            RecurringJob.AddOrUpdate("DownloadSongsJob", () => Download(), Cron.Minutely); // 0 8 * * *
        }

        public async Task Download()
        {
            try
            {
                
                var response = await new HttpClient().GetAsync("https://www.google.com");
                if (response.IsSuccessStatusCode)
                {
                    eventLog1.WriteEntry("Successfully polled google.com.", EventLogEntryType.Information, eventId++);
                }
                else
                {
                    eventLog1.WriteEntry($"Failed to poll google.com. Status code: {response.StatusCode}", EventLogEntryType.Warning, eventId++);
                }
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry($"Exception occurred while polling google.com: {ex.Message}", EventLogEntryType.Error, eventId++);
            }
        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("In OnStop.");
            _hangfireServer.Dispose();
        }

        protected override void OnContinue()
        {
            eventLog1.WriteEntry("In OnContinue.");
        }
    }
}
