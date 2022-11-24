using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using OpenCvSharp;
using TN.Tools.Debug;

namespace CheckOffset.ImageTools
{
    internal class Insp_Base_Tools
    {
        public static Rectangle To_System_Rect(Rect rt)
        {
            return new Rectangle(rt.X, rt.Y, rt.Width, rt.Height);
        }

        public static System.Drawing.Point To_System_Point(OpenCvSharp.Point pt)
        {
            return new System.Drawing.Point(pt.X, pt.Y);
        }

        public static bool Mat_Fill(Mat<byte> buffer, OpenCvSharp.Rect rt_fill, byte fill_val)
        {
            try
            {
                OpenCvSharp.Rect rt_buffer = new OpenCvSharp.Rect(0, 0, buffer.Width, buffer.Height);
                OpenCvSharp.Rect rt_intersect = rt_buffer.Intersect(rt_fill);

                if (rt_intersect.Width <= 0 || rt_intersect.Height <= 0)
                {
                    Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                       , $"rt_intersect: W:{rt_intersect.Width}, H:{rt_intersect.Height}, input rt_fill:{rt_fill}");
                    return false;
                }

                Mat<byte> init_buf = new Mat<byte>(buffer.Width, buffer.Height, fill_val);
                buffer[rt_fill.Y, rt_fill.Bottom, rt_fill.X, rt_fill.Right] = init_buf;

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

        public static double AngleBetween(OpenCvSharp.Point p1, OpenCvSharp.Point p2)
        {
            //double len1 = System.Math.Sqrt(v1.X * v1.X + v1.Y * v1.Y);
            //double len2 = System.Math.Sqrt(v2.X * v2.X + v2.Y * v2.Y);

            //double dot = v1.X * v2.X + v1.Y * v2.Y;

            //double a = dot / (len1 * len2);

            //if (a >= 1.0)
            //    return 0.0;
            //else if (a <= -1.0)
            //{
            //    //return CvConst.CV_PI;
            //    return 0.0;
            //}
            //else
            //    return System.Math.Acos(a); // 0..PI
            double deltaY = (p1.Y - p2.Y);
            double deltaX = (p2.X - p1.X);
            //double result = Math.toDegrees(Math.Atan2(deltaY, deltaX));
            double result = Math.Atan2(deltaY, deltaX) * 180 / Math.PI;
            return (result < 0) ? (360d + result) : result;
        }
    }   // end of Insp_Base_Tools
} // end of CheckOffset.ImageTools

