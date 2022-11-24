using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TN.Tools.Debug;
using OpenCvSharp;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using TN.ImageTools;

namespace CheckOffset.ImageTools
{
    internal class OpenCVMatTool
    {

        public static System.Drawing.Point ToSystemPoint(OpenCvSharp.Point cv_pt)
        {
            return new System.Drawing.Point(cv_pt.X, cv_pt.Y);
        }

        public static OpenCvSharp.Point ToOpenCvPoint(System.Drawing.Point drawing_pt)
        {
            return new OpenCvSharp.Point(drawing_pt.X, drawing_pt.Y);
        }

        public static System.Drawing.Rectangle ToSystemRect(OpenCvSharp.Rect cv_rt)
        {
            return new System.Drawing.Rectangle(cv_rt.X, cv_rt.Y, cv_rt.Width, cv_rt.Height);
        }

        public static OpenCvSharp.Rect ToOpenCvRect(System.Drawing.Rectangle drawing_rt)
        {
            return new OpenCvSharp.Rect(drawing_rt.X, drawing_rt.Y, drawing_rt.Width, drawing_rt.Height);
        }

        public static Mat<byte>? Clone_Bmp_2_Mat(Bitmap bmp)
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

                    int pixel_size = bmp_data.Stride / bmp_data.Width;

                    //Mat<byte> mat_buffer_1d = new Mat<byte>(bmp_data.Height, bmp_data.Stride, MatType.CV_8UC1);
                    //Marshal.Copy(bmp_data.Scan0, mat_buffer_1d, 0, bmp_data.Height * bmp_data.Stride);

                    Mat<byte> mat_buffer = new Mat<byte>(bmp_data.Height, bmp_data.Stride);
                    //byte[,] buffer = new byte[bmp.Height, bmp.Width];
                    //unsafe
                    //{
                    //    for (int y = 0; y < bmp.Height; y++)
                    //    {
                    //        Buffer.MemoryCopy((void*) (bmp_data.Scan0 + y * bmp_data.Stride), (void*)mat_buffer.Get<byte>(y*bmp_data.Stride), y * bmp_data.Stride, bmp_data.Stride);
                    //        //Marshal.Copy(bmp_data.Scan0, mat_buffer, y * bmp_data.Stride, bmp_data.Stride);
                    //        //Buffer.BlockCopy(bmp_data.Scan0, y * bmp_data.Stride, mat_buffer, y * bmp_data.Stride, bmp_data.Stride);
                    //    }
                    //}
                    unsafe
                    {
                        for (int y = 0; y < bmp.Height; y++)
                        {
                            IntPtr row = (IntPtr)(bmp_data.Scan0);
                            row += y * bmp_data.Stride;
                            for (int x = 0; x < bmp.Width; x++)
                            {
                                byte clr = (*(byte*)(row + x * pixel_size));
                                mat_buffer.Set<byte>(y, x, *((byte*)(row + x * pixel_size)));
                            }
                        }
                    }

                    Image_Buffer_Gray.ReleaseBuffer(bmp, ref bmp_data);
                    return mat_buffer;
                }
                else if (PixelFormat.Format32bppArgb == bmp.PixelFormat)
                {
                    BitmapData? bmp_data = null;
                    Image_Buffer_Gray.GetBuffer(bmp, ref bmp_data);
                    if (bmp_data == null)
                        return null;

                    int pixel_size = bmp_data.Stride / bmp_data.Width;
                    //Mat<byte> mat_buffer_1d = new Mat<byte>(bmp_data.Height, bmp_data.Stride, MatType.CV_8UC1);
                    //Marshal.Copy(bmp_data.Scan0, mat_buffer_1d, 0, bmp_data.Height * bmp_data.Stride);

                    Mat<byte> mat_buffer = new Mat<byte>(bmp_data.Height, bmp_data.Width);
                    //byte[,] buffer = new byte[bmp.Height, bmp.Width];
                    unsafe
                    {
                        for (int y = 0; y < bmp.Height; y++)
                        {
                            IntPtr row = (IntPtr)(bmp_data.Scan0);
                            row += y * bmp_data.Stride;
                            for (int x = 0; x < bmp.Width; x++)
                            {
                                byte clr = (*(byte*)(row + x * pixel_size));
                                mat_buffer.Set<byte>(y, x, *((byte*)(row + x * pixel_size)) );
                            }
                        }
                    }
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        //Marshal.Copy(bmp_data.Scan0, mat_buffer, y * bmp_data.Stride, bmp_data.Stride);
                        //Buffer.BlockCopy(bmp_data.Scan0, y * bmp_data.Stride, mat_buffer, y * bmp_data.Stride, bmp_data.Stride);
                    }

                    Image_Buffer_Gray.ReleaseBuffer(bmp, ref bmp_data);
                    return mat_buffer;
                }

                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                    , $"pixelformat:{bmp.PixelFormat} not support");
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

        public static Mat<byte> Extract_Buffer(Mat<byte> src_buffer, System.Drawing.Rectangle rt_select)
        {
            try
            {
                System.Drawing.Rectangle rt_bound = new System.Drawing.Rectangle(0, 0, src_buffer.Width, src_buffer.Height);
                System.Drawing.Rectangle rt_intersect = System.Drawing.Rectangle.Intersect(rt_bound, rt_select);

                rt_intersect.Width = (rt_intersect.Width + 3) / 4 * 4;
                if (rt_intersect.Width <= 0 || rt_intersect.Height <= 0)
                {
                    Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                       , $"rt_intersect is 0 for rt_select:{rt_intersect}");
                    return new Mat<byte>(0, 0);
                }

                Mat mat_clone = src_buffer.Clone(new OpenCvSharp.Rect(rt_intersect.X, rt_intersect.Y, rt_intersect.Width, rt_intersect.Height));
                Mat<byte> mat_dest = new Mat<byte>(mat_clone.Height, mat_clone.Width);
                //foreach (var mat in mat_clone)
                {
                    mat_clone.CopyTo(mat_dest);
                }

                return mat_dest;
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                   , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }

            return new Mat<byte>(0, 0);
        }

    } // end of     internal class OpenCVMatTool
} // end of namespace CheckOffset.ImageTools
