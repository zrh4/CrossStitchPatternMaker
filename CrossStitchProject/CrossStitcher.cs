using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Reflection;
using CrossStitchProject.Helpers;

namespace CrossStitchProject
{
    public class CrossStitcher
    {
        private Bitmap Image{ get; }
        private bool IsColorPattern { get; }
        private bool ShouldDitherImage { get; }
        private string ProjectName { get; }
        private Dictionary<Color,Floss> FlossDict { get; }
        public CrossStitcher(Bitmap b, bool isColorPattern, bool shouldDitherImage, string projectName)
        {
            Image = b;
            IsColorPattern = isColorPattern;
            ShouldDitherImage = shouldDitherImage;
            ProjectName = projectName;
            FlossDict = GenerateFlossDictionary();
        }
        public void GenerateCrossStitch()
        {
            var csBitmap = GenerateStitchBitmap();
            var htmlWriter = new HtmlWriter
                (csBitmap,FlossDict,IsColorPattern,ProjectName);
            htmlWriter.BuildAndSavePattern();
        }
        public Bitmap GenerateStitchBitmap()
        {
            var validColors = FlossDict.Keys.ToList();
            var colorMap = new Dictionary<Color, Color>();
            for (var y = 0; y < Image.Height; y++)
            {
                for (var x = 0; x < Image.Width; x++)
                {
                    var origColor = Image.GetPixel(x, y);
                    if(!colorMap.TryGetValue(origColor,out var closest))
                    {
                        closest = ColorMath.GetClosestColor(validColors, origColor);
                        colorMap.Add(origColor, closest);
                    }
                    Image.SetPixel(x,y,closest);
                    if (!ShouldDitherImage) continue;
                    ColorMath.FloydSteinberg(Image,origColor,closest,x,y);

                }
            }
            return Image;
        }
        private static Dictionary<Color, Floss> GenerateFlossDictionary()
        {
            var hex2Floss = new Dictionary<Color, Floss>();
            using (var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("CrossStitchProject.floss2hex.dat"))
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var splitLine = line.Split(' ');
                    var flossColor = ColorTranslator.FromHtml(splitLine[1]);
                    hex2Floss.Add(flossColor, new Floss { DMC = splitLine[0], Symbol = splitLine[2] });
                }
            }
            return hex2Floss;
        }
    }
}
