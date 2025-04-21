using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;

namespace AppDevCodingChallenge.Models
{
    /// <summary>
    /// Simple class to represent a reading from the CSV file.
    /// </summary>
    public class Reading
    {
        [Name("Device ID")]
        public int DeviceID { get; set; }

        [Name("Time")]
        public DateTime Time { get; set; }

        [Name("Rainfall")]
        public int Rainfall { get; set; }
    }

    public class  ReadingMap : CsvHelper.Configuration.ClassMap<Reading>
    {
        public ReadingMap()
        {
            Map(m => m.DeviceID).Name("Device ID");
            Map(m => m.Time).Name("Time").TypeConverterOption.Format("dd-mm-YYYY HH:mm:ss");
            Map(m => m.Rainfall).Name("Rainfall");
        }
    }
}
