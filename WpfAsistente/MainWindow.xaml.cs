using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace WpfAsistente
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public readonly Capturadora Capt = new Capturadora();
        public DispatcherTimer _timer = new DispatcherTimer();
        public DispatcherTimer TimerActivity = new DispatcherTimer();

        //Eventos
        public delegate void PlayVideoManager(object datos);

        public event PlayVideoManager PlayVideo;
        //

        public BitmapSource Result;
        //Ventanas
        private Menu _menu;
        private SearchWindow _searchWindow;
        private VideoPlayer _videoPlayer;
        private Browser _browser;
        private VideosFull _videoPlayerFull;
        private Selfie _selfie;

        //URLS
        readonly string _urlCatalogo = ConfigurationManager.AppSettings["catalogo"];
        readonly string _urlinfosearch = ConfigurationManager.AppSettings["infosearch"];
        readonly string _urlejournals = ConfigurationManager.AppSettings["ejournals"];
        readonly string _urlbibliotecadigital = ConfigurationManager.AppSettings["bibliotecadigital"];
        readonly string _urlbasedatos = ConfigurationManager.AppSettings["basedatos"];
        readonly int _timecapt = Convert.ToInt32(ConfigurationManager.AppSettings["captimer"]);
        readonly int _timeinact = Convert.ToInt32(ConfigurationManager.AppSettings["inactimer"]);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContainer.Instance().MainWndow = this;

            _timer.Tick += _timer_Tick;
            _timer.Interval = new TimeSpan(0, 0, 0, _timecapt, 0);
            _timer.Start();

            TimerActivity.Tick += _timerActiviti_Tick;
            TimerActivity.Interval = new TimeSpan(0, 0, _timeinact, 0, 0);


            Capt.OnCapturing += IsPerson;
            Capt.OnNoCapturing += GoStart;           
           //  showSerach();
           ShowMenu();
            // Hide();
        }

     
        private void _timerActiviti_Tick(object sender, EventArgs e)
        {
            DataContainer.Instance().Actividad = false;
        }

        private void GoStart()
        {
            //Cerrando Activos
            DataContainer.Instance().Actividad = false;
            if (DataContainer.Instance().IsinMenu)
            {
                CloseMenu();
                TimerActivity.Stop();
                showSerach();

            }
            if (DataContainer.Instance().IsinVideoPlayer)
            {
                CloseVideoPlayer();
                TimerActivity.Stop();
                showSerach();
            }
            if (DataContainer.Instance().IsinSelfie)
            {
                CloseSelfie();
                TimerActivity.Stop();
                showSerach();
            }
            if (DataContainer.Instance().IsInBrowsewr)
                CloseBrowser();
        }

        public void IsPerson()
        {
            if (DataContainer.Instance().IsSearching)
            {
                _searchWindow.Close();
                DataContainer.Instance().Actividad = true;
                ActivityRestart();
                ShowMenu();
            }
            if (DataContainer.Instance().IsinMenu || DataContainer.Instance().IsinVideoPlayer ||
                DataContainer.Instance().IsinSelfie)
            {
                ActivityRestart();
                DataContainer.Instance().Actividad = true;
            }
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            if (DataContainer.Instance().GoCaputure)
            {
                Capt.Capture(ref DataContainer.Instance().Elapsedtime,
                    ref DataContainer.Instance().Nocaptimevar);
                DataContainer.Instance().Elapsedtime += _timer.Interval.TotalMilliseconds;
            }
        }

        private void showSerach()
        {
            _searchWindow = new SearchWindow();
            _searchWindow.Show();
        }

        public void ShowMenu()
        {
            ActivityRestart();
            _menu = new Menu();
            _menu.OnmenuPost += _menu_OnmenuPost;
            _menu.Show();
        }

        private void ShowVideoPlayer()
        {
            ActivityRestart();
            _videoPlayer = new VideoPlayer();
            _videoPlayer.Show();
        }

        public void ErrorBehabior()
        {
            if (DataContainer.Instance().IsinVideoPlayer)
            {
                DataContainer.Instance().IsinVideoPlayer = false;
                CloseVideoPlayer();
            }
            if (DataContainer.Instance().IsInVideoFull)
            {
                DataContainer.Instance().IsInVideoFull = false;
                CloseVideoFull();
            }

            if (DataContainer.Instance().IsInBrowsewr)
            {
                DataContainer.Instance().IsInBrowsewr = false;
                CloseBrowser();
                CloseVideoPlayer();
            }
            if (DataContainer.Instance().IsinSelfie)
            {
                DataContainer.Instance().IsinSelfie = false;
                CloseSelfie();
                ShowMenu();
            }
          //  ShowMenu();
        }

        private void _menu_OnmenuPost(string accion)
        {
            try
            {
                CloseMenu();
                if (accion == "catalogo")
                {
                    DataContainer.Instance().Accion = "catalogo";
                    DataContainer.Instance().VideoName = "catalogo.mp4";
                    ShowVideoPlayer();
                    PlayVideo?.Invoke(null);
                    DataContainer.Instance().Url = _urlCatalogo;

                    // DataContainer.Instance().Url = " file:///E:/descargas/patricio/6-4/cgisirsi.exe.html";

                    ShowBrowser();
                }
                else if (accion == "infosearch")
                {
                    DataContainer.Instance().Accion = "infosearch";
                    DataContainer.Instance().VideoName = "iaesearch.mp4";
                    ShowVideoPlayer();
                    PlayVideo?.Invoke(null);
                    DataContainer.Instance().Url = _urlinfosearch;
                    // DataContainer.Instance().Url = "http://chat.netlab.snet/channel/general";
                    ShowBrowser();
                }
                else if (accion == "ejournals" || accion== "bibliotecadigital")
                {
                    DataContainer.Instance().Accion = accion;
                    DataContainer.Instance().VideoName = accion+".mp4";
                    ShowVideoPlayer();
                    PlayVideo?.Invoke(null);
                    if(accion== "ejournals")
                    DataContainer.Instance().Url = _urlejournals;
                    else
                       DataContainer.Instance().Url = _urlbibliotecadigital;
                     //  DataContainer.Instance().Url = "https://login.bidi.la/Saml/login/adfs.iae.edu.ar";
                       // DataContainer.Instance().Url = "D:/DOCS/scrapsalva/data/20170607174732/index.html";
                    ShowBrowser();
                }
                else if (accion == "basedatos")
                {
                    DataContainer.Instance().Accion = "basedatos";
                    DataContainer.Instance().VideoName = "basesdedatos.mp4";
                    ShowVideoPlayer();
                    PlayVideo?.Invoke(null);
                    DataContainer.Instance().Url = _urlbasedatos;
                    ShowBrowser();
                }
                else if (accion == "serviciosbiblioteca" || accion == "devolucionlibros" ||
                           accion == "unio" || accion == "estadotiempo")
                {
                    DataContainer.Instance().Accion = accion;
                    DataContainer.Instance().VideoName = accion + ".mp4";
                    ShowVideoFull();
                }

                else if (accion == "selfie")
                {
                    ShowSelfie();
                }
            }
            catch (Exception)
            {

                ShowMenu();
            }
          
        }

        private void CloseMenu()
        {
            DataContainer.Instance().IsinMenu = false;
            _menu.Close();
        }

        public void CloseVideoPlayer()
        {
            DataContainer.Instance().IsinVideoPlayer = false;
            _videoPlayer.Close();

        }

        /// <summary>
        /// Reseteando contador de actividad
        /// </summary>
        public void ActivityRestart()
        {
            if (TimerActivity.IsEnabled)
                TimerActivity.Stop();
            TimerActivity.Start();
        }

        private void ShowBrowser()
        {

            _browser = new Browser();
            if (DataContainer.Instance().Accion == "catalogo")
                _browser.OnFindBook += _browser_OnFindBook;
            _browser.Show();
        }

        private void _browser_OnFindBook(string pasillo)
        {
            DataContainer.Instance().VideoName = "libroencontrado.mp4";
            PlayVideo?.Invoke(pasillo);
        }

        private void CloseBrowser()
        {
            DataContainer.Instance().IsInBrowsewr = false;
            _browser.Close();
        }

        private void ShowVideoFull()
        {
            _videoPlayerFull = new VideosFull();
            _videoPlayerFull.Show();
        }

        private void CloseVideoFull()
        {
            DataContainer.Instance().IsInVideoFull = false;
            DataContainer.Instance().VideosFull.Close();
        }

        private void ShowSelfie()
        {
            RestarTimerForSelfie();
            ActivityRestart();
            _selfie = new Selfie();
            Capt.SetCaptureFull();
            DataContainer.Instance().IsinSelfie = true;
            _selfie.Show();
        }

        public void CloseSelfie()
        {
          
            DataContainer.Instance().IsinSelfie = false;
            Capt.SetCaptureStandar();
            DataContainer.Instance().Selfie.Close();
            RestarTimerCapt();
        }

        public void RestarTimerCapt()
        {
            _timer.Interval = new TimeSpan(0, 0, 0, _timecapt, 0);
            _timer.Stop();
            _timer.Start();
        }

        public void RestarTimerForSelfie()
        {
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            _timer.Stop();
            _timer.Start();
        }

    }
}
