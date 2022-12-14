using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TN.Tools.Debug;
using TNControls;

namespace CheckOffset.Insp
{
    public class InspGlueOverflow
    {

        // Pin 腳位置
        private Mat<byte> _golden_sample;

        // 檢測區(_golden_sample)外擴 tnGlobal.Insp_Param.Insp_Param_Pin.Ext_WH，該Pixel = 255
        private Mat<byte> _insp_roi_sample;

        // 每個pixel屬於哪個檢測區，該Pixel = 檢測區index
        private Mat<byte> _insp_roi_index_sample;

        // 如果相減後落在這區域內，代表缺陷，需要用檢規判斷是否是缺陷
        // _insp_roi_sample 內， Pin 腳 boundingbox 往外 檢規大小
        public Mat<byte> _defect_candidate;

        public OpenCvSharp.Size _align_offset = new OpenCvSharp.Size(0, 0);
        // 缺陷
        public Mat<byte> _diff_result;

        public InspGlueOverflow()
        {
            _golden_sample = new Mat<byte>();
        }

        public bool Insp(Mat<byte> insp_buffer)
        {
            try
            {
                // pattern match 找位移 + 旋轉角度
                if ( !Find_Offset(insp_buffer, out _align_offset) )
                {
                    Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                       , $"Find_Offset return false");
                    return false;
                }

                // 填出比對基礎圖
                // 填出 ROI index(每個Pixel屬於哪個檢測區) 
                if (!Make_Diff_Base(_align_offset))
                {
                    Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                       , $"Make_Diff_Base return false");
                    return false;
                }

                // 填出要檢測的區域
                if (!Make_Insp_Roi(_align_offset))
                {
                    Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                       , $"Make_Ignore return false");
                    return false;
                }

