using Assets.Script;
using Assets.Script.Config;
using Assets.Script.Duel;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuessFirstSceneScript : MonoBehaviour {

    public GameObject selectCardGroupPanel;
    public GameObject guessFirstPanel;
    public GameObject selectFirstPanel;
    public Transform cardGroupScrollViewTransform;

    public GameObject cardGroupItemPrefab;

    DuelScene duelScene;

    int iGuessWin = 0;//0-未知 1-成功 2-失败

    void Start()
    {
        //先选择卡组再猜先
        selectCardGroupPanel.SetActive(true);
        guessFirstPanel.SetActive(false);
        selectFirstPanel.SetActive(false);
        duelScene = GameManager.GetDuelScene();

        GameManager.CleanPanelContent(cardGroupScrollViewTransform);
        UserData userData = GameManager.GetSingleInstance().GetUserData();

        foreach (var item in userData.userCardGroupList)
        {
            AddCardGroupItemToScrollView(item);
        }
    }

    void AddCardGroupItemToScrollView(UserCardGroup userCardGroup)
    {
        GameObject gameObject = Instantiate(cardGroupItemPrefab, cardGroupScrollViewTransform);
        CardCroupItemScript cardCroupItemScript = gameObject.GetComponent<CardCroupItemScript>();
        cardCroupItemScript.InitInfo(userCardGroup.cardGroupName, null, this);
    }

    /// <summary>
    /// 通过名称选中指定卡组
    /// </summary>
    /// <param name="cardGroupName"></param>
    public void SelectCardGroupByName(string cardGroupName)
    {
        //先检测
        if (GameManager.CheckCardGroupLegal(cardGroupName))
        {
            //暂时将我方和对方卡组设为一样
            GameManager.GetDuelScene().GetMyPlayer().SetCardGroupName(cardGroupName);
            GameManager.GetDuelScene().GetOpponentPlayer().SetCardGroupName(cardGroupName);
            selectCardGroupPanel.SetActive(false);
            guessFirstPanel.SetActive(true);
            selectFirstPanel.SetActive(false);
        }
    }

    public bool SetMyGuess(GuessEnum guessEnum)
    {
        if (duelScene.GetMyPlayer().SetGuessEnum(guessEnum))
        {
            GameObject.Find("myPanel").transform.GetChild((int)duelScene.GetMyPlayer().GetGuessEnum() - 1).GetComponent<GuessFirstScript>().SetChooseState();
            DecideGuessFirst();
            return true;
        }
        else
        {
            GameManager.ShowMessage("选择后不能修改！");
        }
        return false;
    }

    public bool SetOpponentGuess(GuessEnum guessEnum)
    {
        if (duelScene.GetOpponentPlayer().SetGuessEnum(guessEnum))
        {
            GameObject.Find("opponentPanel").transform.GetChild((int)duelScene.GetOpponentPlayer().GetGuessEnum() - 1).GetComponent<GuessFirstScript>().SetChooseState();
            DecideGuessFirst();
            return true;
        }
        else
        {
            GameManager.ShowMessage("选择后不能修改！");
        }
        return false;
    }

    /// <summary>
    /// 决定谁先出牌
    /// </summary>
    public void DecideGuessFirst()
    {
        if(iGuessWin!=0)
        {
            return;
        }
        GuessEnum myGuessEnum = duelScene.GetMyPlayer().GetGuessEnum();
        GuessEnum opponentGuessEnum = duelScene.GetOpponentPlayer().GetGuessEnum();
        if (myGuessEnum != GuessEnum.Unknown && opponentGuessEnum != GuessEnum.Unknown)
        {
            if (myGuessEnum == opponentGuessEnum)
            {
                TimerFunction reguessTimeFunction = new TimerFunction();
                reguessTimeFunction.SetFunction(1, () =>
                {
                    ClearChoose();
                });

                GameManager.AddTimerFunction(reguessTimeFunction);
                GameManager.ShowMessage("双方选择相同，需重新选择！");

                return;
            }

            int tempValue = (int)myGuessEnum - (int)opponentGuessEnum;
            StringResConfig stringResConfig = ConfigManager.GetConfigByName("StringRes") as StringResConfig;
            string title;
            if (tempValue == 1 || tempValue == -2)
            {
                iGuessWin = 1;
                title = stringResConfig.GetRecordById(13).value;
            }
            else
            {
                iGuessWin = 2;
                title = stringResConfig.GetRecordById(14).value;
            }

            TimerFunction timerFunction = new TimerFunction();

            timerFunction.SetFunction(1, () => 
            {
                selectCardGroupPanel.SetActive(false);
                guessFirstPanel.SetActive(false);
                selectFirstPanel.SetActive(true);
                selectFirstPanel.transform.GetChild(0).GetComponent<Text>().text = title;

                if (iGuessWin == 1)
                {
                    duelScene.GetMyPlayer().SelectFristOrBack();
                }

                if (iGuessWin == 2)
                {
                    duelScene.GetOpponentPlayer().SelectFristOrBack();
                }
            });
            GameManager.AddTimerFunction(timerFunction);
        }
    }

    /// <summary>
    /// 清空猜先选择
    /// </summary>
    public void ClearChoose()
    {
        if(duelScene.GetMyPlayer().GetGuessEnum()!=GuessEnum.Unknown)
        {
            GameObject.Find("myPanel").transform.GetChild((int)duelScene.GetMyPlayer().GetGuessEnum() - 1).GetComponent<GuessFirstScript>().ClearChooseState();
            duelScene.GetMyPlayer().SetGuessEnum(GuessEnum.Unknown);
        }
        if (duelScene.GetOpponentPlayer().GetGuessEnum() != GuessEnum.Unknown)
        {
            GameObject.Find("opponentPanel").transform.GetChild((int)duelScene.GetOpponentPlayer().GetGuessEnum() - 1).GetComponent<GuessFirstScript>().ClearChooseState();
            duelScene.GetOpponentPlayer().SetGuessEnum(GuessEnum.Unknown);
        }
    }

    /// <summary>
    /// 选择先手事件
    /// </summary>
    public void SelectFirstHandEvent()
    {
        if(iGuessWin == 1)
        {
            duelScene.SetFirst(true);
            TimerFunction timeFunction = new TimerFunction();
            timeFunction.SetFunction(1, () =>
            {
                GameManager.GetSingleInstance().EnterDuelScene();
            });

            GameManager.AddTimerFunction(timeFunction);
            selectFirstPanel.SetActive(false);
        }
        else
        {
            GameManager.ShowMessage("请等待对方选择！");
        }
    }

    /// <summary>
    /// 选择后手事件
    /// </summary>
    public void SelectBackHandEvent()
    {
        if (iGuessWin == 1)
        {
            duelScene.SetFirst(false);
            TimerFunction timeFunction = new TimerFunction();
            timeFunction.SetFunction(1, () =>
            {
                GameManager.GetSingleInstance().EnterDuelScene();
            });

            GameManager.AddTimerFunction(timeFunction);
            selectFirstPanel.SetActive(false);
        }
        else
        {
            GameManager.ShowMessage("请等待对方选择！");
        }
    }

    /// <summary>
    /// 隐藏选先界面
    /// </summary>
    public  void HideSelectFirstPanel()
    {
        selectFirstPanel.SetActive(false);
    }
}
