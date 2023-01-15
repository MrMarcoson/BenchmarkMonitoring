using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenHardwareMonitor.Hardware;

namespace BenchmarkLibrary
{
    public class GPUInfo
    {
        public Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>();

        public GPUInfo()
        {
            Computer computer = new Computer { GPUEnabled = true };
            computer.Open();

            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("select * from Win32_VideoController");
            foreach (ManagementObject obj in managementObjectSearcher.Get())
            {
                Dictionary<string, string> gpu = new Dictionary<string, string>();
                gpu["name"] = obj["Name"].ToString();
                gpu["videoProcessor"] = obj["VideoProcessor"].ToString();
                gpu["memory"] = obj["AdapterRAM"].ToString();
                gpu["refreshRate"] = obj["CurrentRefreshRate"].ToString();
                gpu["horizontalResolution"] = obj["CurrentHorizontalResolution"].ToString();
                gpu["verticalResolution"] = obj["CurrentVerticalResolution"].ToString();
                gpu["driverVersion"] = obj["DriverVersion"].ToString();
                data.Add(gpu["name"], gpu);
            }

            foreach (var hardware in computer.Hardware)
            {
                hardware.Update();

                foreach (var sensor in hardware.Sensors)
                {
                    if(sensor.Value.HasValue)
                    {
                        data[hardware.Name].Add(sensor.Name + " " + sensor.SensorType.ToString(), sensor.Value.ToString());
                    }
                }
            }
        }
    }
}
