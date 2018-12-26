using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Config
{
    /// <summary>
    /// 怪兽种族配置
    /// </summary>
    public struct MonsterTypeRecord
    {
        [System.Xml.Serialization.XmlAttribute]
        public int id;

        [System.Xml.Serialization.XmlAttribute]
        public string value;
    }

    [System.Xml.Serialization.XmlRoot(ElementName = "MonsterType")]
    public class MonsterTypeConfig : ConfigBase
    {
        [System.Xml.Serialization.XmlElement(ElementName = "RecordList")]
        public List<MonsterTypeRecord> recordList;

        public int GetRecordCount()
        {
            return recordList.Count;
        }

        public MonsterTypeRecord GetRecordById(int id)
        {
            return recordList[id];
        }
    }
}
