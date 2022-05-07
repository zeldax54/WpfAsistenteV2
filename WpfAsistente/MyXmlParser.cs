using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using mshtml;
using System.Xml.Serialization;
using System.IO;

namespace WpfAsistente
{
  public  class MyXmlParser
  {
      private XmlDocument _documento;
      public MyXmlParser(XmlDocument documento)
      {
          _documento = documento;
      }


      public string GetDataDay(string datoendia,string tag,string tagchild)
      {
            XmlNodeList nodes = _documento.GetElementsByTagName(tag);
            foreach (XmlNode nodo in nodes)
            {
                if (nodo.HasChildNodes)
                {
                    foreach (XmlNode hijo in nodo)
                    {
                        if (hijo.Name == tagchild)
                        {
                          
                            if (CompareFechasAsString(ParseFecha(hijo.InnerXml),DateTime.Now))
                            {
                                var xmlElement = nodo[datoendia];
                                if (xmlElement != null) return xmlElement.InnerXml;
                            }
                        }
                    }
                }
            }
            return "";
      }

      public string GetDataFromHora(string tagname,string data)
      {
          XmlNodeList nodes = _documento.GetElementsByTagName(tagname);
          foreach (XmlNode nodo in nodes)
          {
              if (nodo.HasChildNodes)
              {
                  if (CompareFechasAsString(ParseFecha(GetDataChild(nodo, "fecha")), DateTime.Now))
                  {
                      if (CompareHoras(GetDataChild(nodo, "hora_datos")))
                      {
                          return GetDataChild(nodo, data);
                      }
                  }

              }
          }
          return "";
      }


      public string GetDataChild(XmlNode nodo,string childname)
      {
          if (nodo.HasChildNodes)
          {
              foreach (XmlNode child in nodo.ChildNodes)
              {
                  if (child.Name == childname)
                      return child.InnerXml;
              }
          }
          return "-1";
      }

      private DateTime ParseFecha(string fecha)
      {
            string Format = "yyyyMMdd";
            IFormatProvider culture = new CultureInfo("en-US", true);
            string[] fechas = fecha.Split(new[] { '-' });
            if (fechas[1].Length == 1)
                fechas[1] = "0" + fechas[1];
            if (fechas[2].Length == 1)
                fechas[2] = "0" + fechas[2];
            string fecfaginal = fechas.Aggregate("", (current, f) => current + f);
            return DateTime.ParseExact(fecfaginal, Format, culture);
      }

      private bool CompareFechasAsString(DateTime a, DateTime b)
      {
          return a.ToShortDateString() == b.ToShortDateString();
      }

      private bool CompareHoras(string hora)
      {
          return int.Parse( hora.Split(':')[0]) == DateTime.Now.Hour;
      }

        public List<T> GetTypeListFromXML<T>(string tagchild)
        {
            XmlNodeList nodes = _documento.GetElementsByTagName(tagchild);
            List<T> final = new List<T>();
            foreach (XmlNode nodo in nodes)
            {
                Type typeParameterType = typeof(T);
                System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T));
                using (StringReader sr = new StringReader(nodo.OuterXml))
                {
                    final.Add((T)ser.Deserialize(sr)); ;
                }         
            }
            return final;
        }

        public T Deserialize<T>(string input) where T : class
        {
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T));
            using (StringReader sr = new StringReader(input))
            {
                return (T)ser.Deserialize(sr);
            }
        }

        public string Serialize<T>(T ObjectToSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(ObjectToSerialize.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, ObjectToSerialize);
                return textWriter.ToString();
            }
        }
    }
}
