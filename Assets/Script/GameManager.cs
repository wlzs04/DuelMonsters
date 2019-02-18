using Assets.Script.Card;
using Assets.Script.Duel;
using Assets.Script.Duel.Rule;
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
using System.Threading;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XLua;

namespace Assets.Script
{
    /// <summary>
    /// 游戏管理类
    /// </summary>
    class GameManager
    {
        static string cardResourceRootDirectory = "CardData";
        static string rootPath;

        //单例
        static LuaEnv luaEnv = new LuaEnv(); //all lua behaviour shared one luaenv only!
        static GameManager gameManagerInstance = new GameManager();

        bool initDataFinish = false;
        AudioScript audioScript;
        GameState currentGameState;

        #region 路径与文件名
        string saveFileName = "UserDate.xml";
        string bgmName = "DuelBGM";
        #endregion

        #region 用户和卡组信息
        UserData userData;
        Dictionary<int, CardBase> allCardInfoList;
        #endregion

        #region 决斗
        DuelScene duelScene = null;
        Sprite cardBackImage = null;
        GameObject cardOperationButtonPrefab = null;
        #endregion

        #region 网络
        ServerManager serverManager = null;
        ClientManager clientManager = null;
        Queue<ClientProtocol> protocolQueue = new Queue<ClientProtocol>();
        bool isServer = false;
        #endregion

        List<TimerFunction> timerFunctions = new List<TimerFunction>();
        static List<GameObject> messageTipList = new List<GameObject>();

        string cardGroupNameForCardGroupEditScene = "";

        private GameManager()
        {
            currentGameState = GameState.Init;
            rootPath = Application.dataPath;

            InitData();
            InitDataFinishCallBack();
        }
        
