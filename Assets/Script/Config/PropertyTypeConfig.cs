using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Config
{
    /// <summary>
    /// 属性类型配置
    /// </summary>
    public struct PropertyTypeRecord
    {
        [System.Xml.Serialization.XmlAttribute]
        public int id;

        [System.Xml.Serialization.XmlAttribute]
        public string value;
    }

    [System.Xml.Serialization.XmlRoot(ElementName = "PropertyType")]
    public class PropertyTypeConfig : ConfigBase
    {
        [System.Xml.Serialization.XmlElement(ElementName = "RecordList")]
        public List<PropertyTypeRecord> recordList;

        public int GetRecordCount()
        {
            return recordList.Count;
        }

        public PropertyTypeRecord GetRecordById(int id)
        {
            return recordList[id];
        }
    }
}
