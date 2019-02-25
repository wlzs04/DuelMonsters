using Assets.Script.Card;
using Assets.Script.Config;
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

        [XmlArray("Main")]
        public List<UserCardData> mainCardList = new List<UserCardData>();

        [XmlArray("Extra")]
        public List<UserCardData> extraCardList = new List<UserCardData>();

        [XmlArray("Deputy")]
        public List<UserCardData> deputyCardList = new List<UserCardData>();

        /// <summary>
        /// 获得主卡组列表中卡牌数量
        /// </summary>
        public int GetMainCardCount()
        {
            int count = 0;
            foreach (var item in mainCardList)
            {
                count += item.number;
            }
            return count;
        }

        /// <summary>
        /// 获得副卡组列表中卡牌数量
        /// </summary>
        public int GetExtraCardCount()
        {
            int count = 0;
            foreach (var item in extraCardList)
            {
                count += item.number;
            }
            return count;
        }

        /// <summary>
        /// 获得额外卡组列表中卡牌数量
        /// </summary>
        public int GetDeputyCardCount()
        {
            int count = 0;
            foreach (var item in deputyCardList)
            {
                count += item.number;
            }
            return count;
        }
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
        public bool guessMustWin = false;

        [XmlAttribute]
        public bool showOpponentHandCard = false;

        [XmlAttribute]
        public bool opponentCanCallMonster = false;

        [XmlAttribute]
        public bool opponentCanLaunchEffect = false;

        [XmlAttribute]
        public bool opponentCanAttack = false;

        [XmlAttribute]
        public string ip ="127.0.0.1";

        [XmlAttribute]
        public int port = 7777;

        [XmlArray("UserCardList")]
        public List<UserCardData> userCardList = new List<UserCardData>();

        [XmlArray("UserCardGroupList")]
        public List<UserCardGroup> userCardGroupList=new List<UserCardGroup>();

        /// <summary>
        /// 通过名称获得卡组
        /// </summary>
        /// <param name="cardGroupName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 通过名称删除卡组
        /// </summary>
        /// <param name="cardGroupName"></param>
        public void DeleteCardGroupByName(string cardGroupName)
        {
            foreach (var item in userCardGroupList)
            {
                if (item.cardGroupName == cardGroupName)
                {
                    userCardGroupList.Remove(item);
                    return;
                }
            }
        }

        /// <summary>
        /// 添加新卡组并返回名称
        /// </summary>
        /// <returns></returns>
        public UserCardGroup AddNewCardGroup()
        {
            StringResConfig stringResConfig = ConfigManager.GetConfigByName("StringRes") as StringResConfig;
            string newCardGroupName = stringResConfig.GetRecordById(0).value;

            int index = 1;
            while (GetCardGroupByName(newCardGroupName + index) != null)
            {
                index++;
            }
            newCardGroupName = newCardGroupName + index;

            UserCardGroup userCardGroup = new UserCardGroup();
            userCardGroup.cardGroupName = newCardGroupName;
            userCardGroupList.Add(userCardGroup);

            return userCardGroup;
        }

        /// <summary>
        /// 判断是否拥有此卡
        /// </summary>
        /// <returns></returns>
        public bool IsOwnCard(int cardNo)
        {
            UserCardData userCardData = userCardList.Find(card => card.cardNo== cardNo);
            
            return userCardData!=null;
        }

        /// <summary>
        /// 添加新卡片
        /// </summary>
        public void AddNewCard(int cardNo)
        {
            if(!IsOwnCard(cardNo))
            {
                UserCardData userCardData = new UserCardData();
                userCardData.cardNo = cardNo;
                userCardData.number = 3;
                userCardList.Add(userCardData);
            }
            else
            {
                UserCardData userCardData = userCardList.Find(card => card.cardNo == cardNo);
                userCardData.number += 1;
            }
        }
    }
}
