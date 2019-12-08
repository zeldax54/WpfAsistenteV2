using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfAsistente
{
    /// <summary>
    /// Interaction logic for SearchWindow.xaml
    /// </summary>
    public partial class SearchWindow : Window
    {
        public SearchWindow()
        {
            InitializeComponent();
        }

  
        private List<string> _buscandoVideos;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    

        private void MainWindow_OnInitialized(object sender, EventArgs e)
        {
            DataContainer.Instance().IsSearching = true;
            Width = SystemParameters.FullPrimaryScreenWidth;
            Height = SystemParameters.FullPrimaryScreenHeight;
            mediaElement.Width = Width;
            mediaElement.Height = Height;
            _buscandoVideos = Helper.ListFiles("enBusqueda");
            mediaElement.MediaEnded += MediaElement_MediaEnded;
            mediaElement.Source = new Uri(Helper.GetFile(_buscandoVideos), UriKind.Absolute);
            mediaElement.MouseDown += MediaElement_MouseDown;
        }

        private void MediaElement_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DataContainer.Instance().MainWndow.IsPerson();
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            mediaElement.Source = new Uri(Helper.GetFile(_buscandoVideos), UriKind.Absolute);
        }

        private void SearchWindow_OnClosing(object sender, CancelEventArgs e)
        {
            DataContainer.Instance().IsSearching = false;
        }
    }
}
