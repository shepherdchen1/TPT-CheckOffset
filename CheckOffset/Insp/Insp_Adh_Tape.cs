using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckOffset.ImageTools;
using OpenCvSharp;
using TN.Tools.Debug;
using TNControls;

namespace CheckOffset.Insp
{
    public class DS_Insp_Param_Adh_Tape
    {
        private int _max_fiff_wh = 5;

        public int Max_Diff_WH { get => _max_fiff_wh; set => _max_fiff_wh = value; }
    }

    public class Insp_Adh_Tape
    {
        private OpenCvSharp.Point _left_center = new OpenCvSharp.Point(0, 0);
        private OpenCvSharp.Point _right_center = new OpenCvSharp.Point(0, 0);

        private OpenCvSharp.Point[] _content_detail_center = new OpenCvSharp.Point[0];
        private OpenCvSharp.Point _content_center = new OpenCvSharp.Point(0, 0);

        private ImageTools.Band_Info[] _Band_Info_Res = new ImageTools.Band_Info[3];

        private DS_CAM_Adh_Tape_Info _insp_res = new DS_CAM_Adh_Tape_Info();

        public OpenCvSharp.Point Left_center { get => _left_center; set => _left_center = value; }
        public OpenCvSharp.Point Right_center { get => _right_center; set => _right_center = value; }
        public OpenCvSharp.Point[] Content_detail_center { get => _content_detail_center; set => _content_detail_center = value; }
        public OpenCvSharp.Point Content_center { get => _content_center; set => _content_center = value; }
        public Band_Info[] Band_Info_Res { get => _Band_Info_Res; set => _Band_Info_Res = value; }
        public DS_CAM_Adh_Tape_Info Insp_res { get => _insp_res; set => _insp_res = value; }

        /////////////////////////////////////////////////////////////////
        /// <summary>
        /// 

        public void Insp_Right(Mat _from_file_tobe_insp_buffer, DS_CAM_Adh_Tape_Info _insp_res)
        {
            System.Drawing.Rectangle rt_center_ext = OpenCVMatTool.ToSystemRect(tnGlobal.CAM_Info.Adh_Tape_Info.Right_center);
            rt_center_ext.Inflate(rt_center_ext.Width / 2, rt_center_ext.Height / 2);
            Select_Adh_Tape_Blob(_from_file_tobe_insp_buffer
                                , OpenCVMatTool.ToOpenCvRect(rt_center_ext) //tnGlobal.CAM_Info.Adh_Tape_Info.Left_center
                                , tnGlobal.CAM_Info.Adh_Tape_Info.Blob_is_white /* chkBlobIsWhite.Checked */
                                , tnGlobal.CAM_Info.Adh_Tape_Info.Threshold_slot
                                , out OpenCvSharp.Rect rt_bounding_box);
            _insp_res.Right_center = rt_bounding_box;

            System.Drawing.Rectangle rt_center_pin_ext = OpenCVMatTool.ToSystemRect(tnGlobal.CAM_Info.Adh_Tape_Info.Right_center_pin);
            rt_center_pin_ext.Inflate(rt_center_pin_ext.Width / 4, rt_center_pin_ext.Height / 4);
            Select_Adh_Tape_Blob(_from_file_tobe_insp_buffer
                    , OpenCVMatTool.ToOpenCvRect(rt_center_pin_ext) // tnGlobal.CAM_Info.Adh_Tape_Info.Left_center_pin
                    , tnGlobal.CAM_Info.Adh_Tape_Info.Blob_is_white/* chkBlobIsWhite.Checked */
                    , tnGlobal.CAM_Info.Adh_Tape_Info.Threshold_pin
                    , out rt_bounding_box);
            _insp_res.Right_center_pin = rt_bounding_box;
        }

