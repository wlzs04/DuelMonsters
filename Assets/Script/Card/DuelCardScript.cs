using Assets.Script.Duel;
using Assets.Script.Duel.Rule;
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

        int atkNumber = 0;
        bool canShowInfo = false;
        bool isPrepareATK = false;
        GameObject atkImage = null;

        void Start()
        {
            duelScene = GameManager.GetSingleInstance().GetDuelScene();
            atkImage = gameObject.transform.GetChild(0).gameObject;
        }

        void Update()
        {
            if(isPrepareATK)
            {
                Vector3 mousePosition = Input.mousePosition;
                Vector3 atkImagePosition = atkImage.transform.position;

                atkImage.transform.rotation = Quaternion.Euler(0,0, Mathf.Atan2(atkImagePosition.x - mousePosition.x, mousePosition.y - atkImagePosition.y) *180/Mathf.PI);
            }
        }

        public void SetCard(CardBase card)
        {
            this.card = card;
        }

        public void SetOwner(Player player)
        {
            ownerPlayer = player;
        }

        public Player GetOwner()
        {
            return ownerPlayer;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(ownerPlayer.IsMyTurn())
            {
                if(ownerPlayer.CanCallMonster()&& card.GetCardType() == CardType.Monster)
                {
                    ownerPlayer.CallMonster((MonsterCard)card);
                }
                else if(ownerPlayer.IsMyPlayer()&& !isPrepareATK && duelScene.currentDuelProcess == DuelProcess.Battle && CanATK())
                {
                    atkImage.SetActive(true);
                    isPrepareATK = true;
                    duelScene.SetCanChoose(true);
                    duelScene.ChooseCard(card);
                }
            }
            duelScene.ChooseCard(card);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            gameObject.transform.localScale = new Vector3(selectScale, selectScale, 1);
            if(canShowInfo)
            {
                duelScene.ShowCardInfo(card);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            gameObject.transform.localScale = Vector3.one;
        }

        public void SetATKNumber()
        {
            atkNumber = DuelRule.monsterATKNumberEveryTurn;
        }

        public int GetATKNumber()
        {
            return atkNumber;
        }

        public void SetATKNumber(int number)
        {
            atkNumber = number;
        }

        public void Attack()
        {
            atkNumber--;
            ClearPrepareATKState();
        }

        public bool CanATK()
        {
            return atkNumber > 0;
        }

        public void SetCanShowInfo(bool canShowInfo)
        {
            this.canShowInfo = canShowInfo;
        }

        /// <summary>
        /// 清除攻击准备状态
        /// </summary>
        public void ClearPrepareATKState()
        {
            isPrepareATK = false;
            atkImage.SetActive(false);
        }
    }
}
