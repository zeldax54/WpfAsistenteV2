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
using System.Globalization;


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
        private ChromeBrowser _cbrowser;
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
            try
            {
                DataContainer.Instance().MainWndow = this;
                DataContainer.Instance().DefaultButtonPos = Convert.ToBoolean(ConfigurationManager.AppSettings["startdefaultbuttons"]);

                _timer.Tick += _timer_Tick;
                _timer.Interval = new TimeSpan(0, 0, 0, _timecapt, 0);
                _timer.Start();

                TimerActivity.Tick += _timerActiviti_Tick;
                TimerActivity.Interval = new TimeSpan(0, 0, _timeinact, 0, 0);


                Capt.OnCapturing += IsPerson;
                Capt.OnNoCapturing += GoStart;

                // showSerach();
                ShowMenu();
                // Hide();
            }
            catch (Exception ee)
            {

                System.Windows.Forms.MessageBox.Show(ee.Message);
            }
           
        }

     
        private void _timerActiviti_Tick(object sender, EventArgs e)
        {
            DataContainer.Instance().Actividad = false;
        }

        private void GoStart()
        {
            //Cerrando Activos
            DataContainer.Instance().Actividad = false;
            DataContainer.Instance().shortMenu = false;
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
            if (DataContainer.Instance().isIncBrowser)
            {
                DataContainer.Instance().isIncBrowser = false;
                ClosecBrowser();

            }
            if (DataContainer.Instance().IsinSelfie)
            {
                DataContainer.Instance().IsinSelfie = false;
                CloseSelfie();
                ShowMenu();
            }
            //  ShowMenu();
        }

        private void _menu_OnmenuPost(TypeClass.Button button)
        {
            try
            {
                CloseMenu();                         
                DataContainer.Instance().Accion = button.Action;
                DataContainer.Instance().VideoName = button.videoname;
                DataContainer.Instance().Url = button.BrowserURL;
                DataContainer.Instance().clickedButton = button;
                if (button.Name == "Selfie")
                    ShowSelfie();
                else if (button.Name == "estadotiempo")
                {
                      DataContainer.Instance().Accion = "estadotiempo";
                      DataContainer.Instance().VideoName = button.videoname;
                      ShowVideoFull();
                }
                else if (!button.OnlyVideo)
                {
                    ShowVideoPlayer();
                    PlayVideo?.Invoke(null);
                    ShowcBrowser();
                }
                else if (button.OnlyBrowser)
                {

                    ShowcBrowser(true);
                }
                else {
                    ShowVideoFull();
                    PlayVideo?.Invoke(null);
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

        private void ShowcBrowser(bool onlyBlowser = false)
        {

            _cbrowser = new ChromeBrowser(onlyBlowser);

            _cbrowser.OnmenuPost += _menu_OnmenuPost;
            if (DataContainer.Instance().Accion == "catalogo")
                _cbrowser.OnFindBook += _browser_OnFindBook;
            _cbrowser.Show();
        }



    

        public void CloseAndgoMenu()
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

            }
            if (DataContainer.Instance().isIncBrowser)
            {
                DataContainer.Instance().isIncBrowser = false;
                ClosecBrowser();

            }
            DataContainer.Instance().shortMenu = true;

            ShowMenu();
        }
        private void ClosecBrowser()
        {
            DataContainer.Instance().isIncBrowser = false;
            _cbrowser.Close();
        }

    }
}
