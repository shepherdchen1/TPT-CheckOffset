using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.VisualBasic.Devices;
using OpenSource;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using TN.Tools.Debug;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.LinkLabel;

namespace CheckOffset.ImageTools
{
    public enum EN_Insp_Type
    {
        EN_INSP_TYPE_NONE = 0
        , EN_INSP_TYPE_PIN = 1
    }

    // Inspect_Type Detect
    public class IT_Detect
    {
        public IT_Detect(byte[,] buffer)
        {
            _buffer = buffer;
        }

        private byte[,] _buffer = new byte[0, 0];

        private byte[,] _edges = new byte[0, 0];  // edge detect 找到的邊.

        // 權重.
        public int[,] _v_weight = new int[0, 0];
        public int[,] _h_weight = new int[0, 0];

        // 線中央 = 1 垂直線, = 2: 水平線,. 
        private byte[,] _pins = new byte[0, 0];

        public byte[,] Pins { get => _pins; set => _pins = value; }

        private void Clear_Result()
        {
            try
            {
                _v_weight = new int[_buffer.GetLength(0), _buffer.GetLength(1)];
                _h_weight = new int[_buffer.GetLength(0), _buffer.GetLength(1)];
                _pins = new byte[_buffer.GetLength(0), _buffer.GetLength(1)];

                byte[] zero_buf = new byte[_edges.GetLength(1)];
                for (int y = 0; y < _edges.GetLength(0); y++)
                {
                    Buffer.BlockCopy(zero_buf, 0, _edges, y * _edges.GetLength(1), zero_buf.GetLength(0));
                }

                for (int y = 0; y < _pins.GetLength(0); y++)
                {
                    Buffer.BlockCopy(zero_buf, 0, _v_weight, y * _v_weight.GetLength(1), zero_buf.GetLength(0));
                    Buffer.BlockCopy(zero_buf, 0, _h_weight, y * _h_weight.GetLength(1), zero_buf.GetLength(0));

                    Buffer.BlockCopy(zero_buf, 0, _pins, y * _h_weight.GetLength(1), zero_buf.GetLength(0));
                }

            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                   , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }
        }

