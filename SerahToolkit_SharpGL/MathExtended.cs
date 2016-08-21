using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SerahToolkit_SharpGL
{
    static class MathExtended
    {
        public static int Clamp(int value, int maxValue = int.MaxValue, int minValue = int.MinValue)
        {
            if (value > maxValue)
                return maxValue;
            if (value < minValue)
                return minValue;
            return value;
        }

        public static uint TotalLength(string[] buffer)
        {
            uint size = 0;
            for (int i = 0; i != buffer.Length; i++)
                size += (uint)buffer[i].Length;
            return size;
        }
    }
}
