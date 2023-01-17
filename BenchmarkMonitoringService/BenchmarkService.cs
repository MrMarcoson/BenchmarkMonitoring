using BenchmarkLibrary;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;

namespace BenchmarkMonitoringService
{
    public partial class BenchmarkService : ServiceBase
    {
        EventLog eventLog { get; set; }
        String databasePath { get; set; }
        int workingInterval { get; set; }
        String eventLogName { get; set; }
        String source { get; set; }
        System.Timers.Timer timer { get; set; }
        int numberOfSaves = 0;

        private void SetupProperties()
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            AppSettingsSection appSettings = configuration.AppSettings;
            databasePath = appSettings.Settings["DatabasePath"].Value;
            workingInterval = Int32.Parse(appSettings.Settings["WorkingInterval"].Value);
            eventLogName = appSettings.Settings["EventLogName"].Value;
            source = appSettings.Settings["Source"].Value;
        }

        private void SetupEventLog(string name, string source)
        {
            eventLog = new EventLog(name);

            if (!EventLog.SourceExists(source, "."))
            {
                EventLog.CreateEventSource(source, name);
                Console.WriteLine("Created Event Source");
            }

            eventLog.Source = source;
        }

        private void SetupDatabase(string databasePath)
        {
            if (!File.Exists(databasePath + "/" + "CPU.json"))
            {
                File.Create(databasePath + "/" + "CPU.json");
            }

            if (!File.Exists(databasePath + "/" + "GPU.json"))
            {
                File.Create(databasePath + "/" + "GPU.json");
            }

            if (!File.Exists(databasePath + "/" + "RAM.json"))
            {
                File.Create(databasePath + "/" + "RAM.json");
            }
        }

        private void SetupTimer()
        {
            timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(OnTimerFinish);
            timer.Interval = workingInterval;
            timer.Enabled = true;
            timer.AutoReset = true;
        }

        public BenchmarkService()
        {
            InitializeComponent();
        }

        private void WriteToFile(string fileName, string data)
        {
            using (StreamWriter writer = new StreamWriter(databasePath + "/" + fileName, false))
            {
                writer.Write(data);
            }
        }

        private void CreateAndSaveBenchmarkInfo()
        {
            BenchmarkInfo benchmarkInfo = new BenchmarkInfo();
            WriteToFile("CPU.json", benchmarkInfo.CPU);
            WriteToFile("GPU.json", benchmarkInfo.GPU);
            WriteToFile("RAM.json", benchmarkInfo.RAM);
            EventLog.WriteEntry("Benchmark Info saved to files.");
        }

        private void OnTimerFinish(object source, ElapsedEventArgs e)
        {
            CreateAndSaveBenchmarkInfo();
        }

        protected override void OnStart(string[] args)
        {
            SetupProperties();
            SetupEventLog(eventLogName, source);
            SetupDatabase(databasePath);
            CreateAndSaveBenchmarkInfo();
            SetupTimer();
            EventLog.WriteEntry("BenchmarkService starts.");
        }

        protected override void OnStop()
        {
            EventLog.WriteEntry("BenchmarkService stops.");
        }
    }
}
