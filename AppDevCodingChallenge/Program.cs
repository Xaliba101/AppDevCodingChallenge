using System.Formats.Asn1;
using System.Globalization;
using AppDevCodingChallenge.Models;
using CsvHelper;

namespace AppDevCodingChallenge
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /* quick list of what to do, not final:
             * display welcome message
             * check csv files exists
             * read csv files
             * group by devices
             * get last 4 hours of data for each device and display:
             * Average rainfall
             * Green, amber or red
             * is avergate rainfall increasing or decreasing
             */

            Console.WriteLine("Fuzion Inc");
            Console.WriteLine("Flood detection programme");

            // Initialise and set the assumedCurrentTime to 2pm based on the latest data from data files.
            DateTime assumedCurrentDateTime = new DateTime(2020, 5, 6, 14, 0, 0);

            // set up file and folder paths
            string devicesFilePath = "Data/devices.csv";
            string deviceReadingsFolderPath = "Data/Readings/";

            // Error check if devices file exists
            if (!File.Exists(devicesFilePath))
            {
                Console.Error.WriteLine("Devices file not found. Please confirm file exists in: " + devicesFilePath);
                Console.ReadKey();
                return;
            }

            // Error check if device readings folder exists
            if (!Directory.Exists(deviceReadingsFolderPath))
            {
                Console.WriteLine($"Device readings folder. Please confirm folder exists in: {deviceReadingsFolderPath}");
                Console.ReadKey();
                return;
            }

            // Error check if theres at least 1 device readings file
            string[] deviceReadingsFiles = Directory.GetFiles(deviceReadingsFolderPath, "*.csv");
            if (deviceReadingsFiles.Length == 0)
            {
                Console.WriteLine($"No device readings files found in: {deviceReadingsFolderPath}");
                Console.ReadKey();
                return;
            }


            // Load devices from CSV
            using var devicesReader = new StreamReader(devicesFilePath);
            using var devCsv = new CsvReader(devicesReader, CultureInfo.InvariantCulture);

            // Load device readings from CSV
            var deviceReadings = devCsv.GetRecords<Device>();

            // Initialise a list to store all joined and grouped data
            var allGroupedData = new List<IGrouping<dynamic, dynamic>>();

            // For each readings file, stream readings and do a LINQ Join
            foreach (var dataFile in deviceReadingsFiles)
            {
                // Read the device readings file
                using var dataReader = new StreamReader(dataFile);
                using var dataCsv = new CsvReader(dataReader, CultureInfo.InvariantCulture);

                // Load device readings from CSV
                var readings = dataCsv.GetRecords<Reading>().ToList();

                // Join the device readings with the devices
                var joinedData = from reading in readings
                                 join device in deviceReadings on reading.DeviceID equals device.DeviceID
                                 select new
                                 {
                                     DeviceID = device.DeviceID,
                                     DeviceName = device.DeviceName,
                                     Location = device.Location,
                                     Time = reading.Time,
                                     Rainfall = reading.Rainfall
                                 };

                // Group by device and add to the collection
                var groupedDataFromFile = joinedData.GroupBy(x => new { x.DeviceID, x.DeviceName, x.Location }).ToList();
                allGroupedData.AddRange(groupedDataFromFile);
            }

            // Display grouped data
            foreach (var group in allGroupedData)
            {
                // Display the device information and the number of readings
                Console.WriteLine($"Device ID: \t{group.Key.DeviceID}");
                Console.WriteLine($"Device Name: \t{group.Key.DeviceName}");
                Console.WriteLine($"Location: \t{group.Key.Location}");
                Console.WriteLine($"Number of readings: \t{group.Count()}");

                // Get the last 4 hours of data
                var last4HoursData = group.Where(x => x.Time >= assumedCurrentDateTime.AddHours(-4)).ToList();

                // Check if there is any data in the last 4 hours
                if (last4HoursData.Count == 0)
                {
                    Console.WriteLine("Average rainfall: \tNo Data found");
                    continue;
                }

                // Get average rainfall
                double averageRainfall = last4HoursData.Average(x => x.Rainfall);

                // Check if any single reading is over 30
                bool anyRainfallOver30 = last4HoursData.Any(x => x.Rainfall > 30);

                // Apply the status rules
                if (averageRainfall >= 15 || anyRainfallOver30)
                {
                    Console.WriteLine("Average rainfall: \tRed");
                }
                else if (averageRainfall >= 10 && averageRainfall < 15)
                {
                    Console.WriteLine("Average rainfall: \tAmber");
                }
                else 
                {
                    Console.WriteLine("Average rainfall: \tGreen");
                }

                // Display blank line for readability
                Console.WriteLine();
            }





            // Completed
            //Console.WriteLine("Press any key to exit...");
            //Console.ReadKey();
        }
    }
}
