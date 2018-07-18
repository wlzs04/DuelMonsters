using Assets.Script.Duel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Script.Card
{
    class DuelCardScript : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        float selectScale = 1.2f;//当鼠标移动到卡牌上时卡牌方法的倍数。

        CardBase card;
        Player ownerPlayer;
        DuelScene duelScene = null;

        void Start()
        {
            duelScene = GameManager.GetSingleInstance().GetDuelScene();
        }

        public void SetCard(CardBase card)
        {
            this.card = card;
        }

        public void SetOwner(Player player)
        {
            ownerPlayer = player;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(ownerPlayer.IsMyTurn())
            {
                if (card.GetCardType() == CardType.Monster)
                {
                    ownerPlayer.CallMonster((MonsterCard)card);
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            gameObject.transform.localScale = new Vector3(selectScale, selectScale, 1);
            if(ownerPlayer==duelScene.myPlayer)
            {
                duelScene.ShowCardInfo(card);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            gameObject.transform.localScale = Vector3.one;
        }
    }
}
