using System;
using System.Linq;

namespace HardwareDetector
{
    class Program
    {
        private const int DefaultWebPort = 8090;

        static void Main(string[] args)
        {
            if (args.Contains("--web") || args.Contains("web"))
            {
                int port = DefaultWebPort;
                string? portArg = args.FirstOrDefault(a => a.StartsWith("--port="));
                if (portArg != null && int.TryParse(portArg["--port=".Length..], out int parsedPort))
                {
                    port = parsedPort;
                }

                WebServer.Run(port);
                return;
            }

            try
            {
                var snapshot = HardwareInspector.Collect();
                ConsoleReportWriter.Write(snapshot);

                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}
