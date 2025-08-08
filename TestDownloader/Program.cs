// See https://aka.ms/new-console-template for more information
using BSSD.DownloadService.Downloader;
using System;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

//await SongInfoDownloader.StartAsync();

internal class Program
{
    private static async Task Main(string[] args)
    {
        

        //ws.Options.HttpVersion = HttpVersion.Version20;
        //ws.Options.HttpVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;

        while (true)
        {
            try
            {
                await Start();
            }
            catch (Exception ex)
            {
               Console.WriteLine($"Error in WebSocket connection: {ex.Message}");
            }
        }
    }

    async static Task Start()
    {
        using SocketsHttpHandler handler = new();
        using ClientWebSocket ws = new();
        await ws.ConnectAsync(new Uri("wss://ws.beatsaver.com/maps"), new HttpMessageInvoker(handler), new CancellationToken());

        var buffer = new byte[1024 * 32];
        while (ws.State != WebSocketState.CloseReceived)
        {
            try
            {
                var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).ConfigureAwait(false);

                //Here is the received message as string
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var type = Regex.Match(message, @"type"":\s*""(\w+)""").Groups[1].Value;
                var id = type == "MAP_DELETE" ? Regex.Match(message, @"msg"":\s*""(\w+)""").Groups[1].Value : Regex.Match(message, @"id"":\s*""(\w+)""").Groups[1].Value;
                Console.WriteLine($"Received message: {type} Id: {id}");

                File.WriteAllText(GetFileName(id), message); // Save the message to a file
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in receiving messages: {err}", ex.Message);
            }
        }
    }

    static string GetFileName(string id)
    {
        var fileNum = 1;
        var fileName = @$"D:\BeatSaverData\{id}.json";
        while (File.Exists(fileName))
        {
            fileName = @$"D:\BeatSaverData\{id}_{fileNum++}.json";
        }
        return fileName;
    }
}