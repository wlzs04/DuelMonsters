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
using XLua;

namespace Assets.Script.Card
{
    /// <summary>
    /// 对卡片的操作
    /// </summary>
    public enum CardOperation
    {
        NormalCall,//通常召唤
        SacrificCall,//祭品召唤
        BackPlaceToMonsterArea,//背面放置到怪兽区
        FlipCall,//反转召唤
        SpecailCall,//特殊召唤
        ChangeAttack,//攻击表示
        ChangeDefense,//守备表示
        LaunchEffect,//发动效果 
        BackPlaceToMagicTrapArea,//背面放置到魔法陷阱区
    }

    public class DuelCardScript : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        float selectScale = 1.2f;//当鼠标移动到卡牌上时卡牌放大的倍数。

        CardBase card;
        Player ownerPlayer;
        DuelScene duelScene = null;
        
        bool canShowInfo = false;
        bool isPrepareAttack = false;
        bool showAttackImage = false;
        GameObject cardImage = null;
        GameObject attackImage = null;
        GameObject borderImage = null;
        GameObject attackAndDefenseText = null;
        
        Sprite frontImage = null;

        bool haveBeChosen = false;

        Transform operationPanelTransform = null;

        CardBase launchEffectCard=null;
        Action<CardBase, CardBase> clickCallback = null;

