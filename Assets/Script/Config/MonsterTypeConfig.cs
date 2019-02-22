using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Assets.Script.Config
{
    /// <summary>
    /// 怪兽种族配置
    /// </summary>
    public struct MonsterTypeRecord
    {
        [XmlAttribute]
        public int id;

        [XmlAttribute]
        public string value;
    }

    [XmlRoot("MonsterType")]
    public class MonsterTypeConfig : ConfigBase
    {
        [XmlArray("RecordList")]
        [XmlArrayItem("Record")]
        public List<MonsterTypeRecord> recordList;

        public int GetRecordCount()
        {
            return recordList.Count;
        }

        public MonsterTypeRecord GetRecordById(int id)
        {
            return recordList[id];
        }

        public override string GetRecordValueById(int id)
        {
            return recordList[id].value;
        }
    }
}
