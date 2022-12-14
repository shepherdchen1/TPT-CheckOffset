using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Configuration;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using TN.Tools.Debug;
using TN.ImageTools;

namespace CheckOffset.ImageTools
{
    public class Pin_Conditions
    {
        public int Min_Len = 10;
        public int Max_Len = 100;
        public int Min_Line_Width = 5;

        public int Close_Size = 1;
    }

    public class Pin_Found
    {
        public System.Drawing.Point PT_Start = new System.Drawing.Point(0,0); 
        public System.Drawing.Point PT_End   = new System.Drawing.Point(0,0);
        public int Pin_Width = 0;
    }

    public class FindPinPosition
    {
        //////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Data member
        /// </summary>
        private List<Pin_Found> _Pin_Founds = new List<Pin_Found>();

        private Pin_Conditions _Pin_Conditions = new Pin_Conditions();
        private List<Pin_Found>     _Pin_Found = new List<Pin_Found>();

        public List<Pin_Found> Pin_Founds { get => _Pin_Founds; set => _Pin_Founds = value; }

        public OpenCvSharp.Point[][] Found_Contours { get; set; }

        //////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  member function.
        /// </summary>



        ///   //public void Find_Pin_Position(IT_Detect pin_candidate_pts)
        //{
        //    try
        //    {
        //        // 
        //        Mat<byte> src_buffer = Init_Source(pin_candidate_pts);
        //        //Mat src_buffer = Mat.FromArray(pin_candidate_pts.Pins);
        //        //for(int y =0; y < src_buffer.Rows; y++)
        //        //{
        //        //    for (int x = 0; x < src_buffer.Cols; x++)
        //        //    {
        //        //        if (src_buffer.Get<Byte>(y, x) >= 1)
        //        //            src_buffer.Set<int>(y, x, 255);
        //        //    }
        //        //}

        //        Cv2.ImShow("Init image", src_buffer);
        //        //Mat dest_buffer = new Mat<byte>(pin_candidate_pts.Buffer.GetLength(0), pin_candidate_pts.Buffer.GetLength(1), MatType.CV_8U );
        //        var dest_buffer = new Mat<byte>(pin_candidate_pts.Buffer.GetLength(0), pin_candidate_pts.Buffer.GetLength(1));

        //        byte[,] buffer_2d;
        //        buffer_2d = src_buffer.ToRectangularArray();
        //        Bitmap temp_bmp = To_Bitmap(buffer_2d);
        //        temp_bmp.Save("d:\\test\\Init.bmp");

        //        int size = _Pin_Conditions.Close_Size * 2 + 1;
        //        Mat se = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(size, size), new OpenCvSharp.Point(-1, -1));
        //        //Cv2.ImShow("SE image", se);
        //        Cv2.ImShow("Source image", src_buffer);

        //        Cv2.Dilate(src_buffer, dest_buffer, se, new OpenCvSharp.Point(-1, -1), 1);
        //        Cv2.ImShow("Dilation image", dest_buffer);

        //        buffer_2d = dest_buffer.ToRectangularArray();
        //        temp_bmp = To_Bitmap(buffer_2d);
        //        temp_bmp.Save("d:\\test\\Dilate.bmp");

        //        Cv2.Erode(dest_buffer, dest_buffer, se, new OpenCvSharp.Point(-1, -1), 1);

        //        buffer_2d = dest_buffer.ToRectangularArray();
        //        temp_bmp = To_Bitmap(buffer_2d);
        //        temp_bmp.Save("d:\\test\\Close.bmp");

        //        Cv2.ImShow("Closed image", dest_buffer);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
        //           , $"Exception catched: error:{ex.Message}");
        //        // 儲存Exception到檔案
        //        TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
        //    }
        //}

