using Assets.Script.Duel;
using Assets.Script.Duel.Rule;
using Assets.Script.Net;
using Assets.Script.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

        Sprite backImage = null;
        Sprite frontImage = null;

        bool haveBeChosen = false;

        void Start()
        {
            duelScene = GameManager.GetSingleInstance().GetDuelScene();
            atkImage = gameObject.transform.GetChild(0).gameObject;
            backImage = gameObject.GetComponent<Image>().sprite;
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

        public Vector3 GetPosition()
        {
            return transform.localPosition;
        }

        public void SetCard(CardBase card)
        {
            this.card = card;
            frontImage = card.GetImage();
        }

        /// <summary>
        /// 显示背面
        /// </summary>
        public void ShowBack()
        {
            gameObject.GetComponent<Image>().sprite = backImage;
        }

        /// <summary>
        /// 显示正面
        /// </summary>
        public void ShowFront()
        {
            gameObject.GetComponent<Image>().sprite = frontImage;
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
                if(CanCall())
                {
                    ownerPlayer.CallMonster((MonsterCard)card);
                }
                else if(ownerPlayer.IsMyPlayer()&& !isPrepareATK && duelScene.currentDuelProcess == DuelProcess.Battle && CanATK())
                {
                    Player opponentPlayer = ownerPlayer.GetOpponentPlayer();
                    if (opponentPlayer.CanBeDirectAttacked()&&
                        !opponentPlayer.HaveBeAttackedMonster()&&
                        ownerPlayer.CanDirectAttack&& ((MonsterCard)card).CanDirectAttack)
                    {
                        duelScene.SetAttackAnimationFinishEvent(() => {
                            CAttackDirect cAttackDirect = new CAttackDirect();
                            cAttackDirect.AddContent("cardID", card.GetID());

                            ClientManager.GetSingleInstance().SendProtocol(cAttackDirect);

                            duelScene.AttackDirect((MonsterCard)card); });
                        duelScene.StartPlayAttackAnimation(GetPosition(),ownerPlayer.GetOpponentPlayer().GetHeartPosition());
                    }
                    else
                    {
                        atkImage.SetActive(true);
                        isPrepareATK = true;
                        duelScene.SetCanChoose(true);
                        duelScene.ChooseCard(card);
                    }
                }
                else if(ownerPlayer.GetPlayGameState()==PlayGameState.ChooseSacrifice&&haveBeChosen==false&&card.GetCardType() == CardType.Monster)
                {
                    int tempInt = ((MonsterCard)card).GetCanBeSacrificedNumber();
                    if (tempInt > 0)
                    {
                        if(ownerPlayer.ChooseMonsterAsSacrifice((MonsterCard)card))
                        {
                            haveBeChosen = true;
                        }
                    }
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

        /// <summary>
        /// 判断当前是否可以召唤
        /// </summary>
        /// <returns></returns>
        public bool CanCall()
        {
            if(card.GetCardGameState() == CardGameState.Hand && 
                ownerPlayer.CanCallMonster() && 
                card.GetCardType() == CardType.Monster && 
                ownerPlayer.GetPlayGameState() == PlayGameState.Normal)
            {
                MonsterCard monsterCard = (MonsterCard)card;
                if (monsterCard.GetLevel()<= DuelRule.callMonsterWithoutSacrificeMaxLevel)
                {
                    return true;
                }
                else
                {
                    if(ownerPlayer.GetCanBeSacrificeMonsterNumber()>0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
