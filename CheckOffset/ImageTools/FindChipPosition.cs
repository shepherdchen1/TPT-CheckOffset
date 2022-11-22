using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TN.ImageTools;

using TN.Tools.Debug;
using TN.ImageTools;

namespace CheckOffset.ImageTools
{
    public class FindChipPosition
    {
        //////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Data member
        /// </summary>
        /// 
        private OpenCvSharp.Rect _Select_Chip = new OpenCvSharp.Rect(0, 0, 0, 0);

        private OpenCvSharp.Rect _Select_ABF = new OpenCvSharp.Rect(0, 0, 0, 0);

        private OpenCvSharp.Point[] _Chip_Contour = new OpenCvSharp.Point[0];

        private List<Pin_Found> _Pin_Founds = new List<Pin_Found>();

        private Pin_Conditions _Pin_Conditions = new Pin_Conditions();
        private List<Pin_Found> _Pin_Found = new List<Pin_Found>();

        public List<Pin_Found> Pin_Founds { get => _Pin_Founds; set => _Pin_Founds = value; }

        public OpenCvSharp.Point[][] Found_Contours { get; set; }

        //////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  member function.
        /// </summary>

        public Rect Select_Chip { get => _Select_Chip; set => _Select_Chip = value; }
        public Rect Select_ABF { get => _Select_ABF; set => _Select_ABF = value; }
        public OpenCvSharp.Point[] Chip_Contour { get => _Chip_Contour; set => _Chip_Contour = value; }

        public FindChipPosition()
        {
            _Select_ABF = new OpenCvSharp.Rect(0, 0, 0, 0);
            _Select_Chip = new OpenCvSharp.Rect(0, 0, 0, 0);

            _Chip_Contour = new OpenCvSharp.Point[0];
        }

 
        public void Find_Chip_Position(Bitmap bmp, string file)// <byte> src_buffer)
        {
            try
            {
                // 選哪個 Band.
                _Select_Band(file);

                // 抽 Band.
                _Extract_Src();

                // 二值化
                _Binarize();

                // 找晶片contour.
                _Select_Contour();
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                   , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }
        }

        //private void _Select_Band(string file)// Mat<byte> src_buffer)
        //{
        //    try
        //    {
        //        Mat org_buffer = new Mat(file);

        //        Mat[] mats = new Mat[] {org_buffer };

        //        int[] channels = new int[] { 0, 1, 2 };
        //        int[] histsize = new int[] { 256, 256, 256 };
        //        Rangef[] range = new Rangef[3];

        //        range[0] = new Rangef( 0.0f, 256.0f );
        //        range[1] = range[0];
        //        range[2] = range[0];
        //        Mat mask = new Mat();

        //        //SparseMat hist0 = new SparseMat( histsize, MatType.CV_32F);
        //        //Cv2.CalcHist(mats, channels, mask, hist0, 3, histsize, range);

        //        Mat[] hist = new Mat[] { new Mat(), new Mat(), new Mat() };
        //        Cv2.CalcHist(mats, channels, mask, hist, 3, histsize, range);

        //        //// 選哪個 Band.
        //        //_Select_Band(file);

        //        ////抽 Band.
        //        //_Extract_Src();

        //        //// 二值化
        //        //_Binarize();

        //        //// 找晶片contour.
        //        //_Select_Contour();
        //    }
        //    catch (Exception ex)
        //    {
        //        Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
        //           , $"Exception catched: error:{ex.Message}");
        //        // 儲存Exception到檔案
        //        TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
        //    }
        //}

        //private void _Select_Band(string file)// Mat<byte> src_buffer)
        //{
        //    try
        //    {
        //        Mat org_buffer = new Mat(file);

        //        // split to b g r
        //        Mat[] mats = Cv2.Split(org_buffer);
        //        Mat[] mats_b = new Mat[] { mats[0] };
        //        Mat[] mats_g = new Mat[] { mats[1] };
        //        Mat[] mats_r = new Mat[] { mats[2] };

        //        mats_b[0] = new Mat(file);
        //        //Mat mats0 = new Mat() { mats[0] };
        //        //Mat mats1 = new Mat { mats[1] };
        //        //Mat mats2 = new Mat { mats[2] };

        //        //mats0.save
        //        //Image_Buffer_Gray.Buffer_Save_File(mats0, "d:\\test\\split_band.bmp");

        //        //Mat[] hist = new Mat[] { new Mat(0, 256, MatType.CV_32FC1, new Scalar(0) ), new Mat(0, 256, MatType.CV_32FC1, new Scalar(0)), new Mat(0, 256, MatType.CV_32FC1, new Scalar(0)) };
        //        Mat[] hist = new Mat[] { new Mat() };

        //        int[] channels = new int[] { 0 };
        //        int[] histsize = new int[] { 256 };
        //        Rangef[] range = new Rangef[1];

        //        range[0] = new Rangef(0.0f, 256.0f);

        //        Mat mask = new Mat();

        //        Cv2.CalcHist(mats_b, channels, mask, hist[0], 1, histsize, range);
        //        //Cv2.Normalize(hist[0], hist[0]); 

