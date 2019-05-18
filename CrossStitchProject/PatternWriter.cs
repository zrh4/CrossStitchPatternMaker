using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using RazorEngine;
using RazorEngine.Templating;
using System.Linq;
using RazorEngine.Configuration;
using CrossStitchProject.Models;
using System.Threading.Tasks;

namespace CrossStitchProject
{
    public class PatternWriter
    {
        private readonly List<Bitmap> _imageChunks = new List<Bitmap>();
        private readonly Dictionary<Color, Floss> _color2Floss;
        private readonly bool _isColored;
        private const decimal DefaultChunkWidth = 50.0M;
        private const decimal DefaultChunkHeight = 60.0M;
        private static string _outputPath;
        public List<string> PatternChunks;
        public string PatternLegend;

        public PatternWriter(Bitmap b, Dictionary<Color, Floss> c2F, bool colored, string outDir)
        {
            PatternChunks = new List<string>();
            ChunkifyImage(b);

            _isColored = colored;
            _color2Floss = c2F;

            var pictureFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            if (string.IsNullOrEmpty(outDir)) { outDir = "MyCrossStitchPattern"; }
            _outputPath = Path.Combine(pictureFolder, outDir);

            if (!Directory.Exists(_outputPath)) { Directory.CreateDirectory(_outputPath); }
        }
        public void BuildAndSavePattern()
        {
            var task = Task.Factory.StartNew(() => GetDistinctFlossesInImage());
            BuildCrossStitchPattern();
            GenerateHtmlLegend(task.Result);
            for (var i = 0; i < PatternChunks.Count; i++)
            {
                Save(PatternChunks[i],$"chunk{i+1}.html");
            }
            Save(PatternLegend,"legend.html");
        }

        private void ChunkifyImage(Bitmap b)
        {
            var numHorizontalChunks = (int)Math.Ceiling(b.Width / DefaultChunkWidth);
            var numVerticalChunks = (int)Math.Ceiling(b.Height / DefaultChunkHeight);
            for (var y = 0; y < numVerticalChunks;  y++)
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
        private void Save(string outString, string filename)
        {
            File.WriteAllText(Path.Combine(_outputPath, filename), outString);
            Process.Start(_outputPath);
        }

        private void GenerateHtmlLegend(HashSet<Floss> flosses)
        {
            var model = flosses.ToList();
            var template = File.ReadAllText("Templates/legend_razor.html");
            var result = Engine.Razor.RunCompile(template,"legendKey", null, model);
            PatternLegend = result;
        }
        private void BuildCrossStitchPattern()
        {
            foreach (var chunk in _imageChunks)
            {
                var template = File.ReadAllText("Templates/pattern_razor.html");
                var model = new PatternChunk { Chunk = chunk, ColorToFlossDict = _color2Floss, IsColorPattern = _isColored };
                //Engine.Razor = RazorEngineService.Create(new TemplateServiceConfiguration { Debug = true });
                var result = Engine.Razor.RunCompile(template,"patternKey", null, model);
                PatternChunks.Add(result);
            }
        }
        private HashSet<Floss> GetDistinctFlossesInImage()
        {
            var flossesUsed = new HashSet<Floss>();
            foreach (var chunk in _imageChunks)
                for (var y = 0; y < chunk.Height; y++)
                    for (var x = 0; x < chunk.Width; x++)
                    {
                        flossesUsed.Add(_color2Floss[chunk.GetPixel(x, y)]);
                    }
            return flossesUsed;
        }
    }
}
