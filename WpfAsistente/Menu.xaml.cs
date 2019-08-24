using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using MessageBox = System.Windows.Forms.MessageBox;

namespace WpfAsistente
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
      
      
        public Menu()
        {
            InitializeComponent();
           
        }

        public delegate void OnMenuManager(string accion);
        public event OnMenuManager OnmenuPost;

        private void Menu_OnInitialized(object sender, EventArgs e)
        {

           

            DataContainer.Instance().IsinMenu = true;
            DataContainer.Instance().MenuWndow = this;
            DataContainer.Instance().Actividad = true;

            Width = SystemParameters.FullPrimaryScreenWidth;
            Height = SystemParameters.FullPrimaryScreenHeight;
            mediaElement.Width = Width;
            mediaElement.Height = Height;

            //Eventos
            catalogo.Click += Catalogo_Click;
            infosearch.Click += Infosearch_Click;
            ejournals.Click += Ejournals_Click;
            basedatos.Click += Basedatos_Click;
            serviciosbiblioteca.Click += FullVideo_Click;
            devolucionlibros.Click += FullVideo_Click;
            bibliotecadigital.Click += FullVideo_Click;
            unio.Click += FullVideo_Click;
            estadotiempo.Click += FullVideo_Click;
            selfie.Click += FullVideo_Click;

            mediaElement.Source= new Uri(Helper.GetVideo("menu.mp4"), UriKind.Absolute);
            
            
             //Formateando botones
             Helper.ResizeButtons(new []
             {
                 infosearch,catalogo,ejournals,basedatos,serviciosbiblioteca,devolucionlibros 
             ,bibliotecadigital,unio
             }, 18.75);
            Helper.ResizeLast(new[] {estadotiempo,selfie}, 6.14583,27.7);


            //Ubicando botones
            //First Line start from bottom
            Helper.to_PositionButton(infosearch, 2.5, 15);

            Helper.to_PositionButton(catalogo, 2.5, 35);

            Helper.to_PositionButton(ejournals, 5, 55);

            Helper.to_PositionButton(basedatos, 20, 70);

            //Second Line start from up
            Helper.to_PositionButton(serviciosbiblioteca, 20, 70,true);

            Helper.to_PositionButton(devolucionlibros, 2.5, 55,true);

            Helper.to_PositionButton(bibliotecadigital, 2.5, 35, true);

            Helper.to_PositionButton(unio, 2.5, 15, true);

            //Ultimos (tiempo y selfie)
            Helper.to_PositionButton(estadotiempo, 2.5, 5);
            Helper.to_PositionButton(selfie, 2.5, 5,true);
            //
           
            //

            Storyboard s = (Storyboard)TryFindResource("MStoryboardy");
            s.Begin();
            //Start Timer
            DataContainer.Instance().MainWndow.TimerActivity.Start();
        }

        private void FullVideo_Click(object sender, RoutedEventArgs e)
        {
            var name = ((Button) sender).Name;
            OnmenuPost?.Invoke(name);
        }


        private void Menu_OnClosed(object sender, EventArgs e)
        {
            DataContainer.Instance().IsinMenu = false;
        }

        private void Catalogo_Click(object sender, RoutedEventArgs e)
        {
            OnmenuPost?.Invoke("catalogo");
        }
        private void Infosearch_Click(object sender, RoutedEventArgs e)
        {
            OnmenuPost?.Invoke("infosearch");
        }

        private void Ejournals_Click(object sender, RoutedEventArgs e)
        {
            OnmenuPost?.Invoke("ejournals");
        }
        private void Basedatos_Click(object sender, RoutedEventArgs e)
        {
            OnmenuPost?.Invoke("basedatos");
        }
        
    }
}