                // 檢測
                if (!Insp_Diff(insp_buffer))
                {
                    Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                       , $"Insp return false");
                    return false;
                }

                // 過濾
                if (!Apply_Condition())
                {
                    Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                       , $"Apply_Condition return false");
                    return false;
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

        private bool Find_Offset(Mat<byte> insp_buffer, out OpenCvSharp.Size insp_align_offset)
        {
            insp_align_offset = new OpenCvSharp.Size(0, 0);
            try
            {
                insp_buffer.SaveImage("d:\\temp\\diff_source.jpg");

                int offline_x = 10;
                int offline_y = 10;
                Scalar fg_color = tnGlobal.CAM_Info.Align_Info.Align_Is_White ? Scalar.White : Scalar.Black;
                Scalar bk_color = tnGlobal.CAM_Info.Align_Info.Align_Is_White ? Scalar.Black : Scalar.White;
                Mat<byte> golden_align = new Mat<byte>(tnGlobal.CAM_Info.Align_Info.Align_Rect.Height + offline_y * 2
                                                    , tnGlobal.CAM_Info.Align_Info.Align_Rect.Width + offline_x * 2
                                                    , bk_color);

                Mat<byte> align_buffer = new Mat<byte>(tnGlobal.CAM_Info.Align_Info.Align_Rect.Height, tnGlobal.CAM_Info.Align_Info.Align_Rect.Width
                                        , fg_color );
                golden_align[offline_y, tnGlobal.CAM_Info.Align_Info.Align_Rect.Height + offline_y
                           , offline_x, tnGlobal.CAM_Info.Align_Info.Align_Rect.Width + offline_x]
                                = align_buffer;

                Mat<byte> align_result = new Mat<byte>(tnGlobal.CAM_Info.Align_Info.Align_Rect.Height , tnGlobal.CAM_Info.Align_Info.Align_Rect.Width
                                                , bk_color );
                Cv2.MatchTemplate(insp_buffer, golden_align, align_result, TemplateMatchModes.CCoeffNormed);

                golden_align.SaveImage("d:\\temp\\golden_allign.jpg");
                align_buffer.Dispose();

                OpenCvSharp.Point minLock = new OpenCvSharp.Point(0,0);
                OpenCvSharp.Point maxLock = new OpenCvSharp.Point(0, 0);
                OpenCvSharp.Point matchLock = new OpenCvSharp.Point(0, 0);
                Cv2.MinMaxLoc(align_result, out minLock, out maxLock);
                matchLock = maxLock;

                insp_align_offset = new OpenCvSharp.Size(matchLock.X + offline_x - tnGlobal.CAM_Info.Align_Info.Align_Rect.X
                                                    , matchLock.Y + offline_y - tnGlobal.CAM_Info.Align_Info.Align_Rect.Y );

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

        // 填出比對基礎圖
        private bool Make_Diff_Base(OpenCvSharp.Size insp_align_offset)
        {
            try
            {
                _golden_sample          = new Mat<byte>(tnGlobal.Setting.CCD_Height, tnGlobal.Setting.CCD_Width, new Scalar(0) );
                _insp_roi_index_sample  = new Mat<byte>(tnGlobal.Setting.CCD_Height, tnGlobal.Setting.CCD_Width, new Scalar(0) );

                int insp_roi_index = 1; // 1 base.
                foreach (DS_CAM_Pin_Info pin_info in tnGlobal.CAM_Info.Detect_Pin_Infos)
                {
                    Mat<byte> pin_buffer = new Mat<byte>(pin_info.Detect_Rect.Height, pin_info.Detect_Rect.Width, new Scalar(255) );
                    OpenCvSharp.Rect rect_offset = new OpenCvSharp.Rect(pin_info.Detect_Rect.X + insp_align_offset.Width, pin_info.Detect_Rect.Y + insp_align_offset.Height
                                                                , pin_info.Detect_Rect.Width, pin_info.Detect_Rect.Height );
                    _golden_sample[rect_offset.Y, rect_offset.Bottom
                                    , rect_offset.X, rect_offset.Right] = pin_buffer;
                    pin_buffer.Dispose();

                    Mat<byte> pin_index_buffer = new Mat<byte>(rect_offset.Height, rect_offset.Width, new Scalar(insp_roi_index));
                    _insp_roi_index_sample[rect_offset.Y, rect_offset.Bottom
                                    , rect_offset.X, rect_offset.Right] = pin_index_buffer;
                    pin_index_buffer.Dispose();
                    insp_roi_index++;
                }

                _golden_sample.SaveImage("d:\\temp\\golden_sample.jpg");
                _insp_roi_index_sample.SaveImage("d:\\temp\\pin_index_sample.jpg");

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

        private bool Make_Insp_Roi(OpenCvSharp.Size insp_align_offset )
        {
            try
            {
                _insp_roi_sample = new Mat<byte>(tnGlobal.Setting.CCD_Height, tnGlobal.Setting.CCD_Width, new Scalar(0) );

                foreach (DS_CAM_Pin_Info pin_info in tnGlobal.CAM_Info.Detect_Pin_Infos)
                {
                    Rect rt_ext = new OpenCvSharp.Rect(  pin_info.Detect_Rect.X + insp_align_offset.Width, pin_info.Detect_Rect.Y + insp_align_offset.Height
                                                        , pin_info.Detect_Rect.Width, pin_info.Detect_Rect.Height );

                    if (rt_ext.Width > rt_ext.Height)
                    {
                        rt_ext.Inflate(tnGlobal.Insp_Param.Insp_Param_Pin.Ext_Pin_Dir, tnGlobal.Insp_Param.Insp_Param_Pin.Ext_Pin_Space_Dir);
                    }
                    else
                    {
                        rt_ext.Inflate(tnGlobal.Insp_Param.Insp_Param_Pin.Ext_Pin_Space_Dir, tnGlobal.Insp_Param.Insp_Param_Pin.Ext_Pin_Dir);
                    }

                    TNCustCtrl_Rect.Normalize(ref rt_ext);
                    rt_ext = new Rect(System.Math.Max( 0, rt_ext.X), System.Math.Max( 0, rt_ext.Y)
                                    , System.Math.Min(rt_ext.Right, tnGlobal.Setting.CCD_Width) - rt_ext.Left
                                    , System.Math.Min(rt_ext.Bottom, tnGlobal.Setting.CCD_Height) - rt_ext.Top);

                    Mat<byte> pin_buffer = new Mat<byte>(rt_ext.Height, rt_ext.Width
                                                        , new Scalar(255));
                    _insp_roi_sample[rt_ext.Y, rt_ext.Bottom
                                 , rt_ext.X, rt_ext.Right] 
                                 = pin_buffer;

                    pin_buffer.Dispose();
                }

                _insp_roi_sample.SaveImage("d:\\temp\\insp_roi.jpg");

                // 落在本區即為缺陷
                _defect_candidate = new Mat<byte>(tnGlobal.Setting.CCD_Height, tnGlobal.Setting.CCD_Width, new Scalar(0));
                _defect_candidate[0, tnGlobal.Setting.CCD_Height
                                , 0, tnGlobal.Setting.CCD_Width]
                             = _insp_roi_sample;
                foreach (DS_CAM_Pin_Info pin_info in tnGlobal.CAM_Info.Detect_Pin_Infos)
                {
                    // Pin 腳 外擴檢規 以內為 合法點 填 0
                    // 20221121+ insp_align_offset shift.
                    Rect rt_offset = new OpenCvSharp.Rect(pin_info.Detect_Rect.X + insp_align_offset.Width, pin_info.Detect_Rect.Y + insp_align_offset.Height
                                                    , pin_info.Detect_Rect.Width, pin_info.Detect_Rect.Height);
                    Rect rt_ext = rt_offset;
                    //int tol_w = (int) ( 0.5f + pin_info.Detect_Rect.Width  * tnGlobal.Insp_Param.Insp_Param_Pin.Pin_Tol_W );
                    //int tol_h = (int) ( 0.5f + pin_info.Detect_Rect.Height * tnGlobal.Insp_Param.Insp_Param_Pin.Pin_Tol_H );
                    int tol_w = tnGlobal.Insp_Param.Insp_Param_Pin.Pin_Tol_W;
                    int tol_h = tnGlobal.Insp_Param.Insp_Param_Pin.Pin_Tol_H;

                    rt_ext.Inflate(tol_w, tol_h);
                    TNCustCtrl_Rect.Normalize(ref rt_ext);
                    rt_ext = new Rect(System.Math.Max(0, rt_ext.X), System.Math.Max(0, rt_ext.Y)
                                    , System.Math.Min(rt_ext.Right, tnGlobal.Setting.CCD_Width) - rt_ext.Left
                                    , System.Math.Min(rt_ext.Bottom, tnGlobal.Setting.CCD_Height) - rt_ext.Top);

                    Mat<byte> pin_buffer = new Mat<byte>(rt_ext.Height, rt_ext.Width
                                                        , new Scalar(0));
                    _defect_candidate[rt_ext.Y, rt_ext.Bottom
                                 , rt_ext.X, rt_ext.Right]
                                 = pin_buffer;

                    // Pin 腳 內縮檢規 以內為 缺陷 填 255
                    Rect rt_shrink = rt_offset;

                    rt_shrink.Inflate(-tol_w, -tol_h);
                    TNCustCtrl_Rect.Normalize(ref rt_shrink);
                    rt_shrink = new Rect(System.Math.Max(0, rt_shrink.X), System.Math.Max(0, rt_shrink.Y)
                                    , System.Math.Min(rt_shrink.Right, tnGlobal.Setting.CCD_Width) - rt_shrink.Left
                                    , System.Math.Min(rt_shrink.Bottom, tnGlobal.Setting.CCD_Height) - rt_shrink.Top);

                    // 填 255
                    Mat<byte> pin_buffer_shrink = new Mat<byte>(rt_shrink.Height, rt_shrink.Width
                                                        , new Scalar(255));
                    _defect_candidate[rt_shrink.Y, rt_shrink.Bottom
                                    , rt_shrink.X, rt_shrink.Right]
                                 = pin_buffer_shrink;

                    pin_buffer.Dispose();
                }

                _defect_candidate.SaveImage("d:\\temp\\_defect_candidate.jpg");

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

        // 檢測
        private bool Insp_Diff(Mat<byte> insp_buffer)
        {
            try
            {
                insp_buffer.SaveImage("d:\\temp\\diff_source.jpg");

                _diff_result = new Mat<byte>(_golden_sample.Height, _golden_sample.Width, new Scalar(0));
                Cv2.BitwiseXor(_golden_sample, insp_buffer, _diff_result);

                _diff_result.SaveImage("d:\\temp\\diff_result.jpg");

                Cv2.BitwiseAnd(_diff_result, _insp_roi_sample, _diff_result);
                _diff_result.SaveImage("d:\\temp\\diff_in_insp_result.jpg");

                Cv2.BitwiseAnd(_diff_result, _defect_candidate, _diff_result);
                _diff_result.SaveImage("d:\\temp\\diff_in_def_result.jpg");

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

        // 過濾
        private bool Apply_Condition()
        {
            try
            {
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
    }
}
