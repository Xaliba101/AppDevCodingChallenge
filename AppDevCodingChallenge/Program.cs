using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using AppDevCodingChallenge.Models;
using CsvHelper;

namespace AppDevCodingChallenge
{
    internal class Program
    {
        /// <summary>
        /// Main method to run the flood detection programme.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Display the company name and programme name
            Console.WriteLine("Fuzion Inc");
            Console.WriteLine("Flood detection programme");
            Console.WriteLine("======================================");

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

            // Initialise a list to store all joined readings
            var allJoinedReadings = new List<dynamic>();

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

                // Add the joined data to the list
                allJoinedReadings.AddRange(joinedData);
            }

            // Group all the data now that all files are processed
            var allGroupedData = allJoinedReadings
                .GroupBy(x => new { x.DeviceID, x.DeviceName, x.Location })
                .ToList();

            // Display grouped data
            foreach (var group in allGroupedData)
            {
                // Display the device information and the number of readings
                Console.WriteLine($"Device ID: \t\t{group.Key.DeviceID}");
                Console.WriteLine($"Device Name: \t\t{group.Key.DeviceName}");
                Console.WriteLine($"Location: \t\t{group.Key.Location}");
                //Console.WriteLine($"Total Readings: \t{group.Count()}");


                // Get the last 4 hours of data
                var last4HoursData = group
                    .Where(x => x.Time >= assumedCurrentDateTime
                    .AddHours(-4))
                    .ToList();

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
                    Console.WriteLine($"Average rainfall: \t{averageRainfall}mm (Red)");
                }
                else if (averageRainfall >= 10 && averageRainfall < 15)
                {
                    Console.WriteLine($"Average rainfall: \t{averageRainfall}mm (Amber)");
                }
                else 
                {
                    Console.WriteLine($"Average rainfall: \t{averageRainfall}mm (Green)");
                }

                // Get the rainfall data for the last 4 hours
                var rainfallTrend = last4HoursData
                    .Select(x => (double)x.Rainfall)
                    .ToList();

                // Calculate the average rainfall for the last 4 hours
                var currentAverage = rainfallTrend.Any() ? rainfallTrend.Average() : 0;

                // Get data for the previous 4 hours
                var previousRainfallTrend = group
                    .Where(x => x.Time < assumedCurrentDateTime.AddHours(-4) && // Current 4-hour window
                                x.Time >= assumedCurrentDateTime.AddHours(-8)) // Previous 4-hour window
                    .Select(x => (double)x.Rainfall)
                    .ToList();

                // Calculate the average rainfall for the previous 4 hours
                var previousAverage = previousRainfallTrend.Any() ? previousRainfallTrend.Average() : 0;

                // Determine the rainfall trend
                if (currentAverage > previousAverage)
                {
                    // Rainfall is increasing
                    Console.WriteLine("Rainfall trend: \tIncreasing");
                }
                else if (currentAverage < previousAverage)
                {
                    // Rainfall is decreasing
                    Console.WriteLine("Rainfall trend: \tDecreasing");
                }
                else
                {
                    // Rainfall is stable
                    Console.WriteLine("Rainfall trend: \tStable");
                }

                // Output separator
                Console.WriteLine("=======================================");
            }
        }
    }
}
