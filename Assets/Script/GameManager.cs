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
    /// <summary>
    /// 游戏管理类
    /// </summary>
    class GameManager
    {
        //单例
        static GameManager gameManagerInstance = new GameManager();

        AudioScript audioScript;
        GameState currentGameState;

        #region 路径与文件名
        string saveFileName = "UserDate.xml";
        string cardResourceRootDirectory = "CardData";
        string bgmName = "DuelBGM";
        string duelBgmName = "光宗信吉-神々の戦い";
        #endregion

        #region 用户和卡组信息
        UserData userData;
        public Dictionary<int,CardBase> allCardInfoList { get; private set; }
        #endregion

        #region 决斗
        DuelScene duelScene = null;
        #endregion

        #region 网络
        ServerManager serverManager = null;
        ClientManager clientManager = null;
        Queue<ClientProtocol> protocolQueue = new Queue<ClientProtocol>();
        bool isServer = false;
        #endregion

        List<TimerFunction> timerFunctions = new List<TimerFunction>();

        string cardGroupNameForCardGroupEditScene = "";

        private GameManager()
        {
            currentGameState = GameState.MainScene;

            LoadUserData();
            InitAudio();
            LoadAllCardData();
        }
        
        public static GameManager GetSingleInstance()
        {
            return gameManagerInstance;
        }

        /// <summary>
        /// 初始化声音
        /// </summary>
        void InitAudio()
        {
            audioScript = GameObject.Find("BGMAudio").GetComponent<AudioScript>();
            audioScript.Init();
            audioScript.SetAudioByName(bgmName);
            audioScript.SetVolume(userData.audioValue);
        }

        /// <summary>
        /// 获得用户存档路径
        /// </summary>
        /// <returns></returns>
        public string GetUserDataPath()
        {
            return Application.dataPath + "/Artres/" + saveFileName;
        }

        /// <summary>
        /// 加载玩家数据
        /// </summary>
        void LoadUserData()
        {
            if (File.Exists(GetUserDataPath()))
            {
                userData = XMLHelper.LoadDataFromXML<UserData>(GetUserDataPath());
            }
            else
            {
                Debug.LogError("LoadUserData,存档缺失！");
                userData = new UserData();
            }
        }

        /// <summary>
        /// 保存玩家数据
        /// </summary>
        void SaveUserData()
        {
            XMLHelper.SaveDataToXML(GetUserDataPath(), userData);
        }

        /// <summary>
        /// 获得玩家数据
        /// </summary>
        /// <returns></returns>
        public UserData GetUserData()
        {
            return userData;
        }

        /// <summary>
        /// 获得当前游戏状态
        /// </summary>
        /// <returns></returns>
        public GameState GetCurrentGameState()
        {
            return currentGameState;
        }

        /// <summary>
        /// 获得卡牌路径
        /// </summary>
        /// <returns></returns>
        public string GetCardResourceRootPath()
        {
            return Application.dataPath + "/Artres/" + cardResourceRootDirectory + "/";
        }

        /// <summary>
        /// 设置音量大小
        /// </summary>
        /// <param name="value"></param>
        public void SetAudioVolume(float value)
        {
            value = Mathf.Clamp01(value);
            userData.audioValue = value;
            audioScript.SetVolume(value);
        }

        /// <summary>
        /// 返回上一场景
        /// </summary>
        public void ReturnLastScene()
        {
            switch (currentGameState)
            {
                case GameState.MainScene:
                    break;
                case GameState.SeleteDuelModeScene:
                    SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
                    currentGameState = GameState.MainScene;
                    break;
                case GameState.DuelScene:
                    SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
                    currentGameState = GameState.MainScene;
                    break;
                case GameState.CardGroupScene:
                    SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
                    currentGameState = GameState.MainScene;
                    break;
                case GameState.SettingScene:
                    SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
                    currentGameState = GameState.MainScene;
                    break;
                case GameState.CardGroupEditScene:
                    SceneManager.LoadScene("CardGroupScene", LoadSceneMode.Single);
                    currentGameState = GameState.CardGroupScene;
                    break;
                case GameState.GuessFirstScene:
                    duelScene.myPlayer.StopDuel();
                    SceneManager.LoadScene("SeleteDuelModeScene", LoadSceneMode.Single);
                    currentGameState = GameState.SeleteDuelModeScene;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 加载所有卡片信息
        /// </summary>
        void LoadAllCardData()
        {
            allCardInfoList = new Dictionary<int, CardBase>();
            if(!Directory.Exists(GetCardResourceRootPath()))
            {
                Debug.LogError("卡牌资源路径不存在："+ GetCardResourceRootPath());
                return;
            }

            int i = 1;
            DirectoryInfo directoryInfo = new DirectoryInfo(GetCardResourceRootPath());
            foreach (var item in directoryInfo.GetDirectories())
            {
                WWW www= new WWW(item.FullName + "/image.jpg");
                Texture2D texture = new Texture2D(177,254);
                www.LoadImageIntoTexture(texture);
                Sprite image = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                
                CardBase card = CardBase.LoadCardFromInfo(int.Parse(item.Name), File.ReadAllText(item.FullName + "/script.txt"));
                card.SetImage(image);
                i = int.Parse(item.Name);
                allCardInfoList[i] = card;
            }
            Debug.Log("加载卡牌数量："+allCardInfoList.Count);
        }

        /// <summary>
        /// 进入主场景
        /// </summary>
        public void EnterMainScene()
        {
            if (currentGameState != GameState.MainScene)
            {
                currentGameState = GameState.MainScene;
                SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
            }
            else
            {
                Debug.LogError("进入主场景失败，已经进入主场景。");
            }
        }

        /// <summary>
        /// 进入卡组场景
        /// </summary>
        public void EnterCardGroupScene()
        {
            if (currentGameState == GameState.MainScene)
            {
                currentGameState = GameState.CardGroupScene;
                SceneManager.LoadScene("CardGroupScene", LoadSceneMode.Single);
            }
            else
            {
                Debug.LogError("进入卡组场景失败，只能由主场景进入，当前场景为：" + currentGameState.ToString());
            }
        }

        /// <summary>
        /// 进入卡组编辑场景
        /// </summary>
        public void EnterCardGroupEditScene(string cardGroupName)
        {
            if (currentGameState == GameState.CardGroupScene)
            {
                currentGameState = GameState.CardGroupEditScene;
                SceneManager.LoadScene("CardGroupEditScene", LoadSceneMode.Single);
                cardGroupNameForCardGroupEditScene = cardGroupName;
            }
            else
            {
                Debug.LogError("进入卡组编辑场景失败，只能由卡组场景进入，当前场景为：" + currentGameState.ToString());
            }
        }

        /// <summary>
        /// 进入设置场景
        /// </summary>
        public void EnterSettingScene()
        {
            if (currentGameState == GameState.MainScene)
            {
                currentGameState = GameState.SettingScene;
                SceneManager.LoadScene("SettingScene", LoadSceneMode.Single);
            }
            else
            {
                Debug.LogError("进入设置场景失败，只能由主场景进入，当前场景为：" + currentGameState.ToString());
            }
        }

        /// <summary>
        /// 进入选择模式场景
        /// </summary>
        public void EnterSelectModeScene()
        {
            if (currentGameState == GameState.MainScene)
            {
                currentGameState = GameState.SeleteDuelModeScene;
                SceneManager.LoadScene("SeleteDuelModeScene", LoadSceneMode.Single);
            }
            else
            {
                Debug.LogError("进入决斗场景失败，只能由主场景进入，当前场景为：" + currentGameState.ToString());
            }
        }

        /// <summary>
        /// 进入决斗场景
        /// </summary>
        public void EnterDuelScene()
        {
            if (currentGameState == GameState.GuessFirstScene)
            {
                currentGameState = GameState.DuelScene;
                SceneManager.LoadScene("DuelScene", LoadSceneMode.Single);
            }
            else
            {
                Debug.LogError("进入决斗场景失败，只能由猜先场景进入，当前场景为：" + currentGameState.ToString());
            }
        }

        /// <summary>
        /// 进入猜先场景
        /// </summary>
        public void EnterGuessFirstScene()
        {
            if (currentGameState == GameState.SeleteDuelModeScene)
            {
                currentGameState = GameState.GuessFirstScene;
                SceneManager.LoadScene("GuessFirstScene", LoadSceneMode.Single);
            }
            else
            {
                Debug.LogError("进入决斗场景失败，只能由选择决斗模式场景进入，当前场景为：" + currentGameState.ToString());
            }
        }

        public string GetCardGroupNameForCardGroupEditScene()
        {
            return cardGroupNameForCardGroupEditScene;
        }

        public DuelScene GetDuelScene()
        {
            return duelScene;
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

            if (ServerManager.GetInstance().GetHostIPV4().ToString() == userData.ip)
            {
                serverManager = ServerManager.GetInstance();
                serverManager.AcceptNewSocketEvent += (Socket s) => { Debug.LogError("有新客户端！"); };
                serverManager.SocketDisconnectEvent += (Socket s) => { Debug.LogError("客户端断开！"); };
                if (serverManager.StartListen(userData.port))
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
                clientManager.StartConnect(userData.ip, userData.port);
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
                if(duelScene.myPlayer.SetGuessEnum(guessEnum))
                {
                    if(duelScene.GetDuelMode()==DuelMode.Net)
                    {
                        CGuessFirst guessFirst = new CGuessFirst();
                        guessFirst.AddContent("guess", duelScene.myPlayer.GetGuessEnum().ToString());
                        clientManager.SendProtocol(guessFirst);
                    }
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
        /// 停止决斗
        /// </summary>
        public void StopDuel()
        {
            ShowMessage("停止决斗。");
            duelScene = null;
        }

        /// <summary>
        /// 设置对方猜先的选择
        /// </summary>
        /// <param name="opponentGuess"></param>
        public void SetOpponentGuess(GuessEnum opponentGuess)
        {
            if (duelScene != null)
            {
                if (duelScene.opponentPlayer.SetGuessEnum(opponentGuess))
                {
                    GameObject.Find("opponentPanel").transform.GetChild((int)opponentGuess - 1).GetComponent<GuessFirstScript>().SetChooseState();
                    DecideGuessFirst();
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
        /// 决定谁先出牌
        /// </summary>
        void DecideGuessFirst()
        {
            if(duelScene.GetDuelMode()==DuelMode.Single)
            {
                duelScene.opponentPlayer.SetGuessEnum((GuessEnum)UnityEngine.Random.Range(1, 4));
                GameObject.Find("opponentPanel").transform.GetChild((int)duelScene.opponentPlayer.GetGuessEnum() - 1).GetComponent<GuessFirstScript>().SetChooseState();
            }
            GuessEnum myGuessEnum = duelScene.myPlayer.GetGuessEnum();
            GuessEnum opponentGuessEnum = duelScene.opponentPlayer.GetGuessEnum();
            if (myGuessEnum!=GuessEnum.Unknown&&opponentGuessEnum!=GuessEnum.Unknown)
            {
                if(myGuessEnum == opponentGuessEnum)
                {
                    TimerFunction reguessTimeFunction = new TimerFunction();
                    reguessTimeFunction.SetFunction(1, () =>
                    {
                        ClearChoose();
                    });

                    AddTimerFunction(reguessTimeFunction);
                    ShowMessage("双方选择相同，需重新选择！");

                    return;
                }

                TimerFunction timeFunction =  new TimerFunction();
                timeFunction.SetFunction(1,()=> 
                {
                    EnterDuelScene();
                    audioScript.SetAudioByName(duelBgmName);
                    int tempValue = (int)myGuessEnum - (int)opponentGuessEnum;
                    if (tempValue == 1 || tempValue == -2)
                    {
                        ShowMessage("您先手！");
                        duelScene.SetFirst(true);
                    }
                    else
                    {
                        ShowMessage("您后手！");
                        duelScene.SetFirst(false);
                    }
                });

                AddTimerFunction(timeFunction);
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

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            CheckTimerFunction();
            if (currentGameState==GameState.DuelScene)
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

        /// <summary>
        /// 检测计时器时间
        /// </summary>
        void CheckTimerFunction()
        {
            for (int i = timerFunctions.Count - 1; i >= 0; i--)
            {
                timerFunctions[i].Update(Time.deltaTime);
                if(timerFunctions[i].GetRemainTime()<=0)
                {
                    timerFunctions[i].DoFunction();
                    timerFunctions.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 清空容器内控件
        /// </summary>
        public static void CleanPanelContent(Transform transform)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                GameObject go = transform.GetChild(i).gameObject;
                UnityEngine.Object.DestroyImmediate(go);
            }
        }

        /// <summary>
        /// 设置单人模式
        /// </summary>
        public void SetSingleMode()
        {
            duelScene = new DuelScene(DuelMode.Single);
            EnterGuessFirstScene();
        }

        /// <summary>
        /// 设置网络模式
        /// </summary>
        public void SetNetMode()
        {
            duelScene = new DuelScene(DuelMode.Net);
            EnterGuessFirstScene();
        }

        /// <summary>
        /// 添加计时器
        /// </summary>
        public static void AddTimerFunction(TimerFunction timerFunction)
        {
            gameManagerInstance.timerFunctions.Add(timerFunction);
        }
    }
}
