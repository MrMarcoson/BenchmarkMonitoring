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
        string CPU { get; set; }
        string GPU { get; set; }
        string RAM { get; set; }

        public BenchmarkInfo()
        {
            CPU = serializeData(new CPUInfo().data);
            GPU = serializeData(new CPUInfo().data);
            RAM = serializeData(new CPUInfo().data);
        }

        private string serializeData(Dictionary<string, Dictionary<string, string>> data)
        {
            return JsonConvert.SerializeObject(data, Formatting.Indented);
        }
    }
}
