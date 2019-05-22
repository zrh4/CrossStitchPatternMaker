using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossStitchProject.Helpers
{
    public class ImageHelper
    {
        public static HashSet<Color> GetColors(Bitmap b)
        {
            var colorSet = new HashSet<Color>();
            for (var y = 0; y < b.Height; y++)
                for (var x = 0; x < b.Width; x++)
                    colorSet.Add(b.GetPixel(x, y));
            return colorSet;
        }
    }
}