        public void Insp_Left(Mat _from_file_tobe_insp_buffer, DS_CAM_Adh_Tape_Info _insp_res)
        {
            System.Drawing.Rectangle rt_left_center_ext = OpenCVMatTool.ToSystemRect( tnGlobal.CAM_Info.Adh_Tape_Info.Left_center );
            rt_left_center_ext.Inflate(rt_left_center_ext.Width / 2, rt_left_center_ext.Height / 2);    
            Select_Adh_Tape_Blob(_from_file_tobe_insp_buffer
                                , OpenCVMatTool.ToOpenCvRect( rt_left_center_ext ) //tnGlobal.CAM_Info.Adh_Tape_Info.Left_center
                                , tnGlobal.CAM_Info.Adh_Tape_Info.Blob_is_white /* chkBlobIsWhite.Checked */
                                , tnGlobal.CAM_Info.Adh_Tape_Info.Threshold_slot
                                , out OpenCvSharp.Rect rt_bounding_box);
            _insp_res.Left_center = rt_bounding_box;

            System.Drawing.Rectangle rt_left_center_pin_ext = OpenCVMatTool.ToSystemRect(tnGlobal.CAM_Info.Adh_Tape_Info.Left_center_pin);
            rt_left_center_pin_ext.Inflate(rt_left_center_pin_ext.Width / 2, rt_left_center_pin_ext.Height / 2);
            Select_Adh_Tape_Blob(_from_file_tobe_insp_buffer
                    , OpenCVMatTool.ToOpenCvRect(rt_left_center_pin_ext) // tnGlobal.CAM_Info.Adh_Tape_Info.Left_center_pin
                    , tnGlobal.CAM_Info.Adh_Tape_Info.Blob_is_white/* chkBlobIsWhite.Checked */
                    , tnGlobal.CAM_Info.Adh_Tape_Info.Threshold_pin
                    , out rt_bounding_box);
            _insp_res.Left_center_pin = rt_bounding_box;
        }

        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="rt_roi"></param>
        /// <param name="threshold"></param>
        public void Blob_Analyze(Mat buffer, OpenCvSharp.Rect rt_roi, int threshold, bool blob_is_white)
        {
            try
            {
                Mat extract_buffer = buffer[rt_roi.Y, rt_roi.Bottom
                                                , rt_roi.X, rt_roi.Right];

                // split to bgr
                Mat[] mats = Cv2.Split(extract_buffer);
                //Mat[] mats_b = new Mat[] { mats[0] };
                //Mat[] mats_g = new Mat[] { mats[1] };
                //Mat[] mats_r = new Mat[] { mats[2] };

                mats[0].SaveImage("d:\\temp\\adhtape\\mat_b.jpg");
                mats[1].SaveImage("d:\\temp\\adhtape\\mat_g.jpg");
                mats[2].SaveImage("d:\\temp\\adhtape\\mat_r.jpg");


                //Mat[] mats_a = new Mat[] { mats[3] };
                Mat[] mat_binary = new Mat[] { new Mat(), new Mat(), new Mat() };

                InputArray kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(3, 3), new OpenCvSharp.Point(-1, -1));
                //Cv2.MorphologyEx(mats[0], mats[0], MorphTypes.Close, kernel);
                //Cv2.MorphologyEx(mats[1], mats[1], MorphTypes.Close, kernel);
                //Cv2.MorphologyEx(mats[2], mats[2], MorphTypes.Close, kernel);
                //Cv2.Threshold(mats[0], mat_binary[0], _Band_Info_ABF[0].Max_gray_level, int.MaxValue, ThresholdTypes.Binary);

                Cv2.Threshold(mats[0], mat_binary[0], threshold, int.MaxValue, ThresholdTypes.Binary);
                Cv2.Threshold(mats[1], mat_binary[1], threshold, int.MaxValue, ThresholdTypes.Binary);
                Cv2.Threshold(mats[2], mat_binary[2], threshold, int.MaxValue, ThresholdTypes.Binary);

                if (!blob_is_white)
                {
                    Cv2.BitwiseNot(mat_binary[0], mat_binary[0]);
                    Cv2.BitwiseNot(mat_binary[1], mat_binary[1]);
                    Cv2.BitwiseNot(mat_binary[2], mat_binary[2]);
                }

                mat_binary[0].SaveImage("d:\\temp\\adhtape\\bin_mat_b.jpg");
                mat_binary[1].SaveImage("d:\\temp\\adhtape\\bin_mat_g.jpg");
                mat_binary[2].SaveImage("d:\\temp\\adhtape\\bin_mat_r.jpg");

                tnGlobal.CAM_Info.Adh_Tape_Info.Band_Info_Res[0] = new Band_Info();
                Cv2.FindContours(mat_binary[0]
                    //, out OpenCvSharp.Point[][] contours
                    , out tnGlobal.CAM_Info.Adh_Tape_Info.Band_Info_Res[0].contours
                    , out HierarchyIndex[] hierarchy_0
                    , RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);
                tnGlobal.CAM_Info.Adh_Tape_Info.Band_Info_Res[1] = new Band_Info();
                Cv2.FindContours(mat_binary[1]
                    , out tnGlobal.CAM_Info.Adh_Tape_Info.Band_Info_Res[1].contours
                    , out HierarchyIndex[] hierarchy_1
                    , RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);
                tnGlobal.CAM_Info.Adh_Tape_Info.Band_Info_Res[2] = new Band_Info();
                Cv2.FindContours(mat_binary[2]
                    , out tnGlobal.CAM_Info.Adh_Tape_Info.Band_Info_Res[2].contours
                    , out HierarchyIndex[] hierarchy_2
                    , RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);

                Cv2.ImShow("ABF B", mat_binary[0]);
                Cv2.ImShow("ABF G", mat_binary[1]);
                Cv2.ImShow("ABF R", mat_binary[2]);
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                   , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }
        }


