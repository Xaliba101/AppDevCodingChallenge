using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;

namespace AppDevCodingChallenge.Models
{
    internal class Devices
    {
        [Name("Device ID")]
        public int DeviceID { get; set; }

        [Name("Device Name")]
        public string DeviceName { get; set; }

        [Name("Location")]
        public string Location { get; set; }
    }
}
