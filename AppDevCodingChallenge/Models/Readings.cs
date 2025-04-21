using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;

namespace AppDevCodingChallenge.Models
{
    internal class Readings
    {
        [Name("Device ID")]
        public int DeviceID { get; set; }

        [Name("Time")]
        public DateTime Time { get; set; }

        [Name("Rainfall")]
        public double Rainfall { get; set; }
    }
}
