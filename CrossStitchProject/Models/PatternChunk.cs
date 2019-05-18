using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossStitchProject.Models
{
    public class PatternChunk
    {
        public bool IsColorPattern { get; set; }
        public Bitmap Chunk { get; set; }
        public Dictionary<Color, Floss> ColorToFlossDict { get; set; }
    }
}
