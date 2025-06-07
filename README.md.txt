#File Indexer System – Final OOP Project

University: Vilnius University Šiauliai Academy
Author: Eliyas Ahmed Sadhin
Course: Object-Oriented Programming (Final Submission)
GitHub Repository: https://github.com/sadh1n/FileIndexerSystem

#Project Overview
This project is a distributed file indexing system made with C# and .NET. It uses three separate console applications that work together:

ScannerA and ScannerB: Scan .txt files from different folders and send word counts.

Master: Collects word counts from both scanners via named pipes, combines them, and displays the final results.

The goal is to demonstrate inter-process communication, multithreading, and CPU core control.

#Project Folder Structure

FileIndexerSystem/
├── Master/ ← Receives data, aggregates, and displays final results
├── ScannerA/ ← Scans folder A and sends data via pipe 'agent1'
├── ScannerB/ ← Scans folder B and sends data via pipe 'agent2'
├── UML_Diagram.png ← UML class diagram (or .pdf)
├── Test_Report.pdf ← Final report with screenshots and explanation
├── README.md ← You're reading this!
└── FileIndexerSystem.sln

**Technologies Used

C# .NET 9.0

Named Pipes (NamedPipeServerStream, NamedPipeClientStream)

Multithreading (Thread)

CPU Core Binding (Processor.ProcessorAffinity)

#How to Run the System
Open the solution in Visual Studio and build all three projects.

Create two folders on your system:

C:\ScannerAData – place .txt files here for ScannerA

C:\ScannerBData – place .txt files here for ScannerB

Open three separate terminal windows and run:

Terminal 1: Master.exe

Terminal 2: ScannerA.exe "C:\ScannerAData"

Terminal 3: ScannerB.exe "C:\ScannerBData"

Each scanner will read the text files, count the words, and send data to the master. The master will print a combined list of word counts.

**Example Output: 

Waiting for agent1...
Waiting for agent2...
agent1 connected.
agent2 connected.

Aggregated Word Counts:
a1.txt:hello:1
a1.txt:world:1
a1.txt:this:1
a1.txt:is:1
a1.txt:a:1
a1.txt:scanner:1
b1.txt:hello:2
b1.txt:from:1
b1.txt:again:1
b1.txt:b:1
b1.txt:scanner:1

Master completed.

*Notes
Each executable is pinned to a different CPU core for performance testing.
Named pipes are used for reliable and isolated communication.
Threads are used for file scanning and data transmission separately.
