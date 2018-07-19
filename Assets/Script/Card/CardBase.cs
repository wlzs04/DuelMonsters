using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.Card
{
    public enum CardType
    {
        Unknown,//未知
        Monster,//怪兽
        Magic,//魔法
        Trap//陷阱
    }

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

        internal void SetImage(Sprite image)
        {
            this.image = image;
        }

        internal Sprite GetImage()
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

        public abstract void LoadInfo(string info);

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
            switch (value)
            {
                case "怪兽":
                    return CardType.Monster;
                case "魔法":
                    return CardType.Magic;
                case "陷阱":
                    return CardType.Trap;
                default:
                    return CardType.Unknown;
            }
        }

        /// <summary>
        /// 将卡片种类转换为汉字
        /// </summary>
        /// <param name="monsterType"></param>
        /// <returns></returns>
        public static string GetStringByCardType(CardType cardType)
        {
            switch (cardType)
            {
                case CardType.Monster:
                    return "怪兽";
                case CardType.Magic:
                    return "魔法";
                case CardType.Trap:
                    return "陷阱";
                default:
                    return "未知";
            }
        }
    }
}
