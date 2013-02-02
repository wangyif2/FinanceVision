using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Hawaii;
using Microsoft.Hawaii.Ocr.Client;

namespace FinanceVision
{
    public class OcrClient : AddPage
    {
        private const double ImageMaxSizeDiagonal = 600;

        public void StartOcrConversion(Stream photoStream)
        {
            // Images that are too large will take too long to transfer to the Hawaii OCR service.
            // Also images that are too large may contain text that is too big and that will be excluded from the OCR process.
            // If necessary, we will scale-down the image.
            photoStream = LimitImageSize(photoStream, ImageMaxSizeDiagonal);

            // Convert the photo stream to bytes.
            byte[] photoBuffer = StreamToByteArray(photoStream);

            // Instantiate the service proxy, set the oncomplete event handler, and trigger the asynchronous call.
            OcrService.RecognizeImageAsync(
                HawaiiClient.HawaiiApplicationId,
                photoBuffer,
                (output) => Dispatcher.BeginInvoke(() => OnOcrCompleted(output)));
        }

        private void OnOcrCompleted(OcrServiceResult result)
        {
            if (result.Status == Status.Success)
            {
                int wordCount = 0;
                StringBuilder sb = new StringBuilder();
                foreach (OcrText item in result.OcrResult.OcrTexts)
                {
                    wordCount += item.Words.Count;
                    sb.Append(item.Text);
                    sb.Append("\n");
                }

            }
            else
            {
//                txtError.Text = "[The OCR conversion failed]\n" + result.Exception.Message;
//                txtError.Visibility = Visibility.Visible;
            }
        }

        private static Stream LimitImageSize(Stream imageStream, double imageMaxDiagonalSize)
        {
            // In order to determine the size of the image we will transfer it to a writable bitmap.
            WriteableBitmap wb = new WriteableBitmap(1, 1);
            wb.SetSource(imageStream);

            // Check if we need to scale it down.
            double imageDiagonalSize = Math.Sqrt(wb.PixelWidth * wb.PixelWidth + wb.PixelHeight * wb.PixelHeight);
            if (imageDiagonalSize > imageMaxDiagonalSize)
            {
                // Calculate the new image size that corresponds to imageMaxDiagonalSize for the 
                // diagonal size and that preserves the aspect ratio.
                int newWidth = (int)(wb.PixelWidth * imageMaxDiagonalSize / imageDiagonalSize);
                int newHeight = (int)(wb.PixelHeight * imageMaxDiagonalSize / imageDiagonalSize);

                Stream resizedStream = null;
                Stream tempStream = null;

                // This try/finally block is needed to avoid CA2000: Dispose objects before losing scope
                // See http://msdn.microsoft.com/en-us/library/ms182289.aspx for more details.
                try
                {
                    tempStream = new MemoryStream();
                    Extensions.SaveJpeg(wb, tempStream, newWidth, newHeight, 0, 100);
                    resizedStream = tempStream;
                    tempStream = null;
                }
                finally
                {
                    if (tempStream != null)
                    {
                        tempStream.Close();
                        tempStream = null;
                    }
                }

                return resizedStream;
            }
            else
            {
                // No need to scale down. The image diagonal is less than or equal to imageMaxSizeDiagonal.
                return imageStream;
            }
        }

        private static byte[] StreamToByteArray(Stream stream)
        {
            byte[] buffer = new byte[stream.Length];

            long seekPosition = stream.Seek(0, SeekOrigin.Begin);
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            seekPosition = stream.Seek(0, SeekOrigin.Begin);

            return buffer;
        }

    }
}