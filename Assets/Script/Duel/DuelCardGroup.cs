using Assets.Script.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Duel
{
    public class DuelCardGroup
    {
        List<CardBase> cards=new List<CardBase>();

        public void AddCard(int cardNo)
        {
            CardBase card = GameManager.GetSingleInstance().allCardInfoList[cardNo].GetInstance();
            cards.Add(card);
        }

        public void AddCard(int cardNo,int ID)
        {
            CardBase card = GameManager.GetSingleInstance().allCardInfoList[cardNo].GetInstance();
            card.SetID(ID);
            cards.Add(card);
        }

        public List<CardBase> GetCards()
        {
            return cards;
        }

        /// <summary>
        /// 洗牌
        /// </summary>
        public void ShuffleCardGroup()
        {
            int count = cards.Count;
            while (count>1)
            {
                int randomIndex = UnityEngine.Random.Range(0, count);
                count--;
                CardBase card = cards[randomIndex];
                cards[randomIndex] = cards[count];
                cards[count] = card;
            }
        }
    }
}
