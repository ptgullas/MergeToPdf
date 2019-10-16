using MergeToPdf.Services;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;

namespace MergeToPdf {
    class Program {

        public static IConfigurationRoot Configuration;

        static void Main(string[] args) {
            DisplayIntro();
            SetUpConfiguration();
            SetUpLogging();
            try {
                if (args.Length == 0) {
                    DisplayHelpText();
                }
                else {
                    ProcessArgs(args);
                }
            }
            catch (Exception e) {
                ColorHelpers.WriteLineColor("Uh-oh, got some errors!", ConsoleColor.Red);
                ColorHelpers.WriteLineColor($"{e.GetBaseException()}");
            }

            // have a switch for text file import
            // take in a text file that contains the paths to documents to convert to PDF, then merge into 1
            // second argument is the target document name

            // RunNormalTest();

        }

        static void DisplayIntro() {
            Console.WriteLine("MergeToPdf!");
            Console.WriteLine("Combine images into a PDF without having to install or pay for Adobe Acrobat!");
        }

        private static void SetUpConfiguration() {
            string projectRoot = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.LastIndexOf(@"\bin"));
            var builder = new ConfigurationBuilder()
                .SetBasePath(projectRoot)
                .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true);

            // IConfigurationRoot configuration = builder.Build();
            Configuration = builder.Build();

        }
        
        private static void SetUpLogging() {
            try {
                string logFolder = Configuration.GetSection("LocalFolders").GetValue<string>("logFolder");
                string logFile = Configuration.GetSection("LocalFolders").GetValue<string>("logFileName");
                string logPath = Path.Combine(logFolder, logFile);

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
                    .CreateLogger();
            }
            catch (Exception e) {
                Console.WriteLine($"Could not set up logging");
                Console.WriteLine(e.GetBaseException());
            }
        }

        private static void RunNormalTest() {
            string pathToImageFolder = @"C:\Users\Prime Time Pauly G\Pictures\Downloaded Lock Screens\2019 Q2\portrait";
            string imageName1 = "f98c210.jpg";
            string imageName2 = "70de15f.jpg";
            string imageName3 = "cc19c60.jpg";

            string path1 = Path.Combine(pathToImageFolder, imageName1);
            string path2 = Path.Combine(pathToImageFolder, imageName2);
            string path3 = Path.Combine(pathToImageFolder, imageName3);
            List<string> images = new List<string>() {
                path1,
                path2,
                path3
            };

            string destinationFile = @"c:\temp\mytestPdf.pdf";
            PdfConverter.AddMultipleImagesToNewPdf(images, destinationFile);
        }

        static void DisplayHelpText() {
            string applicationName = "dotnet run --";
            Console.WriteLine("Usage:");
            ColorHelpers.WriteLineColor($"{applicationName}: ");
            Console.WriteLine("\tDisplay this help text");
            ColorHelpers.WriteColor($"{applicationName} ");
            ColorHelpers.WriteColor($"<text file containing paths> ", ConsoleColor.Yellow);
            ColorHelpers.WriteLineColor($"<name of destination PDF>:", ConsoleColor.Magenta);
            Console.WriteLine("\tRead .txt file containing image paths, combine them into destination PDF.");
            ColorHelpers.WriteColor($"{applicationName} ");
            ColorHelpers.WriteColor($"<CSV file containing paths & desired quality (0-100)> ", ConsoleColor.Yellow);
            ColorHelpers.WriteLineColor($"<name of destination PDF>:", ConsoleColor.Magenta);
            Console.WriteLine("\tRead .csv file with lines in the form of '<path to image>,<integer 0-100>'. Combines them into destination PDF, each saved with desired quality level.");
        }

        static void ProcessArgs(string[] args) {
            if (args.Length != 2) {
                ColorHelpers.WriteLineColor("Need 2 arguments!", ConsoleColor.Red);
                DisplayHelpText();
            }
            else {
                if (args[0].EndsWith(".txt")) {
                    // handle regular .txt file
                    string textFile = args[0];
                    string destinationFile = args[1];
                    if (File.Exists(textFile)) {
                        Log.Information("Processing file {textFile}", textFile);
                        string[] imagesFromFile = File.ReadAllLines(textFile);
                        List<string> images = new List<string>(imagesFromFile);
                        PdfConverter.AddMultipleImagesToNewPdf(images, destinationFile);
                    }
                }
                else if (args[0].EndsWith(".csv")) {
                    // handle .csv file
                }
            }
        }


    }
}
