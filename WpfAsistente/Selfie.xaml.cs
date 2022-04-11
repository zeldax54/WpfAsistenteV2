using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Emgu.CV;
using ZXing;
using ZXing.Common;
using MessageBox = System.Windows.Forms.MessageBox;
using Path = System.IO.Path;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using System.Net;

namespace WpfAsistente
{
    /// <summary>
    /// Interaction logic for Selfie.xaml
    /// </summary>
    public partial class Selfie : Window
    {
        public Selfie()
        {
            InitializeComponent();
        }

        
        readonly DispatcherTimer _timer = new DispatcherTimer();
        DispatcherTimer _timerself=new DispatcherTimer();
        readonly float _textBlockpos =float.Parse(ConfigurationManager.AppSettings["textBlockPos"], NumberStyles.Any, CultureInfo.InvariantCulture);
        readonly float _qrCOntainerHorizontal = float.Parse(ConfigurationManager.AppSettings["qrCOntainerHORIZONTAL"],NumberStyles.Any,CultureInfo.InvariantCulture);
        readonly float _qrCOntainerVertical = float.Parse(ConfigurationManager.AppSettings["qrCOntainerVERTICAL"], NumberStyles.Any, CultureInfo.InvariantCulture);
        readonly float _otraTextHorizontal = float.Parse(ConfigurationManager.AppSettings["otraTextHORIZONTAL"], NumberStyles.Any, CultureInfo.InvariantCulture);
        readonly float _otraTextVertical = float.Parse(ConfigurationManager.AppSettings["otraTextVERTICAL"], NumberStyles.Any, CultureInfo.InvariantCulture);
        readonly float _otrabuttonHorizontal = float.Parse(ConfigurationManager.AppSettings["otrabuttonHORIZONTAL"], NumberStyles.Any, CultureInfo.InvariantCulture);
        readonly float _otrabuttonVertical = float.Parse(ConfigurationManager.AppSettings["otrabuttonVERTICAL"], NumberStyles.Any, CultureInfo.InvariantCulture);
        readonly float _botonVolverHorizontal = float.Parse(ConfigurationManager.AppSettings["botonVolverHORIZONTAL"], NumberStyles.Any, CultureInfo.InvariantCulture);
        readonly float _botonVolverVertical = float.Parse(ConfigurationManager.AppSettings["botonVolverVERTICAL"], NumberStyles.Any, CultureInfo.InvariantCulture);
        readonly float _otraTextFontSize = float.Parse(ConfigurationManager.AppSettings["otraTextFontSize"], NumberStyles.Any, CultureInfo.InvariantCulture);

        readonly float _botonTamanoHeight = float.Parse(ConfigurationManager.AppSettings["botonTamanoHeight"], NumberStyles.Any, CultureInfo.InvariantCulture);
        readonly float _botonTamanoWidth = float.Parse(ConfigurationManager.AppSettings["botonTamanoWidth"], NumberStyles.Any, CultureInfo.InvariantCulture);

        readonly float _otrabuttonSize = float.Parse(ConfigurationManager.AppSettings["otrabuttonSize"], NumberStyles.Any, CultureInfo.InvariantCulture);
        readonly float _textFontSizeCountDown = float.Parse(ConfigurationManager.AppSettings["textFontSizeCountDown"], NumberStyles.Any, CultureInfo.InvariantCulture);
        readonly float _textFontSizeMsj = float.Parse(ConfigurationManager.AppSettings["textFontSizeMsj"], NumberStyles.Any, CultureInfo.InvariantCulture);


        //Accesos
        readonly string _rutaLocal = ConfigurationManager.AppSettings["rutaLocal"];
        readonly string _fullrutaLan = ConfigurationManager.AppSettings["fullrutaLAN"];
        readonly string _dominioLan =ConfigurationManager.AppSettings["dominioLAN"];
        readonly string _usuarioLan = ConfigurationManager.AppSettings["usuarioLAN"];
        readonly string _passwordLan = ConfigurationManager.AppSettings["passwordLAN"];
        readonly string _rutaFtp = ConfigurationManager.AppSettings["rutaFTP"];
        readonly string _nombrepcLan = ConfigurationManager.AppSettings["nombrepcLAN"];

        readonly string _marcaAgua = ConfigurationManager.AppSettings["marcaAgua"];

