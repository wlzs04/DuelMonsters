using Assets.Script.Card;
using Assets.Script.Config;
using Assets.Script.Duel;
using Assets.Script.Duel.Rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Script
{ 
    class DuelSceneScript : MonoBehaviour
    {
        public GameObject monsterCardPre = null;
        public GameObject magicTrapCardPre = null;

        public Transform infoContentTransform = null;

        GameObject durlProcessPanel = null;
        GameObject attackOrDefensePanel = null;
        GameObject helpInfoPanel = null;

        GameObject cardListPanel = null;
        //此列表是否可以由玩家隐藏(点击右键)
        bool canHideByPlayerForCardListPanel;

        GameObject duelResultPanel = null;
        Transform cardListContentTransform = null;

        UnityAction<CardGameState> selectAttackOrDefenseFinishAction = null;

        Image cardImage;

        DuelScene duelScene = null;
        string duelBgmName = "光宗信吉-神々の戦い";

        private void Awake()
        {
            helpInfoPanel = GameObject.Find("HelpInfoPanel");
        }

        void Start()
        {
            GameManager.GetSingleInstance().SetAudioByName(duelBgmName);
            duelScene = GameManager.GetSingleInstance().GetDuelScene();
            duelScene.Init();
            cardImage = GameObject.Find("cardImage").GetComponent<Image>();

            durlProcessPanel = GameObject.Find("DuelProcessPanel");
            durlProcessPanel.SetActive(false);

            DuelProcessConfig duelProcessConfig = ConfigManager.GetConfigByName("DuelProcess") as DuelProcessConfig;

            for (int i = 1; i < durlProcessPanel.transform.GetChild(0).childCount; i++)
            {
                durlProcessPanel.transform.GetChild(0).GetChild(i).GetComponent<Text>().text = duelProcessConfig.GetRecordById(i).value;
            }

            attackOrDefensePanel = GameObject.Find("AttackOrDefensePanel");
            attackOrDefensePanel.SetActive(false);

            cardListPanel = GameObject.Find("CardListPanel");
            cardListContentTransform = cardListPanel.transform.GetChild(0).GetChild(0).GetChild(0);
            cardListPanel.SetActive(false);

            duelResultPanel = GameObject.Find("DuelResultPanel");
            duelResultPanel.SetActive(false);
        }

        /// <summary>
        /// 进入战斗流程事件
        /// </summary>
        public void EnterBattleProcessEvent()
        {
            GameManager.GetSingleInstance().GetDuelScene().myPlayer.Battle();
            durlProcessPanel.SetActive(false);
        }

        /// <summary>
        /// 进入第二流程事件
        /// </summary>
        public void EnterSecondProcessEvent()
        {
            GameManager.GetSingleInstance().GetDuelScene().myPlayer.Second();
            durlProcessPanel.SetActive(false);
        }

        /// <summary>
        /// 结束流程事件
        /// </summary>
        public void EndProcessEvent()
        {
            GameManager.GetSingleInstance().GetDuelScene().myPlayer.EndTurn();
            durlProcessPanel.SetActive(false);
        }

        /// <summary>
        /// 玩家投降事件
        /// </summary>
        public void SurrenderEvent()
        {
            GameManager.GetSingleInstance().GetDuelScene().myPlayer.Surrender();
        }

        /// <summary>
        /// 设置决斗流程面板是否显示
        /// </summary>
        /// <param name="show"></param>
        public void SetDuelProcessPanel(bool show)
        {
            durlProcessPanel.SetActive(show);
            if(show)
            {
                ResetDuelProcessPanelInfo();
            }
        }

        /// <summary>
        /// 重新设置决斗流程面板信息面板
        /// </summary>
        public void ResetDuelProcessPanelInfo()
        {
            if(!durlProcessPanel.activeSelf)
            {
                return;
            }
            bool isMyTurn = GameManager.GetSingleInstance().GetDuelScene().myPlayer.IsMyTurn();
            Color color = isMyTurn ? Color.green : Color.red;
            int currentDuelProcess = (int)GameManager.GetSingleInstance().GetDuelScene().currentDuelProcess;
            for (int i = 1; i < durlProcessPanel.transform.GetChild(0).childCount; i++)
            {
                if(currentDuelProcess > i)
                {
                    if(durlProcessPanel.transform.GetChild(0).GetChild(i).GetComponent<Button>()!=null)
                    {
                        durlProcessPanel.transform.GetChild(0).GetChild(i).GetComponent<Button>().interactable = false;
                    }
                    durlProcessPanel.transform.GetChild(0).GetChild(i).GetComponent<Text>().color = Color.gray;
                }
                else if(currentDuelProcess==i)
                {
                    if (durlProcessPanel.transform.GetChild(0).GetChild(i).GetComponent<Button>() != null)
                    {
                        durlProcessPanel.transform.GetChild(0).GetChild(i).GetComponent<Button>().interactable = false;
                    }
                    durlProcessPanel.transform.GetChild(0).GetChild(i).GetComponent<Text>().color = Color.yellow;
                }
                else
                {
                    if (durlProcessPanel.transform.GetChild(0).GetChild(i).GetComponent<Button>() != null)
                    {
                        durlProcessPanel.transform.GetChild(0).GetChild(i).GetComponent<Button>().interactable = isMyTurn;
                    }
                    durlProcessPanel.transform.GetChild(0).GetChild(i).GetComponent<Text>().color = color;
                }
            }
            StringResConfig stringResConfig = ConfigManager.GetConfigByName("StringRes") as StringResConfig;
            durlProcessPanel.transform.GetChild(1).GetComponent<Text>().text = stringResConfig.GetRecordById(15).value+duelScene.GetCurrentTurnNumber();
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
                //MonsterCard monsterCard = (MonsterCard)card;
                gameObject = Instantiate(monsterCardPre, infoContentTransform);
                gameObject.transform.GetChild(0).GetComponent<Text>().text = "名称：" + card.GetName();
                gameObject.transform.GetChild(1).GetComponent<Text>().text = "属性：" + card.GetPropertyTypeString() + "/" + card.GetMonsterTypeString() + "/" + card.GetLevel();
                gameObject.transform.GetChild(2).GetComponent<Text>().text = "攻击力：" + card.GetAttackNumber();
                gameObject.transform.GetChild(3).GetComponent<Text>().text = "防御力：" + card.GetDefenseNumber();
                gameObject.transform.GetChild(4).GetComponent<Text>().text = "效果：" + card.GetEffectInfo();
            }
            else if (card.GetCardType() == CardType.Magic)
            {
                //MagicCard magicCard = (MagicCard)card;
                gameObject = Instantiate(magicTrapCardPre, infoContentTransform);
                gameObject.transform.GetChild(0).GetComponent<Text>().text = "名称：" + card.GetName();
                gameObject.transform.GetChild(1).GetComponent<Text>().text = "类型：" + card.GetMagicTypeString() + card.GetCardTypeString();
                gameObject.transform.GetChild(2).GetComponent<Text>().text = "效果：" + card.GetEffectInfo();
            }
            else if (card.GetCardType() == CardType.Trap)
            {
                //TrapCard trapCard = (TrapCard)card;
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
        /// 显示卡牌列表面板
        /// </summary>
        public void ShowCardListPanel(List<CardBase> cardList,string title, bool canHideByPlayerForCardListPanel, Action<Player,CardBase> clickCalback)
        {
            this.canHideByPlayerForCardListPanel = canHideByPlayerForCardListPanel;
            cardListPanel.SetActive(true);
            cardListPanel.transform.GetChild(1).GetComponent<Text>().text = title;
            foreach (var item in cardList)
            {
                item.GetDuelCardScript().SetParent(cardListContentTransform);
                item.GetDuelCardScript().SetClickCallback(clickCalback);
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
            for (int i = 0; i < cardListContentTransform.childCount; i++)
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
            else if (winnerPlayer == duelScene.myPlayer)
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
    }
}
