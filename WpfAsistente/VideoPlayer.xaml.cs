using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfAsistente
{
    /// <summary>
    /// Interaction logic for VideoPlayer.xaml
    /// </summary>
    public partial class VideoPlayer : Window
    {

        private string labelcontent;

        public VideoPlayer()
        {
            InitializeComponent();
        }
       
        private void VideoPlayer_OnInitialized(object sender, EventArgs e)
        {
            DataContainer.Instance().VideoPlayer = this;
            DataContainer.Instance().IsinVideoPlayer = true;
            DataContainer.Instance().Actividad = true;
            Hidelabel();
            DataContainer.Instance().MainWndow.PlayVideo += MainWndow_PlayVideo;
            mediaElement.MediaEnded += MediaElement_MediaEnded;
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (DataContainer.Instance().VideoName == "libroencontrado.mp4")
            {
               ShowLabel(labelcontent);
            }

        }

        private void MainWndow_PlayVideo(object datos)
        {
            if (DataContainer.Instance().VideoName == "libroencontrado.mp4")
            {
                label.Opacity = 0;
                labelcontent = datos.ToString();
                Hidelabel();
                //Posicionar Label
                Helper.Posicionate(label, 40, Width, 15, Height);
                //Resize label
                Helper.PutLabelFOntSize(label, 25, Height);
            }
            PlayVideo(Helper.GetVideo(DataContainer.Instance().VideoName), 29.16);
        }

        private void PlayVideo(string videoruta,double porcientoalto)
        {
          
          ResizeElements(porcientoalto);
          mediaElement.Source = new Uri(videoruta, UriKind.Absolute);
        }

        private void ResizeElements(double porcientoAlto)
        {
            Helper.ResizeScreen(Helper.Porciento(porcientoAlto, SystemParameters.FullPrimaryScreenHeight), SystemParameters.FullPrimaryScreenWidth, this,0,0,0,0);
            Helper.ResizeMedia(Helper.Porciento(porcientoAlto, SystemParameters.FullPrimaryScreenHeight), Width, mediaElement);
        }

        private void Hidelabel()
        {
            BeginStoryboard s = label.FindResource("SotoryBOut") as BeginStoryboard;
            s.Storyboard.Begin();
        }

        private void ShowLabel(string content)
        {
            label.Content = content;
            BeginStoryboard s = label.FindResource("SotoryB") as BeginStoryboard;
            s.Storyboard.Begin();
        }
    }
}
