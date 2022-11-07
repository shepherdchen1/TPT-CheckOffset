using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TN.ImageTools;
using TN.Tools.Debug;

namespace TN.ImageTools
{
    public partial class SubPixel
    {
        public static bool Apply_Interpolate(Bitmap bmp_src
                      , EN_SubPixel_Type subpixel_type
                      , EN_SubPixel_Num en_subpixel_num        /// 一 Pixel 拆成 subpixel_num * subpixel_num pixels
                      , out Bitmap bmp_dest                 /// sub pixel result.
                      )
        {
            bmp_dest = new Bitmap(0,0);
            bool res = false;

            BitmapData? bmp_data_src = null;
            BitmapData? bmp_data_dest = null;
            bool subpixel_failed = false;

            try
            {
                int subpixel_num = Get_Subpixel_Num(en_subpixel_num);
                if (subpixel_num <= 0)
                {
                    Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                        , $"subpixel_num <= 0 for EN_SubPixel_Num:{en_subpixel_num}");
                    return false;
                }

                if (!Image_Buffer_Gray.GetBuffer((Bitmap)bmp_src, ref bmp_data_src))
                    return false;

                if ( null == bmp_data_src)
                {
                    Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                         , $"bmp_data_src is null");
                    return false;
                }

                IntPtr ptr_buffer_src = bmp_data_src.Scan0;
                int pixel_size_src = bmp_data_src.Stride / bmp_data_src.Width;

                int[,] buffer = new int[bmp_data_src.Height * subpixel_num, bmp_data_src.Width * subpixel_num];

                bmp_dest = new Bitmap(bmp_src.Width * subpixel_num, bmp_src.Height * subpixel_num, bmp_src.PixelFormat);

                if (!Image_Buffer_Gray.GetBuffer((Bitmap)bmp_dest, ref bmp_data_dest))
                    return false;

                if ( null == bmp_data_dest)
                    return false;

                IntPtr ptr_buffer_dest = bmp_data_dest.Scan0;
                int pixel_size_dest = bmp_data_dest.Stride / bmp_data_dest.Width;

                ////////////////////////////////////////////////////
                // assign source to destion for all center pixel.
                unsafe
                {
                    for (int x = 0; x < bmp_src.Width; x++)
                    {
                        for (int y = 0; y < bmp_src.Height; y++)
                        {
                            buffer[y * subpixel_num + subpixel_num / 2, x * subpixel_num + subpixel_num / 2]
                                        = Image_Buffer_Gray.Get_Pixel(bmp_data_src, (byte*)ptr_buffer_src.ToPointer()
                                                                    , x, y);
                        }
                    }
                }

                ////////////////////////////////////////////////////
                // extend column for N * subpixel_num + subpixel_num / 2 row.
                for(int x = 0; x < bmp_src.Width; x++)
                {
                    for(int y = 0; y < bmp_src.Height; y++)
                    {
                        // update y = subpixel_num / 2, 1 * subpixel_num + subpixel_num / 2, 2 * subpixel_num + subpixel_num / 2, ...., (height - 1) * subpixel_num + subpixel_num / 2 
                        if (!Apply_Interpolate_Col(bmp_data_src, x, y, EN_SubPixel_Type.EN_SubPixel_Linear, subpixel_num
                                                , bmp_data_dest, ptr_buffer_dest, pixel_size_dest
                                                , ref buffer) )
                        {
                            subpixel_failed = true;
                            Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                                                             , $"Apply_Linear failed");
                            break;
                        }
                    }

                    if (subpixel_failed)
                        break;
                }

