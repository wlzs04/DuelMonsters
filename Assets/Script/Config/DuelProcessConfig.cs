using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Assets.Script.Config
{
    /// <summary>
    /// 决斗回合流程配置
    /// </summary>
    public struct DuelProcessRecord
    {
        [XmlAttribute]
        public int id;

        [XmlAttribute]
        public string value;
    }

    [XmlRoot("DuelProcess")]
    public class DuelProcessConfig : ConfigBase
    {
        [XmlArray("RecordList")]
        [XmlArrayItem("Record")]
        public List<DuelProcessRecord> recordList;

        public int GetRecordCount()
        {
            return recordList.Count;
        }

        public DuelProcessRecord GetRecordById(int id)
        {
            return recordList[id];
        }
    }
}
