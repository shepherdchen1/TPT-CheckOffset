using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using TN.Tools.Debug;
using System.Drawing.Drawing2D;
using TNControls;

namespace TN.ImageTools
{
    internal static class EdgeDetect
    {
        /// <summary>
        /// find x dir edge
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="y"></param>
        /// <param name="x_start"></param>
        /// <param name="x_end"></param>
        /// <param name="x_inc"></param>
        /// <param name="first_white"></param>
        /// <param name="threshold"></param>
        /// <param name="edge_pos"></param>
        /// <returns></returns>
        public static bool Find_Edge_X(Bitmap bmp
                                , int y
                                , int x_start
                                , int x_end
                                , bool first_white      /// true: 要找第一個超過 threshold 的點， false: 要找第一個低於 threshold 的點
                                , int threshold         /// 要找第一個超過 threshold 的點        
                                , out float edge_pos    /// reserved float for subpixel 
                                )
        {
            edge_pos = -1;
            System.Drawing.Imaging.BitmapData? bmp_data = null;
            try
            {
                if (!Image_Buffer_Gray.GetBuffer(bmp, ref bmp_data))
                    return false;

                bool res = _Find_Edge_X(bmp_data, x_start, x_end, y, first_white, threshold, out edge_pos );

                Image_Buffer_Gray.ReleaseBuffer(bmp, ref bmp_data);
                return res;
            }
            catch(Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                               , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }
            finally
            {
                Image_Buffer_Gray.ReleaseBuffer(bmp, ref bmp_data);
            }

            return false;
        }

