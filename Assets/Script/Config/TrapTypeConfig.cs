using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Config
{
    /// <summary>
    /// 魔法卡类型配置
    /// </summary>
    public struct TrapTypeRecord
    {
        [System.Xml.Serialization.XmlAttribute]
        public int id;

        [System.Xml.Serialization.XmlAttribute]
        public string value;
    }

    [System.Xml.Serialization.XmlRoot(ElementName = "TrapType")]
    public class TrapTypeConfig : ConfigBase
    {
        [System.Xml.Serialization.XmlElement(ElementName = "RecordList")]
        public List<TrapTypeRecord> recordList;

        public int GetRecordCount()
        {
            return recordList.Count;
        }

        public TrapTypeRecord GetRecordById(int id)
        {
            return recordList[id];
        }
    }
}
