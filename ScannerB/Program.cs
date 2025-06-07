using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

class ScannerB
{
    static void Main(string[] args)
    {
        // Set CPU affinity to Core 2
        Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)0x4;

        if (args.Length == 0)
        {
            Console.WriteLine("Please provide a directory path.");
            return;
        }

        string dirPath = args[0];
        if (!Directory.Exists(dirPath))
        {
            Console.WriteLine("Directory not found.");
            return;
        }

        var wordData = new Dictionary<string, Dictionary<string, int>>();

        Thread readThread = new Thread(() =>
        {
            foreach (var file in Directory.GetFiles(dirPath, "*.txt"))
            {
                var fileName = Path.GetFileName(file);
                var content = File.ReadAllText(file);
                var wordCount = new Dictionary<string, int>();

                foreach (var word in content.Split(new[] { ' ', '\n', '\r', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string w = word.ToLower();
                    if (wordCount.ContainsKey(w))
                        wordCount[w]++;
                    else
                        wordCount[w] = 1;
                }

                wordData[fileName] = wordCount;
            }
        });

        readThread.Start();
        readThread.Join();

        Thread sendThread = new Thread(() =>
        {
            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "agent2", PipeDirection.Out))
            using (StreamWriter writer = new StreamWriter(pipeClient))
            {
                pipeClient.Connect();
                writer.AutoFlush = true;

                foreach (var fileEntry in wordData)
                {
                    foreach (var wordEntry in fileEntry.Value)
                    {
                        writer.WriteLine($"{fileEntry.Key}:{wordEntry.Key}:{wordEntry.Value}");
                    }
                }
            }
        });

        sendThread.Start();
        sendThread.Join();

        Console.WriteLine("ScannerB completed.");
    }
}