        private static bool _Find_Edge_X(System.Drawing.Imaging.BitmapData? bmp_data
                                    , int x_start
                                    , int x_end
                                    , int y
                                    , bool first_white      /// true: 要找第一個超過 threshold 的點， false: 要找第一個低於 threshold 的點
                                    , int threshold         /// 要找第一個超過 threshold 的點        
                                    , out float edge_pos    /// reserved float for subpixel 
                                    )
        {
            edge_pos = -1;
            if (null == bmp_data)
                return false;

            int pixel_size = 1;

            /// true: 由前往後， false: 由後往前
            bool x_inc = x_start < x_end ? true : false;
            if (x_inc)
            {
                //////////////////////
                //由前往後
                byte[] row_buffer = new byte[bmp_data.Stride * bmp_data.Height];

                // Copy the RGB values into the array.
                //System.Runtime.InteropServices.Marshal.Copy(bmp_data.Scan0 , row_buffer, y * bmp_data.Stride, bmp_data.Stride);
                System.Runtime.InteropServices.Marshal.Copy(bmp_data.Scan0, row_buffer, 0, bmp_data.Stride * bmp_data.Height);

                if (first_white)
                {
                    //要找第一個超過 threshold 的點
                    for (int x = x_start; x < x_end; x++)
                    {
                        if (row_buffer[y * bmp_data.Stride + x*pixel_size] >= threshold)
                        {
                            edge_pos = (float)x;
                            return true;
                        }
                    }
                }
                else
                {
                    //要找第一個低於 threshold 的點
                    for (int x = x_start; x < x_end; x++)
                    {
                        if (row_buffer[y * bmp_data.Stride + x * pixel_size] <= threshold)
                        {
                            edge_pos = (float)x;
                            return true;
                        }
                    }
                }
            }
            else
            {
                /////////////////////////////////
                //由後往前
                byte[] row_buffer = new byte[bmp_data.Stride * bmp_data.Height];

                // Copy the RGB values into the array.
                //System.Runtime.InteropServices.Marshal.Copy(bmp_data.Scan0, row_buffer, y* bmp_data.Stride, bmp_data.Stride);
                System.Runtime.InteropServices.Marshal.Copy(bmp_data.Scan0, row_buffer, 0, bmp_data.Stride * bmp_data.Height);
                if (first_white)
                {
                    //要找第一個超過 threshold 的點
                    for (int x = x_start; x > x_end; x--)
                    {
                        if (row_buffer[y * bmp_data.Stride + x * pixel_size] >= threshold)
                        {
                            edge_pos = (float)x;
                            return true;
                        }
                    }
                }
                else
                {
                    //要找第一個低於 threshold 的點
                    for (int x = x_start; x > x_end; x--)
                    {
                        if (row_buffer[y * bmp_data.Stride + x * pixel_size] <= threshold)
                        {
                            edge_pos = (float)x;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static bool _Find_Edge_X(IntPtr ptr_buffer
                            , int x_start
                            , int x_end
                            , float diff_percentage
                            , out float edge_pos    /// reserved float for subpixel 
                            )
        {
            edge_pos = 0;
            return false;
        }
        /// <summary>
        ///  find x dir edge
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="y"></param>
        /// <param name="x_start"></param>
        /// <param name="x_end"></param>
        /// <param name="x_inc"></param>
        /// <param name="first_white"></param>
        /// <param name="threshold"></param>
        /// <param name="edge_pos"></param>
        /// <returns></returns>
        public static bool Find_Edge_Y(Bitmap? bmp
                        , int x
                        , int y_start
                        , int y_end
                        , bool first_white      /// true: 要找第一個超過 threshold 的點， false: 要找第一個低於 threshold 的點
                        , int threshold         /// 要找第一個超過 threshold 的點        
                        , out float edge_pos    /// reserved float for subpixel 
                        )
        {
            edge_pos = -1;
            if (null == bmp)
                return false;

            System.Drawing.Imaging.BitmapData? bmp_data = null;
            try
            {
                if (!Image_Buffer_Gray.GetBuffer(bmp, ref bmp_data))
                    return false;

                bool res = _Find_Edge_Y(bmp_data, y_start, y_end, x, first_white, threshold, out edge_pos);

                Image_Buffer_Gray.ReleaseBuffer(bmp, ref bmp_data);
                return res;
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
                Image_Buffer_Gray.ReleaseBuffer(bmp, ref bmp_data);
            }

            return false;
        }

        private static bool _Find_Edge_Y(System.Drawing.Imaging.BitmapData? bmp_data
                        , int x
                        , int y_start
                        , int y_end
                        , bool first_white      /// true: 要找第一個超過 threshold 的點， false: 要找第一個低於 threshold 的點
                        , int threshold         /// 要找第一個超過 threshold 的點        
                        , out float edge_pos    /// reserved float for subpixel 
                            )
        {
            edge_pos = -1;
            if (null == bmp_data)
                return false;

            int pixel_size = 1;
            try
            {
                byte[] row_buffer = new byte[bmp_data.Stride * bmp_data.Height];

                // Copy the RGB values into the array.
                System.Runtime.InteropServices.Marshal.Copy(bmp_data.Scan0, row_buffer, 0, bmp_data.Height * bmp_data.Stride);

                /// true: 由前往後， false: 由後往前
                bool y_inc = y_start < y_end ? true : false;
                if (y_inc)
                {
                    //////////////////////
                    //由前往後
                    if (first_white)
                    {
                        //要找第一個超過 threshold 的點
                        for (int y = y_start; y < y_end; y++)
                        {
                            if (row_buffer[y * bmp_data.Stride + x * pixel_size] >= threshold)
                            {
                                edge_pos = (float)y;
                                return true;
                            }
                        }
                    }
                    else
                    {
                        //要找第一個低於 threshold 的點
                        for (int y = y_start; y < y_end; y++)
                        {
                            if (row_buffer[y * bmp_data.Stride + x * pixel_size] <= threshold)
                            {
                                edge_pos = (float)y;
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    /////////////////////////////////
                    //由後往前
                    if (first_white)
                    {
                        //要找第一個超過 threshold 的點
                        for (int y = y_start; y > y_end; y--)
                        {
                            if (row_buffer[y * bmp_data.Stride + x * pixel_size] >= threshold)
                            {
                                edge_pos = (float)y;
                                return true;
                            }
                        }
                    }
                    else
                    {
                        //要找第一個低於 threshold 的點
                        for (int y = y_start; y > y_end; y--)
                        {
                            if (row_buffer[y * bmp_data.Stride + x * pixel_size] <= threshold)
                            {
                                edge_pos = (float)y;
                                return true;
                            }
                        }
                    }
                }

                return false;
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
    }
}

namespace TN.Insp_Param
{
    public enum EN_Insp_Tol_Dir
    {
        EN_Insp_Tol_None = 0
             , EN_Insp_Tol_Dir_Horz
             , EN_Insp_Tol_Dir_Vert
             , EN_Insp_Tol_Dir_Horz_Horz
             , EN_Insp_Tol_Dir_LT45
             , EN_Insp_Tol_Dir_RT45
             , EN_Insp_Tol_Dir_LB45
             , EN_Insp_Tol_Dir_RB45
    }

    public class DS_Insp_Param
    {
        /// <summary>
        /// data member
        /// </summary>
        /// 
        private DS_Insp_Param_Pin _Insp_Param_Pin = new DS_Insp_Param_Pin();

        /// <summary>
        /// member function
        /// </summary>
        public DS_Insp_Param_Pin DS_Insp_Param_Pin { get => _Insp_Param_Pin; set => _Insp_Param_Pin = value; }


        public DS_Insp_Param()
        {
        }
    }

    public class DS_Insp_Param_Pin
    {
        /// <summary>
        /// data member
        /// </summary>
        /// 
        private EN_Insp_Tol_Dir _Insp_Tol_Dir = EN_Insp_Tol_Dir.EN_Insp_Tol_None;

        private int _Min_Pin_WH = 10;

        /// <summary>
        /// member function
        /// </summary>
        public int Min_Pin_WH { get => _Min_Pin_WH; set => _Min_Pin_WH = value; }

        public EN_Insp_Tol_Dir Insp_Tol_Dir { get => _Insp_Tol_Dir; set => _Insp_Tol_Dir = value; }

        public DS_Insp_Param_Pin()
        {
            _Min_Pin_WH = 10;
            _Insp_Tol_Dir = EN_Insp_Tol_Dir.EN_Insp_Tol_None;
        }

        public string Draw_String()
        {
            return $"Dir:{Insp_Tol_Dir}";
        }
    }

    public class DS_Insp_Param_Edge
    {
        /// <summary>
        /// data member
        /// </summary>
        //private EN_Insp_Tol_Dir _Insp_Tol_Dir = EN_Insp_Tol_Dir.EN_Insp_Tol_None;

        /// <summary>
        /// member function
        /// </summary>
        //public EN_Insp_Tol_Dir Insp_Tol_Dir { get => _Insp_Tol_Dir; set => _Insp_Tol_Dir = value; }

        public DS_Insp_Param_Edge()
        {
            //_Insp_Tol_Dir = EN_Insp_Tol_Dir.EN_Insp_Tol_None;
        }

        //public string Draw_String()
        //{
            //return $"Dir:{Insp_Tol_Dir}";
        //}
    }

    public enum EN_Insp_Result_Type
    {
        EN_Insp_Result_None = 0
            , EN_Insp_Result_Success = 1
            , EN_Insp_Result_Failure = 2
    }
    public class DS_Insp_Result
    {
        public EN_Insp_Result_Type Insp_Result_Type { get; set; }

        public Rectangle Defect_Pos { get; set; }

        public DS_Insp_Result()
        {
            Insp_Result_Type = EN_Insp_Result_Type.EN_Insp_Result_None;
            Defect_Pos = new Rectangle(0, 0, 0, 0);
        }

        public string Draw_String()
        {
            return $"Result:{Insp_Result_Type}";
        }

        public void Paint_Defect(Graphics graphics_show, TNPictureBox pb)
        {
            if (EN_Insp_Result_Type.EN_Insp_Result_Failure != Insp_Result_Type)
                return;

            Pen pen_ctrl = new Pen(Color.Red, 1);
            graphics_show.DrawRectangle(pen_ctrl, pb.GetPBRectFromImage(Defect_Pos));
        }
    }
}
