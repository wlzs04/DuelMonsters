using Assets.Script.Card;
using Assets.Script.Config;
using Assets.Script.Duel;
using Assets.Script.Duel.EffectProcess;
using Assets.Script.Duel.Rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Script
{ 
    class DuelSceneScript : MonoBehaviour
    {
        public GameObject monsterCardPre = null;
        public GameObject magicTrapCardPre = null;

        public GameObject buttonPrefab = null;
        public GameObject itemSelectPrefab = null;

        public Transform infoContentTransform = null;

        GameObject duelResultPanel = null;//决斗结果
        GameObject phaseTypePanel = null;//阶段
        GameObject attackOrDefensePanel = null;//攻守选择
        GameObject helpInfoPanel = null;//帮助信息
        GameObject effectSelectPanel = null;//效果选择
        GameObject tossCoinPanel = null;//抛硬币
        GameObject throwDicePanel = null;//掷骰子
        GameObject selectItemPanel = null;//选择项
        GameObject makeSurePanel = null;//确认

        GameObject cardListPanel = null;//卡牌列表
        //此列表是否可以由玩家隐藏(点击右键)
        bool canHideByPlayerForCardListPanel;

        Transform cardListContentTransform = null;

        UnityAction<CardGameState> selectAttackOrDefenseFinishAction = null;

        Image cardImage;

        DuelScene duelScene = null;
        string duelBgmName = "光宗信吉-神々の戦い";

        Action<CoinType, CoinType> actionTossCoin;

        private void Awake()
        {
            helpInfoPanel = GameObject.Find("HelpInfoPanel");
        }

        void Start()
        {
            GameManager.GetSingleInstance().SetAudioByName(duelBgmName);
            duelScene = GameManager.GetDuelScene();
            duelScene.Init();
            cardImage = GameObject.Find("cardImage").GetComponent<Image>();

            Transform canvasTransform = GameObject.Find("Canvas").transform;

            duelResultPanel = canvasTransform.Find("DuelResultPanel").gameObject;
            duelResultPanel.SetActive(false);

            phaseTypePanel = canvasTransform.Find("rightPanel/PhaseTypePanel").gameObject;
            phaseTypePanel.SetActive(false);

            PhaseTypeConfig phaseTypeConfig = ConfigManager.GetConfigByName("PhaseType") as PhaseTypeConfig;

            for (int i = 1; i < phaseTypePanel.transform.GetChild(0).childCount; i++)
            {
                phaseTypePanel.transform.GetChild(0).GetChild(i).GetComponent<Text>().text = phaseTypeConfig.GetRecordById(i).value;
            }

            attackOrDefensePanel = canvasTransform.Find("rightPanel/AttackOrDefensePanel").gameObject;
            attackOrDefensePanel.SetActive(false);

            effectSelectPanel = canvasTransform.Find("rightPanel/EffectSelectPanel").gameObject;
            effectSelectPanel.SetActive(false);

            cardListPanel = canvasTransform.Find("rightPanel/CardListPanel").gameObject;
            cardListContentTransform = cardListPanel.transform.GetChild(0).GetChild(0).GetChild(0);
            cardListPanel.SetActive(false);

            tossCoinPanel = canvasTransform.Find("rightPanel/TossCoinPanel").gameObject;
            tossCoinPanel.SetActive(false);

            throwDicePanel = canvasTransform.Find("rightPanel/ThrowDicePanel").gameObject;
            throwDicePanel.SetActive(false);

            selectItemPanel = canvasTransform.Find("rightPanel/SelectItemPanel").gameObject;
            selectItemPanel.SetActive(false);

            makeSurePanel = canvasTransform.Find("rightPanel/MakeSurePanel").gameObject;
            makeSurePanel.SetActive(false);
        }

        /// <summary>
        /// 进入战斗流程事件
        /// </summary>
        public void EnterBattleProcessEvent()
        {
            GameManager.GetDuelScene().GetMyPlayer().Battle();
            phaseTypePanel.SetActive(false);
        }

        /// <summary>
        /// 进入第二流程事件
        /// </summary>
        public void EnterSecondProcessEvent()
        {
            GameManager.GetDuelScene().GetMyPlayer().Second();
            phaseTypePanel.SetActive(false);
        }

        /// <summary>
        /// 结束流程事件
        /// </summary>
        public void EndProcessEvent()
        {
            GameManager.GetDuelScene().GetMyPlayer().EndTurn();
            phaseTypePanel.SetActive(false);
        }

        /// <summary>
        /// 玩家投降事件
        /// </summary>
        public void SurrenderEvent()
        {
            GameManager.GetDuelScene().GetMyPlayer().Surrender();
        }

        /// <summary>
        /// 设置决斗流程面板是否显示
        /// </summary>
        /// <param name="show"></param>
        public void SetPhaseTypePanel(bool show)
        {
            phaseTypePanel.SetActive(show);
            if(show)
            {
                ResetPhaseTypePanelInfo();
            }
        }

        /// <summary>
        /// 重新设置决斗流程面板信息面板
        /// </summary>
        public void ResetPhaseTypePanelInfo()
        {
            if(!phaseTypePanel.activeSelf)
            {
                return;
            }
            bool isMyTurn = GameManager.GetDuelScene().GetMyPlayer().IsMyTurn();
            Color color = isMyTurn ? Color.green : Color.red;
            int currentPhaseType = (int)GameManager.GetDuelScene().GetCurrentPhaseType();
            for (int i = 1; i < phaseTypePanel.transform.GetChild(0).childCount; i++)
            {
                if(currentPhaseType > i)
                {
                    if(phaseTypePanel.transform.GetChild(0).GetChild(i).GetComponent<Button>()!=null)
                    {
                        phaseTypePanel.transform.GetChild(0).GetChild(i).GetComponent<Button>().interactable = false;
                    }
                    phaseTypePanel.transform.GetChild(0).GetChild(i).GetComponent<Text>().color = Color.gray;
                }
                else if(currentPhaseType == i)
                {
                    if (phaseTypePanel.transform.GetChild(0).GetChild(i).GetComponent<Button>() != null)
                    {
                        phaseTypePanel.transform.GetChild(0).GetChild(i).GetComponent<Button>().interactable = false;
                    }
                    phaseTypePanel.transform.GetChild(0).GetChild(i).GetComponent<Text>().color = Color.yellow;
                }
                else
                {
                    if (phaseTypePanel.transform.GetChild(0).GetChild(i).GetComponent<Button>() != null)
                    {
                        phaseTypePanel.transform.GetChild(0).GetChild(i).GetComponent<Button>().interactable = isMyTurn;
                    }
                    phaseTypePanel.transform.GetChild(0).GetChild(i).GetComponent<Text>().color = color;
                }
            }
            StringResConfig stringResConfig = ConfigManager.GetConfigByName("StringRes") as StringResConfig;
            phaseTypePanel.transform.GetChild(1).GetComponent<Text>().text = stringResConfig.GetRecordById(15).value+duelScene.GetCurrentTurnNumber();
        }

        /// <summary>
        /// 设置卡片详细信息
        /// </summary>
        /// <param name="card"></param>
        public void SetInfoContent(CardBase card)
        {
            GameManager.CleanPanelContent(infoContentTransform);

            cardImage.sprite = card.GetImage();

            GameObject gameObject = null;
            if (card.GetCardType() == CardType.Monster)
            {
                gameObject = Instantiate(monsterCardPre, infoContentTransform);
                gameObject.transform.GetChild(0).GetComponent<Text>().text = "名称：" + card.GetName();
                gameObject.transform.GetChild(1).GetComponent<Text>().text = "属性：" + card.GetPropertyTypeString() + "/" + card.GetMonsterTypeString() + "/" + card.GetLevel();
                gameObject.transform.GetChild(2).GetComponent<Text>().text = "攻击力：" + card.GetAttackValue();
                gameObject.transform.GetChild(3).GetComponent<Text>().text = "防御力：" + card.GetDefenseValue();
                gameObject.transform.GetChild(4).GetComponent<Text>().text = "效果：" + card.GetEffectInfo();
            }
            else if (card.GetCardType() == CardType.Magic)
            {
                gameObject = Instantiate(magicTrapCardPre, infoContentTransform);
                gameObject.transform.GetChild(0).GetComponent<Text>().text = "名称：" + card.GetName();
                gameObject.transform.GetChild(1).GetComponent<Text>().text = "类型：" + card.GetMagicTypeString() + card.GetCardTypeString();
                gameObject.transform.GetChild(2).GetComponent<Text>().text = "效果：" + card.GetEffectInfo();
            }
            else if (card.GetCardType() == CardType.Trap)
            {
                gameObject = Instantiate(magicTrapCardPre, infoContentTransform);
                gameObject.transform.GetChild(0).GetComponent<Text>().text = "名称：" + card.GetName();
                gameObject.transform.GetChild(1).GetComponent<Text>().text = "类型：" + card.GetTrapTypeString() + card.GetCardTypeString();
                gameObject.transform.GetChild(2).GetComponent<Text>().text = "效果：" + card.GetEffectInfo();
            }
            else
            {
                Debug.LogError("卡片类型错误！");
            }
        }

        /// <summary>
        /// 攻击防御选择面板是否显示
        /// </summary>
        public bool AttackOrDefensePanelIsShowing()
        {
            return attackOrDefensePanel.activeSelf;
        }

        /// <summary>
        /// 显示攻击防御选择面板
        /// </summary>
        public void ShowAttackOrDefensePanel(CardBase monsterCard, UnityAction<CardGameState> finishAction)
        {
            attackOrDefensePanel.SetActive(true);
            attackOrDefensePanel.transform.Find("BackPanel").Find("LeftPanel").Find("LeftImage").GetComponent<Image>().sprite = monsterCard.GetImage();
            attackOrDefensePanel.transform.Find("BackPanel").Find("RightPanel").Find("RightImage").GetComponent<Image>().sprite = monsterCard.GetImage();
            selectAttackOrDefenseFinishAction = finishAction;
        }

        /// <summary>
        /// 选择攻击表示召唤
        /// </summary>
        public void SelectAttackToCall()
        {
            attackOrDefensePanel.SetActive(false);
            selectAttackOrDefenseFinishAction(CardGameState.FrontAttack);
        }

        /// <summary>
        /// 选择防守表示召唤
        /// </summary>
        public void SelectDefenseToCall()
        {
            attackOrDefensePanel.SetActive(false);
            selectAttackOrDefenseFinishAction(CardGameState.FrontDefense);
        }

        /// <summary>
        /// 效果选择面板是否显示
        /// </summary>
        /// <returns></returns>
        public bool EffectSelectPanelIsShowing()
        {
            return attackOrDefensePanel.activeSelf;
        }

        /// <summary>
        /// 显示效果选择面板
        /// </summary>
        public void ShowEffectSelectPanel(CardBase card,List<string> effectList, ActionIndex finishAction)
        {
            if(effectList.Count <1)
            {
                Debug.LogError("需要进行选择的效果数量不足!");
                return;
            }
            effectSelectPanel.SetActive(true);
            GameObject selectButtonPanel = effectSelectPanel.transform.Find("BackPanel").Find("SelectButtonPanel").gameObject;
            Text effectInfoText = effectSelectPanel.transform.Find("BackPanel").Find("EffectInfoPanel").Find("EffectInfoText").GetComponent<Text>();

            GameManager.CleanPanelContent(selectButtonPanel.transform);

            for (int i = 0; i < effectList.Count; i++)
            {
                int selectIndex = i;
                GameObject buttonObject = Instantiate(buttonPrefab, selectButtonPanel.transform);
                buttonObject.GetComponent<Button>().onClick.AddListener(()=> 
                {
                    effectSelectPanel.SetActive(false);
                    finishAction(card, selectIndex);
                });
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerEnter;
                entry.callback.AddListener( (baseEventData) =>
                {
                    effectInfoText.text = effectList[selectIndex];
                });
                buttonObject.GetComponent<EventTrigger>().triggers.Add(entry);
                buttonObject.transform.Find("Text").GetComponent<Text>().text = "效果"+(i+1);
            }

            effectSelectPanel.transform.Find("BackPanel").Find("EffectInfoPanel").Find("EffectInfoText").GetComponent<Text>().text = effectList[0];
        }

        /// <summary>
        /// 抛硬币面板是否显示
        /// </summary>
        /// <returns></returns>
        public bool TossCoinPanelIsShowing()
        {
            return tossCoinPanel.activeSelf;
        }

        /// <summary>
        /// 显示抛硬币面板
        /// </summary>
        public void ShowTossCoinPanel(bool showSelectCoinPanel, Action<CoinType, CoinType> actionTossCoin, CoinType coinType=CoinType.Unknown)
        {
            tossCoinPanel.SetActive(true);
            this.actionTossCoin = actionTossCoin;
            if (showSelectCoinPanel)
            {
                tossCoinPanel.transform.GetChild(0).gameObject.SetActive(true);
                tossCoinPanel.transform.GetChild(1).gameObject.SetActive(false);
            }
            else
            {
                tossCoinPanel.transform.GetChild(0).gameObject.SetActive(false);
                tossCoinPanel.transform.GetChild(1).gameObject.SetActive(true);
                StringResConfig stringResConfig = ConfigManager.GetConfigByName("StringRes") as StringResConfig;
                Text playInfoText = tossCoinPanel.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Text>();
                switch (coinType)
                {
                    case CoinType.Unknown:
                        playInfoText.text = "";
                        break;
                    case CoinType.Front:
                        playInfoText.text = stringResConfig.GetRecordById(19).value + "正面";
                        break;
                    case CoinType.Back:
                        playInfoText.text = stringResConfig.GetRecordById(19).value + "反面";
                        break;
                    default:
                        break;
                }
            }

            CoinType resultCoinType = (CoinType)UnityEngine.Random.Range(1, 3);
            Sprite coinSprite = tossCoinPanel.transform.GetChild(0).GetChild((int)resultCoinType).GetChild(0).gameObject.GetComponent<Image>().sprite;
            tossCoinPanel.transform.GetChild(1).GetChild(1).gameObject.GetComponent<Image>().sprite = coinSprite;

            TimerFunction timerFunction = new TimerFunction();
            timerFunction.SetFunction(1, ()=>
            {
                tossCoinPanel.SetActive(false);
                actionTossCoin(coinType, resultCoinType);
            });

            GameManager.AddTimerFunction(timerFunction);
        }

        /// <summary>
        /// 选择正面
        /// </summary>
        public void SelectFrontCoin()
        {
            ShowTossCoinPanel(false, actionTossCoin,CoinType.Front);
        }

        /// <summary>
        /// 选择反面
        /// </summary>
        public void SelectBackCoin()
        {
            ShowTossCoinPanel(false, actionTossCoin, CoinType.Back);
        }

        /// <summary>
        /// 抛硬币面板是否显示
        /// </summary>
        /// <returns></returns>
        public bool ThrowDicePanelIsShowing()
        {
            return throwDicePanel.activeSelf;
        }

        /// <summary>
        /// 显示掷骰子面板
        /// </summary>
        /// <param name="actionIndex"></param>
        public void ShowThrowDicePanel(Action<int> actionIndex)
        {
            throwDicePanel.SetActive(true);
            int resultNumber = UnityEngine.Random.Range(1, 7);
            for (int i = 1; i < 7; i++)
            {
                throwDicePanel.transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
            }
            throwDicePanel.transform.GetChild(0).GetChild(resultNumber).gameObject.SetActive(true);
            StringResConfig stringResConfig = ConfigManager.GetConfigByName("StringRes") as StringResConfig;
            throwDicePanel.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text= stringResConfig.GetRecordById(20).value+resultNumber;

            TimerFunction timerFunction = new TimerFunction();
            timerFunction.SetFunction(1, () =>
            {
                throwDicePanel.SetActive(false);
                actionIndex(resultNumber);
            });

            GameManager.AddTimerFunction(timerFunction);
        }

        /// <summary>
        /// 显示选择项面板
        /// </summary>
        /// <param name="actionIndex"></param>
        public void ShowSelectItemPanel(Type type, UnityAction<int> actionIndex)
        {
            selectItemPanel.SetActive(true);

            GameObject content = selectItemPanel.transform.Find("BackPanel").Find("Scroll View").Find("Viewport").Find("Content").gameObject;

            GameManager.CleanPanelContent(content.transform);

            ConfigBase config;

            if (type== typeof(PropertyType))
            {
            }
            else if(type == typeof(MonsterType))
            {
            }
            else
            {
                Debug.LogError("显示列表选择面板中出现未知类型：" + type);
                selectItemPanel.SetActive(false);
                return;
            }

            config = ConfigManager.GetConfigByName(type.Name);
            Array itemList = Enum.GetValues(type);

            for (int i = 1; i < itemList.Length; i++)
            {
                int selectIndex = i;
                GameObject buttonObject = Instantiate(itemSelectPrefab, content.transform);
                buttonObject.GetComponent<Button>().onClick.AddListener(() =>
                {
                    selectItemPanel.SetActive(false);
                    actionIndex(selectIndex);
                });
                buttonObject.transform.Find("Text").GetComponent<Text>().text = config.GetRecordValueById(i);
            }
        }

        /// <summary>
        /// 显示帮助信息面板
        /// </summary>
        public void ShowHelpInfoPanel(int myRemainCardNumberText,int opponentRemainCardNumberText)
        {
            helpInfoPanel.SetActive(true);
            StringResConfig stringResConfig = ConfigManager.GetConfigByName("StringRes") as StringResConfig;
            helpInfoPanel.transform.Find("MyRemainCardNumberText").GetComponent<Text>().text = stringResConfig.GetRecordById(8).value + myRemainCardNumberText;
            helpInfoPanel.transform.Find("OpponentRemainCardNumberText").GetComponent<Text>().text = stringResConfig.GetRecordById(8).value + opponentRemainCardNumberText;
        }

        /// <summary>
        /// 显示标题，暂时使用帮助信息面板显示标题
        /// </summary>
        /// <param name="titleText"></param>
        public void ShowTitle(string titleText)
        {
            if (string.IsNullOrWhiteSpace(titleText))
            {
                Debug.Log(titleText);
            }
            helpInfoPanel.transform.Find("TitleText").GetComponent<Text>().text = titleText;
        }

        /// <summary>
        /// 抛硬币面板是否显示
        /// </summary>
        /// <returns></returns>
        public bool MakeSurePanelIsShowing()
        {
            return makeSurePanel.activeSelf;
        }

        /// <summary>
        /// 显示确认面板
        /// </summary>
        /// <param name="titleText"></param>
        /// <param name="chooseCanChainCard"></param>
        /// <param name="realProcessFunction"></param>
        public void ShowMakeSurePanel(string titleText, UnityAction chooseCanChainCard, UnityAction realProcessFunction)
        {
            makeSurePanel.SetActive(true);
            makeSurePanel.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = titleText;

            makeSurePanel.transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
            makeSurePanel.transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(() => { makeSurePanel.SetActive(false); chooseCanChainCard(); });

            makeSurePanel.transform.GetChild(0).GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
            makeSurePanel.transform.GetChild(0).GetChild(2).GetComponent<Button>().onClick.AddListener(()=> { makeSurePanel.SetActive(false); realProcessFunction(); });
        }

        /// <summary>
        /// 显示卡牌列表面板
        /// </summary>
        public void ShowCardListPanel(List<CardBase> cardList,string title, bool canHideByPlayerForCardListPanel, CardBase launchEffectCard, Action<CardBase, CardBase> clickCallback)
        {
            this.canHideByPlayerForCardListPanel = canHideByPlayerForCardListPanel;
            cardListPanel.SetActive(true);
            cardListPanel.transform.GetChild(1).GetComponent<Text>().text = title;
            foreach (var item in cardList)
            {
                item.GetDuelCardScript().SetParent(cardListContentTransform);
                item.GetDuelCardScript().SetClickCallback(launchEffectCard, clickCallback);
            }
        }

        /// <summary>
        /// 卡牌列表面板是否显示
        /// </summary>
        /// <returns></returns>
        public bool CardListPanelIsShowing()
        {
            return cardListPanel.activeSelf;
        }

        /// <summary>
        /// 隐藏卡牌列表面板
        /// </summary>
        public void HideCardListPanel(bool fromPlayer)
        {
            if(!canHideByPlayerForCardListPanel && fromPlayer)
            {
                return;
            }
            cardListPanel.SetActive(false);
            for (int i = cardListContentTransform.childCount - 1; i >= 0; i--)
            {
                CardBase card = cardListContentTransform.GetChild(i).GetComponent<DuelCardScript>().GetCard();
                card.GetDuelCardScript().SetParent(duelScene.duelBackImage.transform);
                card.GetDuelCardScript().RemoveClickCallback();
                card.SetCardGameState(card.GetCardGameState());
            }
        }

        /// <summary>
        /// 显示决斗结果面板
        /// </summary>
        /// <param name="winnerPlayer"></param>
        /// <param name="duelEndReason"></param>
        public void ShowDuelResultPanel(Player winnerPlayer, DuelEndReason duelEndReason)
        {
            duelResultPanel.SetActive(true);
            StringResConfig stringResConfig = ConfigManager.GetConfigByName("StringRes") as StringResConfig;
            string resultText = "";
            if (winnerPlayer == null)
            {
                resultText = stringResConfig.GetRecordById(16).value;
            }
            else if (winnerPlayer == duelScene.GetMyPlayer())
            {
                resultText = stringResConfig.GetRecordById(17).value;
            }
            else
            {
                resultText = stringResConfig.GetRecordById(18).value;
            }
            duelResultPanel.transform.GetChild(1).GetComponent<Text>().text = resultText;

            DuelEndReasonConfig duelEndReasonConfig = ConfigManager.GetConfigByName("DuelEndReason") as DuelEndReasonConfig;
            duelResultPanel.transform.GetChild(3).GetComponent<Text>().text = duelEndReasonConfig.GetRecordById((int)duelEndReason).value;
        }

        /// <summary>
        /// 我方墓地点击回调事件
        /// </summary>
        public void MyTombClickCallBack()
        {
            duelScene.ShowCardList(duelScene.GetMyPlayer(), CardGameState.Tomb, true, null, null);
        }

        /// <summary>
        /// 对方墓地点击回调事件
        /// </summary>
        public void OpponentTombClickCallBack()
        {
            duelScene.ShowCardList(duelScene.GetOpponentPlayer(), CardGameState.Tomb, true, null, null);
        }
    }
}