        public void Select_Adh_Tape_Blob(Mat mat, OpenCvSharp.Rect rt, bool blob_is_white, int threshold, out OpenCvSharp.Rect rt_bounding_box)
        {
            rt_bounding_box = new OpenCvSharp.Rect(0, 0, 0, 0);

            //if (!chkSelectCenter.Checked && !chkSelectLeft.Checked && !chkSelectLeftPin.Checked && !chkSelectRight.Checked && !chkSelectRightPin.Checked && !chkSelectSingle.Checked)
            //    return;

            // 新增完畢
            //if (null == _userctrl_image.pb_Image.Editing_Ctrl)
            //    return;

            //TNCustCtrl_Rect editing_rect = (TNCustCtrl_Rect)_userctrl_image.pb_Image.Editing_Ctrl;

            const int min_roi_valid_size = 2;
            //if (null != editing_rect && editing_rect.Pos_Info.Editing_Rect.Width > min_roi_valid_size && editing_rect.Pos_Info.Editing_Rect.Height > min_roi_valid_size)
            if (rt.Width > min_roi_valid_size && rt.Height > min_roi_valid_size)
            {
                Insp_Adh_Tape insp_adh_tape = new Insp_Adh_Tape();
                insp_adh_tape.Blob_Analyze(mat, rt, threshold, blob_is_white);

                double max_blob_area = double.MinValue;
                int max_blob_id = -1;
                OpenCvSharp.Rect rt_bounding_box_max = new OpenCvSharp.Rect(0, 0, 0, 0);
                for (int blob_id = 0; blob_id < tnGlobal.CAM_Info.Adh_Tape_Info.Band_Info_Res[0].contours.Length; blob_id++)
                {
                    double blob_area = Cv2.ContourArea(tnGlobal.CAM_Info.Adh_Tape_Info.Band_Info_Res[0].contours[blob_id]);
                    if (blob_area < max_blob_area)
                        continue;

                    OpenCvSharp.Rect rt_bounding_box_chk = Cv2.BoundingRect(tnGlobal.CAM_Info.Adh_Tape_Info.Band_Info_Res[0].contours[blob_id]);
                    if (rt_bounding_box_chk.Left <= 0 || rt_bounding_box_chk.Top <= 0
                        || rt_bounding_box_chk.Right >= rt.Width
                        || rt_bounding_box_chk.Height >= rt.Height)
                    {
                        continue;
                    }

                    max_blob_area = blob_area;
                    max_blob_id = blob_id;
                    rt_bounding_box_max = rt_bounding_box_chk;
                }

                if (max_blob_id >= 0)
                {
                    System.Drawing.Rectangle rt_temp = OpenCVMatTool.ToSystemRect(rt_bounding_box_max);
                    rt_temp.Offset(rt.X, rt.Y);
                    rt_bounding_box = OpenCVMatTool.ToOpenCvRect(rt_temp);
                }
            }
        }