        ////
        ///
        public void Find_Pin_Position(IT_Detect pin_candidate_pts)
        {
            try
            {
                Mat<byte> src_buffer = Init_Source(pin_candidate_pts);
                var dest_buffer = new Mat<byte>(pin_candidate_pts.Buffer.GetLength(0), pin_candidate_pts.Buffer.GetLength(1));

        Image_Buffer_Gray.Buffer_Save_File(src_buffer, "d:\\test\\Init.bmp");


                int size = _Pin_Conditions.Close_Size * 2 + 1;
        Mat se = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(size, size), new OpenCvSharp.Point(-1, -1));
        //Cv2.ImShow("SE image", se);
        //Cv2.ImShow("Source image", src_buffer);

                Cv2.Dilate(src_buffer, dest_buffer, se, new OpenCvSharp.Point(-1, -1), 1);
                //Cv2.ImShow("Dilation image", dest_buffer);

            Image_Buffer_Gray.Buffer_Save_File(dest_buffer, "d:\\test\\Dilate.bmp");

                Cv2.Erode(dest_buffer, dest_buffer, se, new OpenCvSharp.Point(-1, -1), 1);
            Image_Buffer_Gray.Buffer_Save_File(dest_buffer, "d:\\test\\Close.bmp");
                dest_buffer.SaveImage("d:\\test\\close_2.bmp");

                //Cv2.ImShow("Closed image", dest_buffer);

                Pin_Blob_Detect(dest_buffer, out OpenCvSharp.Point[][] contours);

                Get_Pin_Pos( contours);

                Found_Contours = contours;
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                   , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }
        }

