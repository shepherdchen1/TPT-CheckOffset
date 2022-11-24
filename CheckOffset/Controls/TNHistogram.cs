using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using CheckOffset.ImageTools;

namespace CheckOffset.Controls
{
    public partial class TNHistogram : Form
    {
        private Mat _mat_histogram;
        public TNHistogram()
        {
            InitializeComponent();
        }

        public Mat Set_Histogram(Mat histogram)
        {
            _mat_histogram = histogram;

            Mat render = new Mat(new OpenCvSharp.Size(pbHistogram.Width, pbHistogram.Height), MatType.CV_8UC3, Scalar.All(255));

            //double minVal, maxVal;
            //Cv2.MinMaxLoc(histogram, out minVal, out maxVal);
            //Scalar color = Scalar.All(100);
            //// Scales and draws histogram

            //int bottom_space = 100;
            //int display_height = pbHistogram.Height - bottom_space;

            //double scale = (maxVal != 0 ? display_height / maxVal : 0.0);
            //Mat display_mat_histogram = histogram * scale;
            //int binW = Width / histogram.Rows;
            //for (int j = 0; j < histogram.Rows; ++j)
            //{
            //    //Console.WriteLine($@"j:{j} P1: {j * binW},{render.Rows} P2:{(j + 1) * binW},{render.Rows - (int)hist.Get<float>(j)}");  //for Debug
            //    int num = (int)histogram.Get<float>(j);
            //    render.Rectangle(
            //        new OpenCvSharp.Point(j * binW, render.Rows - (int)histogram.Get<float>(j)),
            //        new OpenCvSharp.Point((j + 1) * binW, render.Rows),
            //        color,
            //        -1);
            //}

            int norm_min = 0, norm_max = 0;
            TNHistogramTools.Find_Normal_Distribution_Range(histogram, 50, ref norm_min, ref norm_max);
            labelNormalDistri.Text = $"{norm_min}~{norm_max}";

            return render;
        }

        private void pbHistogram_MouseMove(object sender, MouseEventArgs e)
        {
            int binW = Width / _mat_histogram.Rows;
            int cur_x = e.Location.X / binW;

            labelGrayLevel.Text = cur_x.ToString();

            int num = (int)_mat_histogram.Get<float>(cur_x);
            labelPixelNum.Text = num.ToString();

            Draw_Histogram();

            // 畫在 gray level 上.
            int draw_x = e.Location.X / binW * binW;
            using (Graphics g = Graphics.FromImage(pbHistogram.Image))
            {
                g.DrawLine(Pens.Red, new System.Drawing.Point(draw_x, 0),       new System.Drawing.Point(draw_x, pbHistogram.Height));
                g.DrawLine(Pens.Red, new System.Drawing.Point(0, e.Location.Y), new System.Drawing.Point(pbHistogram.Width, e.Location.Y));
            }

            pbHistogram.Invalidate();
        }

        private void TNHistogram_Paint(object sender, PaintEventArgs e)
        {
            Draw_Histogram();
        }

        private void Draw_Histogram()
        {
            if (pbHistogram.Image == null)
            {
                Bitmap bmp = new Bitmap(pbHistogram.Width, pbHistogram.Height);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.White);
                }
                pbHistogram.Image = bmp;
            }

            using (Graphics g = Graphics.FromImage(pbHistogram.Image))
            {
                g.Clear(Color.White);

                int bottom_space = 100;
                int display_height = pbHistogram.Height - bottom_space;

                double minVal, maxVal;
                Cv2.MinMaxLoc(_mat_histogram, out minVal, out maxVal);
                double scale = maxVal != 0 ? display_height / maxVal : 0.0;
                Mat display_mat_histogram = _mat_histogram * scale;
                int binW = pbHistogram.Width / display_mat_histogram.Rows;

                for (int j = 0; j < _mat_histogram.Rows; ++j)
                {
                    //Console.WriteLine($@"j:{j} P1: {j * binW},{render.Rows} P2:{(j + 1) * binW},{render.Rows - (int)hist.Get<float>(j)}");  //for Debug
                    int num = (int)display_mat_histogram.Get<float>(j);

                    g.DrawLine(Pens.Black, new System.Drawing.Point(j * binW, pbHistogram.Height - bottom_space), new System.Drawing.Point(j * binW, pbHistogram.Height - bottom_space - num));
                    //g.DrawLine(Pens.Black, new System.Drawing.Point(j * binW, 0), new System.Drawing.Point(j * binW, num));
                }
            }
        }
    }
}
