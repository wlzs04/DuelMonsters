using Assets.Script;
using Assets.Script.Card;
using Assets.Script.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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

    //当前选中的卡组指数
    int currentSelectCardCroupIndex = -1;

    void Start()
    {
        gameManager = GameManager.GetSingleInstance();
        UserData userData = gameManager.GetUserData();

        GameManager.CleanPanelContent(cardGroupScrollViewTransform);

        foreach (var item in userData.userCardGroupList)
        {
            AddCardGroupItemToScrollView(item);
        }
        if(cardCroupItemList.Count>0)
        {
            SelectCardGroupByIndex(0);
        }
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
        if(currentSelectCardCroupIndex == index)
        {
            return;
        }
        if(index>=0 && index<cardCroupItemList.Count)
        {
            currentSelectCardCroupIndex = index;
            cardCroupItemList[currentSelectCardCroupIndex].SetSelectState(true);

            UserData userData = gameManager.GetUserData();

            CleanCardPanel();

            //主卡组
            AddCardItemToPanel(userData.userCardGroupList[currentSelectCardCroupIndex].mainCardList, 
                mainPanelTransform, 15,4);
            mainTotalNumberText.GetComponent<Text>().text = "主卡组：" + mainPanelTransform.childCount;

            //额外卡组
            AddCardItemToPanel(userData.userCardGroupList[currentSelectCardCroupIndex].extraCardList,
                extraPanelTransform, 15, 1);
            extraTotalNumberText.GetComponent<Text>().text = "额外卡组：" + extraPanelTransform.childCount;
            
            //副卡组
            AddCardItemToPanel(userData.userCardGroupList[currentSelectCardCroupIndex].deputyCardList,
                deputyPanelTransform, 15, 1);
            deputyTotalNumberText.GetComponent<Text>().text = "副卡组：" + deputyPanelTransform.childCount;
        }
        else
        {
            Debug.LogError("选中指定卡组错误：第"+ (index+1) + "套卡组不存在！");
        }
    }

    /// <summary>
    /// 添加卡牌item到指定面板
    /// </summary>
    /// <param name="cardList"></param>
    /// <param name="panelTransform"></param>
    /// <param name="maxCountOneRow"></param>
    /// <param name="maxRowCount"></param>
    void AddCardItemToPanel(List<UserCardData> cardList,Transform panelTransform,int maxCountOneRow,int maxRowCount)
    {
        float panelWidth = ((RectTransform)panelTransform).rect.width / (maxCountOneRow + 1);
        float panelHeight = ((RectTransform)panelTransform).rect.height / maxRowCount;

        int cardIndex = 0;
        foreach (var item in cardList)
        {
            for (int i = 0; i < item.number; i++)
            {
                if (gameManager.GetAllCardInfoList().ContainsKey(item.cardNo))
                {
                    GameObject gameObject = Instantiate(cardPrefab, panelTransform);
                    CardBase card = gameManager.GetAllCardInfoList()[item.cardNo];
                    gameObject.GetComponent<Image>().sprite = card.GetImage();
                    CardScript cardScript = gameObject.GetComponent<CardScript>();
                    cardScript.SetCard(card);
                    float row = -(cardIndex / maxCountOneRow) - 0.5f;
                    float col = cardIndex % maxCountOneRow + 1;
                    gameObject.transform.localPosition = new Vector3(col * panelWidth, row * panelHeight, 0);
                    cardIndex++;
                }
                else
                {
                    GameManager.ShowMessage($"当前卡组：{cardCroupItemList[currentSelectCardCroupIndex].GetCardGroupName()}中存在未知卡牌：{item.cardNo}");
                }
            }
        }
    }

    /// <summary>
    /// 添加卡组item到列表中
    /// </summary>
    void AddCardGroupItemToScrollView(UserCardGroup userCardGroup)
    {
        GameObject gameObject = Instantiate(cardGroupItemPrefab, cardGroupScrollViewTransform);
        CardCroupItemScript cardCroupItemScript = gameObject.GetComponent<CardCroupItemScript>();
        cardCroupItemList.Add(cardCroupItemScript);
        cardCroupItemScript.InitInfo(userCardGroup.cardGroupName, this);
    }

    /// <summary>
    /// 新建卡组按钮点击事件
    /// </summary>
    public void NewCardGroupButtonClick()
    {
        UserData userData = gameManager.GetUserData();
        UserCardGroup newCardGroup = userData.AddNewCardGroup();
        AddCardGroupItemToScrollView(newCardGroup);
    }

    /// <summary>
    /// 通过名称编辑卡组
    /// </summary>
    public void EditCardGroupByName(string cardGroupName)
    {
        gameManager.EnterCardGroupEditScene(cardGroupName);
    }

    /// <summary>
    /// 通过名称删除卡组
    /// </summary>
    public void DeleteCardGroupByName(string cardGroupName)
    {
        for (int i = 0; i < cardCroupItemList.Count; i++)
        {
            if (cardCroupItemList[i].GetCardGroupName() == cardGroupName)
            {
                DeleteCardGroupByIndex(i);
                return;
            }
        }
    }

    /// <summary>
    /// 通过指数删除卡组
    /// </summary>
    public void DeleteCardGroupByIndex(int index)
    {
        if (index < 0 || index >= cardCroupItemList.Count)
        {
            return;
        }

        UserData userData = gameManager.GetUserData();
        userData.DeleteCardGroupByName(cardCroupItemList[index].GetCardGroupName());

        DestroyImmediate(cardCroupItemList[index].gameObject);
        cardCroupItemList.RemoveAt(index);
        if(currentSelectCardCroupIndex==index)
        {
            currentSelectCardCroupIndex = -1;
            if (cardCroupItemList.Count>0)
            {
                SelectCardGroupByIndex(0);
            }
            else
            {
                CleanCardPanel();
            }
        }
    }

    /// <summary>
    /// 清理卡牌面板，一般用于将选中并展示的卡组内容从面板内清空。
    /// </summary>
    void CleanCardPanel()
    {
        GameManager.CleanPanelContent(mainPanelTransform);
        mainTotalNumberText.GetComponent<Text>().text = "主卡组：";

        GameManager.CleanPanelContent(extraPanelTransform);
        extraTotalNumberText.GetComponent<Text>().text = "额外卡组：";

        GameManager.CleanPanelContent(deputyPanelTransform);
        deputyTotalNumberText.GetComponent<Text>().text = "副卡组：";
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
