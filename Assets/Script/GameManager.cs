using Assets.Script.Card;
using Assets.Script.Duel;
using Assets.Script.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Script
{
    class GameManager
    {
        static GameManager gameManagerInstance = new GameManager();
        string gameSavePath = "/UserDate.txt";
        string audioPath = "Audio/";
        string bgmName = "DuelBGM";
        string cardPath = "/Resources/CardData/";
        string resourcesCardPath = "CardData/";

        public UserData Userdata { get; private set; }
        public GameState CurrentGameState { get; private set; }

        AudioSource bgmPlayer;

        public Dictionary<int,CardBase> allCardInfoList { get; private set; }

        DuelScene duelScene = null;
        GuessEnum myGuessEnum ;

        private GameManager()
        {
            CurrentGameState = GameState.MainScene;
            myGuessEnum = GuessEnum.Unknown;

            LoadUserData();

            LoadAllCardData();

            bgmPlayer = GameObject.Find("BGMAudio").GetComponent<AudioSource>();
            bgmPlayer.clip = Resources.Load<AudioClip>(audioPath + bgmName); ;
            bgmPlayer.volume = Userdata.audioValue;
            bgmPlayer.Play();
        }

        public static GameManager GetSingleInstance()
        {
            return gameManagerInstance;
        }

        /// <summary>
        /// 从卡组中移除一张卡
        /// </summary>
        /// <param name="card"></param>
        public void RemoveCardFromCardGroup(CardBase card)
        {
            foreach (var item in Userdata.userCardGroupList)
            {
                if(item.cardNo==card.GetCardNo())
                {
                    Userdata.userCardGroupList.Remove(item);
                    break;
                }
            }
        }
        
        /// <summary>
        /// 向卡组中添加一张卡
        /// </summary>
        /// <param name="card"></param>
        public void AddCardToCardGroup(CardBase card)
        {
            foreach (var item in Userdata.userCardGroupList)
            {
                if (item.cardNo == card.GetCardNo())
                {
                    item.number++;
                    return;
                }
            }
            UserCardData ucd = new UserCardData();
            ucd.cardNo = card.GetCardNo();
            ucd.number = 1;
            Userdata.userCardGroupList.Add(ucd);
        }

        /// <summary>
        /// 设置音量大小
        /// </summary>
        /// <param name="value"></param>
        public void SetAudioVolume(float value)
        {
            Userdata.audioValue = value;
            bgmPlayer.volume = value;
        }

        /// <summary>
        /// 返回上一场景
        /// </summary>
        public void ReturnLastScene()
        {
            switch (CurrentGameState)
            {
                case GameState.MainScene:
                    break;
                case GameState.DuelScene:
                    SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
                    CurrentGameState = GameState.MainScene;
                    break;
                case GameState.CardGroupScene:
                    SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
                    CurrentGameState = GameState.MainScene;
                    break;
                case GameState.SettingScene:
                    SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
                    CurrentGameState = GameState.MainScene;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 加载玩家数据
        /// </summary>
        public void LoadUserData()
        {
            if (File.Exists(Application.dataPath + gameSavePath))
            {
                Userdata = XMLHelper.LoadDataFromXML<UserData>(Application.dataPath + gameSavePath);
            }
            else
            {
                Debug.LogError("LoadUserData,存档缺失！");
                //Userdata = new UserData();
                //UserCardData ucd = new UserCardData();
                //ucd.cardNo = 1;
                //ucd.number = 2;
                //Userdata.userCardList.Add(ucd);
            }
        }

        /// <summary>
        /// 保存玩家数据
        /// </summary>
        public void SaveUserData()
        {
            XMLHelper.SaveDataToXML(Application.dataPath + gameSavePath,Userdata);
        }

        /// <summary>
        /// 加载所有卡片信息
        /// </summary>
        public void LoadAllCardData()
        {
            allCardInfoList = new Dictionary<int, CardBase>();
            int i = 1;
            DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath + cardPath);

            foreach (var item in directoryInfo.GetDirectories())
            {
                Sprite image = Resources.Load<Sprite>(resourcesCardPath + item.Name + "/image");
                CardBase card = CardBase.LoadCardFromInfo(int.Parse(item.Name), File.ReadAllText(Application.dataPath + cardPath+ item.Name + "/script.txt"));
                card.SetImage(image);
                i = int.Parse(item.Name);
                allCardInfoList[i] = card;
            }
            //foreach (var item in Directory.GetDirectories(Application.dataPath + cardPath))
            //{
            //    string no = item.Substring(item.LastIndexOf('/') + 1);
            //    Sprite image = Resources.Load<Sprite>(resourcesCardPath + no+"/image");
            //    CardBase card = new CardBase();
            //    card.SetImage(image);
            //    i = int.Parse(no);
            //    allCardInfoList[i] = card;
            //}
        }

        /// <summary>
        /// 进入主场景
        /// </summary>
        public void EnterMainScene()
        {
            if (CurrentGameState != GameState.MainScene)
            {
                CurrentGameState = GameState.MainScene;
            }
            else
            {
                Debug.LogError("EnterMainScene");
            }
        }

        /// <summary>
        /// 进入决斗界面
        /// </summary>
        public void EnterDuelScene()
        {
            if (CurrentGameState == GameState.MainScene)
            {
                CurrentGameState = GameState.DuelScene;
                SceneManager.LoadScene("GuessFirstScene", LoadSceneMode.Single);
                duelScene = new DuelScene();
            }
            else
            {
                Debug.LogError("EnterDuelScene");
            }
        }

        /// <summary>
        /// 设定猜先选择
        /// </summary>
        /// <param name="guessEnum"></param>
        /// <returns></returns>
        public bool SetMyGuess(GuessEnum guessEnum)
        {
            if (duelScene != null)
            {
                if(myGuessEnum == GuessEnum.Unknown|| myGuessEnum == guessEnum)
                {
                    myGuessEnum = guessEnum;
                    return true;
                }
                else
                {
                    ShowMessage("选择后不能修改！");
                }
            }
            else
            {
                Debug.LogError("SetMyGuess");
            }
            return false;
        }

        /// <summary>
        /// 弹出提示框
        /// </summary>
        /// <param name="value"></param>
        public static void ShowMessage(string value)
        {
            GameObject gameObject = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefab/MessageTip"), GameObject.Find("Canvas").transform);
            gameObject.transform.GetChild(0).GetComponent<Text>().text = value;
        }

        /// <summary>
        /// 进入卡组界面
        /// </summary>
        public void EnterCardGroupScene()
        {
            if (CurrentGameState == GameState.MainScene)
            {
                CurrentGameState = GameState.CardGroupScene;
                SceneManager.LoadScene("CardGroupScene", LoadSceneMode.Single);
            }
            else
            {
                Debug.LogError("EnterCardGroupScene");
            }
        }

        /// <summary>
        /// 进入设置场景
        /// </summary>
        public void EnterSettingScene()
        {
            if (CurrentGameState == GameState.MainScene)
            {
                CurrentGameState = GameState.SettingScene;
                SceneManager.LoadScene("SettingScene",LoadSceneMode.Single);
            }
            else
            {
                Debug.LogError("EnterSettingScene");
            }
        }

        /// <summary>
        /// 退出游戏
        /// </summary>
        public void QuitGame()
        {
            SaveUserData();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }
}
