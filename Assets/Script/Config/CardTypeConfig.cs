using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Assets.Script.Config
{
    /// <summary>
    /// 卡牌类型配置
    /// </summary>
    public struct CardTypeRecord
    {
        [XmlAttribute]
        public int id;

        [XmlAttribute]
        public string value;
    }

    [XmlRoot("CardType")]
    public class CardTypeConfig :ConfigBase
    {
        [XmlArray("RecordList")]
        [XmlArrayItem("Record")]
        public List<CardTypeRecord> recordList;

        public int GetRecordCount()
        {
            return recordList.Count;
        }

        public CardTypeRecord GetRecordById(int id)
        {
            return recordList[id];
        }
    }
}
