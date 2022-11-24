using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TN.ImageTools;

using TN.Tools.Debug;
using TN.ImageTools;
using CheckOffset.Controls;
using CheckOffset.ImageTools;

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

        private OpenCvSharp.Rect _Search_Chip = new OpenCvSharp.Rect(0, 0, 0, 0);

        private OpenCvSharp.Point[] _Chip_Contour = new OpenCvSharp.Point[0];

        private List<Pin_Found> _Pin_Founds = new List<Pin_Found>();

        private Pin_Conditions _Pin_Conditions = new Pin_Conditions();
        private List<Pin_Found> _Pin_Found = new List<Pin_Found>();

        private Band_Info[] _Band_Info_ABF  = new ImageTools.Band_Info[3];
        private Band_Info[] _Band_Info_Chip = new ImageTools.Band_Info[3];

        private Band_Info[] _Band_Info_Res = new ImageTools.Band_Info[3];

        public List<Pin_Found> Pin_Founds { get => _Pin_Founds; set => _Pin_Founds = value; }

        public OpenCvSharp.Point[][] Found_Contours { get; set; }

        //////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  member function.
        /// </summary>

        public Rect Select_Chip { get => _Select_Chip; set => _Select_Chip = value; }
        public Rect Select_ABF { get => _Select_ABF; set => _Select_ABF = value; }

        public Rect Search_Chip { get => _Search_Chip; set => _Search_Chip = value; }
        public OpenCvSharp.Point[] Chip_Contour { get => _Chip_Contour; set => _Chip_Contour = value; }
        public Band_Info[] Band_Info_ABF { get => _Band_Info_ABF; set => _Band_Info_ABF = value; }
        public Band_Info[] Band_Info_Chip { get => _Band_Info_Chip; set => _Band_Info_Chip = value; }
        public Band_Info[] Band_Info_Res { get => _Band_Info_Res; set => _Band_Info_Res = value; }

        public FindChipPosition()
        {
            _Select_ABF = new OpenCvSharp.Rect(0, 0, 0, 0);
            _Select_Chip = new OpenCvSharp.Rect(0, 0, 0, 0);

            _Chip_Contour = new OpenCvSharp.Point[0];
        }

 
        public void Find_Chip_Position(Bitmap bmp, string file, int val_threshold)// <byte> src_buffer)
        {
            try
            {
                _Get_Gray_Histogram(file);

                // 選哪個 Band.
                _Select_Band(file);

                _Select_Band_ABF(file);

                _Select_Band_Chip(file);


                // 抽 Band.
                _Extract_Src();

                // 二值化
                _Binarize();

                // 找晶片contour.
                _Select_Contour(file, val_threshold);
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
                for ( int channel_id = 1; channel_id < mats.Length; channel_id++)
                {
                    hist.Append( new Mat() );
                }

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

                //Cv2.ImShow("Histo", org_buffer);
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                   , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }
        }

        private void _Select_Band_ABF(string file)// Mat<byte> src_buffer)
        {
            try
            {
                Mat org_buffer = new Mat(file);

                OpenCvSharp.Rect rt_abf = tnGlobal.CAM_Info.Chip_Info.Chip_Pos_Finder.Select_ABF;
                Mat extract_buffer = org_buffer[  rt_abf.Y, rt_abf.Bottom
                                                , rt_abf.X, rt_abf.Right];

                Mat[] hist = TNHistogramTools.Get_Histogram(extract_buffer);


                for(int i = 0; i < hist.Length; i++)
                {
                    int min_gray_level = 0;
                    int max_gray_level = 0;
                    TNHistogramTools.Find_Normal_Distribution_Range(hist[i], 50, ref min_gray_level, ref max_gray_level);

                    Band_Info band_info = new Band_Info();
                    band_info.Max_gray_level = max_gray_level;
                    band_info.Min_gray_level = min_gray_level;
                    _Band_Info_ABF[i] = band_info;

                    //TNHistogram tNHistogram = new TNHistogram();
                    //tNHistogram.Set_Histogram(hist[i]);
                    //tNHistogram.Text = $"Hist_{i}";
                    //tNHistogram.Show();
                }
                //Cv2.MinMaxLoc(hist[0], out double minVal, out double maxVal);
                //float cur_max = 0.0f;
                //for (int i = 0; i < histsize[0]; i++)
                //{
                //    float num = (int)(hist[0].Get<float>(i));
                //    cur_max = Math.Max(cur_max, num);
                //}

                //int num_0 = (int)(hist[0].Get<int>(0));
                //int num_255 = (int)(hist[0].Get<int>(255));

                ////Mat histo_graph = GetHistogram(hist[0], org_buffer.Cols, org_buffer.Rows);

                //TNHistogram tNHistogram = new TNHistogram();
                //tNHistogram.Set_Histogram(hist[0]);
                //tNHistogram.Show();

                //Mat histo_graph = GetHistogram(hist[0], 640, 640);
                ////Cv2.ImShow("Histogram_B", histo_graph);

                //histo_graph = GetHistogram(hist[1], 640, 640);
                ////Cv2.ImShow("Histogram_G", histo_graph);

                //histo_graph = GetHistogram(hist[2], 640, 640);
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                   , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }
        }

        private void _Select_Band_Chip(string file)// Mat<byte> src_buffer)
        {
            try
            {
                Mat org_buffer = new Mat(file);

                OpenCvSharp.Rect rt_selected = tnGlobal.CAM_Info.Chip_Info.Chip_Pos_Finder.Select_Chip;
                Mat extract_buffer = org_buffer[  rt_selected.Y, rt_selected.Bottom
                                                , rt_selected.X, rt_selected.Right];

                Mat[] hist = TNHistogramTools.Get_Histogram(extract_buffer);


                for (int i = 0; i < hist.Length; i++)
                {
                    TNHistogram tNHistogram = new TNHistogram();
                    tNHistogram.Set_Histogram(hist[i]);
                    tNHistogram.Text = $"Hist_Chip_{i}";
                    tNHistogram.Show();
                }
                //Cv2.MinMaxLoc(hist[0], out double minVal, out double maxVal);
                //float cur_max = 0.0f;
                //for (int i = 0; i < histsize[0]; i++)
                //{
                //    float num = (int)(hist[0].Get<float>(i));
                //    cur_max = Math.Max(cur_max, num);
                //}

                //int num_0 = (int)(hist[0].Get<int>(0));
                //int num_255 = (int)(hist[0].Get<int>(255));

                ////Mat histo_graph = GetHistogram(hist[0], org_buffer.Cols, org_buffer.Rows);

                //TNHistogram tNHistogram = new TNHistogram();
                //tNHistogram.Set_Histogram(hist[0]);
                //tNHistogram.Show();

                //Mat histo_graph = GetHistogram(hist[0], 640, 640);
                ////Cv2.ImShow("Histogram_B", histo_graph);

                //histo_graph = GetHistogram(hist[1], 640, 640);
                ////Cv2.ImShow("Histogram_G", histo_graph);

                //histo_graph = GetHistogram(hist[2], 640, 640);
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
                Mat[] mats_g = new Mat[] { mats[1] };
                Mat[] mats_r = new Mat[] { mats[2] };
                mats_b[0] = new Mat(file);
                Mat[] hist = new Mat[] { new Mat(), new Mat(), new Mat() };

                int[] channels = new int[] { 0 };
                int[] histsize = new int[] { 256 };
                Rangef[] range = new Rangef[1];
                range[0] = new Rangef(0.0f, 256.0f);

                Mat mask = new Mat();

                Cv2.CalcHist(mats_b, channels, mask, hist[0], 1, histsize, range);
                Cv2.CalcHist(mats_g, channels, mask, hist[1], 1, histsize, range);
                Cv2.CalcHist(mats_r, channels, mask, hist[2], 1, histsize, range);
                //Cv2.Normalize(hist[0], hist[0]); 

                Cv2.MinMaxLoc(hist[0], out double minVal, out double maxVal);
                float cur_max = 0.0f;
                for (int i = 0; i < histsize[0]; i++)
                {
                    float num = (int)(hist[0].Get<float>(i));
                    cur_max = Math.Max(cur_max, num);
                }

                int num_0 = (int)(hist[0].Get<int>(0));
                int num_255 = (int)(hist[0].Get<int>(255));

                //Mat histo_graph = GetHistogram(hist[0], org_buffer.Cols, org_buffer.Rows);

                TNHistogram tNHistogram = new TNHistogram();
                tNHistogram.Set_Histogram(hist[0]);
                tNHistogram.Show();

                Mat histo_graph = GetHistogram(hist[0], 640, 640);
                //Cv2.ImShow("Histogram_B", histo_graph);

                histo_graph = GetHistogram(hist[1], 640, 640);
                //Cv2.ImShow("Histogram_G", histo_graph);

                histo_graph = GetHistogram(hist[2], 640, 640);
                //Cv2.ImShow("Histogram_R", histo_graph);
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                   , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }
        }

        private void _Get_Gray_Histogram(Mat org_buffer, OpenCvSharp.Rect select_rect )// Mat<byte> src_buffer)
        {
            try
            {
                Mat extract_buffer = org_buffer[select_rect.Y, select_rect.Bottom
                                        , select_rect.X, select_rect.Right];

                Mat[] mats = Cv2.Split(extract_buffer);
                Mat[] mats_b = new Mat[] { mats[0] };
                Mat[] mats_g = new Mat[] { mats[1] };
                Mat[] mats_r = new Mat[] { mats[2] };
                Mat[] hist = new Mat[] { new Mat(), new Mat(), new Mat() };

                int[] channels = new int[] { 0 };
                int[] histsize = new int[] { 256 };
                Rangef[] range = new Rangef[1];
                range[0] = new Rangef(0.0f, 256.0f);

                Mat mask = new Mat();

                Cv2.CalcHist(mats_b, channels, mask, hist[0], 1, histsize, range);
                Cv2.CalcHist(mats_g, channels, mask, hist[1], 1, histsize, range);
                Cv2.CalcHist(mats_r, channels, mask, hist[2], 1, histsize, range);
                //Cv2.Normalize(hist[0], hist[0]); 

                Cv2.MinMaxLoc(hist[0], out double minVal, out double maxVal);
                float cur_max = 0.0f;
                for (int i = 0; i < histsize[0]; i++)
                {
                    float num = (int)(hist[0].Get<float>(i));
                    cur_max = Math.Max(cur_max, num);
                }

                int num_0 = (int)(hist[0].Get<int>(0));
                int num_255 = (int)(hist[0].Get<int>(255));

                //Mat histo_graph = GetHistogram(hist[0], org_buffer.Cols, org_buffer.Rows);

                TNHistogram tNHistogram = new TNHistogram();
                tNHistogram.Set_Histogram(hist[0]);
                tNHistogram.Show();

                Mat histo_graph = GetHistogram(hist[0], 640, 640);
                //Cv2.ImShow("Histogram_B", histo_graph);

                histo_graph = GetHistogram(hist[1], 640, 640);
                //Cv2.ImShow("Histogram_G", histo_graph);

                histo_graph = GetHistogram(hist[2], 640, 640);
                //Cv2.ImShow("Histogram_R", histo_graph);
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                   , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }
        }

        // Draw Histogram from source image - for gray
        public static Mat GetHistogram(Mat hist, int display_width, int display_height)
        {
            Mat render = new Mat(new OpenCvSharp.Size(display_width, display_height), MatType.CV_8UC3, Scalar.All(255));
            double minVal, maxVal;
            Cv2.MinMaxLoc(hist, out minVal, out maxVal);
            Scalar color = Scalar.All(100);
            // Scales and draws histogram
            hist = hist * (maxVal != 0 ? display_height / maxVal : 0.0);
            int binW = display_width / hist.Rows;
            for (int j = 0; j < hist.Rows; ++j)
            {
                Console.WriteLine($@"j:{j} P1: {j * binW},{render.Rows} P2:{(j + 1) * binW},{render.Rows - (int)hist.Get<float>(j)}");  //for Debug
                int num = (int)hist.Get<float>(j);
                render.Rectangle(
                    new OpenCvSharp.Point(j * binW, render.Rows - (int)hist.Get<float>(j)),
                    new OpenCvSharp.Point((j + 1) * binW, render.Rows),
                    color,
                    -1);
            }

            return render;
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

        private void _Select_Contour(string file, int val_threshold)
        {
            try
            {
                Mat org_buffer = new Mat(file);

                OpenCvSharp.Rect rt_selected = tnGlobal.CAM_Info.Chip_Info.Chip_Pos_Finder.Search_Chip;
                Mat extract_buffer = org_buffer[  rt_selected.Y, rt_selected.Bottom
                                                , rt_selected.X, rt_selected.Right];

                // split to bgr
                Mat[] mats = Cv2.Split(extract_buffer);
                //Mat[] mats_b = new Mat[] { mats[0] };
                //Mat[] mats_g = new Mat[] { mats[1] };
                //Mat[] mats_r = new Mat[] { mats[2] };

                //Mat[] mats_a = new Mat[] { mats[3] };
                Mat[] mat_binary = new Mat[] { new Mat(), new Mat(), new Mat() };

                InputArray kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(3, 3), new OpenCvSharp.Point(-1, -1));
                //Cv2.MorphologyEx(mats[0], mats[0], MorphTypes.Close, kernel);
                //Cv2.MorphologyEx(mats[1], mats[1], MorphTypes.Close, kernel);
                //Cv2.MorphologyEx(mats[2], mats[2], MorphTypes.Close, kernel);
                //Cv2.Threshold(mats[0], mat_binary[0], _Band_Info_ABF[0].Max_gray_level, int.MaxValue, ThresholdTypes.Binary);

                int threshold = val_threshold > 0 ? val_threshold : _Band_Info_ABF[0].Max_gray_level;
                Cv2.Threshold(mats[0], mat_binary[0], threshold, int.MaxValue, ThresholdTypes.Binary);
                threshold = val_threshold > 0 ? val_threshold : _Band_Info_ABF[1].Max_gray_level;
                Cv2.Threshold(mats[1], mat_binary[1], threshold, int.MaxValue, ThresholdTypes.Binary);
                threshold = val_threshold > 0 ? val_threshold : _Band_Info_ABF[2].Max_gray_level;
                Cv2.Threshold(mats[2], mat_binary[2], threshold, int.MaxValue, ThresholdTypes.Binary);

                _Band_Info_Res[0] = new Band_Info();
                Cv2.FindContours(mat_binary[0]
                    , out _Band_Info_Res[0].contours
                    , out HierarchyIndex[] hierarchy_0
                    , RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);
                _Band_Info_Res[1] = new Band_Info();
                Cv2.FindContours(mat_binary[1]
                    , out _Band_Info_Res[1].contours
                    , out HierarchyIndex[] hierarchy_1
                    , RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);
                _Band_Info_Res[2] = new Band_Info();
                Cv2.FindContours(mat_binary[2]
                    , out _Band_Info_Res[2].contours
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

        private void _Find_ABF_Threshold()
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

    } // end of     public class FindChipPosition

    public class Band_Info
    {
        private int min_gray_level = 0;
        private int max_gray_level = 0;
        public OpenCvSharp.Point[][] contours;

        public int Min_gray_level { get => min_gray_level; set => min_gray_level = value; }
        public int Max_gray_level { get => max_gray_level; set => max_gray_level = value; }
    }
} // end of namespace CheckOffset.ImageTools