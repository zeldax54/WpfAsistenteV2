using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
using System.Xml;
using MessageBox = System.Windows.Forms.MessageBox;

namespace WpfAsistente
{
    /// <summary>
    /// Interaction logic for VideosFull.xaml
    /// </summary>
    public partial class VideosFull : Window
    {
        public VideosFull()
        {
            InitializeComponent();
        }

        private void VideosFull_OnInitialized(object sender, EventArgs e)
        {
            Width = SystemParameters.FullPrimaryScreenWidth;
            Height = SystemParameters.FullPrimaryScreenHeight;
            mediaElement.Width = Width;
            mediaElement.Height = Height;
            DataContainer.Instance().VideosFull = this;
            DataContainer.Instance().Actividad = true;
            DataContainer.Instance().IsInVideoFull = true;

            mediaElement.MediaEnded += MediaElement_MediaEnded;
            Volvermenu.Click += Volvermenu_Click;


            if (DataContainer.Instance().Accion == "estadotiempo")
            {
                try
                {
                    HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create("http://xml.tutiempo.net/xml/30994.xml");
                    myRequest.Method = "GET";  // or GET - depends 
                    myRequest.ContentType = "text/xml; encoding=utf-8";

                    var myResponse = myRequest.GetResponse();
                  
                    XmlDocument _xmlDoc = new XmlDocument();

                    using (Stream responseStream = myResponse.GetResponseStream())
                    {
                        _xmlDoc.Load(responseStream);
                    }
                    //Posicionando elementos
                    Helper.Posicionate(borderGrid, 2, Width, 43, Height);
                    Helper.ResizeGrids(new Grid[] { tiempoGrid }, 27, 41.2, Height, Width);

                    Helper.PutLabelFOntSize(labelFecha, 1, Height);
                    Helper.PutLabelFOntSize(labeltemp, 4.6, Height);
                    Helper.PutLabelFOntSize(labelnubes, 2.03, Height);
                    Helper.PutLabelFOntSize(labelhumedad, 2.03, Height);

                    labelFecha.VerticalAlignment = VerticalAlignment.Center;
                    labeltemp.VerticalAlignment = VerticalAlignment.Center;
                    labelnubes.VerticalAlignment = VerticalAlignment.Center;
                    labelhumedad.VerticalAlignment = VerticalAlignment.Center;

                    canvas2.Width = Helper.Porciento(32.3, Width);
                    canvas2.Height = Helper.Porciento(0.34, Height);

                    canvas3.Width = Helper.Porciento(32.3, Width);
                    canvas3.Height = Helper.Porciento(0.34, Height);

                    double dis = (tiempoGrid.Width- canvas2.Width)/2;
                    canvas2.Margin=new Thickness(dis,0,0,0);
                    canvas3.Margin = new Thickness(dis, 0, 0, 0);

                    ImageIcon.Margin=new Thickness(ImageIcon.Width*3,0,0,0);

                    //Datos Generales
                    MyXmlParser parser = new MyXmlParser(_xmlDoc);
                    string fecha = parser.GetDataDay("fecha_larga","dia","fecha") + " de " + DateTime.Now.Year;
                    labelFecha.Content = fecha;
                    string temp = parser.GetDataFromHora("hora", "temperatura");
                    if (temp=="")
                        temp = parser.GetDataDay("temp_maxima", "dia", "fecha");

                    labeltemp.Content = temp + @"°C";
                    labelnubes.Content= parser.GetDataFromHora("hora", "texto");
                    labelhumedad.Content ="Humedad: "+ parser.GetDataFromHora("hora", "humedad")+"%";

                    string linkimg= parser.GetDataFromHora("hora", "icono");
                    if (linkimg != "")
                    {
                        BitmapImage src = new BitmapImage();
                        src.BeginInit();
                        src.UriSource = new Uri(linkimg, UriKind.Absolute);
                        src.CacheOption = BitmapCacheOption.OnLoad;
                        src.EndInit();
                        ImageIcon.Source = src;
                        ImageIcon.Stretch = Stretch.Uniform;
                    }
                    mediaElement.Source = new Uri(Helper.GetVideo(DataContainer.Instance().VideoName), 
                        UriKind.Absolute);
                }
                catch (Exception w)
                {
                    MessageBox.Show(@"Ha ourrido un error mientras se cargaba el estado del tiempo!"+'\n'+w.Message,@"Mensaje",MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    DataContainer.Instance().MainWndow.ErrorBehabior();
                }
            }
            else
            {
                borderGrid.Opacity = 0;
                mediaElement.Source = new Uri(Helper.GetVideo(DataContainer.Instance().VideoName),
                    UriKind.Absolute);
            }
        }

        private void Volvermenu_Click(object sender, RoutedEventArgs e)
        {
            DataContainer.Instance().MainWndow.CloseAndgoMenu();
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
          DataContainer.Instance().MainWndow.CloseAndgoMenu();
          Close();
        }
    }
}
