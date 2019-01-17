using Assets.Script;
using Assets.Script.Config;
using Assets.Script.Duel;
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

    string cardGroupName;

    bool iGuessWin = false;//我方是否猜先成功

    void Start()
    {
        //先选择卡组再猜先
        selectCardGroupPanel.SetActive(true);
        guessFirstPanel.SetActive(false);
        selectFirstPanel.SetActive(false);
        duelScene = GameManager.GetSingleInstance().GetDuelScene();

        GameManager.CleanPanelContent(cardGroupScrollViewTransform);
        UserData userData = GameManager.GetSingleInstance().GetUserData();

        foreach (var item in userData.userCardGroupList)
        {
            AddCardGroupItemToScrollView(item);
        }
    }

    void Update()
    {

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
        this.cardGroupName = cardGroupName;
        GameManager.GetSingleInstance().GetDuelScene().SetMyPlayerCardGroupName(cardGroupName);
        selectCardGroupPanel.SetActive(false);
        guessFirstPanel.SetActive(true);
        selectFirstPanel.SetActive(false);
    }

    public bool SetMyGuess(GuessEnum guessEnum)
    {
        if (duelScene.myPlayer.SetGuessEnum(guessEnum))
        {
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
    void DecideGuessFirst()
    {
        if (duelScene.GetDuelMode() == DuelMode.Single)
        {
            if (GameManager.GetSingleInstance().GetUserData().guessMustWin)
            {
                int tempMyGuessEnum = (int)duelScene.myPlayer.GetGuessEnum();
                int tempOpponentGuessEnum = tempMyGuessEnum - 1 > 0 ? tempMyGuessEnum - 1 : 3;
                duelScene.opponentPlayer.SetGuessEnum((GuessEnum)tempOpponentGuessEnum);
            }
            else
            {
                duelScene.opponentPlayer.SetGuessEnum((GuessEnum)UnityEngine.Random.Range(1, 4));
            }
            GameObject.Find("opponentPanel").transform.GetChild((int)duelScene.opponentPlayer.GetGuessEnum() - 1).GetComponent<GuessFirstScript>().SetChooseState();
        }
        GuessEnum myGuessEnum = duelScene.myPlayer.GetGuessEnum();
        GuessEnum opponentGuessEnum = duelScene.opponentPlayer.GetGuessEnum();
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
                iGuessWin = true;
                title = stringResConfig.GetRecordById(13).value;
            }
            else
            {
                iGuessWin = false;
                title = stringResConfig.GetRecordById(14).value;
            }
            selectCardGroupPanel.SetActive(false);
            guessFirstPanel.SetActive(false);
            selectFirstPanel.SetActive(true);
            selectFirstPanel.transform.GetChild(0).GetComponent<Text>().text = title;
            if(!iGuessWin)
            {
                TimerFunction timerFunction = new TimerFunction();
                timerFunction.SetFunction(2, () => 
                {
                    GameManager.ShowMessage("您先手！");
                    duelScene.SetFirst(true);
                    GameManager.GetSingleInstance().EnterDuelScene();
                });
                GameManager.AddTimerFunction(timerFunction);
            }
        }
    }

    /// <summary>
    /// 清空猜先选择
    /// </summary>
    public void ClearChoose()
    {
        GameObject.Find("myPanel").transform.GetChild((int)duelScene.myPlayer.GetGuessEnum() - 1).GetComponent<GuessFirstScript>().ClearChooseState();
        GameObject.Find("opponentPanel").transform.GetChild((int)duelScene.opponentPlayer.GetGuessEnum() - 1).GetComponent<GuessFirstScript>().ClearChooseState();

        duelScene.myPlayer.SetGuessEnum(GuessEnum.Unknown);
        duelScene.opponentPlayer.SetGuessEnum(GuessEnum.Unknown);
    }

    /// <summary>
    /// 选择先手事件
    /// </summary>
    public void SelectFirstHandEvent()
    {
        if(iGuessWin)
        {
            GameManager.ShowMessage("您先手！");
            TimerFunction timeFunction = new TimerFunction();
            timeFunction.SetFunction(1, () =>
            {
                duelScene.SetFirst(true);
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
        if (iGuessWin)
        {
            GameManager.ShowMessage("您后手！");
            TimerFunction timeFunction = new TimerFunction();
            timeFunction.SetFunction(1, () =>
            {
                duelScene.SetFirst(false);
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
}
