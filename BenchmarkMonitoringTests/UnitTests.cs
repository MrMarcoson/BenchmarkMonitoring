using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using BenchmarkLibrary;
using BenchmarkFrontendConsole;
using System.Collections.Generic;
using System.Management;

namespace BenchmarkMonitoringTests
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void TestCPUsNames()
        {
            CPUInfo CPUInfoTest = new CPUInfo();
            List<string> expectedKeys = new List<string>(CPUInfoTest.data.Keys);

            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("select * from Win32_Processor");
            List<string> actualKeys = new List<string>();

            foreach (ManagementObject obj in managementObjectSearcher.Get())
            {
                actualKeys.Add(obj["Name"].ToString());
            }

            CollectionAssert.AreEquivalent(expectedKeys, actualKeys);
        }

        [TestMethod]
        public void TestGPUsNames()
        {
            GPUInfo GPUInfoTest = new GPUInfo();
            List<string> expectedKeys = new List<string>(GPUInfoTest.data.Keys);

            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("select * from Win32_VideoController");
            List<string> actualKeys = new List<string>();

            foreach (ManagementObject obj in managementObjectSearcher.Get())
            {
                actualKeys.Add(obj["Name"].ToString());
            }

            CollectionAssert.AreEquivalent(expectedKeys, actualKeys);
        }

        [TestMethod]
        public void TestRAMNames()
        {
            RAMInfo RamInfoTest = new RAMInfo();
            List<string> expectedKeys = new List<string>(RamInfoTest.data.Keys);

            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("select * from Win32_PhysicalMemory");
            List<string> actualKeys = new List<string>();

            foreach (ManagementObject obj in managementObjectSearcher.Get())
            {
                actualKeys.Add(obj["Tag"].ToString());
            }

            CollectionAssert.IsSubsetOf(actualKeys, expectedKeys);
        }

        [TestMethod]
        public void TestGPUTemperatures()
        {
            Dictionary<string, Dictionary<string, string>> data = new GPUInfo().data;

            foreach (Dictionary<string, string> hardware in data.Values)
            {
                if (hardware.ContainsKey("GPU Core Temperature"))
                {
                    double coreTemp = Convert.ToDouble(hardware["GPU Core Temperature"]);
                    double hotTemp = Convert.ToDouble(hardware["GPU Hot Spot Temperature"]);
                    Assert.IsTrue(coreTemp >= 0 && hotTemp >= 0, "Temperatures are out of rational range");
                }
            }

        }

        [TestMethod]
        public void TestCPUCores()
        {
            CPUInfo CPUInfoTest = new CPUInfo();
            Dictionary<string, int> expectedCores = new Dictionary<string, int>();

            foreach (var hardware in CPUInfoTest.data)
            {
                foreach (var stat in hardware.Value)
                {
                    if (stat.Key.StartsWith("CPU Core #") && stat.Key.EndsWith("Load"))
                    {
                        if (expectedCores.ContainsKey(hardware.Key)) expectedCores[hardware.Key]++;
                        else expectedCores.Add(hardware.Key, 1);
                    }
                }
            }

            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("select * from Win32_Processor");
            Dictionary<string, int> actualCores = new Dictionary<string, int>();
            foreach (ManagementObject obj in managementObjectSearcher.Get())
            {
                actualCores.Add(obj["Name"].ToString(), Convert.ToInt32(obj["NumberOfCores"]));
            }

            foreach (var hardware in actualCores)
            {
                Assert.AreEqual(hardware.Value, expectedCores[hardware.Key]);
            }
        }

        [TestMethod]
        public void TestJSONDeserialization()
        {
            Assert.ThrowsException<Newtonsoft.Json.JsonSerializationException>(() => Program.deserialize("{\r\n  \"string\": \"Bad format\"}\r\n}"));
            Assert.IsInstanceOfType(
                Program.deserialize(@"{
                                  ""Physical Memory 1"": {
                                    ""name"": ""Physical Memory 1"",
                                    ""capacity"": ""8589934592"",
                                    ""clockSpeed"": ""2666"",
                                    ""voltage"": ""1200"",
                                    ""totalWidth"": ""64"",
                                    ""serialNumber"": ""E5F48CAE""
                                  }
                                }"
                ), typeof(Dictionary<string, Dictionary<string, string>>));
        }
    }
}
