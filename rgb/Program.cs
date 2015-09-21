using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace rgb
{
    class Program
    {
        static void Main(string[] args)
        {
            var folder = args[0];
            var allFiles = Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories);
            foreach (var file in allFiles)
            {
                ProcessFile(file);
            }
        }

        private static void ProcessFile(string file)
        {
            Image image = null;
            try {
                image = Image.FromFile(file);
            }
            catch
            {
                //not an image
            }
            if(image != null)
            {
                var pixel = ResizeImage(image, 1, 1).GetPixel(0,0);
                var rgb = HexConverter(pixel); 
                Console.WriteLine(string.Format("{0}, {1} {2} {3}, {4}", file, pixel.R, pixel.G, pixel.B, rgb));
            }
        }

        private static String HexConverter(System.Drawing.Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
