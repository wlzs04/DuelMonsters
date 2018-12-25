using Assets.Script.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script
{
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class UserCardData
    {
        [System.Xml.Serialization.XmlAttribute]
        public int cardNo = 0;//编号

        [System.Xml.Serialization.XmlAttribute]
        public int number = 0;//数量
    }

    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class UserData
    {
        [System.Xml.Serialization.XmlAttribute]
        public string playerName = "LL";

        [System.Xml.Serialization.XmlAttribute]
        public float audioValue = 1;

        [System.Xml.Serialization.XmlAttribute]
        public string ip ="127.0.0.1";

        [System.Xml.Serialization.XmlAttribute]
        public int port = 7777;

        public List<UserCardData> userCardList = new List<UserCardData>();
        public List<UserCardData> userCardGroupList=new List<UserCardData>();
    }
}
