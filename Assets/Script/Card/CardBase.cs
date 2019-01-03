using Assets.Script.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.Card
{
    /// <summary>
    /// 卡牌类型
    /// </summary>
    public enum CardType
    {
        Unknown,//未知
        Monster,//怪兽
        Magic,//魔法
        Trap//陷阱
    }

    /// <summary>
    /// 在决斗中卡牌的状态
    /// </summary>
    public enum CardGameState
    {
        Group,//在卡组中
        Hand,//手牌中
        FrontATK,//表侧表示攻击
        FrontDEF,//表侧表示防守
        Back,//覆盖表示
        Tomb,//在墓地中
        Exclusion//被排除在游戏外
    }

    /// <summary>
    /// 卡牌基类
    /// </summary>
    public abstract class CardBase
    {
        protected CardType cardType = CardType.Monster;//卡片类型
        CardGameState cardGameState = CardGameState.Group;
        Sprite image;
        protected string name = "未命名";//名称
        protected int cardNo = 0;//唯一编号，0代表为假卡。
        protected string effect = "没有效果。";

        protected int cardID = 0;//代表在决斗过程中的唯一标志

        protected string info ="";//卡片信息
        protected int limitNumber = 3;//数量限制

        public GameObject cardObject=null;
        
        //标记map，用来放置一些受效果而产生的标记物
        Dictionary<string, object> contentMap = new Dictionary<string, object>();

        public void AddContent(string key, object value)
        {
            if(contentMap.ContainsKey(key))
            {
                Debug.LogError("已存在key：" + key + "value:" + value);
            }
            contentMap[key] = value;
        }

        public object GetContent(string key)
        {
            if(!contentMap.ContainsKey(key))
            {
                return null;
            }
            return contentMap[key];
        }

        public void RemoveContent(string key)
        {
            contentMap.Remove(key);
        }

        public void ClearAllContent()
        {
            contentMap.Clear();
        }

        public int GetID()
        {
            return cardID;
        }

        public void SetID(int ID)
        {
            cardID=ID;
        }

        public string GetName()
        {
            return name;
        }

        public int GetCardNo()
        {
            return cardNo;
        }

        public void SetCardGameState(CardGameState cardGameState)
        {
            this.cardGameState = cardGameState;
        }

        public CardGameState GetCardGameState()
        {
            return cardGameState;
        }

        public string GetEffect()
        {
            return effect;
        }

        public void SetImage(Sprite image)
        {
            this.image = image;
        }

        public Sprite GetImage()
        {
            return image;
        }

        public CardType GetCardType()
        {
            return cardType;
        }

        public string GetCardTypeString()
        {
            return GetStringByCardType(cardType);
        }

        protected abstract void LoadInfo(string info);

        public static CardBase LoadCardFromInfo(int cardNo,string info)
        {
            string item = info.Substring(0,info.IndexOf(Environment.NewLine));
            string key = item.Substring(0, item.IndexOf(':'));
            string value = item.Substring(item.IndexOf(':') + 1);
            CardBase card = null;
            if(key== "CardType")
            {
                switch (value)
                {
                    case "怪兽":
                        card = new MonsterCard();
                        break;
                    case "魔法":
                        card = new MagicCard();
                        break;
                    case "陷阱":
                        card = new TrapCard();
                        break;
                    default:
                        break;
                }
                card.cardNo = cardNo;
                card.info = info;
                card.LoadInfo(info);
            }
            return card;
        }

        public abstract CardBase GetInstance();

        /// <summary>
        /// 将汉字转换为卡片种类
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static CardType GetCardTypeByString(string value)
        {
            CardTypeConfig config = ConfigManager.GetConfigByName("CardType") as CardTypeConfig;
            int count = config.GetRecordCount();
            for (int i = 0; i < count; i++)
            {
                if(config.GetRecordById(i).value == value)
                {
                    return (CardType)i;
                }
            }
            return 0;
        }

        /// <summary>
        /// 将卡片种类转换为汉字
        /// </summary>
        /// <param name="monsterType"></param>
        /// <returns></returns>
        public static string GetStringByCardType(CardType cardType)
        {
            CardTypeConfig config = ConfigManager.GetConfigByName("CardType") as CardTypeConfig;
            return config.GetRecordById((int)cardType).value;
        }
    }
}
