using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using mshtml;

namespace WpfAsistente
{
    public static class AccesoManager
    {

        private static Stream GetContext()
        {
            Stream s = new FileStream("Resources\\acceso.txt", FileMode.Open);
            return s;
        }

        public static string[] Acceso()
        {
            StreamReader readder = new StreamReader(GetContext());
            string[] arr = new string[7];
            int iterator = 0;
            while (!readder.EndOfStream)
            {
                var readLine = readder.ReadLine();
                if (readLine != null)
                    arr[iterator] = readLine.Split('`')[1];
                iterator++;
            }
            return arr;
        }


       
    }
}
