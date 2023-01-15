using OpenHardwareMonitor.Hardware;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkLibrary
{
    public class CPUInfo
    {
        public Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>();

        public CPUInfo()
        {
            Computer computer = new Computer { CPUEnabled = true };
            computer.Open();

            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("select * from Win32_Processor");
            foreach (ManagementObject obj in managementObjectSearcher.Get())
            {
                Dictionary<string, string> cpu = new Dictionary<string, string>();
                cpu["name"] = obj["Name"].ToString();
                cpu["family"] = obj["Family"].ToString();
                cpu["description"] = obj["Description"].ToString();
                cpu["cores"] = obj["NumberOfCores"].ToString();
                cpu["logicalProcessors"] = obj["NumberOfLogicalProcessors"].ToString();
                cpu["threads"] = obj["ThreadCount"].ToString();
                cpu["architecture"] = obj["Architecture"].ToString();
                cpu["clockSpeed"] = obj["CurrentClockSpeed"].ToString();
                cpu["voltage"] = obj["CurrentVoltage"].ToString();
                data.Add(cpu["name"], cpu);
            }

            foreach (var hardware in computer.Hardware)
            {
                hardware.Update();

                //Check if hardware name is included in dictionary key name.
                //This is created to avoid errors when names from wmi contains more strings like: AMD Ryzen 7 5700G with Radeon Graphics, where in OpenHardwareMonitor it's just AMD Ryzen 7 5700G.
                string cpuKey = "";
                foreach (string key in data.Keys)
                {
                    if(key.Contains(hardware.Name))
                    {
                        cpuKey = key;
                        break;
                    }
                }

                foreach (var sensor in hardware.Sensors)
                {
                    if (sensor.Value.HasValue)
                    {
                        data[cpuKey].Add(sensor.Name + " " + sensor.SensorType.ToString(), sensor.Value.ToString());
                    }
                }
            }

            
        }
    }
}
