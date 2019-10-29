using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Reflection;
using CrossStitchPatternMaker.Helpers;
using CrossStitchPatternMaker.Models;

namespace CrossStitchPatternMaker
{
    public class CrossStitcher
    {
        private Bitmap Picture { get; }
        private bool IsColorPattern { get; }
        private bool ShouldDitherImage { get; }
        private string ProjectName { get; }
        public CrossStitcher(Bitmap b, bool isColorPattern, bool shouldDitherImage, string projectName)
        {
            Picture = b;
            IsColorPattern = isColorPattern;
            ShouldDitherImage = shouldDitherImage;
            ProjectName = projectName;
        }
        public void GenerateCrossStitch()
        {
            var flossColors = GetFlossColors();
            var csBitmap = GenerateStitchBitmap(flossColors);
            var flossDict = GenerateFlossDictionary(csBitmap);
            var patternWriter = new PatternWriter
                (csBitmap, flossDict, IsColorPattern, ProjectName);
            patternWriter.Build();
            patternWriter.Save();
        }
        public Bitmap GenerateStitchBitmap() => GenerateStitchBitmap(GetFlossColors());
        public Bitmap GenerateStitchBitmap(List<Color> validColors)
        {
            var colorMap = new Dictionary<Color, Color>();
            for (var y = 0; y < Picture.Height; y++)
            {
                for (var x = 0; x < Picture.Width; x++)
                {
                    var origColor = Picture.GetPixel(x, y);
                    if (!colorMap.TryGetValue(origColor, out var closest))
                    {
                        closest = ColorMath.GetClosestColor(validColors, origColor);
                        colorMap.Add(origColor, closest);
                    }
                    Picture.SetPixel(x, y, closest);
                    if (!ShouldDitherImage) continue;
                    ColorMath.FloydSteinberg(Picture, origColor, closest, x, y);
                }
            }
            return Picture;
        }
        public static List<Color> GetFlossColors()
        {
            var flosses = new List<Color>();
            using (var flossStream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("CrossStitchPatternMaker.floss2hex.dat"))
            using (var flossReader = new StreamReader(flossStream))
            {
                while (!flossReader.EndOfStream)
                {
                    var line = flossReader.ReadLine();
                    var splitLine = line.Split(' ');
                    var flossColor = ColorTranslator.FromHtml(splitLine[1]);
                    flosses.Add(flossColor);
                }
            }
            return flosses;
        }
        private static Dictionary<Color,Floss> GenerateFlossDictionary(Bitmap image)
        {
            var colorsNeeded = ImageHelper.GetColors(image);
            var dict = new Dictionary<Color, Floss>();
            var symbolEnumerator = Symbols.GetSymbols().GetEnumerator();
            symbolEnumerator.MoveNext();
            using (var flossStream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("CrossStitchPatternMaker.floss2hex.dat"))
            using (var flossReader = new StreamReader(flossStream))
            {
                while (!flossReader.EndOfStream)
                {
                    var line = flossReader.ReadLine();
                    var splitLine = line.Split(' ');
                    var flossColor = ColorTranslator.FromHtml(splitLine[1]);
                    if (!colorsNeeded.Contains(flossColor)) { continue; }
                    dict.Add(flossColor, new Floss
                    {
                        DMC = int.Parse(splitLine[0]),
                        Symbol = symbolEnumerator.Current
                    });
                    symbolEnumerator.MoveNext();
                }
            }
            return dict;
        }
    }
}