                for (int y = 0; y < bmp_src.Height; y++)
                {
                    if (!Apply_Interpolate_Row(bmp_data_src, y,  EN_SubPixel_Type.EN_SubPixel_Linear, subpixel_num
                                            , bmp_data_dest, ptr_buffer_dest, pixel_size_dest
                                            , ref buffer))
                    {
                        subpixel_failed = true;
                        Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                                                            , $"Apply_Linear failed");
                        break;
                    }
                }

                if (!subpixel_failed)
                {
                    if (!Assign_SubPixel_Res(buffer
                                    , bmp_data_dest, ptr_buffer_dest, pixel_size_dest))
                    {
                        subpixel_failed = true;
                        Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                                , $"Assign_SubPixel_Res failed");
                    }

                    ColorPalette pal = bmp_dest.Palette;
                    for (int i = 0; i <= 255; i++)
                    {
                        // create greyscale color table
                        pal.Entries[i] = Color.FromArgb(i, i, i);
                    }
                    bmp_dest.Palette = pal;

                    res = true;
                }
            }
            catch (Exception ex)
            {
                bmp_dest = new Bitmap(0,0);

                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                               , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }
            finally
            {
                if (null != bmp_data_src)
                    Image_Buffer_Gray.ReleaseBuffer((Bitmap)bmp_src, ref bmp_data_src);

                if (null != bmp_data_dest)
                    Image_Buffer_Gray.ReleaseBuffer((Bitmap)bmp_dest, ref bmp_data_dest);
            }

            return res;
        }

       private static bool Apply_Interpolate_Col(BitmapData bmp_data_src
                        , int x
                        , int y

                        , EN_SubPixel_Type en_subpixel_type
                        , int  subpixel_num           /// 一 Pixel 拆成 subpixel_num * subpixel_num pixels

                        , BitmapData bmp_data_interpolated_src
                        , IntPtr ptr_buffer_interpolated_src
                        , int pixel_size_interpolated_src

                        , ref int[,] sub_pixels         /// sub pixel result.
                        )
        {
            try
            {
                if (EN_SubPixel_Type.EN_SubPixel_Linear == en_subpixel_type)
                {
                    return Apply_Linear_Col(bmp_data_src, x, y
                                        , bmp_data_interpolated_src, ptr_buffer_interpolated_src, pixel_size_interpolated_src
                                        , subpixel_num, ref sub_pixels);
                }
                else
                {
                    Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                         , $"subpixel_type:{en_subpixel_type} not defined");
                }

                return false;
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

        private static bool Apply_Interpolate_Row(BitmapData bmp_data_src
                 , int y

                 , EN_SubPixel_Type en_subpixel_type
                 , int subpixel_num           /// 一 Pixel 拆成 subpixel_num * subpixel_num pixels

                 , BitmapData bmp_data_interpolated_src
                 , IntPtr ptr_buffer_interpolated_src
                 , int pixel_size_interpolated_src

                 , ref int[,] sub_pixels         /// sub pixel result.
                 )
        {
            try
            {
                if (EN_SubPixel_Type.EN_SubPixel_Linear == en_subpixel_type)
                {
                    return Apply_Linear_Row(bmp_data_src, y  
                                        , bmp_data_interpolated_src, ptr_buffer_interpolated_src, pixel_size_interpolated_src
                                        , subpixel_num, ref sub_pixels);
                }
                else
                {
                    Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                         , $"subpixel_type:{en_subpixel_type} not defined");
                }

                return false;
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

        private static bool Apply_Linear_Col(BitmapData bmp_data_src
                        , int x
                        , int y

                        , BitmapData bmp_data_interpolated_src
                        , IntPtr ptr_buffer_interpolated_src
                        , int pixel_size_interpolated_src

                        , int subpixel_num           /// 一 Pixel 拆成 subpixel_num * subpixel_num pixels
                        , ref int[,] sub_pixels         /// sub pixel result.
                        )
        {
            try
            {
                if (subpixel_num <= 0)
                {
                    Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                        , $"subpixel_num <= 0 for {subpixel_num}");
                    return false;
                }

                float f_increate_percentage = 1.0f / subpixel_num;
                unsafe
                {
                    ///////////////////////////////////
                    /// calculate y * subpixel_num. y 所在row 內插產生 x= 0-1, 3-4
                    byte gray_level_cur = Image_Binary.Normalize( sub_pixels[ y * subpixel_num + subpixel_num / 2, x * subpixel_num + subpixel_num / 2] );

                    if (0 == x)
                    {
                        for (int interpolate_x = 0; interpolate_x < subpixel_num / 2; interpolate_x++)
                        {
                            sub_pixels[y * subpixel_num + subpixel_num / 2, x * subpixel_num + interpolate_x] 
                                    = gray_level_cur;
                        }
                    }
                    else
                    {
                        byte gray_level_prev = Image_Binary.Normalize( sub_pixels[ y * subpixel_num + subpixel_num / 2, ( x - 1 ) * subpixel_num + subpixel_num / 2 ] );
                        for (int interpolate_x = 0; interpolate_x < subpixel_num / 2; interpolate_x++)
                        {
                            int dist_to_cur = subpixel_num / 2 - interpolate_x;
                            float f_prev_percentage = f_increate_percentage * dist_to_cur;   // ( x-1, y ) 百分比
                            float f_cur_percentage = f_increate_percentage * (subpixel_num - dist_to_cur);                    // ( x, y ) 百分比

                            int gray_level_interpolate = (int)(gray_level_prev * f_prev_percentage + gray_level_cur * f_cur_percentage);
                            sub_pixels[y * subpixel_num + subpixel_num / 2, x * subpixel_num + interpolate_x] = Image_Binary.Normalize(gray_level_interpolate);
                        }
                    }

                    ///////////////////////////////////
                    /// interpolate_x == subpixel_num / 2 正中央，保留
                    //sub_pixels[subpixel_num / 2, subpixel_num / 2] = gray_level_cur;

                    ///////////////////////////////////
                    /// x = ( subpixel_num / 2, subpixel_num - 1 ]
                    if (bmp_data_src.Width - 1 == x)
                    {
                        for (int interpolate_x = subpixel_num / 2 + 1; interpolate_x < subpixel_num; interpolate_x++)
                        {
                            sub_pixels[y * subpixel_num + subpixel_num / 2, x * subpixel_num + interpolate_x] = gray_level_cur;
                        }
                    }
                    else
                    {
                        byte gray_level_next = Image_Binary.Normalize( sub_pixels[y * subpixel_num + subpixel_num / 2, (x + 1) * subpixel_num + subpixel_num / 2] );
                        for (int interpolate_x = 0; interpolate_x < subpixel_num / 2; interpolate_x++)
                        {
                            int dist_to_cur = interpolate_x + 1;
                            float f_next_percentage = f_increate_percentage * dist_to_cur;   // ( x-1, y ) 百分比
                            float f_cur_percentage = f_increate_percentage * (subpixel_num - dist_to_cur);                    // ( x, y ) 百分比

                            int gray_level_interpolate = (int)(gray_level_cur * f_cur_percentage + gray_level_next * f_next_percentage);
                            sub_pixels[y * subpixel_num + subpixel_num / 2, x * subpixel_num +  subpixel_num / 2 + dist_to_cur] = Image_Binary.Normalize(gray_level_interpolate);
                        }
                    }
                }

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


        private static bool Apply_Linear_Row(BitmapData bmp_data_src
          , int y_src

          , BitmapData bmp_data_interpolated_src
          , IntPtr ptr_buffer_interpolated_src      // 從已內插過的影像取上下行 完整 X * stride - subpixelnum / 2 ~ X * stride + subpixelnum / 2
          , int pixel_size_interpolated_src

          , int subpixel_num           /// 一 Pixel 拆成 subpixel_num * subpixel_num pixels
          , ref int[,] sub_pixels         /// sub pixel result.
          )
        {
            try
            {
                if (subpixel_num <= 0)
                {
                    Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                        , $"subpixel_num <= 0 for {subpixel_num}");
                    return false;
                }

                float f_increate_percentage = 1.0f / subpixel_num;
                unsafe
                {
                    byte* buffer_interpolated_src = (byte*)ptr_buffer_interpolated_src.ToPointer();

                    ///////////////////////////////////
                    /// calculate y * subpixel_num. y 所在row 內插產生 x= 0-1

                    if (0 == y_src)
                    {
                        /////////////////////////////////////////////////////////
                        // y : [ 0, subpixel_num / 2 )       : y 方向 共  subpixel_num / 2 rows.      
                        // x : [ x_src * subpixel_num - subpixel_num / 2, x_src * subpixel_num + subpixel_num ): x 方向 共 subpixel_num pixel
                        for (int interpolate_y = 0; interpolate_y < subpixel_num / 2; interpolate_y++)
                        {
                            for (int interpolate_x = 0; interpolate_x < sub_pixels.GetLength(1); interpolate_x++)
                            {
                                sub_pixels[interpolate_y, interpolate_x] = sub_pixels[subpixel_num / 2, interpolate_x];
                            }
                        }
                    }
                    else
                    {
                        for (int interpolate_y = 0; interpolate_y < subpixel_num / 2; interpolate_y++)
                        {
                            for (int interpolate_x = 0; interpolate_x < sub_pixels.GetLength(1); interpolate_x++)
                            {
                                int x_2be_updated = interpolate_x; // x_src * subpixel_num + interpolate_x;
                                int gray_level_prev = sub_pixels[(y_src - 1) * subpixel_num + subpixel_num / 2, x_2be_updated];
                                int gray_level_cur  = sub_pixels[    y_src   * subpixel_num + subpixel_num / 2, x_2be_updated];

                                int dist_to_cur = subpixel_num / 2 - interpolate_y;
                                float f_prev_percentage = f_increate_percentage * dist_to_cur;   // ( x-1, y ) 百分比
                                float f_cur_percentage = f_increate_percentage * (subpixel_num - dist_to_cur);                    // ( x, y ) 百分比

                                int gray_level_interpolate = (int)( gray_level_prev * f_prev_percentage + gray_level_cur * f_cur_percentage);
                                sub_pixels[y_src * subpixel_num + interpolate_y, interpolate_x] = Image_Binary.Normalize(gray_level_interpolate);
                            }
                        }
                    }

                    ///////////////////////////////////
                    /// interpolate_x == subpixel_num / 2 正中央，Apply_Linear_Row 已填

                    ///////////////////////////////////
                    /// x = ( subpixel_num / 2, subpixel_num - 1 ]
                    if (bmp_data_src.Height - 1 == y_src)
                    {
                        /////////////////////////////////////////////////////////
                        // y : [ subpixel_num / 2 + 1, subpixel_num - 1 )       : y 方向 共  subpixel_num / 2 rows.      
                        // x : [ x_src * subpixel_num - subpixel_num / 2, x_src * subpixel_num + subpixel_num ): x 方向 共 subpixel_num pixel
                        for (int interpolate_y = subpixel_num / 2 + 1; interpolate_y < subpixel_num; interpolate_y++)
                        {
                            for (int interpolate_x = 0; interpolate_x < sub_pixels.GetLength(1); interpolate_x++)
                            {
                                sub_pixels[y_src * subpixel_num + interpolate_y, interpolate_x]
                                    = sub_pixels[y_src * subpixel_num + subpixel_num / 2, interpolate_x];
                            }
                        }
                    }
                    else
                    {

                        ///////////////////////////////////
                        /// calculate other y 內插產生 y= 3-4
                        for (int interpolate_y = subpixel_num / 2 + 1; interpolate_y < subpixel_num; interpolate_y++)
                        {
                            for (int interpolate_x = 0; interpolate_x < sub_pixels.GetLength(1); interpolate_x++)
                            {
                                int x_2be_updated = interpolate_x; // x_src * subpixel_num + interpolate_x;
                                int gray_level_cur  = sub_pixels[   y_src     * subpixel_num + subpixel_num / 2, x_2be_updated];
                                int gray_level_next = sub_pixels[ (y_src + 1) * subpixel_num + subpixel_num / 2, x_2be_updated];

                                int dist_to_cur = interpolate_y - subpixel_num / 2;
                                float f_cur_percentage  = f_increate_percentage * (subpixel_num - dist_to_cur);   // ( x-1, y ) 百分比
                                float f_next_percentage = f_increate_percentage * dist_to_cur;                    // ( x, y ) 百分比

                                int gray_level_interpolate = (int)( gray_level_cur * f_cur_percentage + gray_level_next * f_next_percentage);
                                sub_pixels[y_src * subpixel_num + interpolate_y, interpolate_x] 
                                    = Image_Binary.Normalize(gray_level_interpolate);
                            }
                        }
                    }
                }

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

        private static bool Assign_SubPixel_Res( int[,] sub_pixel_res         /// sub pixel result.

                                     , BitmapData bmpdata
                                     , IntPtr ptr_buffer_dest
                                     , int pixel_size_dest
                                     )
        {
            try
            {
                unsafe
                {
                    byte* ptr = (byte*)ptr_buffer_dest.ToPointer();
                    for (int subpixel_x = 0; subpixel_x < sub_pixel_res.GetLength(1); subpixel_x++)
                    {
                        for (int subpixel_y = 0; subpixel_y < sub_pixel_res.GetLength(0); subpixel_y++)
                        {
                            *(ptr + subpixel_y * bmpdata.Stride + subpixel_x * pixel_size_dest) =  Image_Binary.Normalize( sub_pixel_res[subpixel_y, subpixel_x] );
                        }
                    }
                }

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

        //private static bool Assign_SubPixel_Res(int x_src, int y_src
        //                                    , int[,] sub_pixel_res         /// sub pixel result.

        //                                    , EN_SubPixel_Type subpixel_type
        //                                    , EN_SubPixel_Num en_subpixel_num        /// 一 Pixel 拆成 subpixel_num * subpixel_num pixels

        //                                    , BitmapData bmpdata
        //                                    , IntPtr ptr_buffer_dest
        //                                    , int pixel_size_dest
        //                                    )
        //{
        //    int subpixel_num = Get_Subpixel_Num(en_subpixel_num);
        //    if (subpixel_num <= 0)
        //    {
        //        Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
        //            , $"subpixel_num <= 0 for EN_SubPixel_Num:{en_subpixel_num}");
        //        return false;
        //    }

        //    return Assign_SubPixel_Res_n(x_src, y_src, subpixel_num, sub_pixel_res, bmpdata, ptr_buffer_dest, pixel_size_dest);
        //}

        //private static bool Assign_SubPixel_Res_n(int x_src, int y_src

        //                                        , int subpixel_num
        //                                        , int[,] sub_pixel_res         /// sub pixel result.                
        //                                        , BitmapData bmpdata_dest
        //                                        , IntPtr ptr_buffer_dest
        //                                        , int pixel_size_dest
        //                                         )
        //{
        //    try
        //    {
        //        unsafe
        //        {
        //            byte* ptr = (byte*)ptr_buffer_dest.ToPointer();
        //            for (int subpixel_x = 0; subpixel_x < subpixel_num; subpixel_x++)
        //            {
        //                for (int subpixel_y = 0; subpixel_y < subpixel_num; subpixel_y++)
        //                {
        //                    int new_y = y_src * subpixel_num + subpixel_y;
        //                    int new_x = x_src * subpixel_num + subpixel_x;
        //                    * (ptr + new_y * bmpdata_dest.Stride + new_x * pixel_size_dest) = (byte) sub_pixel_res[subpixel_y, subpixel_x];
        //                }
        //            }
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
        //           , $"Exception catched: error:{0}", ex.Message));
        //        // 儲存Exception到檔案
        //        TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
        //    }

        //    return false;
        //}

        //private unsafe static bool Assign_SubPixel_Res(BitmapData bmp_data
        //                                , byte* ptr_buffer
        //                                , int x, int y
        //                                , int gray_level
        //                                  )
        //{
        //    try
        //    {
        //        *(Image_Buffer_Gray.Get_Pointer(bmp_data, ptr_buffer, x, y)) = (byte)gray_level;

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
        //           , $"Exception catched: error:{0}", ex.Message));
        //        // 儲存Exception到檔案
        //        TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
        //    }

        //    return false;
        //}

        private static int Get_Subpixel_Num(EN_SubPixel_Num en_subpixel_num)
        {
            switch (en_subpixel_num)
            {
                case EN_SubPixel_Num.EN_SubPixel_1:
                    return 1;

                case EN_SubPixel_Num.EN_SubPixel_3:
                    return 3;

                case EN_SubPixel_Num.EN_SubPixel_5:
                    return 5;

                case EN_SubPixel_Num.EN_SubPixel_7:
                    return 7;

                default:
                    return 0;
            }
        }
    }

    public enum EN_SubPixel_Type
    {
        EN_SubPixel_Linear = 0
            //, EN_SubPixel_Linear_Binear = 1
    }

    public enum EN_SubPixel_Num
    {
        EN_SubPixel_1 = 0
            , EN_SubPixel_3
            , EN_SubPixel_5
            , EN_SubPixel_7
    }
}
