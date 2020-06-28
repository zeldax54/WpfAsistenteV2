using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using mshtml;
using MessageBox = System.Windows.MessageBox;
using CefSharp;
using WPFTabTip;
using CefSharp.Wpf;

namespace WpfAsistente
{
    /// <summary>
    /// Interaction logic for Browser.xaml
    /// </summary>
    public partial class ChromeBrowser : Window, ILifeSpanHandler
    {

        public delegate void OnFindBookManaher(string pasillo);
        public event OnFindBookManaher OnFindBook;
        readonly double zocaloheight = Convert.ToDouble(ConfigurationManager.AppSettings["zocaloheight"]);
        readonly double volverporcalto = Convert.ToDouble(ConfigurationManager.AppSettings["volverporcalto"]);
        readonly double volverporcancho = Convert.ToDouble(ConfigurationManager.AppSettings["volverporcancho"]);
        readonly double browserheight= Convert.ToDouble(ConfigurationManager.AppSettings["browserheight"]);
        BackgroundWorker worker;
        public delegate void OnMenuManager(WpfAsistente.TypeClass.Button button);
        public event OnMenuManager OnmenuPost;
        bool OnlyBrowser;
        bool isdeleted = false;

        public ChromeBrowser(bool pOnlyBrowser=false)
        {
            OnlyBrowser = pOnlyBrowser;
            InitializeComponent();
        }

   

     

        private void Browser_OnInitialized(object sender, EventArgs e)
        {          

          

            DataContainer.Instance().isIncBrowser = true;
            DataContainer.Instance().cBrowser = this;
            if (!OnlyBrowser)
            {
                double altoVideoP = DataContainer.Instance().VideoPlayer.Height;

                Helper.ResizeScreen(SystemParameters.FullPrimaryScreenHeight - altoVideoP,
                SystemParameters.FullPrimaryScreenWidth, this, altoVideoP, 0, 0, 0);
            }
            else {
                Helper.ResizeScreen(SystemParameters.FullPrimaryScreenHeight,
                   SystemParameters.FullPrimaryScreenWidth, this, 0, 0, 0, 0);
            }
            

            canvasContainerLine.Width = SystemParameters.FullPrimaryScreenWidth;
            canvasContainerLine.Height = Helper.Porciento(5, SystemParameters.FullPrimaryScreenHeight);

            cBrowser.Width= SystemParameters.FullPrimaryScreenWidth;
            cBrowser.Height = Helper.Porciento(browserheight, Height);
            //

            TabTipAutomation.BindTo<System.Windows.Controls.TextBox>();



            zocalo.Width = SystemParameters.FullPrimaryScreenWidth;
            zocalo.Height = Helper.Porciento(zocaloheight, Height);
            
            Helper.ResizeLast(new [] {Volver2}, volverporcalto, volverporcancho);
            //Posicionar label

            //FooterBUttons()
            var footer = Helper.GetFooter();
            bool isdefpos = DataContainer.Instance().DefaultButtonPos;
            decimal startpos = 0;
            NameScope.SetNameScope(this, new NameScope());
            System.Windows.Media.Animation.Storyboard sb = new System.Windows.Media.Animation.Storyboard();
            var botones = Helper.SetPosition(footer, ref zocalo, (Style)Resources["MyButtonStyle"], ref sb, RegisterName, isdefpos, startpos);
            sb.Begin(this, true);
            foreach (var b in botones)
                b.Click += NewBtn_Click;
            DataContainer.Instance().MainWndow.TimerActivity.Start();
            //
            //Boton Volver
           // Volver.Click += Volver_Click;
            Volver2.Click += Volver_Click;
            //
           
            cBrowser.Address = (DataContainer.Instance().Url);
            cBrowser.JavascriptMessageReceived += OnBrowserJavascriptMessageReceived;
            cBrowser.FrameLoadEnd += OnFrameLoadEnd;
        }

        public void OnFrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            if (!isdeleted)
            {
                string predel = @" document.querySelectorAll('.micrositio-header')[0].style.display = 'none'";
                cBrowser.ExecuteScriptAsync(predel);
                predel = @"document.querySelectorAll('.micrositio-menu-container')[0].style.display = 'none'";
                cBrowser.ExecuteScriptAsync(predel);
                isdeleted = true;
            }
            if (e.Frame.IsMain)
            {
                //In the main frame we inject some javascript that's run on mouseUp
                //You can hook any javascript event you like.
                cBrowser.ExecuteScriptAsync(@"

              document.querySelectorAll('input').forEach(element => element.addEventListener('click', ()=>{CefSharp.PostMessage('asdasd');}));             


	  
           
	         ");
            }
        }

