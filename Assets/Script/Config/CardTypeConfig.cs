using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Config
{
    /// <summary>
    /// 卡牌类型配置
    /// </summary>
    public struct CardTypeRecord
    {
        [System.Xml.Serialization.XmlAttribute]
        public int id;

        [System.Xml.Serialization.XmlAttribute]
        public string value;
    }

    [System.Xml.Serialization.XmlRoot(ElementName = "CardType")]
    public class CardTypeConfig :ConfigBase
    {
        [System.Xml.Serialization.XmlElement(ElementName = "RecordList")]
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
