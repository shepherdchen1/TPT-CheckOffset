using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Basler.Pylon;
using TN.Tools.Debug;

namespace TN.CCD
{

    public class BaslerProxy
    {
        /// <summary>
        ///  data member
        /// </summary>
        private string _userID = "";
        private Camera _camera = null;
        private bool _grab_over = false;

        private PixelDataConverter px_convert = new PixelDataConverter();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        public delegate void Camera_Image(Bitmap bmp);
        public event Camera_Image Camera_Image_Event;

        public void Dispose()
        {
            if ( _camera == null)
                return;

            _camera.Dispose();
            _camera = null;
        }

        public void Basler_CameraInit()
        {
            _camera = new Camera();
            _camera.CameraOpened += Configuration.AcquireContinuous;
            _camera.ConnectionLost += Camera_ConnectionLost;
            _camera.StreamGrabber.GrabStarted += StreamGrabber_GrabStarted;
            _camera.StreamGrabber.ImageGrabbed += StreamGrabber_ImageGrabed;
            _camera.StreamGrabber.GrabStopped += StreamGrabber_GrabStoped;
        }

        public bool Basler_CameraInit(string userID)
        {
            try
            {
                // 枚举相机列表
                List<ICameraInfo> allCameraInfos = CameraFinder.Enumerate();
                foreach (ICameraInfo cameraInfo in allCameraInfos)
                {
                    //if (userID == cameraInfo[CameraInfoKey.UserDefinedName])
                    if (userID == cameraInfo[CameraInfoKey.SerialNumber])
                    {
                        this._userID = userID;
                        _camera = new Camera(cameraInfo);
                        _camera.StreamGrabber.ImageGrabbed -= StreamGrabber_ImageGrabed;
                        _camera.StreamGrabber.ImageGrabbed += StreamGrabber_ImageGrabed;
                    }
                }
                if (_camera == null)
                {
                    Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                            , $"CCD SerialNumber:{userID} not found!");
                    //NotifyG.Error("未识别到UserID为“" + UserID + "”的相机！");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Close();

                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                   , $"Exception catched: error:{ex.Message}");
                return false;
            }
        }

        public void Open()
        {
            try
            {
                if (null == _camera)
                {
                    Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                            , $"_camera is null");
                    return;
                }

                _camera.Open();
            }
            catch (Exception ex)
            {
                Close();

                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                   , $"Exception catched: error:{ex.Message}");
            }
        }

        public void Close()
        {
            try
            {
                if (null == _camera)
                {
                    Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                            , $"_camera is null");
                    return;
                }

                _camera.Close();
                _camera.Dispose();

                _camera = null;

            }
            catch (Exception ex)
            {
                Close();

                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                   , $"Exception catched: error:{ex.Message}");
            }
        }

        public bool Is_Valid()
        {
            if (null == _camera)
                return false;

            return _camera.IsOpen;
        }

        private void StreamGrabber_GrabStarted(object sender, EventArgs e)
        {
            _grab_over = true;
        }

        private void StreamGrabber_ImageGrabed(object sender, ImageGrabbedEventArgs e)
        {
            try
            {
                IGrabResult grab_result = e.GrabResult;
                if (grab_result.IsValid)
                {
                    if (_grab_over)
                    {
                        Camera_Image_Event(GrabResult2Bmp(grab_result));
                    }
                }
            }
            catch (Exception ex)
            {
                Close();

                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                   , $"Exception catched: error:{ex.Message}");
                return;
            }
        }

        private void StreamGrabber_GrabStoped(object sender, GrabStopEventArgs e)
        {
            _grab_over = false;
        }

        private void Camera_ConnectionLost(object sender, EventArgs e)
        {
            try
            {
                _camera.StreamGrabber.Stop();
                DestroyCamera();
            }
            catch (Exception ex)
            {
                Close();

                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                   , $"Exception catched: error:{ex.Message}");
            }
        }

        public void OneShot()
        {
            try
            {
                if (null == _camera)
                {
                    Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                            , $"_camera is null");
                    return;
                }

                _camera.Parameters[PLCamera.AcquisitionMode].SetValue(PLCamera.AcquisitionMode.SingleFrame);
                _camera.StreamGrabber.Start(1, GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
            }
            catch (Exception ex)
            {
                Close();

                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                   , $"Exception catched: error:{ex.Message}");
            }
        }

        public void KeepShot()
        {
            try
            {
                if (null == _camera)
                {
                    Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                            , $"_camera is null");
                    return;
                }

                _camera.Parameters[PLCamera.AcquisitionMode].SetValue(PLCamera.AcquisitionMode.Continuous);
                _camera.StreamGrabber.Start(GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
            }
            catch (Exception ex)
            {
                Close();

                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                   , $"Exception catched: error:{ex.Message}");
            }
        }

        public void Stop()
        {
            try
            {
                if (null == _camera)
                {
                    Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                            , $"_camera is null");
                    return;
                }

                _camera.StreamGrabber.Stop();
            }
            catch (Exception ex)
            {
                Close();

                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                   , $"Exception catched: error:{ex.Message}");
            }
        }

        public Bitmap GrabResult2Bmp(IGrabResult grab_result)
        {
            try
            {
                Bitmap b = new Bitmap(grab_result.Width, grab_result.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                BitmapData bmpData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, b.PixelFormat);
                px_convert.OutputPixelFormat = PixelType.BGRA8packed;

                IntPtr bmpIntpr = bmpData.Scan0;
                px_convert.Convert(bmpIntpr, bmpData.Stride * b.Height, grab_result);
                b.UnlockBits(bmpData);

                return b;
            }
            catch (Exception ex)
            {
                Close();

                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                   , $"Exception catched: error:{ex.Message}");
            }
            return new Bitmap(0, 0);
        }

        public void DestroyCamera()
        {
            try
            {
                if (null == _camera)
                {
                    Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                            , $"_camera is null");
                    return;
                }

                _camera.Close();
                _camera.Dispose();
                _camera = null;
            }
            catch (Exception ex)
            {
                Close();

                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                   , $"Exception catched: error:{ex.Message}");
            }
        }

    } // end of     public class BaslerProxy

} // end of namespace TN.CCD
