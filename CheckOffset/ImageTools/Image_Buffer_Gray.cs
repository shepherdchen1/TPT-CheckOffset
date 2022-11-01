using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using System.Drawing;
using System.Drawing.Imaging;
using TN.Tools.Debug;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.CompilerServices;

namespace TN.ImageTools
{
    internal class Image_Buffer_Gray
    {
        public static bool GetBuffer(Bitmap? bmp, ref System.Drawing.Imaging.BitmapData? bmpData)
        {
            try
            {
                if ( null == bmp)
                {
                    Log_Utl.Log_Event(Event_Level.Error, MethodBase.GetCurrentMethod()?.Name
                        , $"bmp is null");
                    return false;
                }

                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                bmpData = bmp.LockBits(rect
                                , System.Drawing.Imaging.ImageLockMode.ReadOnly
                                , bmp.PixelFormat);

                return true;
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, MethodBase.GetCurrentMethod()?.Name
                               , $"Exception catched: error:{ex.Message}");

                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }

            return false;
        }

        public static bool ReleaseBuffer(Bitmap? bmp, ref BitmapData? bmpData)
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
                               , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }

            return false;
        }

        public static unsafe byte* Get_Pointer(BitmapData? bmpdata, byte *ptr_buffer, int x, int y)
        {
            if (bmpdata == null)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                         , $"bmpdata is null");
                return null;
            }

            int pixel_size = bmpdata.Stride / bmpdata.Width;
            return ptr_buffer + y * bmpdata.Stride + x * pixel_size;
        }

        public static unsafe byte* Get_Pointer(BitmapData bmpdata, IntPtr ptr_buffer, int x, int y)
        {
            byte * ptr = (byte*) ptr_buffer.ToPointer();
            return ptr + y * bmpdata.Stride + x;
        }

        public static object? Clone_Bmp_2_2DArray(Bitmap bmp)
        {
            try
            {
                if (null == bmp || bmp.Width <= 0 || bmp.Height <= 0)
                {
                    return null;
                }

                if (PixelFormat.Format8bppIndexed == bmp.PixelFormat)
                {
                    BitmapData? bmp_data = null;
                    Image_Buffer_Gray.GetBuffer(bmp, ref bmp_data);
                    if (bmp_data == null)
                        return null;

                    byte[] bytes = new byte[bmp_data.Height * bmp_data.Stride];
                    Marshal.Copy(bmp_data.Scan0, bytes, 0, bytes.Length);

                    byte[,] buffer = new byte[bmp.Height, bmp.Width];
                    for (int y = 0; y < bmp.Height; y++)
                        Buffer.BlockCopy(bytes, y * bmp_data.Stride, buffer, y * bmp_data.Stride, bmp_data.Stride);

                    Image_Buffer_Gray.ReleaseBuffer(bmp, ref bmp_data);
                    return buffer;
                }
                else if (PixelFormat.Format32bppArgb == bmp.PixelFormat)
                {
                    BitmapData? bmp_data = null;
                    Image_Buffer_Gray.GetBuffer(bmp, ref bmp_data);
                    if (bmp_data == null)
                        return null;

                    byte[] bytes = new byte[bmp_data.Height * bmp_data.Stride];
                    Marshal.Copy(bmp_data.Scan0, bytes, 0, bytes.Length);

                    byte[,] buffer = new byte[bmp.Height, bmp.Width];
                    int pixel_size = bmp_data.Stride / bmp_data.Width;
                    unsafe
                    {
                        for (int y = 0; y < bmp.Height; y++)
                        {
                            IntPtr row = (IntPtr)(bmp_data.Scan0);
                            row += y * bmp_data.Stride;
                            for (int x = 0; x < bmp.Width; x++)
                            {
                                byte clr = (* (byte*) (row + x * pixel_size));
                                buffer[y, x] = *( (byte*) (row + x * pixel_size));
                            }
                        }
                    }
                        //Buffer.BlockCopy(bytes, y * bmp_data.Stride, buffer, y * bmp_data.Stride, bmp_data.Stride);

                    Image_Buffer_Gray.ReleaseBuffer(bmp, ref bmp_data);
                    return buffer;
                }

                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                    , $"pixelformat:{bmp.PixelFormat} not support" );
                return null;
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
    }
}
