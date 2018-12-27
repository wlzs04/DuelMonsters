using Assets.Script;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardGroupScript : MonoBehaviour
{
    GameManager gameManager;

    public Transform cardGroupScrollViewTransform;

    public GameObject cardGroupItemPrefab;

    void Start()
    {
        gameManager = GameManager.GetSingleInstance();
        UserData userData = gameManager.GetUserData();

        GameManager.CleanPanelContent(cardGroupScrollViewTransform);

        foreach (var item in userData.userCardGroupList)
        {
            GameObject gameObject = Instantiate(cardGroupItemPrefab, cardGroupScrollViewTransform);
            gameObject.GetComponent<CardCroupItemScript>().InitCardCroupName(item.cardGroupName);
        }
    }

    void Update()
    {

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
    /// 返回到主场景
    /// </summary>
    public void ReturnMainScene()
    {
        GameManager.GetSingleInstance().ReturnLastScene();
    }
}
