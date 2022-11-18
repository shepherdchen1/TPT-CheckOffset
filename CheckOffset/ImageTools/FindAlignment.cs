using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TN.ImageTools;
using TN.Tools.Debug;
using static System.Net.Mime.MediaTypeNames;
using static System.Reflection.Metadata.BlobBuilder;

namespace CheckOffset.ImageTools
{
    internal class FindAlignment
    {
        private Mat<byte> _buffer;

        public FindAlignment(Mat<byte> src_buffer)
        {
            _buffer = src_buffer;
        }

        public bool Find_Alignment(System.Drawing.Rectangle rt_user_select)
        {
            try
            {
                //Mat<byte> selected_buffer = OpenCVMatTool.Extract_Buffer(_buffer, rt_user_select);
                Mat<byte> selected_buffer = _buffer[ rt_user_select.Y, rt_user_select.Bottom
                                                   , rt_user_select.X, rt_user_select.Right ];

                string export_file = $"d:\\test\\align.bmp";
                if (File.Exists(export_file))
                    File.Delete(export_file);

                Image_Buffer_Gray.Buffer_Save_File(selected_buffer, export_file);
                selected_buffer.SaveImage("d:\\test\\align.jpg");

                Cv2.FindContours(selected_buffer
                        , out OpenCvSharp.Point[][]  contours
                        , out HierarchyIndex[] hierarchy
                        , RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);

                double blob_area = double.MinValue;
                int max_blob_area_id = 0;
                for (int i = 0; i < contours.Length; i++)
                {
                    if (Cv2.ContourArea(contours[i]) < blob_area)
                        continue;

                    blob_area = Cv2.ContourArea(contours[i]);
                    max_blob_area_id = i;
                }

                if (max_blob_area_id > 0)
                {
                    OpenCvSharp.Rect rt = Cv2.BoundingRect(contours[max_blob_area_id]);
                    tnGlobal.CAM_Info.Align_Info.Align_Rect = new Rect(rt_user_select.X + rt.X, rt_user_select.Y + rt.Y, rt.Width, rt.Height);
                }

                selected_buffer.Dispose();

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

        private System.Drawing.Rectangle Get_Contour_Rect(OpenCvSharp.Point[][] contours, int contour_id)
        {
            try
            {
                if (contours[contour_id].Length <= 1)
                {
                    return new System.Drawing.Rectangle(0, 0, 0, 0);
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

                return new System.Drawing.Rectangle(min_x, min_y, max_x - min_x + 1, max_y - min_y + 1);
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                   , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }

            return new System.Drawing.Rectangle(0, 0, 0, 0);
        }

    } // end of     internal class FindAlignment
}   // end of namespace CheckOffset.ImageTools
