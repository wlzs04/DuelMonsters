using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Assets.Script.Helper
{
    class XMLHelper
    {
        public static void SaveDataToXML<T>(string path,T obj)
        {
            using (StringWriter sw = new StringWriter())
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(sw, obj);
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
    }
}
