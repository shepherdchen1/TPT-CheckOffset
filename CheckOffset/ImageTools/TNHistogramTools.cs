using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenCvSharp;
using TN.Tools.Debug;

namespace CheckOffset.ImageTools
{
    public class TNHistogramTools
    {
        public static Mat[] Get_Histogram(Mat buffer)
        {
            try
            {
                Mat[] mats = Cv2.Split(buffer);
                Mat[] mats_b = new Mat[] { mats[0] };
                Mat[] mats_g = new Mat[] { mats[1] };
                Mat[] mats_r = new Mat[] { mats[2] };
                Mat[] hist = new Mat[] { new Mat(), new Mat(), new Mat() };

                int[] channels = new int[] { 0 };
                int[] histsize = new int[] { 256 };
                Rangef[] range = new Rangef[1];
                range[0] = new Rangef(0.0f, 256.0f);

                Mat mask = new Mat();

                Cv2.CalcHist(mats_b, channels, mask, hist[0], 1, histsize, range);
                Cv2.CalcHist(mats_g, channels, mask, hist[1], 1, histsize, range);
                Cv2.CalcHist(mats_r, channels, mask, hist[2], 1, histsize, range);

                return hist;
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                   , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }

            return null;
        }

        public static bool Find_Normal_Distribution_Range(Mat histogram, int min_threshold, ref int range_start, ref int range_end)
        {
            range_start = int.MaxValue;
            range_end   = int.MinValue;
            try
            {
                double minVal, maxVal;
                Cv2.MinMaxLoc(histogram, out minVal, out maxVal);

                int gray_level_max = 0;
                for (int j = 0; j < histogram.Rows; ++j)
                {
                    int num = (int)histogram.Get<float>(j);
                    if (num != maxVal)
                        continue;

                    gray_level_max = j;
                    break;
                }

                range_start = gray_level_max;
                range_end = gray_level_max;

                for (int j = gray_level_max - 1; j >= 0; j--)
                {
                    int num = (int)histogram.Get<float>(j);
                    if (num >= min_threshold)
                    {
                        range_start = j;
                        continue;
                    }

                    break;
                }

                for (int j = gray_level_max + 1; j < histogram.Rows; j++)
                {
                    int num = (int)histogram.Get<float>(j);
                    if (num >= min_threshold)
                    {
                        range_end = j;
                        continue;
                    }

                    break;
                }



                //if (num <= 0)
                //        continue;

                //    if (num < min_threshold)
                //        continue;

                //    if (j < range_start)
                //        range_start = j;

                //    if ( j > range_end )
                //        range_end = j;

                return true;
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                   , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }

            return false;
        }
    }
}
