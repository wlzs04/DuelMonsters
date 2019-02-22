using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Assets.Script.Config
{
    /// <summary>
    /// 属性类型配置
    /// </summary>
    public struct PropertyTypeRecord
    {
        [XmlAttribute]
        public int id;

        [XmlAttribute]
        public string value;
    }

    [XmlRoot("PropertyType")]
    public class PropertyTypeConfig : ConfigBase
    {
        [XmlArray("RecordList")]
        [XmlArrayItem("Record")]
        public List<PropertyTypeRecord> recordList;

        public int GetRecordCount()
        {
            return recordList.Count;
        }

        public PropertyTypeRecord GetRecordById(int id)
        {
            return recordList[id];
        }

        public override string GetRecordValueById(int id)
        {
            return recordList[id].value;
        }
    }
}
