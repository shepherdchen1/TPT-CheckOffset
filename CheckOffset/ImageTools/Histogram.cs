using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CheckOffset.ImageTools
{
    public class Histogram
    {
        public int[] Histogram_Result;

        public int[] Histogram_Range; // 

        public int Max_Histogram_Val;

        public Histogram()
        {
            Reset();
        }
        public bool Calc_Histogram(int[,] gray_levels)
        {
            Reset();
            Init_Size(gray_levels);

            for (int x = 0; x < gray_levels.GetLength(0); x++)
            {
                for (int y = 0; y < gray_levels.GetLength(1); y++)
                {
                    Histogram_Result[gray_levels[x, y]]++;
                }
            }

            int max_value = 0;
            for( int i = 0; i < Histogram_Result.Length; i++)
            {
                if (max_value > Histogram_Result[i])
                    continue;

                max_value = Math.Max(max_value, Histogram_Result[i]);
                Max_Histogram_Val = i;
            }

            //List<int> temp_histogram = Histogram_Result.ToList<int>();
            //temp_histogram.Sort((data1, data2) =>
            //{
            //    if (data1 > data2)
            //        return -1;
            //    if (data1 == data2)
            //        return 0;

            //    return 1;
            //});

            //Histogram_Range = temp_histogram.ToArray();

            return true;
        }

        private void Reset()
        {
            Histogram_Result = null;
            Histogram_Range = null;
            Max_Histogram_Val = 0;
        }

        private void Init_Size(int[,] gray_levels)
        {
            int max_gray_level = 0;
            for (int x = 0; x < gray_levels.GetLength(0); x++)
            {
                for (int y = 0; y < gray_levels.GetLength(1); y++)
                {
                    max_gray_level = Math.Max(max_gray_level, gray_levels[x, y]);
                }
            }
            Histogram_Result = new int[max_gray_level + 1];
            Histogram_Range = new int[max_gray_level + 1];
        }
    }
}