        //        Cv2.MinMaxLoc(hist[0], out double minVal, out double maxVal);
        //        for (int i = 0; i < histsize[0]; i++)
        //        {
        //            float num = (int)(hist[0].Get<float>(i));
        //        }

        //        //int num_0_my = 0;
        //        //int num_255_my = 0;
        //        //int err = 0;
        //        //int width = 100; // 640;
        //        //int height = 50; // 480;
        //        //for (int y = 0; y < height/*mats_b[0].GetLength(0)*/; y++)
        //        //{
        //        //    for (int x = 0; x < width /*mats_b[0].GetLength(1)*/; x++)
        //        //    {
        //        //        if (0 == mats_b[0].Get<byte>(y, x))
        //        //            num_0_my++;
        //        //        else if (255 == mats_b[0].Get<byte>(y, x))
        //        //            num_255_my++;
        //        //        else
        //        //        {
        //        //            byte test = mats_b[0].Get<byte>(y, x);
        //        //            err++;
        //        //        }
        //        //    }
        //        //}

        //        int num_0 = (int)(hist[0].Get<int>(0));
        //        int num_255 = (int)(hist[0].Get<int>(255));

        //        Cv2.ImShow("Histo", org_buffer);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
        //           , $"Exception catched: error:{ex.Message}");
        //        // 儲存Exception到檔案
        //        TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
        //    }
        //}

        private void _Select_Band(string file)// Mat<byte> src_buffer)
        {
            try
            {
                Mat org_buffer = new Mat(file);

                // split to b g r
                Mat[] mats = Cv2.Split(org_buffer);
                Mat[] mats_b = new Mat[] { mats[0] };
                Mat[] mats_g = new Mat[] { mats[1] };
                Mat[] mats_r = new Mat[] { mats[2] };

                mats_b[0] = new Mat(file);
                //Mat mats0 = new Mat() { mats[0] };
                //Mat mats1 = new Mat { mats[1] };
                //Mat mats2 = new Mat { mats[2] };

                //mats0.save
                //Image_Buffer_Gray.Buffer_Save_File(mats0, "d:\\test\\split_band.bmp");

                //Mat[] hist = new Mat[] { new Mat(0, 256, MatType.CV_32FC1, new Scalar(0) ), new Mat(0, 256, MatType.CV_32FC1, new Scalar(0)), new Mat(0, 256, MatType.CV_32FC1, new Scalar(0)) };
                Mat[] hist = new Mat[] { new Mat() };

                int[] channels = new int[] { 0 };
                int[] histsize = new int[] { 256 };
                Rangef[] range = new Rangef[1];

                range[0] = new Rangef(0.0f, 256.0f);

                Mat mask = new Mat();

                Cv2.CalcHist(mats_b, channels, mask, hist[0], 1, histsize, range);
                //Cv2.Normalize(hist[0], hist[0]); 

                Cv2.MinMaxLoc(hist[0], out double minVal, out double maxVal);
                for (int i = 0; i < histsize[0]; i++)
                {
                    float num = (int)(hist[0].Get<float>(i));
                }

                //int num_0_my = 0;
                //int num_255_my = 0;
                //int err = 0;
                //int width = 100; // 640;
                //int height = 50; // 480;
                //for (int y = 0; y < height/*mats_b[0].GetLength(0)*/; y++)
                //{
                //    for (int x = 0; x < width /*mats_b[0].GetLength(1)*/; x++)
                //    {
                //        if (0 == mats_b[0].Get<byte>(y, x))
                //            num_0_my++;
                //        else if (255 == mats_b[0].Get<byte>(y, x))
                //            num_255_my++;
                //        else
                //        {
                //            byte test = mats_b[0].Get<byte>(y, x);
                //            err++;
                //        }
                //    }
                //}

                int num_0 = (int)(hist[0].Get<int>(0));
                int num_255 = (int)(hist[0].Get<int>(255));

                Cv2.ImShow("Histo", org_buffer);
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                   , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }
        }

        private void _Get_Gray_Histogram(string file)// Mat<byte> src_buffer)
        {
            try
            {
                Mat org_buffer = new Mat(file);

                Mat[] mats = Cv2.Split(org_buffer);
                Mat[] mats_b = new Mat[] { mats[0] };
                mats_b[0] = new Mat(file);
                Mat[] hist = new Mat[] { new Mat() };

                int[] channels = new int[] { 0 };
                int[] histsize = new int[] { 256 };
                Rangef[] range = new Rangef[1];
                range[0] = new Rangef(0.0f, 256.0f);

                Mat mask = new Mat();

                Cv2.CalcHist(mats_b, channels, mask, hist[0], 1, histsize, range);
                //Cv2.Normalize(hist[0], hist[0]); 

                Cv2.MinMaxLoc(hist[0], out double minVal, out double maxVal);
                for (int i = 0; i < histsize[0]; i++)
                {
                    float num = (int)(hist[0].Get<float>(i));
                }

                int num_0 = (int)(hist[0].Get<int>(0));
                int num_255 = (int)(hist[0].Get<int>(255));
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                   , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }
        }

        private void _Extract_Src()
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
        }

        private void _Binarize()
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
        }

        private void _Select_Contour()
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
        }
    }
}
