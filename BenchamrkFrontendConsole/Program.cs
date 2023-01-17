using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkLibrary;
using Newtonsoft.Json;

namespace BenchmarkFrontendConsole
{

    public class Program
    {
        static public Dictionary<string, Dictionary<string, string>> deserialize(string json)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json);
        }

        static void printInfo(int choice)
        {
            Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>();
            Console.CursorVisible = false;
            ConsoleKeyInfo key = new ConsoleKeyInfo();

            while (true)
            {
                string filePath = @"D:\Development\BenchmarkMonitoring\";
                switch (choice)
                {
                    case 1:
                        filePath += "CPU.json";
                        break;
                    case 2:
                        filePath += "GPU.json";
                        break;
                    case 3:
                        filePath += "RAM.json";
                        break;
                }

                Stream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader streamReader = new StreamReader(stream);
                string content = streamReader.ReadToEnd();
                streamReader.Close();
                stream.Close();

                data = deserialize(content);

                if (data is null) continue;

                Console.SetCursorPosition(0, 0);
                foreach (var hardware in data)
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.Write(hardware.Key + "\n");
                    Console.ResetColor();

                    foreach (var stat in hardware.Value)
                    {
                        Console.Write("\t" + stat.Key + ": " + stat.Value + "\n");
                    }

                    Console.Write("\n");

                    if(Console.KeyAvailable)
                    {
                        key = Console.ReadKey(true);
                        if (key.Key.Equals(ConsoleKey.Enter))
                        {
                            Console.CursorVisible = true;
                            return;
                        }
                    }

                }

                Console.WriteLine("Press Enter to go back...");
            }

        }

        static void Main(string[] args)
        {
            Console.SetWindowPosition(0, 0);
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);

            while (true)
            {
                Console.Clear();
                Console.SetCursorPosition(0, 0);
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("BenchmarkMonitor by Marek Kasprowicz");
                Console.ResetColor();
                Console.WriteLine("1. CPU \n2. GPU \n3. RAM \n4. Exit\n");

                Console.Write(">");
                int choice = Convert.ToInt32(Console.ReadLine());
                if (choice >= 4 || choice <= 0) break;

                Console.Clear();
                printInfo(choice);
            }
        }
    }
}
