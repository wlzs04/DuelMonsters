using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Assets.Script.Helper
{
    class XMLHelper
    {
        public static void SaveDataToXML<T>(string path,T obj)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Indent = true;
            StringWriter sw = new StringWriter();
            using (XmlWriter xw = XmlWriter.Create(sw, settings))
            {
                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty);
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(xw, obj, namespaces);
                File.WriteAllText(path, sw.ToString());
            }
        }

        public static T LoadDataFromXML<T>(string path) where T : class
        {
            string xmlStr = File.ReadAllText(path);
            using (StringReader sr = new StringReader(xmlStr))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return serializer.Deserialize(sr) as T;
            }
        }

        public static object LoadDataFromXML(Type type, string path)
        {
            string xmlStr = File.ReadAllText(path);
            using (StringReader sr = new StringReader(xmlStr))
            {
                XmlSerializer serializer = new XmlSerializer(type);
                return serializer.Deserialize(sr);
            }
        }
    }
}
