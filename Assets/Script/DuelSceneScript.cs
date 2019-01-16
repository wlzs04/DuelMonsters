using Assets.Script.Card;
using Assets.Script.Config;
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

        UnityAction<CardGameState> selectAttackOrDefenseFinishAction = null;

        Image cardImage;

        private void Awake()
        {
            helpInfoPanel = GameObject.Find("HelpInfoPanel");
        }

        void Start()
        {
            //audioScript.SetAudioByName(duelBgmName);
            GameManager.GetSingleInstance().GetDuelScene().Init();
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
                MonsterCard monsterCard = (MonsterCard)card;
                gameObject = Instantiate(monsterCardPre, infoContentTransform);
                gameObject.transform.GetChild(0).GetComponent<Text>().text = "名称：" + monsterCard.GetName();
                gameObject.transform.GetChild(1).GetComponent<Text>().text = "属性：" + monsterCard.GetPropertyTypeString() + "/" + monsterCard.GetMonsterTypeString() + "/" + monsterCard.GetLevel();
                gameObject.transform.GetChild(2).GetComponent<Text>().text = "攻击力：" + monsterCard.GetAttackNumber();
                gameObject.transform.GetChild(3).GetComponent<Text>().text = "防御力：" + monsterCard.GetDefenseNumber();
                gameObject.transform.GetChild(4).GetComponent<Text>().text = "效果：" + monsterCard.GetEffect();
            }
            else if (card.GetCardType() == CardType.Magic)
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
        /// 攻击防御选择面板是否显示
        /// </summary>
        public bool AttackOrDefensePanelIsShowing()
        {
            return attackOrDefensePanel.activeSelf;
        }

        /// <summary>
        /// 显示攻击防御选择面板
        /// </summary>
        public void ShowAttackOrDefensePanel(MonsterCard monsterCard, UnityAction<CardGameState> finishAction)
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
    }
}
