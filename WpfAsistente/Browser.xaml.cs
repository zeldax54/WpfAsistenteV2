using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Policy;
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
using CefSharp;
using CefSharp.Wpf;
using Microsoft.Win32;
using mshtml;
using MessageBox = System.Windows.MessageBox;


namespace WpfAsistente
{
    /// <summary>
    /// Interaction logic for Browser.xaml
    /// </summary>
    public partial class Browser : Window, ILifeSpanHandler
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

        public Browser(bool pOnlyBrowser=false)
        {
            OnlyBrowser = pOnlyBrowser;
            InitializeComponent();
        }

   

     

        private void Browser_OnInitialized(object sender, EventArgs e)
        {                    

            DataContainer.Instance().IsInBrowsewr = true;
            DataContainer.Instance().Browser = this;
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

            browser.Width= SystemParameters.FullPrimaryScreenWidth;
            browser.Height = Helper.Porciento(browserheight, Height);

          



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
            //Volver.Click += Volver_Click;
            Volver2.Click += Volver_Click;
            //
            //   browser.Navigating += Browser_Navigating;
            browser.LifeSpanHandler = this;
            browser.Address = (DataContainer.Instance().Url);
            browser.JavascriptMessageReceived += OnBrowserJavascriptMessageReceived;
            browser.FrameLoadEnd += OnFrameLoadEnd;
        }

        private void NewBtn_Click(object sender, RoutedEventArgs e)
        {
            var boject = (System.Windows.Controls.Button)sender;
            var b = DataContainer.Instance().menuButtons.Where(a => a.Name == boject.Name).First();
            OnmenuPost?.Invoke(b);
        }

      
        public void OnFrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            if (!isdeleted) {
                string predel = @" document.querySelectorAll('.micrositio-header')[0].style.display = 'none'";
                browser.ExecuteScriptAsync(predel);
                predel = @"document.querySelectorAll('.micrositio-menu-container')[0].style.display = 'none'";
                browser.ExecuteScriptAsync(predel);

                foreach (var item in DataContainer.Instance().clickedButton.deletesections)
                {

                    string script = @" document.querySelectorAll('" +
                        item.tag + "').forEach(element => {if(element.getAttribute('" + item.type + "')=='" + item.name + "')element.style.display = 'none'})";
                    browser.ExecuteScriptAsync(script);

                }


                browser.ExecuteScriptAsync(@"
              document.querySelectorAll('input').forEach(element => element.addEventListener('click', ()=>{CefSharp.PostMessage('asdasd');}));    
           
	         ");

                isdeleted = true;
            }
         
            

            if (e.Frame.IsMain)
            {         
               

            }
        }

        private void OnBrowserJavascriptMessageReceived(object sender, JavascriptMessageReceivedEventArgs e)
        {
            var windowSelection = (string)e.Message;
            VirtualKeyBoardHelper.AttachTabTip();

        }



      /*  private void Browser_NewWindow(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            if (DataContainer.Instance().Accion == "Buscador") {
                var text = browser.Document.GetElementsByTagName("input").Cast<HtmlElement>().FirstOrDefault(a=>a.GetAttribute("type")=="text").GetAttribute("value") ;
                string url = $"https://udesa-primo.hosted.exlibrisgroup.com/primo-explore/search?query=any,contains,{text}&tab=usa_tab&search_scope=usa_scope&sortby=rank&vid=54USA_INST&lang=es_AR&mode=Basic";
                browser.Navigate(url);
            }
            else if (DataContainer.Instance().Accion == "Revista")
            {
                var text = browser.Document.GetElementsByTagName("input").Cast<HtmlElement>().FirstOrDefault(a => a.GetAttribute("type") == "text").GetAttribute("value");
                string url = $"https://udesa-primo.hosted.exlibrisgroup.com/primo-explore/jsearch?journals=title,{text}&query=title,contains,cuba&tab=jsearch_slot&vid=54USA_INST&lang=es_AR&offset=0";
                browser.Navigate(url);
            }
            else
                browser.Navigate(browser.StatusText);
        }*/

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
            browser.ExecuteScriptAsync("history.back()");

        }


    



        protected override void OnClosing(CancelEventArgs e)
        {
            //ClearCookies();
            browser.GetCookieManager().DeleteCookies();
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

        /* private void ClearCookies()

         {

             browser.Navigate("javascript:void((function(){var a,b,c,e,f;f=0;a=document.cookie.split('; ');"

             + "for(e=0;e<a.length&&a[e];e++){f++;for(b='.'+location.host;b;b=b.replace(/^(?:%5C.|[^%5C.]+)/,''))"

             + "{for(c=location.pathname;c;c=c.replace(/.$/,'')){document.cookie=(a[e]+'; domain='+b+'; path='+c+';"

             + "expires='+new Date((new Date()).getTime()-1e11).toGMTString());}}}})())");

         }*/






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
