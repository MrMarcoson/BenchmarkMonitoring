using OpenHardwareMonitor.Hardware;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Reflection;

namespace BenchmarkLibrary
{
    public class RAMInfo
    {
        public Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>();

        public RAMInfo()
        {
            Computer computer = new Computer { RAMEnabled = true };
            computer.Open();

            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("select * from Win32_PhysicalMemory");
            foreach (ManagementObject obj in managementObjectSearcher.Get())
            {
                Dictionary<string, string> ram = new Dictionary<string, string>();
                ram["name"] = obj["Tag"].ToString();
                ram["capacity"] = obj["Capacity"].ToString();
                ram["clockSpeed"] = obj["ConfiguredClockSpeed"].ToString();
                ram["voltage"] = obj["ConfiguredVoltage"].ToString();
                ram["totalWidth"] = obj["TotalWidth"].ToString();
                ram["serialNumber"] = obj["SerialNumber"].ToString();
                data.Add(ram["name"], ram);
            }

            foreach (var hardware in computer.Hardware)
            {
                hardware.Update();
                Dictionary<string, string> ram = new Dictionary<string, string>();

                foreach (var sensor in hardware.Sensors)
                {
                    if (sensor.Value.HasValue)
                    {
                        ram[sensor.Name + " " + sensor.SensorType.ToString()] = (sensor.Value.ToString());
                    }
                }

                data.Add(hardware.Name, ram);
            }
        }
    }
}