        public static GameManager GetSingleInstance()
        {
            return gameManagerInstance;
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public void InitData()
        {
            if(initDataFinish)
            {
                Debug.LogError("游戏中数据仅初始化一次！");
                return;
            }

            string cardBackPath = rootPath + "/Texture/CardBack.jpg";
            WWW www = new WWW(cardBackPath);
            Texture2D texture = new Texture2D(177, 254);
            www.LoadImageIntoTexture(texture);
            cardBackImage = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

            string cardOperationButtonPrefabPath = "Prefab/CardOperationButtonPre";
            cardOperationButtonPrefab = Resources.Load<GameObject>(cardOperationButtonPrefabPath);
            DuelRuleManager.InitDuelRule();

            LoadUserData();
            LoadAllCardData();
        }

        public void InitDataFinishCallBack()
        {
            initDataFinish = true;
            InitAudio();
        }

        public static LuaEnv GetLuaEnv()
        {
            return luaEnv;
        }

        /// <summary>
        /// 初始化声音
        /// </summary>
        void InitAudio()
        {
            audioScript = GameObject.Find("BGMAudio").GetComponent<AudioScript>();
            audioScript.SetAudioByName(bgmName);
            audioScript.SetVolume(userData.audioValue);
        }

        /// <summary>
        /// 播放指定音频文件
        /// </summary>
        /// <param name="audioName"></param>
        public void SetAudioByName(string audioName)
        {
            audioScript.SetAudioByName(audioName);
        }

        /// <summary>
        /// 获得用户存档路径
        /// </summary>
        /// <returns></returns>
        string GetUserDataPath()
        {
            return rootPath + "/Artres/" + saveFileName;
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
        public static string GetCardResourceRootPath()
        {
            return rootPath + "/Artres/" + cardResourceRootDirectory + "/";
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
                    SaveUserData();
                    currentGameState = GameState.MainScene;
                    break;
                case GameState.SettingScene:
                    SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
                    SaveUserData();
                    currentGameState = GameState.MainScene;
                    break;
                case GameState.CardGroupEditScene:
                    SceneManager.LoadScene("CardGroupScene", LoadSceneMode.Single);
                    SaveUserData();
                    currentGameState = GameState.CardGroupScene;
                    break;
                case GameState.GuessFirstScene:
                    duelScene.myPlayer.StopDuel();
                    SceneManager.LoadScene("SeleteDuelModeScene", LoadSceneMode.Single);
                    currentGameState = GameState.SeleteDuelModeScene;
                    break;
                default:
                    Debug.LogError("当前场景未知："+ currentGameState);
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
            
            DirectoryInfo directoryInfo = new DirectoryInfo(GetCardResourceRootPath());
            foreach (var item in directoryInfo.GetDirectories())
            {
                WWW www= new WWW(item.FullName + "/image.jpg");
                Texture2D texture = new Texture2D(177,254);
                www.LoadImageIntoTexture(texture);
                Sprite image = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                
                CardBase card = CardBase.LoadCardFromInfo(int.Parse(item.Name));
                card.SetImage(image);
                allCardInfoList[card.GetCardNo()] = card;
            }
            Debug.Log("加载卡牌数量："+allCardInfoList.Count);
        }

        public void CleanDuelScene()
        {
            duelScene = null;
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
        public void OpenNet()
        {
            clientManager = ClientManager.GetSingleInstance();

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

        /// <summary>
        /// 关闭网络功能
        /// </summary>
        public void CloseNet()
        {
            if(clientManager!=null)
            {
                clientManager.StopConnect();
            }
            if(serverManager != null)
            {
                serverManager.StopListen();
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
        /// 停止决斗
        /// </summary>
        public void StopDuel()
        {
            ShowMessage("停止决斗。");
            CleanDuelScene();
            CloseNet();
        }
        
        /// <summary>
        /// 弹出提示框
        /// </summary>
        /// <param name="value"></param>
        public static void ShowMessage(string value)
        {
            for (int i = messageTipList.Count - 1; i >= 0; i--)
            {
                if (messageTipList[i]!=null)
                {
                    messageTipList[i].transform.localPosition -= new Vector3(0, 100);
                }
                else
                {
                    messageTipList.RemoveAt(i);
                }
            }
            Debug.Log(value);
            GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefab/MessageTip"), GameObject.Find("Canvas").transform);
            gameObject.transform.GetChild(0).GetComponent<Text>().text = value;
            messageTipList.Add(gameObject);
        }

        /// <summary>
        /// 移除提示框
        /// </summary>
        /// <param name="gameObject"></param>
        public static void RemoveMessage(GameObject gameObject)
        {
            messageTipList.Remove(gameObject);
            UnityEngine.Object.DestroyImmediate(gameObject);
        }

        /// <summary>
        /// 退出游戏
        /// </summary>
        public static void QuitGame()
        {
            if(gameManagerInstance!=null)
            {
                gameManagerInstance.SaveUserData();
            }
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
            switch (currentGameState)
            {
                case GameState.Init:
                    if(initDataFinish)
                    {
                        EnterMainScene();
                    }
                    break;
                case GameState.MainScene:
                    break;
                case GameState.SeleteDuelModeScene:
                    break;
                case GameState.GuessFirstScene:
                    break;
                case GameState.DuelScene:
                    ProcessProtocol();
                    break;
                case GameState.CardGroupScene:
                    break;
                case GameState.CardGroupEditScene:
                    break;
                case GameState.SettingScene:
                    break;
                default:
                    break;
            }
            if (duelScene != null)
            {
                duelScene.Update();
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
            OpenNet();
            EnterGuessFirstScene();
        }

        /// <summary>
        /// 添加计时器
        /// </summary>
        public static void AddTimerFunction(TimerFunction timerFunction)
        {
            gameManagerInstance.timerFunctions.Add(timerFunction);
        }

        /// <summary>
        /// 获得卡片背面图片
        /// </summary>
        /// <returns></returns>
        public static Sprite GetCardBackImage()
        {
            return GameManager.GetSingleInstance().cardBackImage;
        }

        /// <summary>
        /// 获得卡牌操作按钮预设
        /// </summary>
        /// <returns></returns>
        public static GameObject GetCardOperationButtonPrefab()
        {
            return GameManager.GetSingleInstance().cardOperationButtonPrefab;
        }

        /// <summary>
        /// 设置是否猜先必胜
        /// </summary>
        public void SetGuessMustWin(bool value)
        {
            userData.guessMustWin = value;
            SaveUserData();
        }

        /// <summary>
        /// 设置是否显示对方手牌
        /// </summary>
        public void SetShowOpponentHandCard(bool value)
        {
            userData.showOpponentHandCard = value;
            SaveUserData();
        }

        public Dictionary<int,CardBase> GetAllCardInfoList()
        {
            return allCardInfoList;
        }

        /// <summary>
        /// 设置对方是否可以召唤怪兽
        /// </summary>
        /// <param name="value"></param>
        public void SetOpponentCanCallMonster(bool value)
        {
            userData.opponentCanCallMonster = value;
            SaveUserData();
        }

        /// <summary>
        /// 设置对方是否可以使用魔法卡
        /// </summary>
        /// <param name="value"></param>
        public void SetOpponentCanLaunchEffect(bool value)
        {
            userData.opponentCanLaunchEffect = value;
            SaveUserData();
        }

        /// <summary>
        /// 设置对方是否可以进行攻击
        /// </summary>
        /// <param name="value"></param>
        public void SetOpponenttCanAttack(bool value)
        {
            userData.opponentCanAttack = value;
            SaveUserData();
        }
    }
}
