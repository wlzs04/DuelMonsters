using Assets.Script;
using Assets.Script.Card;
using Assets.Script.Duel.Rule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardGroupEditScript : MonoBehaviour
{
    GameManager gameManager;
    public Transform allCardScrollViewTransform;
    public Transform mainPanelTransform;
    public Transform extraPanelTransform;
    public Transform deputyPanelTransform;
    public Transform infoContentTransform;

    public GameObject mainTotalNumberText;
    public GameObject extraTotalNumberText;
    public GameObject deputyTotalNumberText;

    public InputField cardGroupNameInputField;

    public GameObject cardPre;
    public GameObject monsterCardPre;
    public GameObject magicTrapCardPre;

    Image cardImage;

    string currentCardGroupName = "";

    UserCardGroup cardGroup=null;

    //是否只显示已拥有的卡牌
    bool onlyShowOwnedCard = false;

    void Start ()
    {
        gameManager = GameManager.GetSingleInstance();
        UserData userData = gameManager.GetUserData();

        ResetAllCard();

        currentCardGroupName = gameManager.GetCardGroupNameForCardGroupEditScene();
        cardGroupNameInputField.text = currentCardGroupName;
        cardGroupNameInputField.onEndEdit.AddListener((value)=> { ChangeCardGroupName(value); });
        cardGroup = userData.GetCardGroupByName(currentCardGroupName);
        if(cardGroup==null)
        {
            Debug.LogError("卡组:"+ currentCardGroupName + "不存在！");
            gameManager.ReturnLastScene();
            return;
        }

        ResetMainCardGroup();
        ResetExtraCardGroup();
        ResetDeputyCardGroup();

        cardImage = GameObject.Find("CardImage").GetComponent<Image>();

        mainPanelTransform.gameObject.GetComponent<DropToPanelScript>().AddDropHandler(OnDropToMainPanel);
        extraPanelTransform.gameObject.GetComponent<DropToPanelScript>().AddDropHandler(OnDropToExtraPanel);
        deputyPanelTransform.gameObject.GetComponent<DropToPanelScript>().AddDropHandler(OnDropToDeputyPanel);
    }
	
	void Update ()
    {
		
	}

    /// <summary>
    /// 重新设置所有卡牌
    /// </summary>
    void ResetAllCard()
    {
        GameManager.CleanPanelContent(allCardScrollViewTransform);
        UserData userData = gameManager.GetUserData();

        bool ownedCard = false;
        foreach (var item in gameManager.allCardInfoList)
        {
            ownedCard = userData.IsOwnCard(item.Value.GetCardNo());
            if(!onlyShowOwnedCard || ownedCard)
            {
                CardBase card = item.Value;
                GameObject go = Instantiate(cardPre, allCardScrollViewTransform);
                go.GetComponent<CardScript>().SetRootScript(this);
                go.GetComponent<CardScript>().SetCard(card, ownedCard);
            }
        }
    }

    /// <summary>
    /// 重新设置主卡组
    /// </summary>
    void ResetMainCardGroup()
    {
        GameManager.CleanPanelContent(mainPanelTransform);
        int maxCountOneRow = 15;//一排最大数量
        float panelWidth = ((RectTransform)mainPanelTransform).rect.width / (maxCountOneRow + 1);
        float panelHeight = ((RectTransform)mainPanelTransform).rect.height / 4;

        int cardIndex = 0;
        foreach (var item in cardGroup.mainCardList)
        {
            for (int i = 0; i < item.number; i++)
            {
                GameObject gameObject = Instantiate(cardPre, mainPanelTransform);
                CardBase card = gameManager.allCardInfoList[item.cardNo];
                CardScript cardScript = gameObject.GetComponent<CardScript>();
                cardScript.SetRootScript(this);
                cardScript.SetCard(card);
                float row = -(cardIndex / maxCountOneRow) - 0.5f;
                float col = cardIndex % maxCountOneRow + 1;
                gameObject.transform.localPosition = new Vector3(col * panelWidth, row * panelHeight, 0);
                cardIndex++;
            }
        }
        mainTotalNumberText.GetComponent<Text>().text = "主卡组：" + mainPanelTransform.childCount;
    }

    /// <summary>
    /// 重新设置额外卡组
    /// </summary>
    void ResetExtraCardGroup()
    {
        GameManager.CleanPanelContent(extraPanelTransform);
        int maxCountOneRow = 15;//一排最大数量
        float panelWidth = ((RectTransform)extraPanelTransform).rect.width / (maxCountOneRow + 1);
        float panelHeight = ((RectTransform)extraPanelTransform).rect.height;

        int cardIndex = 0;
        foreach (var item in cardGroup.extraCardList)
        {
            for (int i = 0; i < item.number; i++)
            {
                GameObject gameObject = Instantiate(cardPre, extraPanelTransform);
                CardBase card = gameManager.allCardInfoList[item.cardNo];
                CardScript cardScript = gameObject.GetComponent<CardScript>();
                cardScript.SetRootScript(this);
                cardScript.SetCard(card);
                float row = -(cardIndex / maxCountOneRow) - 0.5f;
                float col = cardIndex % maxCountOneRow + 1;
                gameObject.transform.localPosition = new Vector3(col * panelWidth, row * panelHeight, 0);
                cardIndex++;
            }
        }
        extraTotalNumberText.GetComponent<Text>().text = "额外卡组：" + extraPanelTransform.childCount;
    }
    
    /// <summary>
    /// 重新设置副卡组
    /// </summary>
    void ResetDeputyCardGroup()
    {
        GameManager.CleanPanelContent(deputyPanelTransform);
        int maxCountOneRow = 15;//一排最大数量
        float panelWidth = ((RectTransform)deputyPanelTransform).rect.width / (maxCountOneRow + 1);
        float panelHeight = ((RectTransform)deputyPanelTransform).rect.height;

        int cardIndex = 0;
        foreach (var item in cardGroup.deputyCardList)
        {
            for (int i = 0; i < item.number; i++)
            {
                GameObject gameObject = Instantiate(cardPre, deputyPanelTransform);
                CardBase card = gameManager.allCardInfoList[item.cardNo];
                CardScript cardScript = gameObject.GetComponent<CardScript>();
                cardScript.SetRootScript(this);
                cardScript.SetCard(card);
                float row = -(cardIndex / maxCountOneRow) - 0.5f;
                float col = cardIndex % maxCountOneRow + 1;
                gameObject.transform.localPosition = new Vector3(col * panelWidth, row * panelHeight, 0);
                cardIndex++;
            }
        }
        deputyTotalNumberText.GetComponent<Text>().text = "副卡组：" + deputyPanelTransform.childCount;
    }

    /// <summary>
    /// 显示详细信息
    /// </summary>
    /// <param name="card"></param>
    public void ShowCardDetailInfo(CardBase card)
    {
        cardImage.sprite = card.GetImage();
        GameManager.CleanPanelContent(infoContentTransform);

        GameObject gameObject = null;
        if (card.GetCardType()==CardType.Monster)
        {
            MonsterCard monsterCard = (MonsterCard)card;
            gameObject = Instantiate(monsterCardPre, infoContentTransform);
            gameObject.transform.GetChild(0).GetComponent<Text>().text = "名称：" + monsterCard.GetName();
            gameObject.transform.GetChild(1).GetComponent<Text>().text = "属性：" + monsterCard.GetPropertyTypeString() + "/" + monsterCard.GetMonsterTypeString()+"/" + monsterCard.GetLevel();
            gameObject.transform.GetChild(2).GetComponent<Text>().text = "攻击力：" + monsterCard.GetAttackNumber();
            gameObject.transform.GetChild(3).GetComponent<Text>().text = "防御力：" + monsterCard.GetDefenseNumber();
            gameObject.transform.GetChild(4).GetComponent<Text>().text = "效果：" + monsterCard.GetEffect();
        }
        else if(card.GetCardType() == CardType.Magic)
        {
            MagicCard magicCard = (MagicCard)card;
            gameObject = Instantiate(magicTrapCardPre, infoContentTransform);
            gameObject.transform.GetChild(0).GetComponent<Text>().text = "名称：" + magicCard.GetName();
            gameObject.transform.GetChild(1).GetComponent<Text>().text = "类型：" + magicCard.GetMagicTypeString() + magicCard.GetCardTypeString();
            gameObject.transform.GetChild(2).GetComponent<Text>().text = "效果：" + magicCard.GetEffect();
        }
        else if (card.GetCardType() == CardType.Trap)
        {
            TrapCard trapCard = (TrapCard)card;
            gameObject = Instantiate(magicTrapCardPre, infoContentTransform);
            gameObject.transform.GetChild(0).GetComponent<Text>().text = "名称：" + trapCard.GetName();
            gameObject.transform.GetChild(1).GetComponent<Text>().text = "类型：" + trapCard.GetTrapTypeString() + trapCard.GetCardTypeString();
            gameObject.transform.GetChild(2).GetComponent<Text>().text = "效果：" + trapCard.GetEffect();
        }
        else
        {
            Debug.LogError("卡片类型错误！");
        }
    }
    
    /// <summary>
    /// 向指定卡组添加卡牌
    /// </summary>
    /// <param name="cardGroupType"></param>
    /// <param name="card"></param>
    public bool AddCardToCardGroup(CardGroupType cardGroupType,CardBase card)
    {
        if (cardGroup == null)
        {
            Debug.LogError("在向卡组中添加卡牌时没有找到卡组：" + currentCardGroupName);
            return false;
        }
        int cardNumberLimit = DuelRuleManager.GetSameCardNumberUpperLimit();
        List<UserCardData> typrCardGroup = null;
        switch (cardGroupType)
        {
            case CardGroupType.Unknown:
                Debug.LogError("未知卡组类型！");
                return false;
            case CardGroupType.Main:
                typrCardGroup = cardGroup.mainCardList;
                break;
            case CardGroupType.Extra:
                typrCardGroup = cardGroup.extraCardList;
                break;
            case CardGroupType.Deputy:
                typrCardGroup = cardGroup.deputyCardList;
                break;
            default:
                break;
        }
        UserCardData userCardData = null;
        foreach (var item in typrCardGroup)
        {
            if (item.cardNo == card.GetCardNo())
            {
                userCardData = item;
            }
        }
        if(userCardData==null)
        {
            userCardData = new UserCardData();
            userCardData.cardNo = card.GetCardNo();
            userCardData.number = 1;
            typrCardGroup.Add(userCardData);
        }
        else
        {
            if (userCardData.number >= cardNumberLimit)
            {
                GameManager.ShowMessage("卡组中" + card.GetName() + "数量超过最大值！");
                return false;
            }
            else
            {
                userCardData.number++;
            }
        }
        switch (cardGroupType)
        {
            case CardGroupType.Unknown:
                break;
            case CardGroupType.Main:
                ResetMainCardGroup();
                break;
            case CardGroupType.Extra:
                ResetExtraCardGroup();
                break;
            case CardGroupType.Deputy:
                ResetDeputyCardGroup();
                break;
            default:
                break;
        }
        return true;
    }

    /// <summary>
    /// 从指定卡组中移除卡牌
    /// </summary>
    /// <param name="cardGroupType"></param>
    /// <param name="card"></param>
    /// <returns></returns>
    public bool RemoveCardFromCardGroup(CardGroupType cardGroupType, CardBase card)
    {
        if (cardGroup == null)
        {
            Debug.LogError("在从卡组中删除卡牌时没有找到卡组：" + currentCardGroupName);
            return false;
        }
        List<UserCardData> typeCardGroup = null;
        switch (cardGroupType)
        {
            case CardGroupType.Unknown:
                Debug.LogError("未知卡组类型！");
                return false;
            case CardGroupType.Main:
                typeCardGroup = cardGroup.mainCardList;
                break;
            case CardGroupType.Extra:
                typeCardGroup = cardGroup.extraCardList;
                break;
            case CardGroupType.Deputy:
                typeCardGroup = cardGroup.deputyCardList;
                break;
            default:
                break;
        }
        bool found = false;
        foreach (var item in typeCardGroup)
        {
            if (item.cardNo == card.GetCardNo())
            {
                if (item.number > 1)
                {
                    item.number--;
                }
                else
                {
                    typeCardGroup.Remove(item);
                }
                found = true;
                break;
            }
        }
        if(found)
        {
            switch (cardGroupType)
            {
                case CardGroupType.Unknown:
                    break;
                case CardGroupType.Main:
                    ResetMainCardGroup();
                    break;
                case CardGroupType.Extra:
                    ResetExtraCardGroup();
                    break;
                case CardGroupType.Deputy:
                    ResetDeputyCardGroup();
                    break;
                default:
                    break;
            }
        }
        return false;
    }

    /// <summary>
    /// 从指定卡组中移除卡牌
    /// </summary>
    /// <param name="cardScript"></param>
    /// <returns></returns>
    public bool RemoveCardFromCardGroup(CardScript cardScript)
    {
        if (cardScript.transform.parent == mainPanelTransform)
        {
            return RemoveCardFromCardGroup(CardGroupType.Main, cardScript.GetCard());
        }
        else if(cardScript.transform.parent == extraPanelTransform)
        {
            return RemoveCardFromCardGroup(CardGroupType.Extra, cardScript.GetCard());
        }
        else if (cardScript.transform.parent == deputyPanelTransform)
        {
            return RemoveCardFromCardGroup(CardGroupType.Deputy, cardScript.GetCard());
        }
        return false;
    }

    /// <summary>
    /// 更换当前卡组的名称
    /// </summary>
    /// <param name="newName"></param>
    void ChangeCardGroupName(string newName)
    {
        if (newName == currentCardGroupName)
        {
        }
        else if (newName == "")
        {
            GameManager.ShowMessage("名称不能为空！");
        }
        else if (GameManager.GetSingleInstance().GetUserData().GetCardGroupByName(newName) == null)
        {
            GameManager.GetSingleInstance().GetUserData().GetCardGroupByName(currentCardGroupName).cardGroupName = newName;
            currentCardGroupName = newName;
            cardGroupNameInputField.text = newName;
        }
        else
        {
            GameManager.ShowMessage("当前名称已存在！");
        }
        cardGroupNameInputField.text = currentCardGroupName;
    }
    
    /// <summary>
    /// 卡牌拖拽到主卡组事件
    /// </summary>
    /// <param name="eventData"></param>
    void OnDropToMainPanel(PointerEventData eventData)
    {
        CardScript cardScript = eventData.pointerDrag.GetComponent<CardScript>();
        
        if (cardScript != null && cardScript.IsOwnedCard() && cardScript.transform.parent != mainPanelTransform)
        {
            bool addSucccess = AddCardToCardGroup(CardGroupType.Main,cardScript.GetCard());
            if(addSucccess)
            {
                if (cardScript.transform.parent==extraPanelTransform)
                {
                    RemoveCardFromCardGroup(CardGroupType.Extra, cardScript.GetCard());
                }
                else if (cardScript.transform.parent == deputyPanelTransform)
                {
                    RemoveCardFromCardGroup(CardGroupType.Deputy, cardScript.GetCard());
                }
            }
        }
    }

    /// <summary>
    /// 卡牌拖拽到额外卡组事件
    /// </summary>
    /// <param name="eventData"></param>
    void OnDropToExtraPanel(PointerEventData eventData)
    {
        CardScript cardScript = eventData.pointerDrag.GetComponent<CardScript>();
        if (cardScript != null && cardScript.IsOwnedCard() && cardScript.transform.parent != extraPanelTransform)
        {
            bool addSucccess = AddCardToCardGroup(CardGroupType.Extra, cardScript.GetCard());
            if (addSucccess)
            {
                if (cardScript.transform.parent == mainPanelTransform)
                {
                    RemoveCardFromCardGroup(CardGroupType.Main, cardScript.GetCard());
                }
                if (cardScript.transform.parent == deputyPanelTransform)
                {
                    RemoveCardFromCardGroup(CardGroupType.Deputy, cardScript.GetCard());
                }
            }
        }
    }

    /// <summary>
    /// 卡牌拖拽到副卡组事件
    /// </summary>
    /// <param name="eventData"></param>
    void OnDropToDeputyPanel(PointerEventData eventData)
    {
        CardScript cardScript = eventData.pointerDrag.GetComponent<CardScript>();
        if (cardScript != null && cardScript.IsOwnedCard() && cardScript.transform.parent != deputyPanelTransform)
        {
            bool addSucccess = AddCardToCardGroup(CardGroupType.Deputy, cardScript.GetCard());
            if (addSucccess)
            {
                if (cardScript.transform.parent == mainPanelTransform)
                {
                    RemoveCardFromCardGroup(CardGroupType.Main, cardScript.GetCard());
                }
                if (cardScript.transform.parent == extraPanelTransform)
                {
                    RemoveCardFromCardGroup(CardGroupType.Extra, cardScript.GetCard());
                }
            }
        }
    }
    
    /// <summary>
    /// 只显示已拥有的卡牌
    /// </summary>
    public void OnlyShowOwnedCardChangeEvent(bool value)
    {
        onlyShowOwnedCard = !onlyShowOwnedCard;
        ResetAllCard();
    }
}