        private bool _resultselfie;
        private BitmapSource _resultselfieimg;

        private void Selfie_OnInitialized(object sender, EventArgs e)
        {
            DataContainer.Instance().IsinSelfie = true;
            DataContainer.Instance().Selfie = this;
            DataContainer.Instance().Actividad = true;
           
            Width = SystemParameters.FullPrimaryScreenWidth;
            Height = SystemParameters.FullPrimaryScreenHeight;
            cameraContainer.Width = Width;
            cameraContainer.Height = Height;
            cameraContainer.VerticalAlignment= VerticalAlignment.Center;
           
            //Posicionando Elementos
            Helper.ResiseImage(imageCOntainer,35.31,100,Height,Width);
            Helper.PosicionateOnlyHeight(imageCOntainer,43,Height);

            Helper.PosicionateOnlyHeight(textBlock, _textBlockpos, Height);

            Helper.Posicionate(qrCOntainer, _qrCOntainerHorizontal, Width, _qrCOntainerVertical, Height);

            //last pos
            Helper.Posicionate(otraText, _otraTextHorizontal, Width, _otraTextVertical, Height);
            Helper.to_PositionButton(otrabutton, _otrabuttonHorizontal, _otrabuttonVertical, true);

            Helper.to_PositionButton(Volver, _botonVolverHorizontal, _botonVolverVertical, true);
            otraText.FontSize = _otraTextFontSize;

            Volver.Opacity = 0;
            Helper.ResizeLast(new[] { Volver }, _botonTamanoHeight, _botonTamanoWidth);

            Helper.ResizeButtons(new[]
         {
               otrabutton
             }, _otrabuttonSize);

          
           
            // Helper.ResiseImage(qrCOntainer,13,10.46,Height,Width,true);

            Helper.PutTextFontSize(textBlock, _textFontSizeCountDown, Height);
            textBlock.Width = Width;

            DataContainer.Instance().MainWndow._timer.Stop();
            DataContainer.Instance().MainWndow._timer.Interval=new TimeSpan(0,0,0,0,1);
            DataContainer.Instance().MainWndow._timer.Start();
          
            //Timer
            _timer.Tick += _timer_Tick; ;
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            _timer.Start();
            //Timer Selfie
            _timerself.Tick += _timerself_Tick;
            _timerself.Interval = new TimeSpan(0, 0, 0, 1, 0);
            _timerself.Start();

            //evento
            otrabutton.Click += Otrabutton_Click;
            Volver.Click += Volver_Click;
        }

        private void Otrabutton_Click(object sender, RoutedEventArgs e)
        {
            DataContainer.Instance().Actividad = true;
            otraText.Opacity = 0;
            otrabutton.Opacity = 0;
            qrCOntainer.Opacity = 0;
            Volver.Opacity = 0;
            textBlock.Text = null;
            Helper.PutTextFontSize(textBlock, _textFontSizeCountDown, Height);
            _resultselfie = false;
            _timerself.Start();
        }

