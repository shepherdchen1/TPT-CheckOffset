using System;
using System.Collections.Generic;
using System.Linq;
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

        public static bool Mat_Fill(Mat<byte> buffer, OpenCvSharp.Rect rt_fill, byte fill_val)
        {
            try
            {
                OpenCvSharp.Rect rt_buffer = new OpenCvSharp.Rect(0, 0, buffer.Width, buffer.Height);
                OpenCvSharp.Rect rt_intersect = rt_buffer.Intersect(rt_fill);

                if ( rt_intersect.Width <= 0 || rt_intersect.Height <= 0 )
                {
                    Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                       , $"rt_intersect: W:{rt_intersect.Width}, H:{rt_intersect.Height}, input rt_fill:{rt_fill}");
                    return false;
                }

                Mat<byte> init_buf = new Mat<byte>(buffer.Width, buffer.Height, fill_val );
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
    }
}