        void Awake()
        {
            duelScene = GameManager.GetDuelScene();
            cardImage = gameObject.transform.GetChild(0).gameObject;
            attackImage = gameObject.transform.GetChild(1).gameObject;
            borderImage = cardImage.transform.GetChild(0).gameObject;
            operationPanelTransform = gameObject.transform.GetChild(2).transform.GetChild(0);
            attackAndDefenseText = gameObject.transform.GetChild(2).transform.GetChild(1).gameObject;
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
        /// 设置当前卡牌状态，由CardBase类调用，仅做卡牌位置，方向，正反等显示处理
        /// </summary>
        /// <param name="cardGameState"></param>
        public void SetCardGameState(CardGameState cardGameState,int index = 0)
        {
            switch (cardGameState)
            {
                case CardGameState.Group:
                    {
                        transform.SetParent(duelScene.duelBackImage.transform);
                        ShowBack();
                        if (GetOwner().IsMyPlayer())
                        {
                            SetAnchorPosition(DuelCommonValue.myCardGroupPositionAnchor);
                        }
                        else
                        {
                            SetAnchorPosition(DuelCommonValue.opponentCardGroupPositionAnchor);
                        }
                        SetCardAngle(0);
                        break;
                    }
                case CardGameState.Hand:
                    SetParent(GetOwner().GetHandPanel().transform);
                    if (GetOwner() == duelScene.myPlayer)
                    {
                        ShowFront();
                        SetCanShowInfo(true);
                    }
                    else if (GetOwner() == duelScene.opponentPlayer)
                    {
                        if (GetOwner() is ComputerPlayer && GameManager.GetSingleInstance().GetUserData().showOpponentHandCard)
                        {
                           SetCanShowInfo(true);
                        }
                    }
                    break;
                case CardGameState.FrontAttack:
                    {
                        SetParent(duelScene.duelBackImage.transform);
                        SetAnchorPosition(duelScene.GetMonsterPositionAnchorByIndex(ownerPlayer, index));
                        ShowFront();
                        SetCardAngle(0);
                        break;
                    }
                case CardGameState.FrontDefense:
                    {
                        SetParent(duelScene.duelBackImage.transform);
                        SetAnchorPosition(duelScene.GetMonsterPositionAnchorByIndex(ownerPlayer, index));
                        ShowFront();
                        SetCardAngle(90);
                        break;
                    }
                case CardGameState.Front:
                    {
                        SetParent(duelScene.duelBackImage.transform);
                        SetAnchorPosition(duelScene.GetMagicTrapPositionAnchorByIndex(ownerPlayer, index));
                        ShowFront();
                        SetCardAngle(0);
                        break;
                    }
                case CardGameState.Back:
                    {
                        ShowBack();
                        SetParent(duelScene.duelBackImage.transform);
                        if (card.GetCardType() == CardType.Monster)
                        {
                            SetAnchorPosition(duelScene.GetMonsterPositionAnchorByIndex(ownerPlayer, index));
                            SetCardAngle(90);
                        }
                        else if (card.GetCardType() == CardType.Magic || card.GetCardType() == CardType.Trap)
                        {
                            SetAnchorPosition(duelScene.GetMagicTrapPositionAnchorByIndex(ownerPlayer, index));
                            SetCardAngle(0);
                        }
                        break;
                    }
                case CardGameState.Tomb:
                    ShowFront();
                    SetParent(duelScene.duelBackImage.transform);
                    SetCardAngle(0);
                    transform.SetParent(duelScene.duelBackImage.transform);
                    if (GetOwner().IsMyPlayer())
                    {
                        SetAnchorPosition(DuelCommonValue.myTombPositionAnchor);
                    }
                    else
                    {
                        SetAnchorPosition(DuelCommonValue.opponentTombPositionAnchor);
                    }
                    SetCardAngle(0);
                    break;
                default:
                    Debug.LogError("未知CardGameState：" + cardGameState);
                    break;
            }
            ResetAttackAndDefenseText();
        }

        /// <summary>
        /// 设置卡牌锚点
        /// </summary>
        public void SetAnchorPosition(Vector2 vector2)
        {
            ((RectTransform)transform).anchorMin = vector2;
            ((RectTransform)transform).anchorMax = vector2;
            ((RectTransform)transform).offsetMin = Vector2.zero;
            ((RectTransform)transform).offsetMax = Vector2.zero;
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
            //根据卡牌拥有者是否为己方玩家，旋转卡牌方向
            if (GetOwner() != GetDuelScene().myPlayer)
            {
                cardImage.transform.localEulerAngles += new Vector3(0, 0, 180);
            }
        }

        /// <summary>
        /// 设置是否显示攻击状态，仅显示攻击图标，并不代表即将进行攻击，或攻击结束
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

        public DuelScene GetDuelScene()
        {
            return duelScene;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(clickCallback!=null)
            {
                clickCallback(launchEffectCard,card);
                return;
            }
            if(card.GetCardGameState() == CardGameState.FrontAttack || 
                card.GetCardGameState() == CardGameState.FrontDefense ||
                card.GetCardGameState() == CardGameState.Back)
            {
                //选择祭品
                if (card.GetCardType() == CardType.Monster && 
                    !haveBeChosen&&
                    ownerPlayer.GetCurrentEffectProcess() is CallMonsterEffectProcess)
                {
                    int tempInt = card.GetCanBeSacrificedNumber();
                    if (tempInt > 0)
                    {
                        CallMonsterEffectProcess callMonsterEffectProcess = ownerPlayer.GetCurrentEffectProcess() as CallMonsterEffectProcess;
                        
                        if (callMonsterEffectProcess.CanChooseMonsterAsSacrifice(card))
                        {
                            haveBeChosen = true;
                            ChooseThisCard();
                            callMonsterEffectProcess.TrySacrificeCall();
                        }
                    }
                }
                //选择我方主动攻击的怪兽
                if (ownerPlayer.CanBattle() && !(ownerPlayer.GetCurrentEffectProcess() is AttackEffectProcess))
                {
                    if (ownerPlayer.IsMyPlayer() && !isPrepareAttack && duelScene.GetCurrentDuelProcess() == DuelProcess.Battle && CanAttack())
                    {
                        Player opponentPlayer = ownerPlayer.GetOpponentPlayer();
                        if (opponentPlayer.CanBeDirectAttacked() &&
                            !opponentPlayer.HaveBeAttackedMonster() &&
                            ownerPlayer.GetCanDirectAttack() && card.CanDirectAttack())
                        {
                            AttackEffectProcess attackEffectProcess = new AttackEffectProcess(card, null, ownerPlayer);
                            ownerPlayer.AddEffectProcess(attackEffectProcess);

                            return;
                        }
                        else
                        {
                            PrepareAttack();

                            AttackEffectProcess attackEffectProcess = new AttackEffectProcess(card, null, ownerPlayer);
                            ownerPlayer.AddEffectProcess(attackEffectProcess);
                            return;
                        }
                    }
                }
                //选择对方被攻击的怪兽
                if (ownerPlayer.GetOpponentPlayer().GetCurrentEffectProcess() is AttackEffectProcess)
                {
                    AttackEffectProcess attackEffectProcess = ownerPlayer.GetOpponentPlayer().GetCurrentEffectProcess() as AttackEffectProcess;
                    if(attackEffectProcess.WaitChooseBeAttackedMonster() && card.GetCardType() == CardType.Monster)
                    {
                        attackEffectProcess.ChooseBeAttackedMonster(card);
                    }
                }
            }
            else if(card.GetCardGameState() == CardGameState.Tomb ||
                card.GetCardGameState() == CardGameState.Exclusion)
            {
                duelScene.ShowCardList(ownerPlayer, card.GetCardGameState(), true, null, null);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            duelScene.SetCurrentPointCard(card);
            //如果卡牌在场上，则置顶，防遮挡
            if (card.GetCardGameState()==CardGameState.FrontAttack||
                card.GetCardGameState() == CardGameState.FrontDefense||
                card.GetCardGameState() == CardGameState.Front ||
                card.GetCardGameState() == CardGameState.Back)
            {
                int count = transform.parent.childCount - 1;
                transform.SetSiblingIndex(count);
            }

            gameObject.transform.localScale = new Vector3(selectScale, selectScale, 1);
            if(canShowInfo)
            {
                duelScene.SetCurrentInfoCard(card);
            }
            if(!duelScene.GetLockScene() && ownerPlayer.IsMyTurn())
            {
                RecheckAllowedOperation();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            duelScene.SetCurrentPointCard(null);
            gameObject.transform.localScale = Vector3.one;
            GameManager.CleanPanelContent(operationPanelTransform);
        }

        /// <summary>
        /// 重新设置攻击力和防御力数值
        /// </summary>
        /// <param name="text"></param>
        public void ResetAttackAndDefenseText()
        {
            if (card.GetCardGameState() == CardGameState.FrontAttack)
            {
                string text = "<color=#FF0000FF>" + card.GetAttackValue() + "</color>/";
                text += "<color=#000000FF>" + card.GetDefenseValue() + "</color>";
                attackAndDefenseText.GetComponent<Text>().text = text;
            }
            else if(card.GetCardGameState() == CardGameState.FrontDefense)
            {
                string text = "<color=#000000FF>" + card.GetAttackValue() + "</color>/";
                text += "<color=#00FF00FF>" + card.GetDefenseValue() + "</color>";
                attackAndDefenseText.GetComponent<Text>().text = text;
            }
            else
            {
                attackAndDefenseText.GetComponent<Text>().text = "";
            }
        }

        /// <summary>
        /// 设置卡牌点击回调事件
        /// </summary>
        /// <param name="clickCalback"></param>
        public void SetClickCallback(CardBase launchEffectCard,Action<CardBase, CardBase> clickCallback)
        {
            if(this.clickCallback!=null)
            {
                Debug.LogError("当前卡牌：" + card.GetName() + "已存在点击回调事件，将被替换。");
            }
            this.launchEffectCard = launchEffectCard;
            this.clickCallback = clickCallback;
        }

        /// <summary>
        /// 移除卡牌点击回调事件
        /// </summary>
        public void RemoveClickCallback()
        {
            launchEffectCard = null;
            clickCallback = null;
        }

        /// <summary>
        /// 判断当前卡牌是否可以攻击
        /// </summary>
        /// <returns></returns>
        public bool CanAttack()
        {
            if (ownerPlayer.IsMyTurn() &&
                duelScene.GetCurrentDuelProcess() == DuelProcess.Battle &&
                card.GetCardType() == CardType.Monster &&
                ownerPlayer.GetCurrentEffectProcess() == null)
            {
                return card.CanAttack() ;
            }
            return false;
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
            return CanNormalCall()|| CanSacrificCall()||CanBackPlaceToMonsterArea();
        }

        /// <summary>
        /// 判断是否可以通常召唤
        /// </summary>
        /// <returns></returns>
        public bool CanNormalCall()
        {
            if (ownerPlayer.IsMyTurn() && 
                card.GetCardGameState() == CardGameState.Hand &&
                card.GetCardType() == CardType.Monster &&
                (duelScene.GetCurrentDuelProcess() == DuelProcess.Main ||
                duelScene.GetCurrentDuelProcess() == DuelProcess.Second) &&
                ownerPlayer.GetCurrentEffectProcess() == null &&
                ownerPlayer.GetCanCallNumber() > 0)
            {
                if (card.GetLevel() <= DuelRuleManager.GetCallMonsterWithoutSacrificeLevelUpperLimit())
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
            if (ownerPlayer.IsMyTurn() && 
                card.GetCardGameState() == CardGameState.Hand &&
                card.GetCardType() == CardType.Monster &&
                (duelScene.GetCurrentDuelProcess() == DuelProcess.Main ||
                duelScene.GetCurrentDuelProcess() == DuelProcess.Second)&&
                ownerPlayer.GetCurrentEffectProcess() == null &&
                ownerPlayer.GetCanCallNumber() > 0)
            {
                if (card.NeedSacrificeMonsterNumer()>0 && ownerPlayer.GetCanBeSacrificeMonsterNumber() >= card.NeedSacrificeMonsterNumer())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断是否可以背面放置到怪兽区
        /// </summary>
        /// <returns></returns>
        public bool CanBackPlaceToMonsterArea()
        {
            if (ownerPlayer.IsMyTurn() && 
                card.GetCardGameState() == CardGameState.Hand &&
                card.GetCardType() == CardType.Monster &&
                (duelScene.GetCurrentDuelProcess() == DuelProcess.Main ||
                duelScene.GetCurrentDuelProcess() == DuelProcess.Second) &&
                ownerPlayer.GetCurrentEffectProcess() == null &&
                ownerPlayer.GetCanCallNumber() > 0)
            {
                if (card.GetLevel() <= DuelRuleManager.GetCallMonsterWithoutSacrificeLevelUpperLimit())
                {
                    return !ownerPlayer.MonsterAreaIsFull();
                }
                else if(card.NeedSacrificeMonsterNumer() > 0 && ownerPlayer.GetCanBeSacrificeMonsterNumber() >= card.NeedSacrificeMonsterNumer())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断是否可以进行反转召唤
        /// </summary>
        /// <returns></returns>
        public bool CanFlipCall()
        {
            if (ownerPlayer.IsMyTurn() && 
                card.GetCardGameState() == CardGameState.Back &&
                card.GetCardType() == CardType.Monster &&
                (duelScene.GetCurrentDuelProcess() == DuelProcess.Main ||
                duelScene.GetCurrentDuelProcess() == DuelProcess.Second) &&
                duelScene.GetCurrentTurnNumber() > card.GetBePlacedAreaTurnNumber() &&
                ownerPlayer.GetCurrentEffectProcess() == null)
            {
                return true;
            }
            return false;
        }

        public bool CanChangeToFrontAttack()
        {
            return card.CanChangeToFrontAttack();
        }

        public bool CanChangeToFrontDefense()
        {
            return card.CanChangeToFrontDefense();
        }

        /// <summary>
        /// 判断是否可以放置到魔法陷阱区
        /// </summary>
        /// <returns></returns>
        public bool CanBackPlaceToMagicTrapArea()
        {
            if (ownerPlayer.IsMyTurn() &&
                (card.GetCardType() == CardType.Magic || 
                card.GetCardType() == CardType.Trap) &&
                card.GetCardGameState() == CardGameState.Hand &&
                (duelScene.GetCurrentDuelProcess() == DuelProcess.Main ||
                duelScene.GetCurrentDuelProcess() == DuelProcess.Second) &&
                ownerPlayer.GetCurrentEffectProcess() == null &&
                !ownerPlayer.MagicTrapAreaIsFull())
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断是否可以发动效果
        /// </summary>
        public bool CanLaunchEffect()
        {
            return card.CanLaunchEffect();
        }

        /// <summary>
        /// 重新检查当前卡牌允许的操作，并添加到操作面板中
        /// </summary>
        public void RecheckAllowedOperation()
        {
            GameManager.CleanPanelContent(operationPanelTransform);
            
            switch (card.GetCardType())
            {
                case CardType.Unknown:
                    break;
                case CardType.Monster:
                    if (CanNormalCall())
                    {
                        AddCardOperationButtonToOperationPanel(CardOperation.NormalCall);
                    }
                    if (CanSacrificCall())
                    {
                        AddCardOperationButtonToOperationPanel(CardOperation.SacrificCall);
                    }
                    if (CanBackPlaceToMonsterArea())
                    {
                        AddCardOperationButtonToOperationPanel(CardOperation.BackPlaceToMonsterArea);
                    }
                    if (CanFlipCall())
                    {
                        AddCardOperationButtonToOperationPanel(CardOperation.FlipCall);
                    }
                    if (CanChangeToFrontAttack())
                    {
                        AddCardOperationButtonToOperationPanel(CardOperation.ChangeAttack);
                    }
                    if (CanChangeToFrontDefense())
                    {
                        AddCardOperationButtonToOperationPanel(CardOperation.ChangeDefense);
                    }
                    break;
                case CardType.Magic:
                case CardType.Trap:
                    if(CanBackPlaceToMagicTrapArea())
                    {
                        AddCardOperationButtonToOperationPanel(CardOperation.BackPlaceToMagicTrapArea);
                    }
                    if(CanLaunchEffect())
                    {
                        AddCardOperationButtonToOperationPanel(CardOperation.LaunchEffect);
                    }
                    break;
                default:
                    break;
            }
            
        }

        /// <summary>
        /// 添加卡牌操作按钮到操作面板中
        /// </summary>
        void AddCardOperationButtonToOperationPanel(CardOperation cardOperation)
        {
            GameObject gameObject = Instantiate(GameManager.GetCardOperationButtonPrefab(), operationPanelTransform);
            CardOperationButtonScript cardOperationButtonScript = gameObject.GetComponent<CardOperationButtonScript>();
            cardOperationButtonScript.SetInfo(this, cardOperation);
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
                            ownerPlayer.CallMonster(card);
                        }
                    }
                    break;
                case CardOperation.BackPlaceToMonsterArea:
                    if (ownerPlayer.CanCallMonster())
                    {
                        if (CanCall())
                        {
                            ownerPlayer.CallMonster(card, CardGameState.Back);
                        }
                    }
                    break;
                case CardOperation.FlipCall:
                    if (CanFlipCall())
                    {
                        ownerPlayer.CallMonster(card);
                    }
                    break;
                case CardOperation.ChangeAttack:
                    if(CanChangeToFrontAttack())
                    {
                        ChangeAttackDefenseEffectProcess changeAttackDefenseEffectProcess = new ChangeAttackDefenseEffectProcess(card, CardGameState.FrontAttack,ownerPlayer);
                        ownerPlayer.AddEffectProcess(changeAttackDefenseEffectProcess);
                    }
                    break;
                case CardOperation.ChangeDefense:
                    if (CanChangeToFrontDefense())
                    {
                        ChangeAttackDefenseEffectProcess changeAttackDefenseEffectProcess = new ChangeAttackDefenseEffectProcess(card, CardGameState.FrontDefense, ownerPlayer);
                        ownerPlayer.AddEffectProcess(changeAttackDefenseEffectProcess);
                    }
                    break;
                case CardOperation.BackPlaceToMagicTrapArea:
                    if (CanBackPlaceToMagicTrapArea())
                    {
                        ownerPlayer.BackPlaceMagicOrTrap(card);
                    }
                    break;
                case CardOperation.LaunchEffect:
                    if (CanLaunchEffect())
                    {
                        ownerPlayer.LaunchEffect(card);
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