        public bool Detect_Pin()
        {
            try
            {
                //int threshold = 80;
                Clear_Result();


                ///////////////////////////////////////////////////////////////////
                // 找垂直線
                // 包含線寬 + 線距 個 pixel，以線的中心 往外 線寬 + 線距，在完美狀況下 應該是接近0 的就是線中心附近
                // 正變負 大概就是線的中央(找到之後 再加是否正負兩邊都是低於閥值的非線區，為避免Edge detect沒有偵測到一邊的點，所以以灰階值判斷)
                // 負變正 大概就是線距的中央
                // x,y : _buffer(x-10 ~ x+10 的和) +  _edges(x-10 ~ x+10 的和) * 權重 填入 _pins.
                // -1 * (Point - N gray_level), -1 * (Point - N + 1 gray_level), -1, ...,  Point gray_level, 1 * (Point + 1 gray_level), ...  
                int num_line_width = 6;
                int check_line_space = 4;
                int ext_half_size = (num_line_width + check_line_space) / 2;
                for (int y = 0; y < _buffer.GetLength(0); y++)
                {
                    for (int x = ext_half_size; x < _buffer.GetLength(1) - ext_half_size; x++)
                    {
                        //_v_weight[y, x] += _buffer[y, x];
                        for (int ext_id = 1; ext_id <= ext_half_size; ext_id++)
                        {
                            _v_weight[y, x] -= _buffer[y, x - ext_id];  // 左邊 * -1
                            _v_weight[y, x] += _buffer[y, x + ext_id];  // 右邊 * +1
                        }
                    }
                }

                // 找水平線
                for (int y = ext_half_size; y < _buffer.GetLength(0) - ext_half_size; y++)
                {
                    for (int x = 0; x < _buffer.GetLength(1); x++)
                    {
                        //_h_weight[y, x] += _buffer[y, x];
                        for (int ext_id = 1; ext_id < ext_half_size; ext_id++)
                        {
                            _h_weight[y, x] -= _buffer[y - ext_id, x];  // 左邊 * +1
                            _h_weight[y, x] += _buffer[y + ext_id, x];  // 右邊 * +1
                        }
                    }
                }

                ///////////////////////////////////////////////////////////////////
                // 找垂直線
                // 包含線寬 + 線距 個 pixel，以線的中心 往外 線寬 + 線距，在完美狀況下 應該是接近0 的就是線中心附近
                // 正變負 大概就是線的中央(找到之後 再加是否正負兩邊都是低於閥值的非線區，為避免Edge detect沒有偵測到一邊的點，所以以灰階值判斷)
                // 負變正 大概就是線距的中央
                // x,y : _buffer(x-10 ~ x+10 的和) +  _edges(x-10 ~ x+10 的和) * 權重 填入 _pins.
                // -1 * (Point - N gray_level), -1 * (Point - N + 1 gray_level), -1, ...,  Point gray_level, 1 * (Point + 1 gray_level), ...  
                //int tol_diff = 10;
                //int tol_weight = 10;
                for (int y = 0; y < _buffer.GetLength(0); y++)
                {
                    for (int x = ext_half_size; x < _buffer.GetLength(1) - ext_half_size; x++)
                    {
                        // > num_line_width - 1 是白的
                        //if (_v_weight[y, x] < 255 * ( num_line_width - 1 ) )
                        //    continue;

                        //// < num_line_width + check_line_space - 2 是白的，避免找到全白.
                        //if (_v_weight[y, x] >= 255 * (num_line_width + check_line_space - 1))
                        //    continue;
                        if (_v_weight[y, x] == 0)
                        {
                            if (_v_weight[y, x - 1] > 0 && _v_weight[y, x + 1] < 0)
                                _pins[y, x] = 1;

                            continue;
                        }
                        else if (_v_weight[y, x] > 0)
                        {
                            if (_v_weight[y, x - 1] > 0 && _v_weight[y, x + 1] < 0)
                                _pins[y, x] = 1;

                            continue;
                        }
                        else if (_v_weight[y, x] < 0)
                        {
                            continue;
                        }

                        _pins[y, x] = 1;
                    }
                }

                // 找水平線
                for (int y = ext_half_size; y < _buffer.GetLength(0) - ext_half_size; y++)
                {
                    for (int x = 0; x < _buffer.GetLength(1); x++)
                    {
                        if (_h_weight[y, x] == 0)
                        {
                            if (_h_weight[y - 1, x] > 0 && _h_weight[y + 1, x] < 0)
                                _pins[y, x] = 2;

                            continue;
                        }
                        else if (_h_weight[y, x] > 0)
                        {
                            if (_h_weight[y - 1, x] > 0 && _h_weight[y + 1, x] < 0)
                                _pins[y, x] = 2;

                            continue;
                        }
                        else if (_h_weight[y, x] < 0)
                        {
                            continue;
                        }

                        //_pins[y, x] = 2;
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

        //public bool Detect_Line()
        //{
        //    try
        //    {

        //        Bitmap image;
        //        int w = image.Width;
        //        int h = image.Height;
        //        BitmapData image_data = image.LockBits(
        //        new Rectangle(0, 0, w, h),
        //        ImageLockMode.ReadOnly,
        //            PixelFormat.Format24bppRgb);

        //        int bytes = image_data.Stride * image_data.Height;
        //        byte[] buffer = new byte[bytes];
        //        byte[] result = new byte[bytes];

        //        Marshal.Copy(image_data.Scan0, buffer, 0, bytes);
        //        image.UnlockBits(image_data);

        //        int[][,] filters =
        //        {
        //            Filters.Horizontal,
        //            Filters.Diagonal1,
        //            Filters.Diagonal2,
        //            Filters.Vertical
        //        };

        //        for (int i = 0; i < filters.Length; i++)
        //        {
        //            //apply filter
        //            byte[] filtered = buffer.Convolute(image_data, filters[i]);

        //            //threshold
        //            for (int j = 0; j < bytes; j++)
        //            {
        //                result[j] = (byte)(filtered[j] > 254 ? 255 : 0);
        //            }
        //        }

        //        Bitmap res_img = new Bitmap(w, h);
        //        BitmapData res_data = res_img.LockBits(
        //            new Rectangle(0, 0, w, h),
        //            ImageLockMode.WriteOnly,
        //            PixelFormat.Format24bppRgb);
        //        Marshal.Copy(result, 0, res_data.Scan0, bytes);
        //        res_img.UnlockBits(res_data);

        //        return res_img;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
        //        , $"Exception catched: error:{ex.Message}");
        //        // 儲存Exception到檔案
        //        TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
        //    }
        //}
    }

    public class Filters
    {
        public int[,] Horizontal =
        {
              {-1, -1, -1}
            , { 2,  2,  2}
            , {-1, -1, -1}
        };

        public int[,] Diagonal1 =
        {
              { 2, -1, -1}
            , {-1,  2, -1}
            , {-1, -1,  2}
        };

        public int[,] Diagonal2 =
        {
              {-1, -1,  2}
            , {-1,  2, -1}
            , { 2, -1, -1}
        };

        public int[,] Vertical =
{
              {-1, 2, -1}
            , {-1, 2, -1}
            , {-1, 2, -1}
        };
    
    }
    // 將IT_Detect 結果轉成起點終點寫到 DS_ProjectInspInfo
    public class IT_Detect_DS
    {
        public static bool Transfer_To_Project(IT_Detect it_detect, Canny canny_edge)
        {
            try
            {

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
            }

            return false;
        }

        private static bool Connect_Pins(IT_Detect it_detect, Canny canny_edge)
        {
            try
            {

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
            }

            return false;
        }

    }
}
