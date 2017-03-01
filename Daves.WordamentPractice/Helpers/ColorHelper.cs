using System.Collections.Generic;
using System.Drawing;

namespace Daves.WordamentPractice.Helpers
{
    public static class ColorHelper
    {
        // 1 step, return start. 2 steps, return start then end. 3 steps:
        // [ | ], return start, then halfway between start and end, then end. And so on...
        public static IEnumerable<Color> GetColorGradient(Color start, Color end, int steps)
        {
            if (steps == 1)
            {
                yield return start;
                yield break;
            }

            int rDifference = end.R - start.R;
            int gDifference = end.G - start.G;
            int bDifference = end.B - start.B;

            // So for the [ | ] 3 step example, this yields:
            // start + (end - start) * 0/2, start + (end - start) * 1/2, start + (end - start) * 2/2.
            for (int s = 0; s < steps; ++s)
            {
                yield return Color.FromArgb(
                    start.R + (rDifference * s) / (steps - 1),
                    start.G + (gDifference * s) / (steps - 1),
                    start.B + (bDifference * s) / (steps - 1));
            }
        }
    }
}
