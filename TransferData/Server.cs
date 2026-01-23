using FastMember;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;
using TransferData.Shared;

const int BATCH_SIZE = 200_000;

string rootPath = @"C:\Users\beatriz.francisca\Downloads\Android_v2";
var logFiles = Directory.GetFiles(rootPath, "applogcat.log", SearchOption.AllDirectories);

var channel = Channel.CreateBounded<List<AndroidLog>>(new BoundedChannelOptions(20) { FullMode = BoundedChannelFullMode.Wait });

Stopwatch sw = Stopwatch.StartNew();
long total = 0;

const int PORT = 5000;

var consumerTask = Task.Run(async () => 
    {
        using var conn = new SqlConnection(@"Server=localhost\SQLEXPRESS01;Database=InternalProject;Trusted_Connection=True;TrustServerCertificate=True;");
        await conn.OpenAsync();

        using var bulk = new SqlBulkCopy(conn, SqlBulkCopyOptions.TableLock, null)
        {
            DestinationTableName = "AndroidLog",
            BatchSize = BATCH_SIZE,
            BulkCopyTimeout = 0
        };

        bulk.ColumnMappings.Add("LogDate", "LogDate");
        bulk.ColumnMappings.Add("Pid", "Pid");
        bulk.ColumnMappings.Add("Tid", "Tid");
        bulk.ColumnMappings.Add("Level", "Level");
        bulk.ColumnMappings.Add("Component", "Component");
        bulk.ColumnMappings.Add("Content", "Content");

        await foreach (var batch in channel.Reader.ReadAllAsync())
        {
            using var reader = ObjectReader.Create(batch, "LogDate", "Pid", "Tid", "Level", "Component", "Content");
            await bulk.WriteToServerAsync(reader);

            Interlocked.Add(ref total, batch.Count);
            Console.WriteLine($"{total:N0} logs inseridos | {total / sw.Elapsed.TotalSeconds:N0} reg/s");
        }
    }
);

TcpListener listener = new TcpListener(IPAddress.Any, PORT);
listener.Start();
Console.WriteLine("Aguardando conexão do cliente...");

using (TcpClient client = await listener.AcceptTcpClientAsync())
using (NetworkStream ns = client.GetStream())
using (StreamReader reader = new StreamReader(ns, System.Text.Encoding.UTF8))
{
    List<AndroidLog> currentBuffer = new(BATCH_SIZE);
    string? line;

    while ((line = await reader.ReadLineAsync()) != null)
    {
        var log = ParseOptimized(line);
        if (log is null) continue;

        currentBuffer.Add(log);

        if (currentBuffer.Count >= BATCH_SIZE)
        {
            await channel.Writer.WriteAsync(currentBuffer);
            currentBuffer = new List<AndroidLog>(BATCH_SIZE);
        }
    }

    if (currentBuffer.Count > 0)
    {
        await channel.Writer.WriteAsync(currentBuffer);
    }
}

channel.Writer.Complete();
await consumerTask;

sw.Stop();
Console.WriteLine($"FINALIZADO: {total:N0} registros em {sw.Elapsed.TotalSeconds:N2}s");


static AndroidLog? ParseOptimized(string line)
{
    ReadOnlySpan<char> span = line.AsSpan().Trim();

    if (span.Length < 30)
        return null;

    int s1 = span.IndexOf(' ');
    if (s1 < 0) return null;

    int s2 = span.Slice(s1 + 1).IndexOf(' ');
    if (s2 < 0) return null;
    s2 += s1 + 1;

    int pStart = s2;
    while (pStart < span.Length && span[pStart] == ' ') pStart++;

    int pEnd = span.Slice(pStart).IndexOf(' ');
    if (pEnd < 0) return null;
    pEnd += pStart;

    if (!int.TryParse(span.Slice(pStart, pEnd - pStart), out int pid))
        return null;

    int tStart = pEnd;
    while (tStart < span.Length && span[tStart] == ' ') tStart++;

    int tEnd = span.Slice(tStart).IndexOf(' ');
    if (tEnd < 0) return null;
    tEnd += tStart;

    if (!int.TryParse(span.Slice(tStart, tEnd - tStart), out int tid))
        return null;

    int levelStart = tEnd + 1;
    int levelEnd = span.Slice(levelStart).IndexOf(' ');
    if (levelEnd < 0) return null;

    string level = span.Slice(levelStart, levelEnd).Trim().ToString();

    int componentStart = levelStart + levelEnd + 1;

    int colon = span.Slice(componentStart).IndexOf(':');

    string component;
    string content;

    if (colon < 0)
    {
        component = "UNKNOWN";
        content = span.Slice(componentStart).Trim().ToString();
    }
    else
    {
        colon += componentStart;

        component = span.Slice(componentStart, colon - componentStart).Trim().ToString();

        if (colon + 1 < span.Length)
            content = span.Slice(colon + 1).Trim().ToString();
        else
            content = string.Empty;
    }

    if (component.Length == 0)
        component = "UNKNOWN";

    if (content.Length == 0)
        content = string.Empty;

    return new AndroidLog
    {
        LogDate = span.Slice(0, s2).ToString(),
        Pid = pid,
        Tid = tid,
        Level = level,
        Component = component,
        Content = content
    };
}
