using Assets.Script.Config;
using Assets.Script.Duel.EffectProcess;
using Assets.Script.Duel.Rule;
using Assets.Script.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using XLua;

namespace Assets.Script.Card
{
    /// <summary>
    /// 魔法卡种类
    /// </summary>
    enum MagicType
    {
        Unknown,//未知
        Normal,//通常
        Equipment,//装备
        Terrain,//地形
        Forever,//永续
        Ceremony,//仪式
        Quick,//速攻
    }

    /// <summary>
    /// 魔法卡
    /// </summary>
    class MagicCard : CardBase
    {
        private LuaTable scriptEnv;
        private Action<CardBase> launchEffect = null;

        private string luaPath;
        private LuaTable cardLuaTable;

        public MagicCard(int cardNo) :base(cardNo)
        {
            cardType = CardType.Magic;

            LuaEnv.CustomLoader method = CustomLoaderMethod;
            GameManager.GetLuaEnv().AddLoader(method);

            scriptEnv = GameManager.GetLuaEnv().NewTable();

            // 为每个脚本设置一个独立的环境，可一定程度上防止脚本间全局变量、函数冲突
            LuaTable meta = GameManager.GetLuaEnv().NewTable();
            meta.Set("__index", GameManager.GetLuaEnv().Global);
            scriptEnv.SetMetaTable(meta);
            meta.Dispose();
            luaPath = cardNo + ".C"+ cardNo;
            GameManager.GetLuaEnv().DoString(@"C" + cardNo + " = require('"+ luaPath + "')", "LuaMagicCard", scriptEnv);
            
            scriptEnv.Set("self", this);

            launchEffect = scriptEnv.GetInPath<Action<CardBase>>("C" + cardNo + ".LaunchEffect");
        }

        private byte[] CustomLoaderMethod(ref string fileName)
        {
            fileName = fileName.Replace('.', '/');
            //找到指定文件  
            fileName = GameManager.GetCardResourceRootPath() + fileName + ".lua";
            if (File.Exists(fileName))
            {
                return File.ReadAllBytes(fileName);
            }
            else
            {
                return null;
            }
        }

        MagicType magicType = MagicType.Normal;

        public string GetMagicTypeString()
        {
            return GetStringByMagicType(magicType);
        }

        protected override void LoadInfo(string info)
        {
            string[] keyValues = info.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.None);
            foreach (var item in keyValues)
            {
                string key = item.Substring(0, item.IndexOf(':'));
                string value = item.Substring(item.IndexOf(':') + 1);
                switch (key)
                {
                    case "Name":
                        name = value;
                        break;
                    case "MagicType":
                        magicType = GetMagicTypeByString(value);
                        break;
                    case "Effect":
                        effect = value;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 将汉字转换为魔法种类
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static MagicType GetMagicTypeByString(string value)
        {
            MagicTypeConfig config = ConfigManager.GetConfigByName("MagicType") as MagicTypeConfig;
            int count = config.GetRecordCount();
            for (int i = 0; i < count; i++)
            {
                if (config.GetRecordById(i).value == value)
                {
                    return (MagicType)i;
                }
            }
            return 0;
        }

        /// <summary>
        /// 将魔法种类转换为汉字
        /// </summary>
        /// <param name="monsterType"></param>
        /// <returns></returns>
        public static string GetStringByMagicType(MagicType magicType)
        {
            MagicTypeConfig config = ConfigManager.GetConfigByName("MagicType") as MagicTypeConfig;
            return config.GetRecordById((int)magicType).value;
        }

        public override CardBase GetInstance()
        {
            MagicCard magicCard = new MagicCard(cardNo);
            magicCard.SetImage(GetImage());
            magicCard.cardNo = cardNo;
            magicCard.cardID = RandomHelper.random.Next();
            magicCard.cardType = cardType;
            magicCard.name = name;
            magicCard.magicType = magicType;
            magicCard.effect = effect;
            return magicCard;
        }

        /// <summary>
        /// 发动效果
        /// </summary>
        /// <returns></returns>
        public override bool CanLaunchEffect()
        {
            if ((GetCardType() == CardType.Magic ||
                GetCardType() == CardType.Trap) &&
                GetCardGameState() == CardGameState.Hand &&
                (duelScene.currentDuelProcess == DuelProcess.Main ||
                duelScene.currentDuelProcess == DuelProcess.Second) &&
                GetDuelCardScript().GetOwner().GetCurrentEffectProcess() == null &&
                !GetDuelCardScript().GetOwner().MagicTrapAreaIsFull())
            {
                return true;
            }
            return false;
        }

        public override void LaunchEffect()
        {
            if(launchEffect!=null)
            {
                launchEffect(this);
            }
            //ReduceLifeEffectProcess reduceLifeEffectProcess = new ReduceLifeEffectProcess(500, ReduceLifeType.Effect, GetDuelCardScript().GetOwner().GetOpponentPlayer());
            //GetDuelCardScript().GetOwner().GetOpponentPlayer().AddEffectProcess(reduceLifeEffectProcess);
        }

        //public void SetDamage(int value)
        //{
        //    ReduceLifeEffectProcess reduceLifeEffectProcess = new ReduceLifeEffectProcess(value, ReduceLifeType.Effect, GetDuelCardScript().GetOwner().GetOpponentPlayer());
        //    GetDuelCardScript().GetOwner().GetOpponentPlayer().AddEffectProcess(reduceLifeEffectProcess);
        //}
    }
}
