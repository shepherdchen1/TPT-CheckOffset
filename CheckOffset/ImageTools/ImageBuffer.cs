using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TN.Tools.Debug;

namespace TN.ImageTools
{
    public static class ImageBuffer
    {
        public static bool GetBuffer(Image bmp)
        {
            try
            {

            }
            catch(Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                               , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }

            return false;
        }

        public static unsafe byte Get_Pixel(BitmapData? bmpdata, byte* ptr_buffer, int x, int y)
        {
            if (bmpdata == null)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                         , $"bmpdata is null");
                return 0;
            }

            int pixel_size = bmpdata.Stride / bmpdata.Width;
            return ptr_buffer[y * bmpdata.Stride + x * pixel_size + pixel_size - 1];
        }
    }
}
