using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Assets.Script.Config
{
    /// <summary>
    /// 魔法卡类型配置
    /// </summary>
    public struct TrapTypeRecord
    {
        [XmlAttribute]
        public int id;

        [XmlAttribute]
        public string value;
    }

    [XmlRoot("TrapType")]
    public class TrapTypeConfig : ConfigBase
    {
        [XmlArray("RecordList")]
        [XmlArrayItem("Record")]
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
