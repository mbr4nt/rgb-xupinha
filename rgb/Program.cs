using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Newtonsoft.Json;

namespace rgb
{
    class Program
    {
        static void Main(string[] args)
        {
            var folder = args[0];
            var format = args.Length > 1 ? args[1] : "csv";
            var allFiles = Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories);

            var colors =
                allFiles
                    .Select(file => new { file = Path.GetFileNameWithoutExtension(file), color = ProcessFile(file) })
                    .Where(r => r.color != Color.Empty)
                    .Select(r => new { r.color.R, r.color.G, r.color.B, Hex = HexConverter(r.color), File = r.file })
                    .ToList();

            if (format == "json")
                Console.Write(JsonConvert.SerializeObject(colors));
            else
                colors.ForEach(r => Console.WriteLine(string.Format("{0}, {1} {2} {3}, {4}", r.File, r.R, r.G, r.B, r.Hex)));

        }

        static Color ProcessFile(string file)
        {
            try
            {
                var image = Image.FromFile(file);
                var pixel = ResizeImage(image, 1, 1).GetPixel(0, 0);
                return pixel;                    
            }
            catch
            {
                return Color.Empty;
            }            
        }

        static String HexConverter(System.Drawing.Color c)
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
