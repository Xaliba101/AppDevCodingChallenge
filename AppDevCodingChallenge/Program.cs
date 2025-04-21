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







            // Completed
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