        public void Select_Adh_Tape_Content_Blob(Mat mat, OpenCvSharp.Rect rt, bool blob_is_white, int threshold, out OpenCvSharp.Rect[] rt_bounding_boxs)
        {
            rt_bounding_boxs = new OpenCvSharp.Rect[0];

            //if (!chkSelectCenter.Checked && !chkSelectLeft.Checked && !chkSelectLeftPin.Checked && !chkSelectRight.Checked && !chkSelectRightPin.Checked && !chkSelectSingle.Checked)
            //    return;

            // 新增完畢
            //if (null == _userctrl_image.pb_Image.Editing_Ctrl)
            //    return;

            //TNCustCtrl_Rect editing_rect = (TNCustCtrl_Rect)_userctrl_image.pb_Image.Editing_Ctrl;

            const int min_roi_valid_size = 2;
            List<OpenCvSharp.Rect> candidate_rts = new List<OpenCvSharp.Rect>();
            //if (null != editing_rect && editing_rect.Pos_Info.Editing_Rect.Width > min_roi_valid_size && editing_rect.Pos_Info.Editing_Rect.Height > min_roi_valid_size)
            if ( rt.Width > min_roi_valid_size && rt.Height > min_roi_valid_size)
            {
                //Mat mat = new Mat(file_name); tbImgFile.Text);
                if (!blob_is_white)
                    Cv2.BitwiseNot(mat, mat);

                Insp_Adh_Tape insp_adh_tape = new Insp_Adh_Tape();
                insp_adh_tape.Blob_Analyze(mat, rt, threshold, blob_is_white);

                OpenCvSharp.Rect rt_pattern = tnGlobal.CAM_Info.Adh_Tape_Info.Single_center;

                for (int blob_id = 0; blob_id < tnGlobal.CAM_Info.Adh_Tape_Info.Band_Info_Res[0].contours.Length; blob_id++)
                {
                    OpenCvSharp.Rect rt_bounding_box_chk = Cv2.BoundingRect(tnGlobal.CAM_Info.Adh_Tape_Info.Band_Info_Res[0].contours[blob_id]);
                    if (   System.Math.Abs(rt_bounding_box_chk.Width - rt_pattern.Width) > tnGlobal.Insp_Param.Insp_Param_Adh_Tape.Max_Diff_WH
                        || System.Math.Abs(rt_bounding_box_chk.Height - rt_pattern.Height) > tnGlobal.Insp_Param.Insp_Param_Adh_Tape.Max_Diff_WH )
                    {
                        continue;
                    }

                    System.Drawing.Rectangle rt_bounding = OpenCVMatTool.ToSystemRect(rt_bounding_box_chk);
                    rt_bounding.Offset(rt.X, rt.Y);
                    candidate_rts.Add(OpenCVMatTool.ToOpenCvRect(rt_bounding));
                }
            }

            rt_bounding_boxs = candidate_rts.ToArray();
        }
    } // end of     public class Insp_Adh_Tape
} // end of namespace CheckOffset.Insp
