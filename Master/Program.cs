using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics;

class Master
{
    static ConcurrentDictionary<string, ConcurrentDictionary<string, int>> aggregatedData = new();

    static void Main(string[] args)
    {
        // Set CPU affinity to Core 3
        Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)0x8;

        // Start threads for each agent
        Thread agent1Thread = new Thread(() => ListenPipe("agent1"));
        Thread agent2Thread = new Thread(() => ListenPipe("agent2"));

        agent1Thread.Start();
        agent2Thread.Start();

        agent1Thread.Join();
        agent2Thread.Join();

        // Print aggregated result
        Console.WriteLine("\nAggregated Word Counts:");
        foreach (var file in aggregatedData)
        {
            foreach (var word in file.Value)
            {
                Console.WriteLine($"{file.Key}:{word.Key}:{word.Value}");
            }
        }

        Console.WriteLine("\nMaster completed.");
    }

    static void ListenPipe(string pipeName)
    {
        using (NamedPipeServerStream pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.In))
        using (StreamReader reader = new StreamReader(pipeServer))
        {
            Console.WriteLine($"Waiting for {pipeName}...");
            pipeServer.WaitForConnection();
            Console.WriteLine($"{pipeName} connected.");

            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                var parts = line.Split(':');
                if (parts.Length != 3) continue;

                string fileName = parts[0];
                string word = parts[1];
                int count = int.Parse(parts[2]);

                aggregatedData.AddOrUpdate(fileName,
                    _ => new ConcurrentDictionary<string, int> { [word] = count },
                    (_, fileDict) =>
                    {
                        fileDict.AddOrUpdate(word, count, (_, oldCount) => oldCount + count);
                        return fileDict;
                    });
            }
        }
    }
}
