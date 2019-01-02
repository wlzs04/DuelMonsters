using Assets.Script;
using Assets.Script.Card;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardGroupScript : MonoBehaviour
{
    GameManager gameManager;

    public Transform cardGroupScrollViewTransform;
    public Transform mainPanelTransform;
    public Transform extraPanelTransform;
    public Transform deputyPanelTransform;

    public GameObject mainTotalNumberText;
    public GameObject extraTotalNumberText;
    public GameObject deputyTotalNumberText;
    
    public GameObject cardGroupItemPrefab;
    public GameObject cardPrefab;

    List<CardCroupItemScript> cardCroupItemList = new List<CardCroupItemScript>();
    List<CardScript> mainCardList = new List<CardScript>();
    List<CardScript> extraCardList = new List<CardScript>();
    List<CardScript> deputyCardList = new List<CardScript>();

    //当前选中的卡组指数
    int currentSelectCardCroupIndex = -1;

    void Start()
    {
        gameManager = GameManager.GetSingleInstance();
        UserData userData = gameManager.GetUserData();

        GameManager.CleanPanelContent(cardGroupScrollViewTransform);

        foreach (var item in userData.userCardGroupList)
        {
            GameObject gameObject = Instantiate(cardGroupItemPrefab, cardGroupScrollViewTransform);
            CardCroupItemScript cardCroupItemScript = gameObject.GetComponent<CardCroupItemScript>();
            cardCroupItemList.Add(cardCroupItemScript);
            cardCroupItemScript.InitInfo(item.cardGroupName,this);
        }
        if(cardCroupItemList.Count>0)
        {
            SelectCardGroupByIndex(0);
        }
    }

    void Update()
    {

    }

    /// <summary>
    /// 通过名称选中指定卡组
    /// </summary>
    public void SelectCardGroupByName(string cardGroupName)
    {
        for (int i = 0; i < cardCroupItemList.Count; i++)
        {
            if(cardCroupItemList[i].GetCardGroupName()== cardGroupName)
            {
                SelectCardGroupByIndex(i);
                return;
            }
        }
    }

    /// <summary>
    /// 通过指数选中指定卡组
    /// </summary>
    public void SelectCardGroupByIndex(int index)
    {
        if(currentSelectCardCroupIndex!=index && index>=0 && index<cardCroupItemList.Count)
        {
            currentSelectCardCroupIndex = index;
            cardCroupItemList[currentSelectCardCroupIndex].SetSelectState(true);

            UserData userData = gameManager.GetUserData();


            //主卡组
            GameManager.CleanPanelContent(mainPanelTransform);
            mainCardList.Clear();

            int maxCountOneRow = 20;//一排最大数量
            float panelWidth = ((RectTransform)mainPanelTransform).rect.width / (maxCountOneRow+1);
            float panelHeight = ((RectTransform)mainPanelTransform).rect.height / 4;
            
            int cardIndex = 0;
            foreach (var item in userData.userCardGroupList[currentSelectCardCroupIndex].mainCardList)
            {
                for (int i = 0; i < item.number; i++)
                {
                    GameObject gameObject = Instantiate(cardPrefab, mainPanelTransform);
                    CardBase card = gameManager.allCardInfoList[item.cardNo];
                    gameObject.GetComponent<Image>().sprite = card.GetImage();
                    CardScript cardScript = gameObject.GetComponent<CardScript>();
                    cardScript.SetCard(card);
                    mainCardList.Add(cardScript);
                    float row = -(cardIndex / maxCountOneRow) - 0.5f;
                    float col = cardIndex % maxCountOneRow + 1;
                    gameObject.transform.localPosition = new Vector3(col * panelWidth, row * panelHeight, 0);
                    cardIndex++;
                }
            }
            mainTotalNumberText.GetComponent<Text>().text = "主卡组：" + mainCardList.Count;

            //额外卡组
            GameManager.CleanPanelContent(extraPanelTransform);
            extraCardList.Clear();

            maxCountOneRow = 15;//一排最大数量
            panelWidth = ((RectTransform)extraPanelTransform).rect.width / (maxCountOneRow + 1);
            panelHeight = ((RectTransform)extraPanelTransform).rect.height;

            cardIndex = 0;
            foreach (var item in userData.userCardGroupList[currentSelectCardCroupIndex].extraCardList)
            {
                for (int i = 0; i < item.number; i++)
                {
                    GameObject gameObject = Instantiate(cardPrefab, extraPanelTransform);
                    CardBase card = gameManager.allCardInfoList[item.cardNo];
                    gameObject.GetComponent<Image>().sprite = card.GetImage();
                    CardScript cardScript = gameObject.GetComponent<CardScript>();
                    cardScript.SetCard(card);
                    extraCardList.Add(cardScript);
                    float row = -(cardIndex / maxCountOneRow) - 0.5f;
                    float col = cardIndex % maxCountOneRow + 1;
                    gameObject.transform.localPosition = new Vector3(col * panelWidth, row * panelHeight, 0);
                    cardIndex++;
                }
            }
            extraTotalNumberText.GetComponent<Text>().text = "额外卡组：" + extraCardList.Count;

            //副卡组
            GameManager.CleanPanelContent(deputyPanelTransform);
            deputyCardList.Clear();

            maxCountOneRow = 15;//一排最大数量
            panelWidth = ((RectTransform)deputyPanelTransform).rect.width / (maxCountOneRow + 1);
            panelHeight = ((RectTransform)deputyPanelTransform).rect.height;

            cardIndex = 0;
            foreach (var item in userData.userCardGroupList[currentSelectCardCroupIndex].deputyCardList)
            {
                for (int i = 0; i < item.number; i++)
                {
                    GameObject gameObject = Instantiate(cardPrefab, deputyPanelTransform);
                    CardBase card = gameManager.allCardInfoList[item.cardNo];
                    gameObject.GetComponent<Image>().sprite = card.GetImage();
                    CardScript cardScript = gameObject.GetComponent<CardScript>();
                    cardScript.SetCard(card);
                    deputyCardList.Add(cardScript);
                    float row = -(cardIndex / maxCountOneRow) - 0.5f;
                    float col = cardIndex % maxCountOneRow + 1;
                    gameObject.transform.localPosition = new Vector3(col * panelWidth, row * panelHeight, 0);
                    cardIndex++;
                }
            }
            deputyTotalNumberText.GetComponent<Text>().text = "副卡组：" + deputyCardList.Count;
        }
        else
        {
            Debug.LogError("选中指定卡组错误：第"+ (index+1) + "套卡组不存在！");
        }
    }

    /// <summary>
    /// 新建卡组按钮点击事件
    /// </summary>
    public void NewCardGroupButtonClick()
    {
        
    }

    /// <summary>
    /// 删除卡组按钮点击事件
    /// </summary>
    public void DeleteCardGroupButtonClick()
    {

    }

    /// <summary>
    /// 清空所有卡组选中状态
    /// </summary>
    public void ClearAllSelectState()
    {
        foreach (var item in cardCroupItemList)
        {
            item.SetSelectState(false);
        }
    }

    /// <summary>
    /// 返回到主场景
    /// </summary>
    public void ReturnMainScene()
    {
        GameManager.GetSingleInstance().ReturnLastScene();
    }
}
