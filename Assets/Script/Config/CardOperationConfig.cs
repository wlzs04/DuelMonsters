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
    public struct CardOperationRecord
    {
        [XmlAttribute]
        public int id;

        [XmlAttribute]
        public string value;
    }

    [XmlRoot("CardOperation")]
    public class CardOperationConfig : ConfigBase
    {
        [XmlArray("RecordList")]
        [XmlArrayItem("Record")]
        public List<CardOperationRecord> recordList;

        public int GetRecordCount()
        {
            return recordList.Count;
        }

        public CardOperationRecord GetRecordById(int id)
        {
            return recordList[id];
        }
    }
}
