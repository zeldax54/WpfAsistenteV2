using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.Cuda;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;

namespace WpfAsistente
{
    public static class Helper
    {
        public static string GetFile(List<string> filestoplay)
        {
            int number = filestoplay.Count;
            Random r = new Random();
            int pos = r.Next(0, number - 1);
            return filestoplay[pos];
        }

        public static List<string> ListFiles(string foldername)
        {
            DirectoryInfo i = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + foldername);
            List<string> fin = new List<string>();
            foreach (var f in i.GetFiles())
                fin.Add(f.FullName);
            return fin;
        }

        public static string GetVideo(string videoname)
        {
            DirectoryInfo i = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "Videos");
            foreach (var f in i.GetFiles())
                if (f.Name == videoname)
                    return f.FullName;
            return "";
        }

        public static void ResizeLast(Button[] b, double porcientoAlto, double porcientoAncho)
        {
            //double porcientoAlto = 6.14583;
            //double porcientoAncho = 27.7;
            double tamanoAlto = (porcientoAlto/100)*SystemParameters.FullPrimaryScreenHeight;
            double tamanoAncho = (porcientoAncho/100)*SystemParameters.FullPrimaryScreenWidth;
            foreach (var button in b)
            {
                button.Height = tamanoAlto;
                button.Width = tamanoAncho;
            }
        }

        public static void to_PositionButton(Button b, double percentLeft, double percentBottom, bool fromright = false)
        {
            double bottomDistance = (percentBottom/100)*SystemParameters.FullPrimaryScreenHeight;
            double leftDistance = (percentLeft/100)*SystemParameters.FullPrimaryScreenWidth;
            if (fromright)
                b.SetValue(Canvas.RightProperty, leftDistance);
            else
                b.SetValue(Canvas.LeftProperty, leftDistance);
            b.SetValue(Canvas.BottomProperty, bottomDistance);
        }

        public static void ResizeButtons(Button[] b, double porciento)
        {

            double tamano = (porciento/100)*SystemParameters.FullPrimaryScreenHeight;
            foreach (var button in b)
            {
                button.Width = tamano;
                button.Height = tamano;

            }

        }

        public static void ResizeButton(Button b, double porciento)
        {
            double tamano = (porciento / 100) * SystemParameters.FullPrimaryScreenHeight;               
            b.Width = tamano;
            b.Height = tamano;
        }

        /// <summary>
        /// Calcular porciento
        /// </summary>
        /// <returns></returns>
        public static double Porciento(double porciento, double total)
        {
            return (porciento/100)*total;
        }

        public static void ResizeScreen(double alto, double ancho, Window w, double top, double left, double right,
            double bottom)
        {
            w.Height = alto;
            w.Width = ancho;
            w.Top = top;
            w.Left = left;
        }

        public static void ResizeMedia(double alto, double ancho, MediaElement e)
        {
            e.Width = ancho;
            e.Height = alto;
        }

        public static string[] PasilloEstante(string var)
        {
            string temp = var.Remove(0, 25);
            string tempnum = "";
            int s = 0;
            if (int.TryParse(temp[0].ToString(), out s))
            {
                foreach (char l in temp)
                {
                    if (!IsLetra(l) || l == '.')
                        tempnum += l;
                    else
                        break;
                }
                return tempnum.Split('.');
            }
            return null;
        }

        private static bool IsLetra(char l)
        {
            if (l == 32)
                return true;
            for (char i = 'A'; i <= 'Z'; i++)
            {
                if (l == i)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Posicionar control
        /// </summary>
        /// <param name="c"> Control a posicionar</param>
        /// <param name="percentleft"> porciento a posicionar desde la isquierda</param>
        /// <param name="referenciapercentleft"> Referencia a tomar en cuenta de ancho para percentleft</param>
        /// <param name="perecntbottom">  porciento a posicionar desde abajo</param>
        /// <param name="referenciapercentbottom">  Referencia a tomar en cuenta de alto para perecntbottom</param>
        public static void Posicionate(UIElement c, double percentleft, double referenciapercentleft,
            double perecntbottom, double referenciapercentbottom, bool soloalto=false,bool soloancho=false)
        {
            double leftdistance = Porciento(percentleft, referenciapercentleft);
            double bottomdistance = Porciento(perecntbottom, referenciapercentbottom);
            if(!soloalto)
            c.SetValue(Canvas.LeftProperty, leftdistance);
            if(!soloancho)
            c.SetValue(Canvas.BottomProperty, bottomdistance);
        }

        /// <summary>
        /// Metodo para hacerle resize a los controles
        /// </summary>
        /// <param name="c"> Lista de Controles</param>
        /// <param name="porcientoAlto">Porciento Alto</param>
        /// <param name="porcuentoAncho"> Porciento Ancho</param>
        /// <param name="refporcientoAlto"> Referencia para Porciento Alto</param>
        /// <param name="refporcientoAncho">Referencia para Porciento Ancho</param>
        /// <param name="ifrefalto">Si es referencia al alto se le aplica el mismo ancho y alto sobre el refporcientoAlto</param>
        /// <param name="ifrefancho">Si es referencia al ancho se le aplica el mismo ancho y alto sobre el refporcientoAncho</param>
        public static void ResizeControl(Control[] c, double porcientoAlto, double porcuentoAncho,
            double refporcientoAlto, double refporcientoAncho, bool ifrefalto = false, bool ifrefancho = false)
        {
            double tamanoAlto = (porcientoAlto/100)*refporcientoAlto;
            double tamanoAncho = (porcuentoAncho/100)*refporcientoAncho;
            foreach (var control in c)
            {
                if (ifrefalto)
                {
                    control.Height = tamanoAlto;
                    control.Width = tamanoAlto;
                }
                else if (ifrefancho)
                {
                    control.Height = tamanoAncho;
                    control.Width = tamanoAncho;
                }
                else
                {
                    control.Height = tamanoAlto;
                    control.Width = tamanoAncho;
                }

            }
        }

        public static void PutLabelFOntSize(Label l, double porciento, double refporciento)
        {
            l.FontSize = Porciento(porciento, refporciento);
        }


        public static void PutTextFontSize(TextBlock t, double porciento, double refporciento)
        {
            t.FontSize = Porciento(porciento, refporciento);
        }



        public static void ResizeGrids(Grid[] c, double porcientoAlto, double porcuentoAncho,
            double refporcientoAlto, double refporcientoAncho, bool ifrefalto = false, bool ifrefancho = false)
        {
            double tamanoAlto = (porcientoAlto/100)*refporcientoAlto;
            double tamanoAncho = (porcuentoAncho/100)*refporcientoAncho;
            foreach (var control in c)
            {
                if (ifrefalto)
                {
                    control.Height = tamanoAlto;
                    control.Width = tamanoAlto;
                }
                else if (ifrefancho)
                {
                    control.Height = tamanoAncho;
                    control.Width = tamanoAncho;
                }
                else
                {
                    control.Height = tamanoAlto;
                    control.Width = tamanoAncho;
                }

            }
        }

        public static void ResiseImage(Image control, double porcientoAlto, double porcuentoAncho,
            double refporcientoAlto, double refporcientoAncho, bool ifrefalto = false, bool ifrefancho = false)

        {
            double tamanoAlto = (porcientoAlto / 100) * refporcientoAlto;
            double tamanoAncho = (porcuentoAncho / 100) * refporcientoAncho;
          
                if (ifrefalto)
                {
                    control.Height = tamanoAlto;
                    control.Width = tamanoAlto;
                }
                else if (ifrefancho)
                {
                    control.Height = tamanoAncho;
                    control.Width = tamanoAncho;
                }
                else
                {
                    control.Height = tamanoAlto;
                    control.Width = tamanoAncho;
                }
        }



        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        public static BitmapSource ToBitmapSource(IImage image)
        {
            using (System.Drawing.Bitmap source = image.Bitmap)
            {
                IntPtr ptr = source.GetHbitmap(); //obtain the Hbitmap

                BitmapSource bs = System.Windows.Interop
                  .Imaging.CreateBitmapSourceFromHBitmap(
                  ptr,
                  IntPtr.Zero,
                  Int32Rect.Empty,
                  System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(ptr); //release the HBitmap
                return bs;
            }
        }

        public static BitmapSource ToBitmapSourceFromBitmap(Bitmap image)
        {
            using (System.Drawing.Bitmap source = image)
            {
                IntPtr ptr = source.GetHbitmap(); //obtain the Hbitmap

                BitmapSource bs = System.Windows.Interop
                  .Imaging.CreateBitmapSourceFromHBitmap(
                  ptr,
                  IntPtr.Zero,
                  Int32Rect.Empty,
                  System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(ptr); //release the HBitmap
                return bs;
            }
        }



        public static void PosicionateOnlyHeight(UIElement c, 
         double perecntbottom, double referenciapercentbottom)
        {
       
            double bottomdistance = Porciento(perecntbottom, referenciapercentbottom);
         
                c.SetValue(Canvas.BottomProperty, bottomdistance);
        }

        /// <summary>
        /// Finds a Child of a given item in the visual tree. 
        /// </summary>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="childName">x:Name or Name of child. </param>
        /// <returns>The first parent item that matches the submitted type parameter. 
        /// If not matching item can be found, 
        /// a null parent is being returned.</returns>
        public static T FindChild<T>(DependencyObject parent, string childName)
           where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        public static List<TypeClass.Button> GetFooter()
        {
            List<string> footerButtons = new List<string>() { "estadotiempo", "atraspie", "mailbutton", "selfie", "tutoriales" };
            return DataContainer.Instance().menuButtons.
                Where(o => footerButtons.Contains(o.Name.ToLower())).ToList();
        }


        public static List<System.Windows.Controls.Button> SetPosition(List<TypeClass.Button> buttons, ref Canvas container, Style style, ref Storyboard sb,
            Action<string, object> RegisterName,
            bool isdefpos = false, decimal startpos = 0)
        {

            List<System.Windows.Controls.Button> lista = new List<Button>();
            foreach (var boton in buttons)
            {
                System.Windows.Controls.Button newBtn = new System.Windows.Controls.Button();
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
                newBtn.Style = style;
                if (isdefpos)
                {
                    Helper.to_PositionButton(newBtn, (double)startpos, 50, false);
                    startpos += 7;
                }
                else
                    Helper.to_PositionButton(newBtn, (double)boton.Postition, (double)boton.FromBotton, boton.FromRight);
                Helper.ResizeButtonProportional(newBtn, (double)boton.Size);
                /*Animation 1*/
                System.Windows.Media.Animation.DoubleAnimation doubleAnimationOpacity = new DoubleAnimation();
                doubleAnimationOpacity.BeginTime = TimeSpan.FromMilliseconds(0);
                doubleAnimationOpacity.AccelerationRatio = 0.3;
                doubleAnimationOpacity.DecelerationRatio = .4;
                doubleAnimationOpacity.From = 0;
                doubleAnimationOpacity.To = 1;
                sb.Children.Add(doubleAnimationOpacity);
                Storyboard.SetTargetName(doubleAnimationOpacity, boton.Name);
                Storyboard.SetTargetProperty(doubleAnimationOpacity, new PropertyPath(System.Windows.Shapes.Rectangle.OpacityProperty));
                container.Children.Add(newBtn);

                lista.Add(newBtn);

            }
            return lista;


        }
        public static void ResizeButtonProportional(Button b, double porciento)
        {
            double tamanoHeight = (porciento / 100) * SystemParameters.FullPrimaryScreenHeight;
            double tamanoWidth = (porciento / 100) * SystemParameters.FullPrimaryScreenWidth;
            b.Width = tamanoWidth;
            b.Height = tamanoHeight;
        }

    }
}
