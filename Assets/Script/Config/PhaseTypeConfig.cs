using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Assets.Script.Config
{
    /// <summary>
    /// 决斗回合阶段配置
    /// </summary>
    public struct PhaseTypeRecord
    {
        [XmlAttribute]
        public int id;

        [XmlAttribute]
        public string value;
    }

    [XmlRoot("PhaseType")]
    public class PhaseTypeConfig : ConfigBase
    {
        [XmlArray("RecordList")]
        [XmlArrayItem("Record")]
        public List<PhaseTypeRecord> recordList;

        public int GetRecordCount()
        {
            return recordList.Count;
        }

        public PhaseTypeRecord GetRecordById(int id)
        {
            return recordList[id];
        }
    }
}
