using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace WpfAsistente
{
    public class DataContainer
    {
        private static DataContainer oInstance;
        public bool Actividad;
        public BitmapSource Result;
        public double Nocaptimevar;
        public double Elapsedtime;
        public bool IsSearching;
        public bool IsinMenu;
        public float InteractingTime;       
        public bool GoCaputure=true;
        public int CapturingTime = 3;
        public MainWindow MainWndow;
        public Menu MenuWndow;
        public SearchWindow SearchWindow;
        //VideoPlayer
        public VideoPlayer VideoPlayer;
        public bool IsinVideoPlayer;
        //Browser
        public bool IsInBrowsewr;
        public bool isIncBrowser;
        public Browser Browser;
        public ChromeBrowser cBrowser;
        public string Url;
        public string VideoName;
        public string Accion;
        //Videos Full
        public VideosFull VideosFull;
        public string VideoFullname;
        //Selfie
        public Selfie Selfie;
        public  bool IsinSelfie;
        public bool IsInVideoFull;
        public List<TypeClass.Button> menuButtons;
        public TypeClass.Button clickedButton;
        public bool DefaultButtonPos;
        //menu Mark
        public bool shortMenu;

        public string CatchUrl;


        protected DataContainer()
        {            
        }
        public static DataContainer Instance()
        {
            if (oInstance == null)
                oInstance = new DataContainer();
            return oInstance;
        }
    }
}