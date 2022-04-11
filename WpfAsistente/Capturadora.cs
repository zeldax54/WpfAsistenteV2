using System;
using System.Linq;
using System.Windows;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using Size = System.Drawing.Size;
using Color=System.Drawing.Color;

namespace WpfAsistente
{
   public class Capturadora
   {

        private readonly CascadeClassifier _cascadeClassifierfaces = new CascadeClassifier(AppDomain.CurrentDomain.BaseDirectory + "face.xml");
        private readonly CascadeClassifier _cascadeClassifiereyes = new CascadeClassifier(AppDomain.CurrentDomain.BaseDirectory + "eye.xml");
   
        //Capturing Events
        public delegate void OnCapturingManager();
        public event OnCapturingManager OnCapturing;
        //No capturing Events
        public delegate void OnNoCapturingManager();
        public event OnNoCapturingManager OnNoCapturing;

        private readonly VideoCapture _capture;
        public bool StopRecon;
        private const int Stoptime = 30000;
        private const int IsPerson = 5000;

        public Capturadora()
        {
            _capture = new VideoCapture();
            _capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameHeight, 640);
            _capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameWidth, 480);
            _capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps, 30);
        }

       public void SetCaptureFull()
       {
           _capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameHeight, 1280);
           _capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameWidth, 720);
       }

        public void SetCaptureStandar()
        {
            _capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameHeight, 640);
            _capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameWidth, 480);
        }

       public void Capture(ref double elapsedTime, ref double nocapttime)
        {           
            if (_capture.QueryFrame() != null)
            {
                var imageFrame = _capture.QueryFrame().ToImage<Bgr, Byte>();                
                if (imageFrame != null && !StopRecon)
                {
                    var grayframe = imageFrame.Convert<Gray, byte>();
                    var faces = _cascadeClassifierfaces.DetectMultiScale(grayframe, 1.1, 10, Size.Empty);
                    var eyes = _cascadeClassifiereyes.DetectMultiScale(grayframe, 1.1, 10, Size.Empty);
                    if (!faces.Any() || !eyes.Any())
                    {
                        nocapttime += elapsedTime;
                        var s = DataContainer.Instance().Actividad;
                        if (nocapttime >= Stoptime && !DataContainer.Instance().Actividad)
                        {
                            OnNoCapturing?.Invoke();
                            elapsedTime = 0;
                            nocapttime = 0;
                        }
                    }
                    else
                    {
                        DataContainer.Instance().Actividad = true;
                        nocapttime = 0;
                        if (elapsedTime >= IsPerson)
                        {
                            OnCapturing?.Invoke();
                            elapsedTime = 0;
                        }
                        //if (!DataContainer.Instance().IsinSelfie)
                        //{
                        //    foreach (var face in faces)
                        //        imageFrame.Draw(face, new Bgr(Color.Aqua), 3, Emgu.CV.CvEnum.LineType.FourConnected);
                        //    foreach (var eye in eyes)
                        //        imageFrame.Draw(eye, new Bgr(Color.Aqua), 3, Emgu.CV.CvEnum.LineType.AntiAlias);
                        //}
                        
                    }
                }
                // imgCamUser.SetZoomScale(0.5, new Point(0, 0));
                if (DataContainer.Instance().IsinSelfie)
                {               
                    imageFrame = imageFrame?.Resize(1280, 720, Emgu.CV.CvEnum.Inter.Linear);
                   // imageFrame= imageFrame?.SmoothGaussian(5, 5, 2, 0);
                    DataContainer.Instance().Result = Helper.ToBitmapSource(imageFrame);
                }              
            }           
        }     
    }
}