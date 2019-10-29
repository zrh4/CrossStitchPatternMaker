using System;
using System.Collections.Generic;
using System.Drawing;

namespace CrossStitchPatternMaker.Helpers
{
    public static class ColorMath
    {
        public static Color GetClosestColor(IEnumerable<Color> acceptableColors, Color testColor)
        {
            var min = double.MaxValue;
            var minColor = Color.Black;
            foreach(var color in acceptableColors)
            {
                var dist = GetColorDistance(color, testColor);
                if (!(dist < min)) continue;
                min = dist;
                minColor = color;
            }
            return minColor;
        }
        public static double GetColorDistance(Color c1, Color c2)
        {
            return Math.Sqrt(Math.Pow(c1.R - c2.R, 2.0d) + Math.Pow(c1.G - c2.G, 2.0d) + Math.Pow(c1.B - c2.B, 2.0d));
        }
        public static void FloydSteinberg(Bitmap b,Color orig, Color alt, int x, int y)
        {
            var width = b.Width;
            var height = b.Height;
            var redError = orig.R - alt.R;
            var bluError = orig.B - alt.B;
            var greError = orig.G - alt.G;
            Color pixel;
            Color newColor;
            if (x + 1 < width)
            {
                pixel = b.GetPixel(x + 1, y);
                newColor = FloydSteinbergErrorColor(pixel, redError, greError, bluError, 7);
                b.SetPixel(x + 1, y, newColor);
            }

            if (y + 1 >= height) return;
            pixel = b.GetPixel(x, y + 1);
            newColor = FloydSteinbergErrorColor(pixel, redError, greError, bluError, 5);
            b.SetPixel(x, y + 1, newColor);

            if (x - 1 > 0)
            {
                pixel = b.GetPixel(x - 1, y + 1);
                newColor = FloydSteinbergErrorColor(pixel, redError, greError, bluError, 3);
                b.SetPixel(x - 1, y + 1, newColor);
            }

            if (x + 1 >= width) return;
            pixel = b.GetPixel(x + 1, y + 1);
            newColor = FloydSteinbergErrorColor(pixel, redError, greError, bluError, 1);
            b.SetPixel(x + 1, y + 1, newColor);
        }
        private static byte PlusTruncate(byte a, int b)
        {
            if ((a & 0xff) + b < 0)
                return 0;
            if ((a & 0xff) + b > 255)
                return 255;
            return (byte)(a + b);
        }

        private static Color FloydSteinbergErrorColor(Color color, int rE, int gE, int bE, int denominator)
        {
            return Color.FromArgb(
               PlusTruncate(color.R, (rE * denominator) >> 4),
               PlusTruncate(color.G, (gE * denominator) >> 4),
               PlusTruncate(color.B, (bE * denominator) >> 4));
        }
    }
}
