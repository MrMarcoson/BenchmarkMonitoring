using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BenchmarkFrontendConsole
{

    public class Program
    {
        static string databasePath { get; set; }
        static string serviceProjectPath { get; set; }

        static void SetupSettings()
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            AppSettingsSection appSettings = configuration.AppSettings;
            databasePath = appSettings.Settings["DatabasePath"].Value;
            serviceProjectPath = appSettings.Settings["ServiceProjectPath"].Value;
        }
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
                string filePath = databasePath;
                switch (choice)
                {
                    case 1:
                        filePath += @"\CPU.json";
                        break;
                    case 2:
                        filePath += @"\GPU.json";
                        break;
                    case 3:
                        filePath += @"\RAM.json";
                        break;
                }

                Stream stream;
                try
                {
                    stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                } catch(Exception ex)
                {
                    Console.SetCursorPosition(0, 0);
                    Console.WriteLine("No file: " + filePath);
                    Console.WriteLine("Press Enter to go back...");
                    if (Console.KeyAvailable)
                    {
                        key = Console.ReadKey(true);
                        if (key.Key.Equals(ConsoleKey.Enter))
                        {
                            Console.CursorVisible = true;
                            return;
                        }
                    }
                    continue;
                }

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
        static void ChangeServiceConfig(Dictionary<string, string> settings, Configuration configuration)
        {
            AppSettingsSection appSettings = configuration.AppSettings;

            foreach (var setting in settings)
            {
                appSettings.Settings[setting.Key].Value = setting.Value;
            }

            configuration.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configuration.AppSettings.SectionInformation.Name);
        }
        static void ServiceDataSetup()
        {
            while (true)
            {
                ExeConfigurationFileMap map = new ExeConfigurationFileMap { ExeConfigFilename = serviceProjectPath + @"\BenchmarkMonitoringService.exe.config" };
                Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
                AppSettingsSection appSettings = configuration.AppSettings;

                string serviceDatabasePath = appSettings.Settings["DatabasePath"].Value;
                string workingInterval = appSettings.Settings["WorkingInterval"].Value;
                string eventLogName = appSettings.Settings["EventLogName"].Value;
                string source = appSettings.Settings["Source"].Value;

                Console.Clear();
                Console.WriteLine("1. DatabasePath = {0} \n2. WorkingInterval = {1} \n3. EventLogName = {2} \n4. Source = {3}  \n5. Exit\n", serviceDatabasePath, workingInterval, eventLogName, source);
                string choice = Console.ReadLine();

                string choiceName = "";
                switch (choice)
                {
                    case "1":
                        choiceName = "DatabasePath";
                        Console.Write("Set new DatabasePath value: ");
                        break;
                    case "2":
                        choiceName = "WorkingInterval";
                        Console.Write("Set new WorkingInterval value: ");
                        break;
                    case "3":
                        choiceName = "EventLogName";
                        Console.Write("Set new EventLogName value: ");
                        break;
                    case "4":
                        choiceName = "Source";
                        Console.Write("Set new Source value: ");
                        break;
                    case "5":
                        return;
                }

                string temp = Console.ReadLine();
                Console.WriteLine("Are you sure? Press y");
                if (Console.ReadLine() == "y")
                {
                    Dictionary<string, string> settings = new Dictionary<string, string>();
                    settings[choiceName] = temp;
                    ChangeServiceConfig(settings, configuration);
                }

                else continue;
            }
        }

        static void ChangeServiceStatus()
        {
            Console.Clear();

            if (!ServiceController.GetServices().Any(serviceController => serviceController.ServiceName.Equals("BenchmarkService")))
            {
                Console.WriteLine("Service does not exist. Press enter to exit...");
                Console.ReadLine();
                return;
            }

            ServiceController service = new ServiceController("BenchmarkService");
            Console.WriteLine("Service status: " + service.Status);
            Console.WriteLine("Press y if you want to change status...\n");
            Console.Write(">");

            string choice = Console.ReadLine();
            if (choice.Equals("y"))
            {
                try
                {
                    if (service.Status == ServiceControllerStatus.Running)
                        service.Stop();

                    if (service.Status == ServiceControllerStatus.Stopped)
                        service.Start();

                    Console.Clear();
                    Console.WriteLine("Changing...");

                    while (true)
                    {
                        service.Refresh();

                        if (service.Status == ServiceControllerStatus.Running || service.Status == ServiceControllerStatus.Stopped)
                        {
                            Console.WriteLine("Status changed to: " + service.Status.ToString());
                            Console.WriteLine("\nPress enter to exit...");
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                Console.ReadLine();
            }

            return;
        }

        static void Main(string[] args)
        {
            SetupSettings();
            Console.SetWindowPosition(0, 0);
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);

            while (true)
            {
                Console.Clear();
                Console.SetCursorPosition(0, 0);
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("BenchmarkMonitor by Marek Kasprowicz");
                Console.ResetColor();
                Console.WriteLine("1. CPU \n2. GPU \n3. RAM \n4. Service data setup \n5. Change Service Status \n6. Exit\n");

                Console.Write(">");
                string choice = Console.ReadLine();
                if (choice == "1" || choice == "2" || choice == "3") printInfo(Convert.ToInt32(choice));
                if (choice == "4") ServiceDataSetup();
                if (choice == "5") ChangeServiceStatus();
                if (choice == "6") break;
                else continue;
            }
        }
    }
}
