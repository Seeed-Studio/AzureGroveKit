using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLib
{
    public class TestData
    {
        public static string GetTestDataString()
        {
            GroveMessage m = new GroveMessage()
            {
                Hum = "45",
                Temp = "25",
                Sound = "199",
                Light = "200",
                GasSO = 99,
                PIR = "1",
                Timestamp = DateTime.Now.ToString()
            };

            return JsonConvert.SerializeObject(m);
        }
    }

    internal class GroveMessage
    {
        public string Hum { get; set; }
        public string Temp { get; set; }
        public string Sound { get; set; }
        public string Light { get; set; }
        public int GasSO { get; set; }
        public string PIR { get; set; }
        public string Timestamp { get; set; }
    }
}
