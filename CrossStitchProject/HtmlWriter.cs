using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace CrossStitchProject
{
    internal class HtmlWriter
    {
        //TODO: make a lot of this stuff resources maybe and refactor to be less ugly
        private readonly List<Bitmap> _imageChunks = new List<Bitmap>();
        private readonly Dictionary<Color, Floss> _color2Floss;
        private readonly bool _isColored;
        private const decimal DefaultChunkWidth = 50.0M;
        private const decimal DefaultChunkHeight = 60.0M;
        private static string _outputPath;
        private const string LegendTableStart = "<table><thead><tr><th>Floss</th><th>Symbol</th></tr></thead><tbody>";
        private const string LegendTableEnd = "</tbody></table>";

        public HtmlWriter(Bitmap b, Dictionary<Color, Floss> c2F, bool colored, string outDir)
        {
            ChunkifyImage(b);

            _isColored = colored;
            _color2Floss = c2F;

            var pictureFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            if (string.IsNullOrEmpty(outDir)) { outDir = "MyCrossStitchPattern"; }
            _outputPath = Path.Combine(pictureFolder, outDir);

            if (!Directory.Exists(_outputPath)) { Directory.CreateDirectory(_outputPath); }
        }

        private void ChunkifyImage(Bitmap b)
        {
            var numHorizontalChunks = (int)Math.Ceiling(b.Width / DefaultChunkWidth);
            var numVerticalChunks = (int)Math.Ceiling(b.Height / DefaultChunkHeight);
            for (var y = 0; y < numVerticalChunks;  y++)
            {
                for (var x = 0; x < numHorizontalChunks; x++)
                {
                    var widthLeft = b.Width - (x + 1) * DefaultChunkWidth;
                    var heightLeft = b.Height - (y + 1) * DefaultChunkHeight;
                    var curChunkWidth = (int)(widthLeft < 0 ? widthLeft + DefaultChunkWidth: DefaultChunkWidth);
                    var curChunkHeight = (int)(heightLeft < 0 ? heightLeft + DefaultChunkHeight: DefaultChunkHeight);
                    var chunk = b.Clone(
                        new Rectangle(x * (int) DefaultChunkWidth, y * (int) DefaultChunkHeight, curChunkWidth,
                            curChunkHeight), PixelFormat.Format32bppArgb);
                    _imageChunks.Add(chunk);
                }
            }

        }
        private void Save(StringBuilder outString, string filename)
        {
            File.WriteAllText(Path.Combine(_outputPath, filename), outString.ToString());
            Process.Start(_outputPath);
        }

        private void SaveChunk(StringBuilder outString, int fileNo)
        {
            File.WriteAllText(Path.Combine(_outputPath, $"chunk{fileNo}.html"), outString.ToString());
        }

        private void GenerateHtmlLegend(IReadOnlyList<Floss> flosses)
        {
            var tableEnded = false;
            var htmlString = new StringBuilder(File.ReadAllText("legend_template.html"));
            htmlString.AppendLine(LegendTableStart);
            for (var i=0; i < flosses.Count;i++)
            {
                tableEnded = false;

                htmlString.AppendLine($"<tr><td>{flosses[i].DMC}</td><td>{flosses[i].Symbol}</td></tr>");

                if (i > 0 && (i+1) % DefaultChunkWidth == 0)
                {
                    htmlString.AppendLine(LegendTableEnd);
                    htmlString.AppendLine(LegendTableStart);
                    tableEnded = true;
                }
            }

            if (!tableEnded)
            {
                htmlString.AppendLine(LegendTableEnd);
            }
            Save(htmlString,"legend.html");
        }
        public void GenerateHtml()
        {
            var distinctFlossesInImage = GenerateCrossStitchPattern();
            GenerateHtmlLegend(distinctFlossesInImage.ToList());
        }
        //TODO: replace html with new class method calls?
        public HashSet<Floss> GenerateCrossStitchPattern()
        {
            var chunkCount = 0;
            var flossesInImage = new HashSet<Floss>();
            foreach (var chunk in _imageChunks)
            {
                var htmlString = new StringBuilder(File.ReadAllText("template.html"));
                for (var y = 0; y < chunk.Height; y++)
                {
                    htmlString.AppendLine("<div class='q'>");
                    for (var x = 0; x < chunk.Width; x++)
                    {
                        var pC = chunk.GetPixel(x, y);
                        var style = _isColored ? $" style='background-color:rgba({pC.R},{pC.G},{pC.B},0.5)' " : "";
                        if (!_color2Floss.TryGetValue(pC, out var floss))
                        {
                            throw new Exception("Floss dictionary invalid.");
                        }

                        flossesInImage.Add(floss);
                        if (x > 0 && x % DefaultChunkWidth == 0)
                        {
                            htmlString.AppendLine($"<div class='u'{style}></div><br/>");
                        }
                        if (x >= 5 && x % 5 == 0)
                        {
                            htmlString.AppendLine($"<div class='w x'{style}>{floss.Symbol}</div>");
                        }
                        else
                        {
                            htmlString.AppendLine($"<div class='x'{style}>{floss.Symbol}</div>");
                        }
                    }
                    if (y > 0 && (y + 1) % 5 == 0)
                    {
                        htmlString.AppendLine($"<div class='u'>{y + 1}</div>");
                    }

                    htmlString.AppendLine("</div>");

                }
                htmlString.AppendLine("</body></html>");
                SaveChunk(htmlString,chunkCount);
                chunkCount++;
            }
            return flossesInImage;
        }
    }
}