        public void Pin_Blob_Detect(Mat<byte> buffer, out OpenCvSharp.Point[][] contours)
        //public void Pin_Blob_Detect(var buffer)
        {
            contours = null;
            try
            {
                if (tnGlobal.Insp_Param.Insp_Param_Pin.Pin_Tol_W < 0.1)
                {
                    Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                     , $"Insp_Param_PinPin_Tol_W is:{tnGlobal.Insp_Param.Insp_Param_Pin.Pin_Tol_W}");
                    return;
                }

            Image_Buffer_Gray.Buffer_Save_File(buffer, "d:\\test\\blob_analyze_Source.bmp");

                Cv2.FindContours(buffer
                        , out contours
                        , out HierarchyIndex[] hierarchy
                        , RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);

                Mat<byte> mat_zero = new Mat<byte>(buffer.Height, buffer.Width);
                for(int y = 0; y < mat_zero.Height; y++)
                {
                    for(int x = 0; x < mat_zero.Width; x++)
                    {
                        mat_zero.Set<byte>(y, x, 0);
                    }
                }
            Image_Buffer_Gray.Buffer_Save_File(mat_zero, "d:\\test\\blob_analyze_rect_array.bmp");

                List<Rect> blobs = new List<Rect>();
                Mat imgWithContours = Mat.Zeros(buffer.Rows, buffer.Cols, MatType.CV_8UC1);
                for(int i = 0; i < contours.Length; i++)
                {
                    if ( Cv2.ContourArea(contours[i]) >= 1)
                    {
                        Rect rt = Get_Contour_Rect(contours, i);

                        if (   rt.Width >= tnGlobal.Insp_Param.Insp_Param_Pin.Pin_Tol_W
                            || rt.Height >= tnGlobal.Insp_Param.Insp_Param_Pin.Pin_Tol_H)
                        {
                            blobs.Add(rt);

                            Scalar color = new Scalar(255);
                            Cv2.DrawContours(mat_zero, contours, i, color, 2, LineTypes.Link8, hierarchy, 0);
                        }
                    }
                }

            Image_Buffer_Gray.Buffer_Save_File(mat_zero, "d:\\test\\blob_analyze_rect_array.bmp");

                buffer.SaveImage("d:\\test\\blob_analyze.bmp");
                //Cv2.ImShow("Blob Detect", buffer);
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                   , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }
        }
     

        private Mat<byte>? Init_Source(IT_Detect pin_candidate_pts)
        {
            try
            {
                Mat<byte> src_buffer = Mat.FromArray(pin_candidate_pts.Pins);
                //Mat<byte> src_buffer = Mat.FromArray(pin_candidate_pts.Buffer);
                //for (int y = 0; y < src_buffer.Rows; y++)
                //{
                //    for (int x = 0; x < src_buffer.Cols; x++)
                //    {
                //        byte src = src_buffer.Get<Byte>(y, x);
                //        if (src_buffer.Get<Byte>(y, x) >= 1)
                //            src_buffer.Set<int>(y, x, 255);
                //    }
                //}

                return src_buffer;

                //Mat mat = new Mat(pin_candidate_pts.Buffer.GetLength(0), pin_candidate_pts.Buffer.GetLength(1), MatType.CV_8U);
                //int length = pin_candidate_pts.Buffer.GetLength(0) * pin_candidate_pts.Buffer.GetLength(1);

                //Marshal.Copy(pin_candidate_pts.Buffer, 0, mat.Data, length);
                //Marshal.Copy(pin_candidate_pts.Buffer, 0, mat.ImageData, length);
 ///               using Mat mat = Mat.FromArray(pin_candidate_pts.Buffer);
                //for (int y = 0; y < pin_candidate_pts.Buffer.GetLength(0); y++)
                //{
                //    ////Marshal.Copy(pin_candidate_pts.Buffer[y, 0], 0, mat, pin_candidate_pts.Buffer.GetLength(1));
                //    //Buffer.BlockCopy(pin_candidate_pts.Buffer[y,0], 0, mat[y,0], 0, pin_candidate_pts.Buffer.GetLength(0));
                //    for (int x = 0; x < pin_candidate_pts.Buffer.GetLength(1); x++)
                //    {
                //        mat[y, x] = pin_candidate_pts.Buffer[y, x];
                //    }
                //}
                //for (int y = 0; y < pin_candidate_pts.Buffer.GetLength(0); y++)
                //{
                //    for (int x = 0; x < pin_candidate_pts.Buffer.GetLength(1); x++)
                //    {
                //        byte blue  = pin_candidate_pts[y, x].Blue;
                //        byte green = pin_candidate_pts[y, x].Green;
                //        byte red   = pin_candidate_pts[y, x].Red;
                //        Vec3b newValue = new Vec3b(blue, green, red);
                //        mat[y, x] = newValue;
                //    }
                //}

                //return mat;
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

        private void Get_Pin_Pos(OpenCvSharp.Point[][] contours)
        {
            try
            {
                for(int contour_id = 0; contour_id < contours.GetLength(0); contour_id++)
                {
                    Rect rt = Get_Contour_Rect(contours, contour_id);
                    if (   rt.Width  < tnGlobal.Insp_Param.Insp_Param_Pin.Pin_Tol_W 
                        && rt.Height < tnGlobal.Insp_Param.Insp_Param_Pin.Pin_Tol_H)
                    {
                        continue;
                    }

                    DS_CAM_Pin_Info new_pin = new DS_CAM_Pin_Info();
                    new_pin.Detect_Rect = rt;
                    tnGlobal.CAM_Info.Detect_Pin_Infos.Add(new_pin);
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

        private Rect Get_Contour_Rect(OpenCvSharp.Point[][] contours, int contour_id)
        {
            try
            {
                if (contours[contour_id].Length <= 1 )
                {
                    return new Rect(0, 0, 0, 0);
                }

                int min_x = Int32.MaxValue;
                int min_y = Int32.MaxValue;
                int max_x = 0;
                int max_y = 0;
                for (int pt_id = 0; pt_id < contours[contour_id].Length; pt_id++)
                {
                    min_x = System.Math.Min(min_x, contours[contour_id][pt_id].X);
                    min_y = System.Math.Min(min_y, contours[contour_id][pt_id].Y);
                    max_x = System.Math.Max(max_x, contours[contour_id][pt_id].X);
                    max_y = System.Math.Max(max_y, contours[contour_id][pt_id].Y);
                }

                return new Rect( min_x, min_y, max_x - min_x + 1, max_y - min_y + 1);
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                   , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }

            return new Rect(0, 0, 0, 0);
        }
    }
}
