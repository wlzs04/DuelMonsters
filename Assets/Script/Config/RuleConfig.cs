using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Assets.Script.Config
{
    /// <summary>
    /// 规则配置
    /// </summary>
    public struct RuleRecord
    {
        [XmlAttribute]
        public int id;

        [XmlAttribute]
        public int value;

        [XmlAttribute]
        public string description;
    }

    [XmlRoot("Rule")]
    public class RuleConfig : ConfigBase
    {
        [XmlArray("RecordList")]
        [XmlArrayItem("Record")]
        public List<RuleRecord> recordList;

        public int GetRecordCount()
        {
            return recordList.Count;
        }

        public RuleRecord GetRecordById(int id)
        {
            return recordList[id];
        }
    }


}
