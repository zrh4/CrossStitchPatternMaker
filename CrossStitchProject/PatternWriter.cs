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
using CrossStitchPatternMaker.Models;
using System.Threading.Tasks;

namespace CrossStitchPatternMaker
{
    public class PatternWriter
    {
        public List<List<Bitmap>> _imageChunks = new List<List<Bitmap>>();
        public List<string> PatternChunks;
        public string PatternLegend;
        private readonly Dictionary<Color, Floss> _color2Floss;
        private readonly bool _isColorPattern;
        private const decimal DefaultChunkWidth = 50.0M;
        private const decimal DefaultChunkHeight = 60.0M;
        private static string _outputPath;
        const string CrossStitchFolderName = "Cross Stitch Patterns";

        public PatternWriter(Bitmap b, Dictionary<Color, Floss> c2F, bool colored, string outDir)
        {
            PatternChunks = new List<string>();
            ChunkifyImage(b);

            _isColorPattern = colored;
            _color2Floss = c2F;

            var pictureFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var crossStitchFolder = Path.Combine(pictureFolder, CrossStitchFolderName);
            if (string.IsNullOrEmpty(outDir)) { outDir = "MyCrossStitchPattern"; }
            _outputPath = Path.Combine(crossStitchFolder, outDir);

            if (!Directory.Exists(crossStitchFolder)) { Directory.CreateDirectory(crossStitchFolder); }
            if (!Directory.Exists(_outputPath)) { Directory.CreateDirectory(_outputPath); }
        }
        public void Build()
        {
            BuildCrossStitchPattern();
            BuildLegend();
        }
        public void Save()
        {
            for (var i = 0; i < PatternChunks.Count; i++)
            {
                Save(PatternChunks[i], $"chunk{i + 1}.html");
            }
            Save(PatternLegend, "legend.html");
            Process.Start(_outputPath);
        }

        private void ChunkifyImage(Bitmap b)
        {
            var numHorizontalChunks = (int)Math.Ceiling(b.Width / DefaultChunkWidth);
            var numVerticalChunks = (int)Math.Ceiling(b.Height / DefaultChunkHeight);
            for (var y = 0; y < numVerticalChunks; y++)
            {
                _imageChunks.Add(new List<Bitmap>());
                for (var x = 0; x < numHorizontalChunks; x++)
                {
                    var widthLeft = b.Width - (x + 1) * DefaultChunkWidth;
                    var heightLeft = b.Height - (y + 1) * DefaultChunkHeight;
                    var curChunkWidth = (int)(widthLeft < 0 ? widthLeft + DefaultChunkWidth : DefaultChunkWidth);
                    var curChunkHeight = (int)(heightLeft < 0 ? heightLeft + DefaultChunkHeight : DefaultChunkHeight);
                    var chunk = b.Clone(
                        new Rectangle(x * (int)DefaultChunkWidth, y * (int)DefaultChunkHeight, curChunkWidth,
                            curChunkHeight), PixelFormat.Format32bppArgb);
                    _imageChunks[y].Add(chunk);
                }
            }
        }
        private void Save(string outString, string filename)
        {
            File.WriteAllText(Path.Combine(_outputPath, filename), outString);
        }

        private void BuildLegend()
        {
            var model = _color2Floss.Values.OrderBy(x => x.DMC).ToList();
            var template = File.ReadAllText("Templates/legend_razor.html");
            var result = Engine.Razor.RunCompile(template, "legendKey", null, model);
            PatternLegend = result;
        }
        private void BuildCrossStitchPattern()
        {
            for (var y = 0; y < _imageChunks.Count; y++)
                for (var x = 0; x < _imageChunks[y].Count; x++)
                {
                    var startY = y * (int)DefaultChunkHeight;
                    var startX = x * (int)DefaultChunkWidth;
                    var template = File.ReadAllText("Templates/pattern_razor.html");
                    var model = new PatternChunk
                    {
                        Chunk = _imageChunks[y][x],
                        ColorToFlossDict = _color2Floss,
                        IsColorPattern = _isColorPattern,
                        StartX = startX,
                        StartY = startY
                    };
                    var result = Engine.Razor.RunCompile(template, "patternKey", null, model);
                    PatternChunks.Add(result);
                }
        }
    }
}
