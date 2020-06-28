using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml;
using System.Xml.Linq;
using MessageBox = System.Windows.Forms.MessageBox;

using System.Globalization;
using System.ComponentModel;

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

        public delegate void OnMenuManager(WpfAsistente.TypeClass.Button button);
        public event OnMenuManager OnmenuPost;
        private BackgroundWorker worker=new BackgroundWorker();
        private Timer timer;
        private bool _enableRealTimeEdit = false;

        private void Menu_OnInitialized(object sender, EventArgs e)
        {

            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            
            DataContainer.Instance().IsinMenu = true;
            DataContainer.Instance().MenuWndow = this;
            DataContainer.Instance().Actividad = true;

            Width = SystemParameters.FullPrimaryScreenWidth;
            Height = SystemParameters.FullPrimaryScreenHeight;
            mediaElement.Width = Width;
            mediaElement.Height = Height;
            Storyboard s = (Storyboard)TryFindResource("MStoryboardy");

            //Eventos           
            mediaElement.Source= new Uri(Helper.GetVideo("menu.mp4"), UriKind.Absolute);
            //Dynamic buttons                  
            XmlDocument _xmlDoc = new XmlDocument();
            _xmlDoc.Load(@"localconfigs.xml");
            MyXmlParser parser = new MyXmlParser(_xmlDoc);
            if(DataContainer.Instance().menuButtons==null)
              DataContainer.Instance().menuButtons =  parser.GetTypeListFromXML<TypeClass.Button>("Button");
            var botones = DataContainer.Instance().menuButtons;
            var sb = new Storyboard();
            NameScope.SetNameScope(this, new NameScope());
            bool isdefpos = DataContainer.Instance().DefaultButtonPos;
            decimal startpos = 0;
          
            foreach (var boton in botones)
            {
                if (boton.Name.ToLower() == "atras")
                    continue;
                Button newBtn = new Button();
                newBtn.Name = boton.Name;
                RegisterName(newBtn.Name, newBtn);
                newBtn.Content = "";
                newBtn.Opacity = 0;
                Image img = new Image();
                string strUri2 = Directory.GetCurrentDirectory() + $"/Img/MenuButtons/{boton.ImageName}";
                img.Source = new BitmapImage(new Uri(strUri2));
                img.Stretch = Stretch.Uniform;
                newBtn.Background = new ImageBrush(img.Source);
                newBtn.RenderTransformOrigin = new Point(0.5, 0.5);
                newBtn.BorderThickness = new Thickness(0);
                newBtn.Style = (Style)Resources["MyButtonStyle"];
                if (isdefpos)
                {
                    Helper.to_PositionButton(newBtn, (double)startpos, 50, false);
                    startpos += 7;
                }
                else
                    Helper.to_PositionButton(newBtn, (double)boton.Postition, (double)boton.FromBotton, boton.FromRight);
                Helper.ResizeButtonProportional(newBtn, (double)boton.Size);
                /*Animation 1*/
                DoubleAnimation doubleAnimationOpacity = new DoubleAnimation();
                doubleAnimationOpacity.BeginTime = TimeSpan.FromMilliseconds((double)boton.startAnimationTime);
                doubleAnimationOpacity.AccelerationRatio = 0.3;
                doubleAnimationOpacity.DecelerationRatio = .4;
                doubleAnimationOpacity.From = 0;
                doubleAnimationOpacity.To = 100;
                sb.Children.Add(doubleAnimationOpacity);
                Storyboard.SetTargetName(doubleAnimationOpacity, boton.Name);
                Storyboard.SetTargetProperty(doubleAnimationOpacity, new PropertyPath(Rectangle.OpacityProperty));
               if (!newBtn.Name.ToLower().Contains("zocalo"))
                    newBtn.Click += NewBtn_Click;
                this.canvasContainer.Children.Add(newBtn);
            }
            //Formateando botones last       
            //  Helper.ResizeLast(new[] {estadotiempo,selfie}, 6.14583,27.7);


            sb.Begin(this,true);         
            DataContainer.Instance().MainWndow.TimerActivity.Start();
            worker.DoWork += Worker_DoWork;
            
            this.KeyDown += Menu_KeyDown;
            worker.RunWorkerAsync();
        }

        private void Menu_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.GetKeyStates(Key.Q) == KeyStates.Down) {
                _enableRealTimeEdit = !_enableRealTimeEdit;
                CreateHelperButon(!_enableRealTimeEdit);
            } 
               
        }




        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (_enableRealTimeEdit)
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        XmlDocument _xmlDoc = new XmlDocument();
                        _xmlDoc.Load(@"localconfigs.xml");
                        MyXmlParser parser = new MyXmlParser(_xmlDoc);
                        var botones = parser.GetTypeListFromXML<TypeClass.Button>("Button");
                        DataContainer.Instance().menuButtons = botones;
                        foreach (var boton in botones)
                        {
                            var button = (Button)this.FindName(boton.Name);
                            //  Button button = Helper.FindChild<Button>(this.canvasContainer,boton.Name);
                            Helper.to_PositionButton(button, (double)boton.Postition, (double)boton.FromBotton, boton.FromRight);
                            Helper.ResizeButtonProportional(button, (double)boton.Size);
                        }
                    }));
                }
            }
            catch (Exception ee)
            {
                System.Diagnostics.Debug.WriteLine(ee.Message);
            }                  
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            timer = new Timer();
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void NewBtn_Click(object sender, RoutedEventArgs e)
        {
            var boject = (Button)sender;
            var b = DataContainer.Instance().menuButtons.Where(a => a.Name == boject.Name).First();
            OnmenuPost?.Invoke(b);
        }



        private void FullVideo_Click(object sender, RoutedEventArgs e)
        {
            var b = ((TypeClass.Button) sender);
            OnmenuPost?.Invoke(b);
        }


        private void Menu_OnClosed(object sender, EventArgs e)
        {
            DataContainer.Instance().IsinMenu = false;
        }
      

        void CreateHelperButon(bool destroy) {            
            Button newBtn = new Button();
            Button foundTextBox =
              Helper.FindChild<Button>(this.canvasContainer, "helpbutton");
            if (foundTextBox == null)
            {
                newBtn.Name = "helpbutton";
                this.RegisterName(newBtn.Name, newBtn);
                newBtn.Content = "";
                newBtn.Opacity = 0;
                Image img = new Image();
                string strUri2 = Directory.GetCurrentDirectory() + $"/Img/helpers/WX_circle_green.png";
                img.Source = new BitmapImage(new Uri(strUri2));
                img.Stretch = Stretch.Uniform;
                newBtn.Background = new ImageBrush(img.Source);
                newBtn.RenderTransformOrigin = new Point(0.5, 0.5);
                newBtn.BorderThickness = new Thickness(0);
                newBtn.Style = (Style)this.Resources["MyButtonStyle"];
                newBtn.Opacity = 100;
                Helper.to_PositionButton(newBtn, 50, 50);
                Helper.ResizeButton(newBtn, 10);
                this.canvasContainer.Children.Add(newBtn);
            }
            else                
               foundTextBox.Opacity = destroy?0:100;           
        }     

    }
}
