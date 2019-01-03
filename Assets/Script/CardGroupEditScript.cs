using Assets.Script;
using Assets.Script.Card;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardGroupEditScript : MonoBehaviour {
    GameManager gameManager;
    public Transform allCardScrollViewTransform;
    public Transform mainPanelTransform;
    public Transform extraPanelTransform;
    public Transform deputyPanelTransform;
    public Transform infoContentTransform;

    public InputField cardGroupNameInputField;

    public GameObject cardPre;
    public GameObject monsterCardPre;
    public GameObject magicTrapCardPre;

    Image cardImage;

    string currentCardGroupName = "";

    void Start ()
    {
        gameManager = GameManager.GetSingleInstance();

        UserData userData = gameManager.GetUserData();

        GameManager.CleanPanelContent(allCardScrollViewTransform);

        foreach (var item in userData.userCardList)
        {
            for (int i = 0; i < item.number; i++)
            {
                GameObject go = Instantiate(cardPre, allCardScrollViewTransform);
                CardBase card = gameManager.allCardInfoList[item.cardNo];
                
                go.GetComponent<Image>().sprite = card.GetImage();
                go.GetComponent<CardScript>().SetRootScript(this);
                go.GetComponent<CardScript>().SetCard(card);
                go.GetComponent<CardScript>().allCardTransform = allCardScrollViewTransform;
                go.GetComponent<CardScript>().cardGroupTransform = mainPanelTransform;
            }
        }

        currentCardGroupName = gameManager.GetCardGroupNameForCardGroupEditScene();
        cardGroupNameInputField.text = currentCardGroupName;
        cardGroupNameInputField.onEndEdit.AddListener((value)=> { ChangeCardGroupName(value); });
        UserCardGroup cardGroup = userData.GetCardGroupByName(currentCardGroupName);
        if(cardGroup==null)
        {
            Debug.LogError("卡组:"+ currentCardGroupName + "不存在！");
            gameManager.ReturnLastScene();
            return;
        }

        GameManager.CleanPanelContent(mainPanelTransform);

        foreach (var item in cardGroup.mainCardList)
        {
            for (int i = 0; i < item.number; i++)
            {
                GameObject go = Instantiate(cardPre, mainPanelTransform);
                CardBase card = gameManager.allCardInfoList[item.cardNo];
                go.GetComponent<Image>().sprite = card.GetImage();
                go.GetComponent<CardScript>().SetRootScript(this);
                go.GetComponent<CardScript>().SetCard(card);
                go.GetComponent<CardScript>().allCardTransform = allCardScrollViewTransform;
                go.GetComponent<CardScript>().cardGroupTransform = mainPanelTransform;
            }
        }

        cardImage = GameObject.Find("CardImage").GetComponent<Image>();
    }
	
	void Update ()
    {
		
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

    public void AddCardToCardGroup(CardBase card)
    {
        gameManager.AddCardToCardGroup(currentCardGroupName,card);
        GameObject go = Instantiate(cardPre, mainPanelTransform);
        go.GetComponent<Image>().sprite = card.GetImage();
        go.GetComponent<CardScript>().SetRootScript(this);
        go.GetComponent<CardScript>().SetCard(card);
        go.GetComponent<CardScript>().allCardTransform = allCardScrollViewTransform;
        go.GetComponent<CardScript>().cardGroupTransform = mainPanelTransform;
    }

    public void RemoveCardFromCardGroup(GameObject gameObject, CardBase card)
    {
        gameManager.RemoveCardFromCardGroup(currentCardGroupName,card);
        Destroy(gameObject);
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
}
