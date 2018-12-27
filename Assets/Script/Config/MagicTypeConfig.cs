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
    public struct MagicTypeRecord
    {
        [XmlAttribute]
        public int id;

        [XmlAttribute]
        public string value;
    }

    [XmlRoot("MagicType")]
    public class MagicTypeConfig : ConfigBase
    {
        [XmlArray("RecordList")]
        [XmlArrayItem("Record")]
        public List<MagicTypeRecord> recordList;

        public int GetRecordCount()
        {
            return recordList.Count;
        }

        public MagicTypeRecord GetRecordById(int id)
        {
            return recordList[id];
        }
    }
}
