using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TN.Tools.Debug;

namespace TN.ImageTools
{
    public static class Image_Binary
    {
        public static bool Binary_Image(Bitmap src_bmp, int threshold, out Bitmap? bmp_binary)
        {
            bmp_binary = null;
            bool res = false;

            System.Drawing.Imaging.BitmapData? src_bmp_data = null;
            System.Drawing.Imaging.BitmapData? dest_bmp_data = null;
            try
            {
                if (null == src_bmp)
                    return false;

                bmp_binary = new Bitmap(src_bmp.Width, src_bmp.Height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

                Image_Buffer_Gray.GetBuffer(src_bmp, ref src_bmp_data);
                Image_Buffer_Gray.GetBuffer(bmp_binary, ref dest_bmp_data);

                if (null == src_bmp_data)
                    return false;

                if (null == dest_bmp_data)
                    return false;

                int pixel_size_src  = src_bmp_data.Stride / src_bmp_data.Width;
                int pixel_size_dest = dest_bmp_data.Stride / dest_bmp_data.Width;
                unsafe
                {
                    for (int y = 0; y < src_bmp_data.Height; y++)
                    {
                        byte* src_buf = (byte*) src_bmp_data.Scan0.ToPointer();
                        src_buf += y * src_bmp_data.Stride;
                        byte* dest_buf = (byte*) dest_bmp_data.Scan0.ToPointer();
                        dest_buf += y * dest_bmp_data.Stride;

                        // 1129+ get g band
                        for (int x = 0; x < src_bmp_data.Width; x++)
                        {
                            if ( src_buf[x * pixel_size_src + 1] > threshold) // +1 for g band
                                dest_buf[x * pixel_size_dest] = 255;
                            else
                                dest_buf[x * pixel_size_dest] = 0;
                        }
                    }
                }
                res = true;
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                               , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }
            finally
            {
                Image_Buffer_Gray.ReleaseBuffer(src_bmp, ref src_bmp_data);
                Image_Buffer_Gray.ReleaseBuffer(bmp_binary, ref dest_bmp_data);
            }

            return res;
        }

        public static byte Normalize(int gray_level)
        {
            if (gray_level > 255)
                return 255;

            if (gray_level < 0)
                return 0;

            return (byte) gray_level;
        }
    }
}
