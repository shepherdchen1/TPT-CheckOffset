using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using System.Drawing;
using System.Drawing.Imaging;
using TN.Tools.Debug;

namespace TN.ImageTools
{
    internal class Image_Buffer_Gray
    {
        public static bool GetBuffer(Bitmap bmp, ref System.Drawing.Imaging.BitmapData? bmpData)
        {
            try
            {
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                bmpData = bmp.LockBits(rect
                                , System.Drawing.Imaging.ImageLockMode.ReadOnly
                                , bmp.PixelFormat);

                return true;
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, MethodBase.GetCurrentMethod()?.Name
                                , string.Format("Exception catched: error:{0}", ex.Message));

                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }

            return false;
        }

        public static bool ReleaseBuffer(Bitmap? bmp, BitmapData? bmpData)
        {
            try
            {
                if (bmp == null)
                    return true;

                if (bmpData == null)
                    return true;

                bmp.UnlockBits(bmpData);
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                       , string.Format("Exception catched: error:{0}", ex.Message));
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }

            return false;
        }

        public static unsafe byte* Get_Pointer(BitmapData bmpdata, byte *ptr_buffer, int x, int y)
        {
            return ptr_buffer + y * bmpdata.Stride + x;
        }
    }
}
