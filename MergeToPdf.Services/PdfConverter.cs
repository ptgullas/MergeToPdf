using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using System;
using System.IO;
using iText.Layout.Element;
using iText.IO.Image;
using System.Collections.Generic;
using Serilog;

namespace MergeToPdf.Services {
    public static class PdfConverter {

        public static void AddMultipleImagesToNewPdf(List<string> imagePaths, string destinationFile) {
            destinationFile = RenameDestinationFileIfExists(destinationFile);
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFile));
            string initialImagePath = imagePaths[0];
            VerifyImageFileExists(initialImagePath);

            Image image = new Image(ImageDataFactory.Create(initialImagePath));
            PageSize imageSize = GetImageSize(image);
            Document doc = new Document(pdfDoc, imageSize);
            Document letterDoc = new Document(pdfDoc, PageSize.LETTER);

            foreach (string path in imagePaths) {
                try {
                    VerifyImageFileExists(path);
                    Log.Information("Adding file {path} to {destinationFile}", path, destinationFile);
                    image = new Image(ImageDataFactory.Create(path));
                    imageSize = GetImageSize(image);
                    pdfDoc.AddNewPage(imageSize);
                    doc.Add(image);
                }
                catch (Exception e) {
                    Log.Error(e, "Error adding image to PDF");
                }
            }
            doc.Close();
        }

        private static void VerifyImageFileExists(string initialImagePath) {
            if (!File.Exists(initialImagePath)) {
                throw new FileNotFoundException("Could not find image file!", initialImagePath);
            }
        }

        public static string RenameDestinationFileIfExists(string givenDestinationPath) {
            string targetFolder = System.IO.Path.GetDirectoryName(givenDestinationPath);
            Directory.CreateDirectory(targetFolder);
            string newDestinationFile = givenDestinationPath;
            string newDestinationPath = System.IO.Path.Combine(targetFolder, newDestinationFile);
            if (File.Exists(givenDestinationPath)) {
                string filenameRoot = System.IO.Path.GetFileNameWithoutExtension(givenDestinationPath);
                string fileExtension = System.IO.Path.GetExtension(givenDestinationPath);
                int uniqueNumber = 1;
                newDestinationFile = $"{filenameRoot}_{uniqueNumber.ToString("D2")}{fileExtension}";
                newDestinationPath = System.IO.Path.Combine(targetFolder, newDestinationFile);
                while (File.Exists(newDestinationPath)) {
                    uniqueNumber++;
                    newDestinationFile = $"{filenameRoot}_{uniqueNumber.ToString("D2")}{fileExtension}";
                    newDestinationPath = System.IO.Path.Combine(targetFolder, newDestinationFile);
                }
            }
            Log.Information("Will save images to new PDF file {destinationFile}", newDestinationPath);
            return newDestinationPath; 
        }

        public static void ConvertSingleImageToPdf(string destinationFile, string imageFilePath) {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFile));
            Image image = new Image(ImageDataFactory.Create(imageFilePath));
            var imageSize = GetImageSize(image);

            Document doc = new Document(pdfDoc, imageSize);

            pdfDoc.AddNewPage(imageSize);
            doc.Add(image);
            doc.Close();
        }

        public static void AddImageToPdfDoc(PdfDocument pdfDoc, Image image) {

            //SKImageInfo sKImageInfo = new SKImageInfo()
            //SKBitmap skBitmap = new SKBitmap()
        }

        private static PageSize GetImageSize(Image image) {
            return new PageSize(image.GetImageWidth(), image.GetImageHeight());
        }
    }
}
