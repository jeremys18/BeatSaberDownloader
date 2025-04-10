using System.ServiceProcess;

namespace BeatSaberDownloader.Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new DownloadService() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
