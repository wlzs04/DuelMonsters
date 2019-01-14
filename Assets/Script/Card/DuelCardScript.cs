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
    /// <summary>
    /// 对卡片的操作
    /// </summary>
    public enum CardOperation
    {
        NormalCall,//通常召唤
        SacrificCall,//祭品召唤
        BackPlace,//背面放置
        SpecailCall,//特殊召唤
        LaunchEffect,//发动效果 
    }

    public class DuelCardScript : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        float selectScale = 1.2f;//当鼠标移动到卡牌上时卡牌放大的倍数。

        CardBase card;
        Player ownerPlayer;
        DuelScene duelScene = null;

        int attackNumber = 0;
        bool canShowInfo = false;
        bool isPrepareAttack = false;
        bool showAttackImage = false;
        GameObject attackImage = null;
        GameObject borderImage = null;

        Sprite frontImage = null;

        bool haveBeChosen = false;

        string cardOperationButtonPrefabPath = "Prefab/CardOperationButtonPre";
        Transform operationPanelTransform = null;
        
        void Start()
        {
            duelScene = GameManager.GetSingleInstance().GetDuelScene();
            attackImage = gameObject.transform.GetChild(0).gameObject;
            operationPanelTransform = gameObject.transform.GetChild(1);
            borderImage = gameObject.transform.GetChild(2).gameObject;
        }

        void Update()
        {
            if(isPrepareAttack)
            {
                Vector3 mousePosition = Input.mousePosition;
                Vector3 atkImagePosition = attackImage.transform.position;

                attackImage.transform.rotation = Quaternion.Euler(0,0, Mathf.Atan2(atkImagePosition.x - mousePosition.x, mousePosition.y - atkImagePosition.y) *180/Mathf.PI);
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
            gameObject.GetComponent<Image>().sprite = GameManager.GetCardBackImage();
        }

        /// <summary>
        /// 显示正面
        /// </summary>
        public void ShowFront()
        {
            gameObject.GetComponent<Image>().sprite = frontImage;
        }

        /// <summary>
        /// 设置是否显示攻击状态，仅显示攻击图标，并不代表即将进行攻击
        /// </summary>
        public void SetAttackState(bool showAttackImage)
        {
            this.showAttackImage = showAttackImage;
            attackImage.SetActive(this.showAttackImage);
            attackImage.transform.localEulerAngles = Vector3.zero;
        }

        /// <summary>
        /// 准备攻击
        /// </summary>
        public void PrepareAttack()
        {
            isPrepareAttack = true;
            attackImage.transform.localEulerAngles = Vector3.zero;
        }

        /// <summary>
        /// 清除攻击准备状态
        /// </summary>
        public void ClearPrepareAttackState()
        {
            isPrepareAttack = false;
            attackImage.transform.localEulerAngles = Vector3.zero;
        }
        
        /// <summary>
        /// 选中此卡
        /// </summary>
        public void ChooseThisCard()
        {
            borderImage.SetActive(true);
        }

        /// <summary>
        /// 清空选中状态
        /// </summary>
        public void ClearChooseState()
        {
            borderImage.SetActive(false);
        }

        /// <summary>
        /// 清除当前状态
        /// </summary>
        public void ClearCurrentState()
        {
            ClearPrepareAttackState();
            ClearChooseState();
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
            if (ownerPlayer.GetPlayGameState() == PlayGameState.ChooseSacrifice && !haveBeChosen && card.GetCardType() == CardType.Monster)
            {
                int tempInt = ((MonsterCard)card).GetCanBeSacrificedNumber();
                if (tempInt > 0)
                {
                    if (ownerPlayer.CanChooseMonsterAsSacrifice((MonsterCard)card))
                    {
                        haveBeChosen = true;
                        ChooseThisCard();
                        ownerPlayer.TrySacrificeCall();
                    }
                }
            }
            if(ownerPlayer.CanBattle())
            {
                if (ownerPlayer.IsMyPlayer() && !isPrepareAttack && duelScene.currentDuelProcess == DuelProcess.Battle && CanAttack())
                {
                    Player opponentPlayer = ownerPlayer.GetOpponentPlayer();
                    if (opponentPlayer.CanBeDirectAttacked() &&
                        !opponentPlayer.HaveBeAttackedMonster() &&
                        ownerPlayer.CanDirectAttack && ((MonsterCard)card).CanDirectAttack)
                    {
                        duelScene.SetAttackAnimationFinishEvent(() =>
                        {
                            ownerPlayer.GetOpponentPlayer().BeDirectAttackedNotify(card.GetID());

                            duelScene.AttackDirect((MonsterCard)card);
                        });
                        duelScene.StartPlayAttackAnimation(GetPosition(), ownerPlayer.GetOpponentPlayer().GetHeartPosition());
                    }
                    else
                    {
                        PrepareAttack();
                        duelScene.SetCanChoose(true);
                        duelScene.ChooseCard(card);
                    }
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            gameObject.transform.localScale = new Vector3(selectScale, selectScale, 1);
            if(canShowInfo)
            {
                duelScene.ShowCardInfo(card);
            }
            if(ownerPlayer.IsMyTurn())
            {
                RecheckAllowedOperation();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            gameObject.transform.localScale = Vector3.one;
            GameManager.CleanPanelContent(operationPanelTransform);
        }

        public void SetAttackNumber(int number)
        {
            attackNumber = number;
        }

        public void SetAttackNumber()
        {
            SetAttackNumber(DuelRule.monsterATKNumberEveryTurn);
        }

        public int GetAttackNumber()
        {
            return attackNumber;
        }

        /// <summary>
        /// 进行攻击
        /// </summary>
        public void Attack()
        {
            attackNumber--;
            ClearPrepareAttackState();
            if(attackNumber <= 0)
            {
                SetAttackState(false);
            }
        }

        /// <summary>
        /// 判断当前卡牌是否可以攻击
        /// </summary>
        /// <returns></returns>
        public bool CanAttack()
        {
            if (ownerPlayer.IsMyTurn() &&
                duelScene.currentDuelProcess ==DuelProcess.Battle &&
                card.GetCardType() == CardType.Monster &&
                ownerPlayer.GetPlayGameState() == PlayGameState.Normal)
            {
                MonsterCard monsterCard = card as MonsterCard;
                return monsterCard.CanAttack() && attackNumber > 0;
            }
            return false;
        }

        public void SetCanShowInfo(bool canShowInfo)
        {
            this.canShowInfo = canShowInfo;
        }

        /// <summary>
        /// 判断当前是否可以召唤
        /// </summary>
        /// <returns></returns>
        public bool CanCall()
        {
            return CanNormalCall()|| CanSacrificCall()||CanBackPlace();
        }

        /// <summary>
        /// 判断是否可以通常召唤
        /// </summary>
        /// <returns></returns>
        public bool CanNormalCall()
        {
            if (card.GetCardGameState() == CardGameState.Hand &&
                   card.GetCardType() == CardType.Monster &&
                   ownerPlayer.GetPlayGameState() == PlayGameState.Normal &&
                   ownerPlayer.GetCanCallNumber() > 0)
            {
                MonsterCard monsterCard = (MonsterCard)card;
                if (monsterCard.GetLevel() <= DuelRule.callMonsterWithoutSacrificeMaxLevel)
                {
                    bool monsterAreaFull = true;
                    foreach (var item in ownerPlayer.monsterCardArea)
                    {
                        if (item == null)
                        {
                            monsterAreaFull = false;
                            break;
                        }
                    }
                    return !monsterAreaFull;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断是否可以祭品召唤
        /// </summary>
        /// <returns></returns>
        public bool CanSacrificCall()
        {
            if (card.GetCardGameState() == CardGameState.Hand &&
                card.GetCardType() == CardType.Monster &&
                ownerPlayer.GetPlayGameState() == PlayGameState.Normal &&
                   ownerPlayer.GetCanCallNumber() > 0)
            {
                MonsterCard monsterCard = (MonsterCard)card;
                if (monsterCard.NeedSacrificeMonsterNumer()>0 && ownerPlayer.GetCanBeSacrificeMonsterNumber() >= monsterCard.NeedSacrificeMonsterNumer())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断是否可以背面放置
        /// </summary>
        /// <returns></returns>
        public bool CanBackPlace()
        {
            if (card.GetCardGameState() == CardGameState.Hand &&
                   card.GetCardType() == CardType.Monster &&
                   ownerPlayer.GetPlayGameState() == PlayGameState.Normal &&
                   ownerPlayer.GetCanCallNumber() > 0)
            {
                MonsterCard monsterCard = (MonsterCard)card;
                if (monsterCard.GetLevel() <= DuelRule.callMonsterWithoutSacrificeMaxLevel)
                {
                    bool monsterAreaFull = true;
                    foreach (var item in ownerPlayer.monsterCardArea)
                    {
                        if (item == null)
                        {
                            monsterAreaFull = false;
                            break;
                        }
                    }
                    return !monsterAreaFull;
                }
                else if(monsterCard.NeedSacrificeMonsterNumer() > 0 && ownerPlayer.GetCanBeSacrificeMonsterNumber() >= monsterCard.NeedSacrificeMonsterNumer())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 重新检查当前卡牌允许的操作，并添加到操作面板中
        /// </summary>
        public void RecheckAllowedOperation()
        {
            GameManager.CleanPanelContent(operationPanelTransform);

            GameObject cardOperationButtonPre = Resources.Load<GameObject>(cardOperationButtonPrefabPath);

            if (CanNormalCall())
            {
                GameObject gameObject = Instantiate(cardOperationButtonPre, operationPanelTransform);
                CardOperationButtonScript cardOperationButtonScript = gameObject.GetComponent<CardOperationButtonScript>();
                cardOperationButtonScript.SetInfo(this,CardOperation.NormalCall);
            }
            if (CanSacrificCall())
            {
                GameObject gameObject = Instantiate(cardOperationButtonPre, operationPanelTransform);
                CardOperationButtonScript cardOperationButtonScript = gameObject.GetComponent<CardOperationButtonScript>();
                cardOperationButtonScript.SetInfo(this,CardOperation.SacrificCall);
            }
            if(CanBackPlace())
            {
                GameObject gameObject = Instantiate(cardOperationButtonPre, operationPanelTransform);
                CardOperationButtonScript cardOperationButtonScript = gameObject.GetComponent<CardOperationButtonScript>();
                cardOperationButtonScript.SetInfo(this, CardOperation.BackPlace);
            }
        }

        /// <summary>
        /// 对卡牌进行操作
        /// </summary>
        /// <param name="cardOperation"></param>
        public void Operation(CardOperation cardOperation)
        {
            switch (cardOperation)
            {
                case CardOperation.NormalCall:
                case CardOperation.SacrificCall:
                    if (ownerPlayer.CanCallMonster())
                    {
                        if (CanCall())
                        {
                            ownerPlayer.CallMonster((MonsterCard)card);
                        }
                    }
                    break;
                case CardOperation.BackPlace:
                    if (ownerPlayer.CanCallMonster())
                    {
                        if (CanCall())
                        {
                            ownerPlayer.CallMonster((MonsterCard)card, CardGameState.Back);
                        }
                    }
                    break;
                default:
                    Debug.LogError("未知操作命令："+cardOperation);
                    break;
            }
            GameManager.CleanPanelContent(operationPanelTransform);
        }
    }
}
