using System.Net.Sockets;
using System.Diagnostics;

string serverIp = "127.0.0.1";
int serverPort = 5000;
string rootPath = @"C:\Users\beatriz.francisca\Downloads\Android_v2";

Console.WriteLine("Starting to seach for logs...");

var logFiles = Directory.GetFiles(rootPath, "applogcat.log", SearchOption.AllDirectories);

Console.WriteLine($"Found {logFiles.Length} log files to be sent.");

try
{
    using TcpClient client = new TcpClient(serverIp, serverPort);
    using NetworkStream ns = client.GetStream();
    using StreamWriter writer = new StreamWriter(ns, System.Text.Encoding.UTF8, 65536) { AutoFlush = false};

    Stopwatch sw = Stopwatch.StartNew();

    foreach (var file in logFiles)
    {
        Console.WriteLine($"Sending file: {file}");

        using (StreamReader reader = new StreamReader(file))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                writer.WriteLine(line);
            }
        }
        writer.Flush();
    }

    sw.Stop();
    Console.WriteLine($"All log files sent in {sw.Elapsed.TotalSeconds} seconds.");
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}