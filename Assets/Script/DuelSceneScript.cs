using Assets.Script.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script
{
    class DuelSceneScript : MonoBehaviour
    {
        public GameObject monsterCardPre = null;
        public GameObject magicTrapCardPre = null;

        public Transform infoContentTransform = null;
        Image cardImage;

        void Start()
        {
            GameManager.GetSingleInstance().GetDuelScene().Init();
            cardImage= GameObject.Find("cardImage").GetComponent<Image>();
        }

        public void EnterBattleProcessEvent()
        {
            GameManager.GetSingleInstance().GetDuelScene().myPlayer.Battle();
        }

        public void EndProcessEvent()
        {
            GameManager.GetSingleInstance().GetDuelScene().myPlayer.EndTurn();
        }

        public void SurrenderEvent()
        {
            GameManager.GetSingleInstance().GetDuelScene().myPlayer.Surrender();
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
            if (card.GetCardType() == CardType.Monster)
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
            else if (card.GetCardType() == CardType.Magic)
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
    }
}
