using System.ComponentModel;
using System.ServiceProcess;

namespace BeatSaberDownloader.Service
{
    [RunInstaller(true)]
    public partial class ProjectInstaller: System.Configuration.Install.Installer
    {
        private ServiceProcessInstaller serviceProcessInstaller;
        private ServiceInstaller serviceInstaller;

        public ProjectInstaller()
        {
            InitializeComponent();

            // Initialize the ServiceProcessInstaller
            serviceProcessInstaller = new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalService        // Password for the user account
            };

            // Initialize the ServiceInstaller
            serviceInstaller = new ServiceInstaller
            {
                ServiceName = "BeatSaberDownloaderService3", // Name of the service
                DisplayName = "Beat Saber Downloader Service", // Display name in the Services Manager
                Description = "A service that downloads Beat Saber content.", // Description of the service
                StartType = ServiceStartMode.Automatic // Start the service automatically
            };

            // Add installers to the Installers collection
            Installers.Add(serviceProcessInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
