using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using mshtml;
using MessageBox = System.Windows.MessageBox;


namespace WpfAsistente
{
    /// <summary>
    /// Interaction logic for Browser.xaml
    /// </summary>
    public partial class Browser : Window
    {

        public delegate void OnFindBookManaher(string pasillo);
        public event OnFindBookManaher OnFindBook;
        public Browser()
        {
            InitializeComponent();
        }

        private void Browser_OnInitialized(object sender, EventArgs e)
        {
            DataContainer.Instance().IsInBrowsewr = true;
            DataContainer.Instance().Browser = this;

            double altoVideoP = DataContainer.Instance().VideoPlayer.Height;

            Helper.ResizeScreen(SystemParameters.FullPrimaryScreenHeight-altoVideoP,
                 SystemParameters.FullPrimaryScreenWidth, this, altoVideoP, 0, 0, 0);

            canvasContainerLine.Width = SystemParameters.FullPrimaryScreenWidth;
            canvasContainerLine.Height = Helper.Porciento(5, SystemParameters.FullPrimaryScreenHeight);

            BrowserFormsHost.Width= SystemParameters.FullPrimaryScreenWidth;
            BrowserFormsHost.Height = Helper.Porciento(75, Height);

            canvasContainerLine2.Width = SystemParameters.FullPrimaryScreenWidth;
            canvasContainerLine2.Height = Helper.Porciento(5, SystemParameters.FullPrimaryScreenHeight);



            zocalo.Width = SystemParameters.FullPrimaryScreenWidth;
            zocalo.Height = Helper.Porciento(50, Height);
            
            Helper.ResizeLast(new [] {Volver,Volver2}, 3.38,12);
            //Posicionar label

            //Boton Volver
            Volver.Click += Volver_Click;
            Volver2.Click += Volver_Click;
            //
            browser.DocumentCompleted += LoadCompleteEventHandler;
            browser.ScriptErrorsSuppressed = true;
            browser.Navigate(DataContainer.Instance().Url);

        }

        private void Volver_Click(object sender, RoutedEventArgs e)
        {
            DataContainer.Instance().MainWndow.CloseVideoPlayer();
            DataContainer.Instance().MainWndow.ShowMenu();
            Close();

        }

        private void LoadCompleteEventHandler(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                DataContainer.Instance().MainWndow.ActivityRestart();//Reseteando contador de actividad
                if (browser.Document != null)
                {
                    HtmlElementCollection elements = browser.Document.GetElementsByTagName("input");

                    foreach (HtmlElement input in elements)
                    {
                        string x = input.GetAttribute("id");
                        if (input.GetAttribute("type").ToLower() == "text" || input.GetAttribute("type").ToLower() == "password"
                            ||input.GetAttribute("id")== "title-search-input-typeahead-false")
                        {
                            input.GotFocus += (o, args) => VirtualKeyBoardHelper.AttachTabTip();
                            //  input.LostFocus += (o, args) => VirtualKeyBoardHelper.RemoveTabTip();
                        }
                    }
                }
                if (DataContainer.Instance().Accion != "catalogo")
                    return;
                if (browser.Document != null && browser.Document.Title.Contains("Resultados del catálogo"))
                {
                    HtmlElementCollection uls = browser.Document.GetElementsByTagName("form");
                    HtmlElementCollection links = null;
                    HtmlElementCollection inputs = null;
                    HtmlElement resultform = uls.Cast<HtmlElement>().Where(elem => elem.Id == "hitlist").ToList().FirstOrDefault();
                    if (resultform != null && resultform.Children.Count > 0)
                    {
                        links = resultform.GetElementsByTagName("a");
                        // inputs = resultform.GetElementsByTagName("input");
                    }

                    if (links != null && links.Count > 0)
                        foreach (HtmlElement a in links)
                        {
                            a.Click -= A_Click;
                            a.Click += A_Click;
                        }
                }
            }
            catch (Exception)
            {
                
               DataContainer.Instance().MainWndow.ErrorBehabior();
            }
          

        }

        protected override void OnClosing(CancelEventArgs e)
        {
            ClearCookies();
            base.OnClosing(e);
            VirtualKeyBoardHelper.RemoveTabTip();
        }

        private void ClearCookies()

        {

            browser.Navigate("javascript:void((function(){var a,b,c,e,f;f=0;a=document.cookie.split('; ');"

            + "for(e=0;e<a.length&&a[e];e++){f++;for(b='.'+location.host;b;b=b.replace(/^(?:%5C.|[^%5C.]+)/,''))"

            + "{for(c=location.pathname;c;c=c.replace(/.$/,'')){document.cookie=(a[e]+'; domain='+b+'; path='+c+';"

            + "expires='+new Date((new Date()).getTime()-1e11).toGMTString());}}}})())");

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
