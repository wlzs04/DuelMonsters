using Assets.Script.Card;
using Assets.Script.Duel;
using Assets.Script.Helper;
using Assets.Script.Net;
using Assets.Script.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
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
        string bgmName = "DuelBGM";
        string duelBgmName = "光宗信吉-神々の戦い";

        string resourcesCardPath = "/CardData/";
        string ip = "10.237.20.13";
        int port = 7777;
        ServerManager serverManager = null;
        ClientManager clientManager = null;
        Queue<ClientProtocol> protocolQueue = new Queue<ClientProtocol>();
        bool isServer = false;

        public UserData Userdata { get; private set; }
        public GameState CurrentGameState { get; private set; }

        //AudioSource bgmPlayer;
        AudioScript audioScript;

        public Dictionary<int,CardBase> allCardInfoList { get; private set; }

        DuelScene duelScene = null;
        GuessEnum myGuessEnum ;
        GuessEnum opponentGuessEnum;

        private GameManager()
        {
            CurrentGameState = GameState.MainScene;
            myGuessEnum = GuessEnum.Unknown;

            LoadUserData();
            LoadAllCardData();

            audioScript = GameObject.Find("BGMAudio").GetComponent<AudioScript>();
            audioScript.Init();
            audioScript.SetAudioByName(bgmName);
            audioScript.SetVolume(Userdata.audioValue);
        }

        public static GameManager GetSingleInstance()
        {
            return gameManagerInstance;
        }

        public DuelScene GetDuelScene()
        {
            return duelScene;
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
            audioScript.SetVolume(value);
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
                Userdata = new UserData();
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
            if(!Directory.Exists(Application.dataPath + resourcesCardPath))
            {
                Debug.LogError("此路径没有找到卡牌："+ Application.dataPath + resourcesCardPath);
                return;
            }
            DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath + resourcesCardPath);

            foreach (var item in directoryInfo.GetDirectories())
            {
                WWW www= new WWW(item.FullName + "\\image.jpg");
                Texture2D texture = new Texture2D(177,254);
                www.LoadImageIntoTexture(texture);
                Sprite image = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                //Resources.Load<Sprite>(resourcesCardPath + item.Name + "/image");
                
                CardBase card = CardBase.LoadCardFromInfo(int.Parse(item.Name), File.ReadAllText(item.FullName + "/script.txt"));
                card.SetImage(image);
                i = int.Parse(item.Name);
                allCardInfoList[i] = card;
            }
            //foreach (var item in Directory.GetDirectories(Application.persistentDataPath + cardPath))
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
                SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
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
                StartNet();
            }
            else
            {
                Debug.LogError("EnterDuelScene");
            }
        }

        /// <summary>
        /// 启动网络功能
        /// </summary>
        public void StartNet()
        {
            clientManager = ClientManager.GetSingleInstance();
            
            clientManager.AddLegalProtocol(new CGuessFirst());
            clientManager.AddLegalProtocol(new CDrawCard());
            clientManager.AddLegalProtocol(new CCardGroup());
            clientManager.AddLegalProtocol(new CCallMonster());
            clientManager.AddLegalProtocol(new CEndTurn());
            clientManager.AddLegalProtocol(new CEnterDuelProcess());
            clientManager.AddLegalProtocol(new CAttackMonster());
            clientManager.AddLegalProtocol(new CAttackDirect());
            clientManager.AddLegalProtocol(new CCallMonsterBySacrifice());

            if (ServerManager.GetInstance().GetHostIPV4().ToString() == ip )
            {
                serverManager = ServerManager.GetInstance();
                serverManager.AcceptNewSocketEvent += (Socket s) => { Debug.LogError("有新客户端！"); };
                serverManager.SocketDisconnectEvent += (Socket s) => { Debug.LogError("客户端断开！"); };
                if (serverManager.StartListen(port))
                {
                    serverManager.ProcessProtocolEvent += ProcessProtocolEvent;
                    isServer = true;
                }
            }

            if(!isServer)
            {
                clientManager.ConnectSuccessEvent += (Socket s) => { Debug.LogError("连接成功！"); };
                clientManager.ConnectFailEvent += (Socket s) => { Debug.LogError("连接失败！"); };
                clientManager.DisconnectEvent += (Socket s) => { Debug.LogError("连接断开！"); };
                clientManager.StartConnect(ip, port);
                clientManager.ProcessProtocolEvent += ProcessProtocolEvent;
            }
        }
        
        void ProcessProtocolEvent(ClientProtocol protocol)
        {
            protocolQueue.Enqueue(protocol);
        }

        void ProcessProtocol()
        {
            while (protocolQueue.Count>0)
            {
                ClientProtocol protocol = protocolQueue.Dequeue();
                protocol.Process();
            }
        }

        /// <summary>
        /// 设定我方猜先选择
        /// </summary>
        /// <param name="guessEnum"></param>
        /// <returns></returns>
        public bool SetMyGuess(GuessEnum guessEnum)
        {
            if (duelScene != null)
            {
                if(myGuessEnum == GuessEnum.Unknown)
                {
                    myGuessEnum = guessEnum;
                    CGuessFirst guessFirst = new CGuessFirst();
                    guessFirst.AddContent("guess", myGuessEnum.ToString());
                    clientManager.SendProtocol(guessFirst);
                    DecideGuessFirst();
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
        /// 设置对方猜先的选择
        /// </summary>
        /// <param name="opponentGuess"></param>
        public void SetOpponentGuess(GuessEnum opponentGuess)
        {
            if (duelScene != null)
            {
                opponentGuessEnum = opponentGuess;
                GameObject.Find("opponentPanel").transform.GetChild((int)opponentGuess-1).GetComponent<GuessFirstScript>().SetChooseState();
                DecideGuessFirst();
            }
        }

        public void ClearChoose()
        {
            GameObject.Find("myPanel").transform.GetChild((int)myGuessEnum - 1).GetComponent<GuessFirstScript>().ClearChooseState();
            GameObject.Find("opponentPanel").transform.GetChild((int)opponentGuessEnum - 1).GetComponent<GuessFirstScript>().ClearChooseState();
            
            myGuessEnum = GuessEnum.Unknown;
            opponentGuessEnum = GuessEnum.Unknown;
        }

        /// <summary>
        /// 决定谁先出牌
        /// </summary>
        void DecideGuessFirst()
        {
            if(myGuessEnum!=GuessEnum.Unknown&&opponentGuessEnum!=GuessEnum.Unknown)
            {
                if(myGuessEnum==opponentGuessEnum)
                {
                    ClearChoose();
                    ShowMessage("重新选择！");
                    return;
                }
                SceneManager.LoadScene("DuelScene",LoadSceneMode.Single);
                audioScript.SetAudioByName(duelBgmName);
                int tempValue = (int)myGuessEnum - (int)opponentGuessEnum;
                if(tempValue == 1|| tempValue == - 2)
                {
                    ShowMessage("您先手！");
                    duelScene.SetFirst(true);
                }
                else
                {
                    ShowMessage("您后手！");
                    duelScene.SetFirst(false);
                }
            }
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

        public void Update()
        {
            if(CurrentGameState==GameState.DuelScene)
            {
                ProcessProtocol();
            }
            if(Input.GetMouseButtonDown(1))
            {
                if(duelScene!=null)
                {
                    duelScene.MouseRightButtonDown();
                }
            }
        }
    }
}
