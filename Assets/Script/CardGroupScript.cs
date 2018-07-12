using Assets.Script;
using Assets.Script.Card;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardGroupScript : MonoBehaviour {
    GameManager gameManager;
    public Transform allCardScrollViewTransform;
    public Transform cardGroupScrollViewTransform;
    public GameObject cardPre;
    public Transform infoContentTransform;

    public GameObject monsterCardPre;
    public GameObject magicTrapCardPre;

    Image cardImage;

    // Use this for initialization
    void Start () {
        gameManager = GameManager.GetSingleInstance();

        UserData userData = gameManager.Userdata;
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
                go.GetComponent<CardScript>().cardGroupTransform = cardGroupScrollViewTransform;
            }
        }

        foreach (var item in userData.userCardGroupList)
        {
            for (int i = 0; i < item.number; i++)
            {
                GameObject go = Instantiate(cardPre, cardGroupScrollViewTransform);
                CardBase card = gameManager.allCardInfoList[item.cardNo];
                go.GetComponent<Image>().sprite = card.GetImage();
                go.GetComponent<CardScript>().SetRootScript(this);
                go.GetComponent<CardScript>().SetCard(card);
                go.GetComponent<CardScript>().allCardTransform = allCardScrollViewTransform;
                go.GetComponent<CardScript>().cardGroupTransform = cardGroupScrollViewTransform;
            }
        }

        cardImage = GameObject.Find("CardImage").GetComponent<Image>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetInfoContent(CardBase card)
    {
        cardImage.sprite = card.GetImage();
        for (int i = 0; i < infoContentTransform.childCount; i++)
        {
            GameObject go = infoContentTransform.GetChild(i).gameObject;
            Destroy(go);
        }
        GameObject gameObject = null;
        if (card.GetCardType()==CardType.Monster)
        {
            MonsterCard monsterCard = (MonsterCard)card;
            gameObject = Instantiate(monsterCardPre, infoContentTransform);
            gameObject.transform.GetChild(0).GetComponent<Text>().text = "名称：" + monsterCard.GetName();
            gameObject.transform.GetChild(1).GetComponent<Text>().text = "等级：" + monsterCard.GetLevel();
            gameObject.transform.GetChild(2).GetComponent<Text>().text = "卡片类型：" + monsterCard.GetCardTypeString();
            gameObject.transform.GetChild(3).GetComponent<Text>().text = "类型：" + monsterCard.GetMonsterTypeString();
            gameObject.transform.GetChild(4).GetComponent<Text>().text = "攻击力：" + monsterCard.GetAttackNumber();
            gameObject.transform.GetChild(5).GetComponent<Text>().text = "防御力：" + monsterCard.GetDefenseNumber();
            gameObject.transform.GetChild(6).GetComponent<Text>().text = "效果：" + monsterCard.GetEffect();
        }
        else if(card.GetCardType() == CardType.Magic)
        {
            gameObject = Instantiate(magicTrapCardPre, infoContentTransform);
            MagicCard magicCard = (MagicCard)card;
            gameObject = Instantiate(magicTrapCardPre, infoContentTransform);
            gameObject.transform.GetChild(0).GetComponent<Text>().text = "名称：" + magicCard.GetName();
            gameObject.transform.GetChild(1).GetComponent<Text>().text = "卡片类型：" + magicCard.GetCardTypeString();
            gameObject.transform.GetChild(2).GetComponent<Text>().text = "类型：" + magicCard.GetMagicTypeString();
            gameObject.transform.GetChild(3).GetComponent<Text>().text = "效果：" + magicCard.GetEffect();
        }
        else if (card.GetCardType() == CardType.Trap)
        {
            gameObject = Instantiate(magicTrapCardPre, infoContentTransform);
            TrapCard trapCard = (TrapCard)card;
            gameObject = Instantiate(magicTrapCardPre, infoContentTransform);
            gameObject.transform.GetChild(0).GetComponent<Text>().text = "名称：" + trapCard.GetName();
            gameObject.transform.GetChild(1).GetComponent<Text>().text = "卡片类型：" + trapCard.GetCardTypeString();
            gameObject.transform.GetChild(2).GetComponent<Text>().text = "类型：" + trapCard.GetTrapTypeString();
            gameObject.transform.GetChild(3).GetComponent<Text>().text = "效果：" + trapCard.GetEffect();
        }
        else
        {
            Debug.LogError("卡片类型错误！");
        }
    }

    public void AddCardToCardGroup(CardBase card)
    {
        GameObject go = Instantiate(cardPre, cardGroupScrollViewTransform);
        go.GetComponent<Image>().sprite = card.GetImage();
        go.GetComponent<CardScript>().SetRootScript(this);
        go.GetComponent<CardScript>().SetCard(card);
        go.GetComponent<CardScript>().allCardTransform = allCardScrollViewTransform;
        go.GetComponent<CardScript>().cardGroupTransform = cardGroupScrollViewTransform;
    }

    public void RemoveCardFromCardGroup(GameObject gameObject, CardBase card)
    {
        gameManager.RemoveCardFromCardGroup(card);
        Destroy(gameObject);
    }
}
