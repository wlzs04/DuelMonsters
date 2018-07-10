using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Script
{
    class GameManager
    {
        GameState gameState = new GameState();
        static GameManager gameManagerInstance = new GameManager();
        UserData userdata;
        string gameSavePath = "/UserDate.txt";

        private GameManager()
        {
            gameState = GameState.MainScene;

            LoadUserData();
        }

        public static GameManager GetSingleInstance()
        {
            return gameManagerInstance;
        }

        /// <summary>
        /// 返回上一场景
        /// </summary>
        public void ReturnLastScene()
        {
            switch (gameState)
            {
                case GameState.MainScene:
                    break;
                case GameState.DuelScene:
                    SceneManager.LoadScene("MainScene",LoadSceneMode.Single);
                    gameState = GameState.MainScene;
                    break;
                case GameState.CardGroupScene:
                    SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
                    gameState = GameState.MainScene;
                    break;
                case GameState.SettingScene:
                    SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
                    gameState = GameState.MainScene;
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
            if(File.Exists(Application.dataPath + gameSavePath))
            {

                string xmlStr = File.ReadAllText(Application.dataPath + gameSavePath);
                userdata = DESerializer<UserData>(xmlStr);
            }
            else
            {
                userdata = new UserData();
                UserCardData ucd = new UserCardData();
                ucd.cardNo = 1;
                ucd.number = 2;
                userdata.userCardList.Add(ucd);
            }
        }

        /// <summary>
        /// 保存玩家数据
        /// </summary>
        public void SaveUserData()
        {
            string strxml = XmlSerialize<UserData>(userdata);
            File.WriteAllText(Application.dataPath + gameSavePath, strxml);
            
            //BinaryFormatter bf = new BinaryFormatter();
            //FileStream file = new FileStream();
            //bf.Serialize(file, userdata);
            //file.Close();
        }

        /// <summary>
        /// 获得玩家数据
        /// </summary>
        public UserData GetUserData()
        {
            return userdata;
        }

        public string XmlSerialize<T>(T obj)
        {
            using (StringWriter sw = new StringWriter())
            {
                Type t = obj.GetType();
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(sw, obj);
                sw.Close();
                return sw.ToString();
            }
        }

        public T DESerializer<T>(string strXML) where T : class
        {

            using (StringReader sr = new StringReader(strXML))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return serializer.Deserialize(sr) as T;
            }
        }

        /// <summary>
        /// 进入主场景
        /// </summary>
        public void EnterMainScene()
        {
            if (gameState != GameState.MainScene)
            {
                gameState = GameState.MainScene;
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
            if (gameState == GameState.MainScene)
            {
                gameState = GameState.DuelScene;
            }
            else
            {
                Debug.LogError("EnterDuelScene");
            }
        }

        /// <summary>
        /// 进入卡组界面
        /// </summary>
        public void EnterCardGroupScene()
        {
            if (gameState == GameState.MainScene)
            {
                gameState = GameState.CardGroupScene;
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
            if (gameState == GameState.MainScene)
            {
                gameState = GameState.SettingScene;
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
