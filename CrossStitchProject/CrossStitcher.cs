using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Reflection;
using CrossStitchProject.Helpers;

namespace CrossStitchProject
{
    public class CrossStitcher
    {
        private string Filename { get; }
        private bool ColorPattern { get; }
        private bool DitherImage { get; }
        private string OutputFilename { get; }
        private Dictionary<Color,FlossInfo> FlossDict { get; }
        public CrossStitcher(string filename, bool colorPattern, bool ditherImage, string outputFilename)
        {
            Filename = filename;
            ColorPattern = colorPattern;
            DitherImage = ditherImage;
            OutputFilename = outputFilename;
            FlossDict = GenerateFlossDictionary();
        }
        public void GenerateCrossStitch()
        {
            var csBitmap = GenerateStitchBitmap();
            var htmlWriter = new HtmlWriter
                (csBitmap,FlossDict,ColorPattern,OutputFilename);
            htmlWriter.GenerateHtml();
        }
        public Bitmap GenerateStitchBitmap()
        {
            var bitmap = new Bitmap(Filename);
            var validColors = FlossDict.Keys.ToList();
            var colorMap = new Dictionary<Color, Color>();
            for (var y = 0; y < bitmap.Height; y++)
            {
                for (var x = 0; x < bitmap.Width; x++)
                {
                    var origColor = bitmap.GetPixel(x, y);
                    if(!colorMap.TryGetValue(origColor,out var closest))
                    {
                        closest = ColorMath.GetClosestColor(validColors, origColor);
                        colorMap.Add(origColor, closest);
                    }
                    bitmap.SetPixel(x,y,closest);
                    if (!DitherImage) continue;
                    ColorMath.FloydSteinberg(bitmap,origColor,closest,x,y);

                }
            }
            return bitmap;
        }
        private static Dictionary<Color, FlossInfo> GenerateFlossDictionary()
        {
            var hex2Floss = new Dictionary<Color, FlossInfo>();
            using (var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("CrossStitchProject.floss2hex.dat"))
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var splitLine = line.Split(' ');
                    var hex = Convert.ToInt32(splitLine[1], 16);
                    var newColor = Color.FromArgb(hex >> 16, (hex & 0x00ff00) >> 8, hex & 0x0000ff);
                    hex2Floss.Add(newColor, new FlossInfo { FlossId = splitLine[0], FlossSymbol = splitLine[2] });
                }
            }
            return hex2Floss;
        }
    }
}
