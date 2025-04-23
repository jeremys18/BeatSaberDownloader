To install the service use powershell and run the following command:
```powershell sc.exe create "BeatSaber Song Downloader Service" binPath= "C:\Users\grabb\source\repos\BeatSaberDownloader\BSSD.DownloadService\bin\Release\net8.0\win-x64\publish\BSSD.DownloadService.exe" start= auto
```

To uninstall the service use powershell and run the following command:
```powershell sc.exe delete "BeatSaber Song Downloader Service"
```
To start the service use powershell and run the following command:
```powershell sc.exe start "BeatSaber Song Downloader Service"
```
To stop the service use powershell and run the following command:
```powershell sc.exe stop "BeatSaber Song Downloader Service"
```
To check the status of the service use powershell and run the following command:
```powershell sc.exe query "BeatSaber Song Downloader Service"
```
To check the logs of the service use powershell and run the following command:
```powershell Get-EventLog -LogName Application -Source "BeatSaber Song Downloader Service" | Select-Object -First 10
```
