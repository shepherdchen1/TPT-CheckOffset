#include "OpenCV_Bridge.h"

//#include <opencv2/core.hpp>
//#include <opencv2/imgproc.hpp>
//#include <opencv2/opencv.hpp>
#include <opencv2/opencv.hpp>
//
using namespace cv;
//
void OpenCV_Bridge::OpenCB_1()
{
	int test = 0;
	Mat image = imread("d:\\test\\blobSource_24.bmp", 0);

	// Save grayscale image
	imwrite("d:\\test\\blobSource_opencv_8.bmp", image);

	//imshow("Grayscale image", image);
	//waitKey(0);
}