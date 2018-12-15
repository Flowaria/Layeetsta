using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layeetsta.Util
{
    public class LapFile
    {
        public string Name { get; set; }
        public string Designer { get; set; }
        public string ProjectFolder { get; set; }
        public string ChartPath { get; set; }
        public string MusicPath { get; set; }
        public string BGA0Path { get; set; }
        public string BGA1Path { get; set; }
        public string BGA2Path { get; set; }

        public string Serialization()
        {
            return JsonConvert.SerializeObject(this).Replace(Environment.NewLine, "");
        }
    }
}
