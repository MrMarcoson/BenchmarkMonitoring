using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkLibrary
{
    public class BenchmarkInfo
    {
        public string CPU { get; set; }
        public string GPU { get; set; }
        public string RAM { get; set; }

        public BenchmarkInfo()
        {
            CPU = serializeData(new CPUInfo().data);
            GPU = serializeData(new GPUInfo().data);
            RAM = serializeData(new RAMInfo().data);
        }

        private string serializeData(Dictionary<string, Dictionary<string, string>> data)
        {
            return JsonConvert.SerializeObject(data, Formatting.Indented);
        }
    }
}
