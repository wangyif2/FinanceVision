using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Drawing;
using System.IO;
using Emgu.CV;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.UI;
using Emgu.CV.Util;
using System.Text.RegularExpressions;

namespace RubidiumTextFinder
{
    class Word
    {
        public List<Tesseract.Charactor> Characters;

        public Word()
        {
            Characters = new List<Tesseract.Charactor>();
        }

        public int getX()
        {
            return Characters.FirstOrDefault<Tesseract.Charactor>().Region.X + (getWidth() / 2);
        }

        public int getWidth()
        {
            return (Characters.Last<Tesseract.Charactor>().Region.X + Characters.Last<Tesseract.Charactor>().Region.Width) - Characters.FirstOrDefault<Tesseract.Charactor>().Region.X;
        }

        public int getY()
        {
            return Characters.FirstOrDefault<Tesseract.Charactor>().Region.Y + (getHeight() / 2);
        }

        public int getHeight()
        {
            return Characters.Last<Tesseract.Charactor>().Region.Height;
        }

        public string ToString()
        {
            string text = "";
            foreach (Tesseract.Charactor c in Characters)
            {
                text += c.Text;
            }
            return text.Replace("\"","").Replace("\'","");
        }

        public string ToJSON()
        {
            string text = "{";
            text += "\"text\":\"" + ToString() + "\",";
            text += "\"x\":\"" + getX() + "\",";
            text += "\"y\":\"" + getY() + "\"";
            text += "}";
            return text;
        }
    }

    class Program
    {

        static void Main(string[] args)
        {
            if (args[0] == "-text")
            {
                findText(args[1], args[2]);
            }
            else if (args[0] == "-image")
            {
                findImage(args[1], args[2]);
            }
        }

        public static void findImage(String imageToFind, String image)
        {
            Image<Gray, Byte> modelImage = new Image<Gray, byte>(imageToFind);
            Image<Gray, Byte> observedImage = new Image<Gray, byte>(image);
            HomographyMatrix homography = null;

            SURFDetector surfCPU = new SURFDetector(500, false);

            VectorOfKeyPoint modelKeyPoints;
            VectorOfKeyPoint observedKeyPoints;
            Matrix<int> indices;
            Matrix<float> dist;
            Matrix<byte> mask;

            //extract features from the object image
            modelKeyPoints = surfCPU.DetectKeyPointsRaw(modelImage, null);
            //MKeyPoint[] kpts = modelKeyPoints.ToArray();
            Matrix<float> modelDescriptors = surfCPU.ComputeDescriptorsRaw(modelImage, null, modelKeyPoints);


            // extract features from the observed image
            observedKeyPoints = surfCPU.DetectKeyPointsRaw(observedImage, null);
            Matrix<float> observedDescriptors = surfCPU.ComputeDescriptorsRaw(observedImage, null, observedKeyPoints);

            BruteForceMatcher matcher = new BruteForceMatcher(BruteForceMatcher.DistanceType.L2F32);
            matcher.Add(modelDescriptors);
            int k = 2;
            indices = new Matrix<int>(observedDescriptors.Rows, k);
            dist = new Matrix<float>(observedDescriptors.Rows, k);
            matcher.KnnMatch(observedDescriptors, indices, dist, k, null);

            mask = new Matrix<byte>(dist.Rows, 1);

            mask.SetValue(255);

            Features2DTracker.VoteForUniqueness(dist, 0.8, mask);

            int nonZeroCount = CvInvoke.cvCountNonZero(mask);
            if (nonZeroCount >= 4)
            {
                nonZeroCount = Features2DTracker.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints, indices, mask, 1.5, 20);
                if (nonZeroCount >= 4)
                    homography = Features2DTracker.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints, observedKeyPoints, indices, mask, 3);
            }


            #region draw the projected region on the image
            if (homography != null)
            {  //draw a rectangle along the projected model
                Rectangle rect = modelImage.ROI;
                PointF[] pts = new PointF[] { 
               new PointF(rect.Left, rect.Bottom),
               new PointF(rect.Right, rect.Bottom),
               new PointF(rect.Right, rect.Top),
               new PointF(rect.Left, rect.Top)};
                homography.ProjectPoints(pts);
                int centerx = ((int)(pts[0].X + ((pts[2].X - pts[0].X) / 2)));
                int centery = ((int)(pts[0].Y + ((pts[2].Y - pts[0].Y) / 2)));
                Console.WriteLine("{\"x\":" + centerx + "\",\"y\":" + centery + "}");
            }
            #endregion
        }

        public static void findText(String testDataPath, String imagePath)
        {
            Tesseract ocr = new Tesseract(testDataPath, "eng", Tesseract.OcrEngineMode.OEM_TESSERACT_CUBE_COMBINED);
            try
            {
                Image<Bgr, Byte> image = new Image<Bgr, byte>(imagePath);

                using (Image<Gray, byte> gray = image.Convert<Gray, Byte>())
                {
                    ocr.Recognize(gray);
                    Tesseract.Charactor[] charactors = ocr.GetCharactors();

                    List<Word> words = new List<Word>();
                    Word word = new Word();
                    foreach (Tesseract.Charactor c in charactors)
                    {
                        Tesseract.Charactor copy = c;
                        copy.Text= Regex.Replace(c.Text, @"[^\u0000-\u007F]", string.Empty);
                        if (c.Text != " ")
                        {
                            word.Characters.Add(c);
                        }
                        else
                        {
                            words.Add(word);
                            word = new Word();
                        }
                    }
                    words.Add(word);
                    Console.WriteLine("{\"words\":[");
                    foreach (Word w in words)
                    {
                        Console.WriteLine(w.ToJSON() + ",");
                    }
                    Console.WriteLine("{\"text\":\"\",\"x\":\"0\",\"y\":\"0\"}");
                    Console.WriteLine("]}");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
}