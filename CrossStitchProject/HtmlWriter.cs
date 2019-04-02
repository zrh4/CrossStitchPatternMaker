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
        private readonly Dictionary<Color, FlossInfo> _color2Floss;
        private readonly bool _isColored;
        private const decimal PixW= 50.0M;
        private const decimal PixH= 60.0M;
        private static string _outputPath;
        private const string TableStart = "<table><thead><tr><th>Floss</th><th>Symbol</th></tr></thead><tbody>";

        public HtmlWriter(Bitmap b, Dictionary<Color, FlossInfo> c2F, bool colored, string outDir)
        {
            ChunkifyImage(b);

            _isColored = colored;
            _color2Floss = c2F;

            var pictureFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            _outputPath = string.IsNullOrEmpty(outDir) 
                ? Path.Combine(pictureFolder, "MyCrossStitch") 
                : Path.Combine(pictureFolder, outDir);

            if (!Directory.Exists(_outputPath)) { Directory.CreateDirectory(_outputPath); }
        }

        private void ChunkifyImage(Bitmap b)
        {
            var chunksX = (int)Math.Ceiling(b.Width / PixW);
            var chunksY = (int)Math.Ceiling(b.Height / PixH);
            for (var y = 0; y < chunksY; y++)
            {
                for (var x = 0; x < chunksX; x++)
                {
                    var wLeft = b.Width - (x + 1) * PixW;
                    var hLeft = b.Height - (y + 1) * PixH;
                    var width = (int)(wLeft < 0 ? wLeft + PixW: PixW);
                    var height = (int)(hLeft < 0 ? hLeft + PixH: PixH);
                    _imageChunks.Add(b.Clone(new Rectangle(x * (int)PixW, y * (int)PixH, width, height), PixelFormat.Format32bppArgb));
                }
            }

        }

        private void Save(StringBuilder outString, string filename="chunk",int fileNo=-1)
        {
            File.WriteAllText(
                filename == "chunk"
                    ? Path.Combine(_outputPath, $"chunk{fileNo}.html")
                    : Path.Combine(_outputPath, filename), outString.ToString());
            Process.Start(_outputPath);
        }

        private void GenerateHtmlLegend(IReadOnlyList<FlossInfo> flossInfos)
        {
            var tableEnded = false;
            var htmlString = new StringBuilder(File.ReadAllText("legend_template.html"));
            htmlString.AppendLine(TableStart);
            for (var i=0; i < flossInfos.Count;i++)
            {
                tableEnded = false;

                htmlString.AppendLine($"<tr><td>{flossInfos[i].FlossId}</td><td>{flossInfos[i].FlossSymbol}</td></tr>");

                if (i > 0 && (i+1) % 50 == 0)
                {
                    htmlString.AppendLine("</tbody></table>");
                    htmlString.AppendLine("<table><thead><tr><th>Floss</th><th>Symbol</th></tr></thead><tbody>");
                    tableEnded = true;
                }
            }

            if (!tableEnded)
            {
                htmlString.AppendLine("</tbody></table>");
            }
            Save(htmlString,"legend.html");
        }
        public void GenerateHtml()
        {
            var distinctFlossesInImage = GenerateCrossStitch();
            GenerateHtmlLegend(distinctFlossesInImage.ToList());
        }

        public HashSet<FlossInfo> GenerateCrossStitch()
        {
            var chunkCount = 0;
            var flossesInImage = new HashSet<FlossInfo>();
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
                        if (!_color2Floss.TryGetValue(pC, out var flossInfo))
                        {
                            throw new Exception("Floss dictionary invalid.");
                        }

                        flossesInImage.Add(flossInfo);
                        if (x > 0 && x % 50 == 0)
                        {
                            htmlString.AppendLine($"<div class='u'{style}></div><br/>");
                        }
                        if (x >= 5 && x % 5 == 0)
                        {
                            htmlString.AppendLine($"<div class='w x'{style}>{flossInfo.FlossSymbol}</div>");
                        }
                        else
                        {
                            htmlString.AppendLine($"<div class='x'{style}>{flossInfo.FlossSymbol}</div>");
                        }
                    }
                    if (y > 0 && (y + 1) % 5 == 0)
                    {
                        htmlString.AppendLine($"<div class='u'>{y + 1}</div>");
                    }

                    htmlString.AppendLine("</div>");

                }
                htmlString.AppendLine("</body></html>");
                Save(htmlString,fileNo:chunkCount);
                chunkCount++;
            }
            return flossesInImage;
        }
    }
}