        private void OnBrowserJavascriptMessageReceived(object sender, JavascriptMessageReceivedEventArgs e)
        {
            var windowSelection = (string)e.Message;
            VirtualKeyBoardHelper.AttachTabTip();

        }
       


    


   
      

      

        private void NewBtn_Click(object sender, RoutedEventArgs e)
        {
            var boject = (System.Windows.Controls.Button)sender;
            var b = DataContainer.Instance().menuButtons.Where(a => a.Name == boject.Name).First();
            OnmenuPost?.Invoke(b);
        }

      

       

    

        private void Browser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            bool allow = false;
            foreach (var item in DataContainer.Instance().clickedButton.allowedurlscope)
            {
                if (e.Url.AbsoluteUri.StartsWith(item)){
                    allow = true;break;
                }
            }
            if (!allow)
                e.Cancel=true;

        }

        private void Volver_Click(object sender, RoutedEventArgs e)
        {
            cBrowser.ExecuteScriptAsync("history.back()");

        }





        protected override void OnClosing(CancelEventArgs e)
        {
            //ClearCookies();
            cBrowser.GetCookieManager().DeleteCookies();
            foreach (IWebBrowser b in popus)
                b.GetBrowser().CloseBrowser(false);

            base.OnClosing(e);
            VirtualKeyBoardHelper.RemoveTabTip();
        }

        List<IWebBrowser> popus = new List<IWebBrowser>();

        public bool OnBeforePopup(IWebBrowser pchromiumWebBrowser, IBrowser pbrowser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
        {
            newBrowser = new ChromiumWebBrowser(targetUrl);
            popus.Add(newBrowser);
            return false;
        }

        public void OnAfterCreated(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {

        }

        public bool DoClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {

            browser.CloseBrowser(false);
            return true;
        }

        public void OnBeforeClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {

        }


        private void A_Click(object sender, HtmlElementEventArgs e)
        {
            DataContainer.Instance().MainWndow.ActivityRestart();

            HtmlElement elem = (HtmlElement)sender;
            string biblioteca="";
            if (elem.Document != null)
            {
                var element = elem.Document.GetElementById("library");
                dynamic dom = element.DomElement;
                int index = (int)dom.selectedIndex();
                if (index != -1)
                {
                    biblioteca = element.Children[index].InnerText;
                }
            }

            if (elem.Parent != null)
            {
                if (elem.OuterHtml.ToLower().Contains("</a>"))
                {
                    HtmlElement parent = elem.Parent.Parent;
                    var stat= parent?.GetElementsByTagName("dd").Cast<HtmlElement>().FirstOrDefault(a => a.OuterHtml.ToLower().Contains("holdings_statement"));

                    var contains = stat?.OuterText?.Contains("Biblioteca IAE");
                    if ((contains != null &&  (bool) contains) || biblioteca== "Biblioteca IAE")
                    {
                        var dd = parent?.GetElementsByTagName("dd").Cast<HtmlElement>().FirstOrDefault(a => a.OuterHtml.ToLower().Contains("call_number"));
                        if (dd != null)
                        {
                            string[] datos = Helper.PasilloEstante(dd.InnerHtml);
                            if (datos != null)
                                OnFindBook?.Invoke(datos[0]);
                        }
                    }
                }
            }
        }




        internal class VirtualKeyBoardHelper
        {
            public static void AttachTabTip()
            {
                DataContainer.Instance().MainWndow.ActivityRestart();//Reseteando contador de actividad
               //  OpenAndMoveTabTipWindow();
                //if (!IsStart("TabTip"))
               //   Process.Start("TabTip.exe");
                Process.Start("osk.exe");
            }

            public static void RemoveTabTip()
            {
                // int idproc = GetIdProcces("TabTip"); //cerrando proceso
                int idproc = GetIdProcces("osk"); //cerrando proceso
                if (idproc != -1)
                    Process.GetProcessById(idproc).Kill();
            }

           

            private static bool IsStart(string nameProcces)
            {
                var asProccess = Process.GetProcessesByName(nameProcces);
                return asProccess.Any();
            }

            private static int GetIdProcces(string nameProcces)
            {
                try
                {
                    Process[] asProccess = Process.GetProcessesByName(nameProcces);
                    //foreach (Process pProccess in asProccess.Where(pProccess => pProccess.MainWindowTitle == ""))
                    //    return pProccess.Id;
                    foreach (Process pProccess in asProccess)
                        return pProccess.Id;
                    return -1;
                }
                catch
                {
                    return -1;
                }
            }
        }

    }
}
