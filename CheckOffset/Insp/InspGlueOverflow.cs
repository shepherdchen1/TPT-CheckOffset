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
    internal class InspGlueOverflow
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
                if ( !Find_Offset() )
                {
                    Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                       , $"Find_Offset return false");
                    return false;
                }

                // 填出比對基礎圖
                // 填出 ROI index(每個Pixel屬於哪個檢測區) 
                if (!Make_Diff_Base())
                {
                    Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                       , $"Make_Diff_Base return false");
                    return false;
                }

                // 填出藥檢測的區域
                if (!Make_Insp_Roi())
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

        private bool Find_Offset()
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

        // 填出比對基礎圖
        private bool Make_Diff_Base()
        {
            try
            {
                _golden_sample          = new Mat<byte>(tnGlobal.Setting.CCD_Height, tnGlobal.Setting.CCD_Width);
                _insp_roi_index_sample  = new Mat<byte>(tnGlobal.Setting.CCD_Height, tnGlobal.Setting.CCD_Width);

                int insp_roi_index = 1; // 1 base.
                foreach (DS_CAM_Pin_Info pin_info in tnGlobal.CAM_Info.Detect_Pin_Infos)
                {
                    Mat<byte> pin_buffer = new Mat<byte>(pin_info.Detect_Rect.Height, pin_info.Detect_Rect.Width, new Scalar(255) );
                    _golden_sample[pin_info.Detect_Rect.Y, pin_info.Detect_Rect.Bottom
                                    , pin_info.Detect_Rect.X, pin_info.Detect_Rect.Right] = pin_buffer;
                    pin_buffer.Dispose();

                    Mat<byte> pin_index_buffer = new Mat<byte>(pin_info.Detect_Rect.Height, pin_info.Detect_Rect.Width, new Scalar(insp_roi_index));
                    _insp_roi_index_sample[pin_info.Detect_Rect.Y, pin_info.Detect_Rect.Bottom
                                    , pin_info.Detect_Rect.X, pin_info.Detect_Rect.Right] = pin_index_buffer;
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

        private bool Make_Insp_Roi()
        {
            try
            {
                _insp_roi_sample = new Mat<byte>(tnGlobal.Setting.CCD_Height, tnGlobal.Setting.CCD_Width);

                foreach (DS_CAM_Pin_Info pin_info in tnGlobal.CAM_Info.Detect_Pin_Infos)
                {
                    Rect rt_ext = pin_info.Detect_Rect;
                    rt_ext.Inflate(tnGlobal.Insp_Param.Insp_Param_Pin.Ext_WH, tnGlobal.Insp_Param.Insp_Param_Pin.Ext_WH);
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
                _defect_candidate = new Mat<byte>(tnGlobal.Setting.CCD_Height, tnGlobal.Setting.CCD_Width);
                _defect_candidate[0, tnGlobal.Setting.CCD_Height
                                , 0, tnGlobal.Setting.CCD_Width]
                             = _insp_roi_sample;
                foreach (DS_CAM_Pin_Info pin_info in tnGlobal.CAM_Info.Detect_Pin_Infos)
                {
                    // Pin 腳 外擴檢規 以內為 合法點 填 0
                    Rect rt_ext = pin_info.Detect_Rect;
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
                    Rect rt_shrink = pin_info.Detect_Rect;

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
