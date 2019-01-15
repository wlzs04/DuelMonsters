using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Assets.Script.Config
{
    /// <summary>
    /// 卡牌操作配置
    /// </summary>
    public struct DuelEndReasonRecord
    {
        [XmlAttribute]
        public int id;

        [XmlAttribute]
        public string value;
    }

    [XmlRoot("DuelEndReason")]
    public class DuelEndReasonConfig : ConfigBase
    {
        [XmlArray("RecordList")]
        [XmlArrayItem("Record")]
        public List<DuelEndReasonRecord> recordList;

        public int GetRecordCount()
        {
            return recordList.Count;
        }

        public DuelEndReasonRecord GetRecordById(int id)
        {
            return recordList[id];
        }
    }
}
