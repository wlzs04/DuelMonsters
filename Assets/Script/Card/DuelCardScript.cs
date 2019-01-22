using Assets.Script.Duel;
using Assets.Script.Duel.EffectProcess;
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
        ChangeAttack,//攻击表示
        ChangeDefense,//守备表示
        LaunchEffect,//发动效果 
    }

    public class DuelCardScript : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        float selectScale = 1.2f;//当鼠标移动到卡牌上时卡牌放大的倍数。

        CardBase card;
        Player ownerPlayer;
        DuelScene duelScene = null;
        
        int attackNumber = 0; //攻击次数
        int changeAttackOrDefenseNumber = 0;//攻守转换次数
        bool canShowInfo = false;
        bool isPrepareAttack = false;
        bool showAttackImage = false;
        GameObject cardImage = null;
        GameObject attackImage = null;
        GameObject borderImage = null;
        GameObject attackAndDefenseText = null;
        
        Sprite frontImage = null;

        bool haveBeChosen = false;

        string cardOperationButtonPrefabPath = "Prefab/CardOperationButtonPre";
        Transform operationPanelTransform = null;
        
        void Start()
        {
            duelScene = GameManager.GetSingleInstance().GetDuelScene();
            cardImage = gameObject.transform.GetChild(0).gameObject;
            attackImage = gameObject.transform.GetChild(1).gameObject;
            operationPanelTransform = gameObject.transform.GetChild(2);
            borderImage = gameObject.transform.GetChild(3).gameObject;
            attackAndDefenseText = gameObject.transform.GetChild(4).gameObject;
            attackAndDefenseText.GetComponent<Text>().text = "";
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

        public CardBase GetCard()
        {
            return card;
        }

        /// <summary>
        /// 显示背面
        /// </summary>
        public void ShowBack()
        {
            cardImage.GetComponent<Image>().sprite = GameManager.GetCardBackImage();
        }

        /// <summary>
        /// 显示正面
        /// </summary>
        public void ShowFront()
        {
            cardImage.GetComponent<Image>().sprite = frontImage;
            SetCanShowInfo(true);
        }

        /// <summary>
        /// 设置卡片角度，一般用于防守或对面卡牌
        /// </summary>
        /// <param name="angle"></param>
        public void SetCardAngle(float angle)
        {
            cardImage.transform.localEulerAngles = new Vector3(0, 0, angle);
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

        /// <summary>
        /// 设置父控件
        /// </summary>
        /// <param name="parentTransform"></param>
        public void SetParent(Transform parentTransform)
        {
            transform.SetParent(parentTransform);
        }

        /// <summary>
        /// 设置新位置
        /// </summary>
        /// <param name="newPosition"></param>
        public void SetLocalPosition(Vector3 newPosition)
        {
            transform.localPosition = newPosition;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(card.GetCardGameState() == CardGameState.FrontAttack || 
                card.GetCardGameState() == CardGameState.FrontDefense ||
                card.GetCardGameState() == CardGameState.Back)
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

                if (ownerPlayer.CanBattle())
                {
                    if (ownerPlayer.IsMyPlayer() && !isPrepareAttack && duelScene.currentDuelProcess == DuelProcess.Battle && CanAttack())
                    {
                        Player opponentPlayer = ownerPlayer.GetOpponentPlayer();
                        if (opponentPlayer.CanBeDirectAttacked() &&
                            !opponentPlayer.HaveBeAttackedMonster() &&
                            ownerPlayer.GetCanDirectAttack() && ((MonsterCard)card).CanDirectAttack)
                        {
                            card.GetDuelCardScript().Attack();
                            duelScene.SetAttackAnimationFinishEvent(() =>
                            {
                                ownerPlayer.GetOpponentPlayer().BeDirectAttackedNotify(card.GetID());

                                duelScene.AttackDirect((MonsterCard)card);
                            });
                            duelScene.StartPlayAttackAnimation(GetPosition(), ownerPlayer.GetOpponentPlayer().GetHeartPosition());
                            return;
                        }
                        else
                        {
                            PrepareAttack();
                            duelScene.SetCanChoose(true);
                            duelScene.ChooseCard(card);
                            return;
                        }
                    }
                }
                duelScene.ChooseCard(card);
            }
            else if(card.GetCardGameState() == CardGameState.Tomb ||
                card.GetCardGameState() == CardGameState.Exclusion)
            {
                duelScene.ShowCardList(ownerPlayer, card.GetCardGameState());
            }
            else if(card.GetCardGameState() == CardGameState.Hand)
            {
                if(ownerPlayer==duelScene.myPlayer && ownerPlayer.GetCurrentEffectProcess() is DiscardHandCardEffectProcess)
                {
                    DiscardHandCardEffectProcess discardHandCardEffectProcess = ownerPlayer.GetCurrentEffectProcess() as DiscardHandCardEffectProcess;
                    ownerPlayer.MoveCardToTomb(card);
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

        public void SetAttackAndDefenseText(string text)
        {
            attackAndDefenseText.GetComponent<Text>().text = text;
        }

        public void SetAttackNumber()
        {
            SetAttackNumber(DuelRuleManager.GetMonsterAttackNumberEveryTurn());
        }

        public int GetAttackNumber()
        {
            return attackNumber;
        }
        
        public void SetChangeAttackOrDefenseNumber(int number)
        {
            changeAttackOrDefenseNumber = number;
        }

        public void SetChangeAttackOrDefenseNumber()
        {
            SetChangeAttackOrDefenseNumber(DuelRuleManager.GetMonsterChangeAttackOrDefenseNumberEveryTurn());
        }

        public int GetChangeAttackOrDefenseNumber()
        {
            return changeAttackOrDefenseNumber;
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

        /// <summary>
        /// 转换成攻击表示
        /// </summary>
        public void ChangeToFrontAttack()
        {
            if(changeAttackOrDefenseNumber>0)
            {
                changeAttackOrDefenseNumber--;
                card.SetCardGameState(CardGameState.FrontAttack);
            }
        }

        /// <summary>
        /// 转换成攻击表示
        /// </summary>
        public void ChangeToFrontDefense()
        {
            if (changeAttackOrDefenseNumber > 0)
            {
                changeAttackOrDefenseNumber--;
                card.SetCardGameState(CardGameState.FrontDefense);
            }
        }

        /// <summary>
        /// 设置是否可以显示卡牌详细信息
        /// </summary>
        /// <param name="canShowInfo"></param>
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
                (duelScene.currentDuelProcess == DuelProcess.Main ||
                duelScene.currentDuelProcess == DuelProcess.Second) &&
                ownerPlayer.GetPlayGameState() == PlayGameState.Normal &&
                ownerPlayer.GetCanCallNumber() > 0)
            {
                MonsterCard monsterCard = (MonsterCard)card;
                if (monsterCard.GetLevel() <= DuelRuleManager.GetCallMonsterWithoutSacrificeLevelUpperLimit())
                {
                    return !ownerPlayer.MonsterAreaIsFull();
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
                (duelScene.currentDuelProcess == DuelProcess.Main ||
                duelScene.currentDuelProcess == DuelProcess.Second)&&
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
                (duelScene.currentDuelProcess == DuelProcess.Main ||
                duelScene.currentDuelProcess == DuelProcess.Second) &&
                ownerPlayer.GetPlayGameState() == PlayGameState.Normal &&
                ownerPlayer.GetCanCallNumber() > 0)
            {
                MonsterCard monsterCard = (MonsterCard)card;
                if (monsterCard.GetLevel() <= DuelRuleManager.GetCallMonsterWithoutSacrificeLevelUpperLimit())
                {
                    return !ownerPlayer.MonsterAreaIsFull();
                }
                else if(monsterCard.NeedSacrificeMonsterNumer() > 0 && ownerPlayer.GetCanBeSacrificeMonsterNumber() >= monsterCard.NeedSacrificeMonsterNumer())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断是否可以转换成攻击表示
        /// </summary>
        /// <returns></returns>
        public bool CanChangeToFrontAttack()
        {
            if (card.GetCardType() == CardType.Monster &&
                (card.GetCardGameState() == CardGameState.FrontDefense ||
                card.GetCardGameState() == CardGameState.Back) &&
                GetChangeAttackOrDefenseNumber()>0 &&
                (duelScene.currentDuelProcess == DuelProcess.Main ||
                duelScene.currentDuelProcess == DuelProcess.Second) &&
                ownerPlayer.GetPlayGameState() == PlayGameState.Normal)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断是否可以转换成防守表示
        /// </summary>
        /// <returns></returns>
        public bool CanChangeToFrontDefense()
        {
            if (card.GetCardType() == CardType.Monster &&
                (card.GetCardGameState() == CardGameState.FrontAttack ||
                card.GetCardGameState() == CardGameState.Back) &&
                GetChangeAttackOrDefenseNumber() > 0 &&
                (duelScene.currentDuelProcess == DuelProcess.Main ||
                duelScene.currentDuelProcess == DuelProcess.Second) &&
                ownerPlayer.GetPlayGameState() == PlayGameState.Normal)
            {
                return true;
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
            if (CanChangeToFrontAttack())
            {
                GameObject gameObject = Instantiate(cardOperationButtonPre, operationPanelTransform);
                CardOperationButtonScript cardOperationButtonScript = gameObject.GetComponent<CardOperationButtonScript>();
                cardOperationButtonScript.SetInfo(this, CardOperation.ChangeAttack);
            }
            if (CanChangeToFrontDefense())
            {
                GameObject gameObject = Instantiate(cardOperationButtonPre, operationPanelTransform);
                CardOperationButtonScript cardOperationButtonScript = gameObject.GetComponent<CardOperationButtonScript>();
                cardOperationButtonScript.SetInfo(this, CardOperation.ChangeDefense);
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
                case CardOperation.ChangeAttack:
                    if(CanChangeToFrontAttack())
                    {
                        ChangeToFrontAttack();
                    }
                    break;
                case CardOperation.ChangeDefense:
                    if (CanChangeToFrontDefense())
                    {
                        ChangeToFrontDefense();
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
