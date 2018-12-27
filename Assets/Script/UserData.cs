using Assets.Script.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Assets.Script
{
    /// <summary>
    /// 用户卡牌
    /// </summary>
    public class UserCardData
    {
        [XmlAttribute]
        public int cardNo = 0;//编号

        [XmlAttribute]
        public int number = 0;//数量
    }

    /// <summary>
    /// 用户卡组
    /// </summary>
    public class UserCardGroup
    {
        [XmlAttribute]
        public string cardGroupName = "未命名";//卡组名称

        [XmlArray("UserCardList")]
        public List<UserCardData> userCardList = new List<UserCardData>();
    }

    /// <summary>
    /// 用户数据
    /// </summary>
    public class UserData
    {
        [XmlAttribute]
        public string playerName = "LL";

        [XmlAttribute]
        public float audioValue = 1;

        [XmlAttribute]
        public string ip ="127.0.0.1";

        [XmlAttribute]
        public int port = 7777;

        [XmlArray("UserCardList")]
        public List<UserCardData> userCardList = new List<UserCardData>();

        [XmlArray("UserCardGroupList")]
        public List<UserCardGroup> userCardGroupList=new List<UserCardGroup>();

        public UserCardGroup GetCardGroupByName(string cardGroupName)
        {
            foreach (var item in userCardGroupList)
            {
                if(item.cardGroupName==cardGroupName)
                {
                    return item;
                }
            }
            return null;
        }
    }
}