        private int _iterator = 6;
        private void _timerself_Tick(object sender, EventArgs e)
        {
            _iterator =_iterator-1;
            textBlock.Text = "Selfie en "+_iterator;
            if (_iterator > 0) return;

            _iterator = 6;
            _timerself.Stop();
            textBlock.Text = null;
            textBlock.Opacity = 100;
            Volver.Opacity = 100;
            _resultselfie = true;
            _resultselfieimg = (BitmapSource) imageCOntainer.Source;

            Bitmap imagenresult = GetBitmap((BitmapSource)imageCOntainer.Source);
           
            Bitmap wm = new Bitmap("Resources\\"+ _marcaAgua);
           // Bitmap wm2=new Bitmap("Resources\\bibliotecaiae.png");
            DrawWatermark(wm, imagenresult, 10,10);
           // DrawWatermark(wm2, imagenresult, 5 , 5);

         //   string[] datos = AccesoManager.Acceso();
            Guid id = Guid.NewGuid();
            string namepic = id.ToString().Replace("-","") + ".jpeg";
            //Copiando Local
            string dir = Path.Combine(_rutaLocal, namepic);
            imagenresult.Save(dir,ImageFormat.Jpeg);
            //
            //Copiando a Remoto
            try
            {
                using (var client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(_usuarioLan, _passwordLan);                    
                    client.UploadFile(_fullrutaLan+ namepic, WebRequestMethods.Ftp.UploadFile, dir);
                }
            }
            catch (Exception w)
            {
                MessageBox.Show(@"Ha ourrido un error! " + w.Message, @"Mensaje", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                DataContainer.Instance().MainWndow.ErrorBehabior();
            }
           

            //     
            createQR2(_rutaFtp + "/" + namepic,(int)Helper.Porciento(13,Height));

            //last
            otraText.Opacity = 100;
            otrabutton.Opacity = 100;
            qrCOntainer.Opacity = 100;

            //Guardndo IMg
        }

        private void Volver_Click(object sender, RoutedEventArgs e)
        {
            DataContainer.Instance().MainWndow.CloseAndgoMenu();
            Close();

        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            if(!_resultselfie)
               imageCOntainer.Source = DataContainer.Instance().Result;
            else
            {
                imageCOntainer.Source = _resultselfieimg;
                Helper.PutTextFontSize(textBlock, _textFontSizeMsj, Height);
                textBlock.Text= "Podes acceder a tu foto desde el siguiente código QR " + '\n' +
                                "para guardarla y compartirla en las redes sociales";
            }
        }

        private static BitmapSource ConvertBitmap(Bitmap source)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                          source.GetHbitmap(),
                          IntPtr.Zero,
                          Int32Rect.Empty,
                          BitmapSizeOptions.FromEmptyOptions());
        }

        private static Bitmap BitmapFromSource(BitmapSource bitmapsource)
        {
            Bitmap bitmap;
            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            }
            return bitmap;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            DataContainer.Instance().IsinSelfie = false;
        }

        private void DrawWatermark(Bitmap watermark_bm, Bitmap result_bm, int x, int y)
        {
            //const byte ALPHA = 128;
            //// Set the watermark's pixels' Alpha components.
            //System.Drawing.Color clr;
            //for (int py = 0; py < watermark_bm.Height; py++)
            //{
            //    for (int px = 0; px < watermark_bm.Width; px++)
            //    {
            //        clr = watermark_bm.GetPixel(px, py);
            //        watermark_bm.SetPixel(px, py,
            //             System.Drawing.Color.FromArgb(ALPHA, clr.R, clr.G, clr.B));
            //    }
            //}

            //// Set the watermark's transparent color.
            //watermark_bm.MakeTransparent(watermark_bm.GetPixel(0, 0));

            // Copy onto the result image.
            using (Graphics gr = Graphics.FromImage(result_bm))
            {
                gr.DrawImage(watermark_bm, x, y);

            }
        }

        Bitmap GetBitmap(BitmapSource source)
        {
            Bitmap bmp = new Bitmap(
              source.PixelWidth,
              source.PixelHeight,
              PixelFormat.Format32bppPArgb);
            BitmapData data = bmp.LockBits(
              new System.Drawing.Rectangle(System.Drawing.Point.Empty, bmp.Size),
              ImageLockMode.WriteOnly,
              PixelFormat.Format32bppPArgb);
            source.CopyPixels(
              Int32Rect.Empty,
              data.Scan0,
              data.Height * data.Stride,
              data.Stride);
            bmp.UnlockBits(data);
            return bmp;
        }

        //private void createQR(string data)
        //{
           
        //    QrEncoder encoder = new QrEncoder(ErrorCorrectionLevel.M);
        //    QrCode qrCode;
        //    encoder.TryEncode(data, out qrCode);
        //    WriteableBitmapRenderer wRenderer = new WriteableBitmapRenderer(new FixedModuleSize(2,
        //        QuietZoneModules.Two), Colors.Black, Colors.White);
        //    WriteableBitmap wBitmap = new WriteableBitmap(210,210,400, 400, PixelFormats.Gray8, null);
        //    wRenderer.Draw(wBitmap, qrCode.Matrix);
            
        //    qrCOntainer.Source = wBitmap;
        //}

        private void createQR2(string data,int anchoqr)
        {
            var qrValue = data;
            var barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE, Options = new EncodingOptions
            { Height = anchoqr, Width = anchoqr, Margin = 0 } };
            var bitmap = barcodeWriter.Write(qrValue) ;
            qrCOntainer.Source = ConvertBitmap(bitmap);
        }

    }
}
