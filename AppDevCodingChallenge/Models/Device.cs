using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;

namespace AppDevCodingChallenge.Models
{
    /// <summary>
    /// Simple class to represent a device from the CSV file.
    /// </summary>
    internal class Device
    {
        [Name("Device ID")]
        public int DeviceID { get; set; }

        [Name("Device Name")]
        public string DeviceName { get; set; }

        [Name("Location")]
        public string Location { get; set; }
    }
}
