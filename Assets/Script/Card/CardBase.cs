using Assets.Script.Config;
using Assets.Script.Duel;
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
    /// 卡牌类型
    /// </summary>
    public enum CardType
    {
        Unknown,//未知
        Monster,//怪兽
        Magic,//魔法
        Trap,//陷阱
    }

    /// <summary>
    /// 卡组类型
    /// </summary>
    public enum CardGroupType
    {
        Unknown,//未知
        Main,//主卡组
        Extra,//额外卡组
        Deputy,//副卡组
    }

    /// <summary>
    /// 在决斗中卡牌的状态
    /// </summary>
    public enum CardGameState
    {
        Unknown,//未知
        Group,//在卡组中
        Hand,//手牌中
        FrontAttack,//表侧表示攻击(怪兽使用)
        FrontDefense,//表侧表示防守(怪兽使用)
        Front,//表侧表示(魔法陷阱使用)
        Back,//覆盖表示
        Tomb,//在墓地中
        Exclusion,//被排除在游戏外
    }

    /// <summary>
    /// 对卡牌的作用效果类型
    /// </summary>
    public enum CardEffectType
    {
        Unknown,//未知
        Attack,//攻击力
        Defense,//守备力
    }

    /// <summary>
    /// 对卡牌的作用效果
    /// </summary>
    public class CardEffect
    {
        CardEffectType cardEffectType;
        int value;

        public CardEffect(CardEffectType cardEffectType, int value)
        {
            this.cardEffectType = cardEffectType;
            this.value = value;
        }

        public CardEffectType GetCardEffectType()
        {
            return cardEffectType;
        }

        public int GetValue()
        {
            return value;
        }
    }

    [CSharpCallLua]
    public delegate bool ActionJudge(CardBase cardBase);

    [CSharpCallLua]
    public delegate void ActionChangeCardGameState(CardBase cardBase, CardGameState oldCardGameState);

    [CSharpCallLua]
    public delegate void ActionIndex(CardBase cardBase,int index);

    /// <summary>
    /// 卡牌基础部分
    /// </summary>
    public partial class CardBase
    {
        protected CardType cardType = CardType.Unknown;//卡片类型
        CardGameState cardGameState = CardGameState.Group;
        Sprite image;
        protected string name = "未命名";//名称
        protected int cardNo = 0;//唯一编号，0代表为假卡。
        protected string effectInfo = "没有效果。";

        protected int cardID = 0;//代表在决斗过程中的唯一标志

        protected string info ="";//卡片信息
        protected int limitNumber = 3;//数量限制

        protected int bePlacedAreaTurnNumber = 0;//被放置到场上的第回合数

        protected GameObject cardObject=null;

        DuelCardScript duelCardScript = null;
        protected DuelScene duelScene = null;

        //标记map，用来放置一些受效果而产生的标记物
        Dictionary<string, object> contentMap = new Dictionary<string, object>();

        //效果map，表示当前卡牌受到其他卡牌的作用
        Dictionary<CardBase, List<CardEffect>> cardEffectMap = new Dictionary<CardBase, List<CardEffect>>();

        //lua部分
        private LuaTable scriptEnv;
        private Action<CardBase> initInfoAction = null;
        private ActionJudge canLaunchEffectAction = null;
        private Action<CardBase> launchEffectAction = null;
        private ActionChangeCardGameState changeCardGameState = null;
        private string luaPath;

        public CardBase(int cardNo)
        {
            this.cardNo = cardNo;
            InitLua();
        }

        /// <summary>
        /// 初始化lua部分
        /// </summary>
        void InitLua()
        {
            LuaEnv.CustomLoader method = CustomLoaderMethod;
            GameManager.GetLuaEnv().AddLoader(method);

            scriptEnv = GameManager.GetLuaEnv().NewTable();

            // 为每个脚本设置一个独立的环境，可一定程度上防止脚本间全局变量、函数冲突
            LuaTable meta = GameManager.GetLuaEnv().NewTable();
            meta.Set("__index", GameManager.GetLuaEnv().Global);
            scriptEnv.SetMetaTable(meta);
            meta.Dispose();
            luaPath = cardNo + ".C" + cardNo;
            GameManager.GetLuaEnv().DoString(@"C" + cardNo + " = require('" + luaPath + "')", "LuaMagicCard", scriptEnv);

            scriptEnv.Set("self", this);

            initInfoAction = scriptEnv.GetInPath<Action<CardBase>>("C" + cardNo + ".InitInfo");
            canLaunchEffectAction = scriptEnv.GetInPath<ActionJudge>("C" + cardNo + ".CanLaunchEffect");
            launchEffectAction = scriptEnv.GetInPath<Action<CardBase>>("C" + cardNo + ".LaunchEffect");
            changeCardGameState = scriptEnv.GetInPath<ActionChangeCardGameState>("C" + cardNo + ".ChangeCardGameState");
            
            if (initInfoAction != null)
            {
                initInfoAction(this);
            }
            else
            {
                Debug.LogError("卡牌：" + cardNo + "缺少SetInfo方法！");
            }
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

        /// <summary>
        /// 为卡牌设置场景内依附的物体
        /// </summary>
        /// <param name="gameObject"></param>
        public void SetCardObject(GameObject gameObject)
        {
            cardObject = gameObject;
            duelCardScript = cardObject.GetComponent<DuelCardScript>();
            if(duelCardScript!=null)
            {
                duelScene = GameManager.GetSingleInstance().GetDuelScene();
            }
        }

        public void AddContent(string key, object value)
        {
            if(contentMap.ContainsKey(key))
            {
                Debug.Log("卡牌的key：" + key + " 由：" + contentMap[key] + " 改为：" + value);
            }
            contentMap[key] = value;
        }

        public object GetContent(string key)
        {
            if(!contentMap.ContainsKey(key))
            {
                return null;
            }
            return contentMap[key];
        }

        public void RemoveContent(string key)
        {
            contentMap.Remove(key);
        }

        public void ClearAllContent()
        {
            contentMap.Clear();
        }

        public int GetID()
        {
            return cardID;
        }

        public void SetID(int ID)
        {
            cardID=ID;
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public string GetName()
        {
            return name;
        }

        public int GetCardNo()
        {
            return cardNo;
        }

        public int GetBePlacedAreaTurnNumber()
        {
            return bePlacedAreaTurnNumber;
        }

        public DuelCardScript GetDuelCardScript()
        {
            return duelCardScript;
        }

        public void SetCardGameState(CardGameState cardGameState)
        {
            CardGameState oldCardGameState = this.cardGameState;
            //卡牌刚进入场上开始计被放置到场上的第回合数
            if (!IsInArea(this.cardGameState) && IsInArea(cardGameState))
            {
                bePlacedAreaTurnNumber = duelScene.GetCurrentTurnNumber();
            }

            this.cardGameState = cardGameState;
            duelCardScript.ResetAttackAndDefenseText();

            switch (cardGameState)
            {
                case CardGameState.Group:
                    break;
                case CardGameState.Hand:
                    break;
                case CardGameState.FrontAttack:
                { 
                    duelCardScript.ShowFront();
                    duelCardScript.SetCardAngle(0);
                    break;
                }
                case CardGameState.FrontDefense:
                {
                    duelCardScript.ShowFront();
                    duelCardScript.SetCardAngle(90);
                    break;
                }
                case CardGameState.Front:
                    duelCardScript.ShowFront();
                    duelCardScript.SetCardAngle(0);
                    break;
                case CardGameState.Back:
                    duelCardScript.ShowBack();
                    if(cardType==CardType.Monster)
                    {
                        duelCardScript.SetCardAngle(90);
                    }
                    else if(cardType == CardType.Magic || cardType == CardType.Trap)
                    {
                        duelCardScript.SetCardAngle(0);
                    }
                    break;
                case CardGameState.Tomb:
                    duelCardScript.ShowFront();
                    duelCardScript.SetCardAngle(0);
                    cardObject.transform.SetParent(GameManager.GetSingleInstance().GetDuelScene().duelBackImage.transform);
                    if (duelCardScript.GetOwner().IsMyPlayer())
                    {
                        cardObject.transform.localPosition = new Vector3(DuelCommonValue.myTombPositionX, DuelCommonValue.myTombPositionY, 0);
                    }
                    else
                    {
                       cardObject.transform.localPosition = new Vector3(DuelCommonValue.opponentTombPositionX, DuelCommonValue.opponentTombPositionY, 0);
                    }
                    duelCardScript.SetCardAngle(0);
                    break;
                case CardGameState.Exclusion:
                    break;
                default:
                    Debug.LogError("未知CardGameState：" + cardGameState);
                    break;
            }
            if(changeCardGameState!=null)
            {
                changeCardGameState(this,oldCardGameState);
            }
        }

        public CardGameState GetCardGameState()
        {
            return cardGameState;
        }

        public void SetEffectInfo(string effectInfo)
        {
            this.effectInfo = effectInfo;
        }

        public string GetEffectInfo()
        {
            return effectInfo;
        }

        public void SetImage(Sprite image)
        {
            this.image = image;
        }

        public Sprite GetImage()
        {
            return image;
        }

        public CardType GetCardType()
        {
            return cardType;
        }

        public void SetCardType(CardType cardType)
        {
            this.cardType = cardType;
        }

        public void SetCardTypeByString(string value)
        {
            cardType = GetCardTypeByString(value);
        }

        public string GetCardTypeString()
        {
            return GetStringByCardType(cardType);
        }

        /// <summary>
        /// 判断当前是否在场上
        /// </summary>
        /// <returns></returns>
        public static bool IsInArea(CardGameState cardGameState)
        {
            return cardGameState==CardGameState.FrontAttack ||
                cardGameState == CardGameState.FrontDefense ||
                cardGameState == CardGameState.Back;
        }

        public static CardBase LoadCardFromInfo(int cardNo)
        {
            CardBase card = new CardBase(cardNo);
            return card;
        }

        public CardBase GetInstance()
        {
            CardBase cardBase = new CardBase(cardNo);
            cardBase.cardID = RandomHelper.random.Next();
            cardBase.SetImage(GetImage());
            return cardBase;
        }

        /// <summary>
        /// 将汉字转换为卡片种类
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static CardType GetCardTypeByString(string value)
        {
            CardTypeConfig config = ConfigManager.GetConfigByName("CardType") as CardTypeConfig;
            int count = config.GetRecordCount();
            for (int i = 0; i < count; i++)
            {
                if(config.GetRecordById(i).value == value)
                {
                    return (CardType)i;
                }
            }
            return 0;
        }

        /// <summary>
        /// 将卡片种类转换为汉字
        /// </summary>
        /// <param name="monsterType"></param>
        /// <returns></returns>
        public static string GetStringByCardType(CardType cardType)
        {
            CardTypeConfig config = ConfigManager.GetConfigByName("CardType") as CardTypeConfig;
            return config.GetRecordById((int)cardType).value;
        }

        /// <summary>
        /// 判断是否可以发动效果
        /// </summary>
        /// <returns></returns>
        public bool CanLaunchEffect()
        {
            if ((GetCardType() == CardType.Magic) &&
                (duelScene.currentDuelProcess == DuelProcess.Main ||
                duelScene.currentDuelProcess == DuelProcess.Second) &&
                GetDuelCardScript().GetOwner().GetCurrentEffectProcess() == null &&
                !GetDuelCardScript().GetOwner().MagicTrapAreaIsFull())
            {
                if(GetCardGameState() == CardGameState.Hand)
                {
                    return canLaunchEffectAction != null ? canLaunchEffectAction(this) : true;
                }
                else if(IsInArea(cardGameState) && duelScene.GetCurrentTurnNumber()> GetBePlacedAreaTurnNumber())
                {
                    return canLaunchEffectAction != null ? canLaunchEffectAction(this) : true;
                }
            }
            return false;
        }

        /// <summary>
        /// 发动效果
        /// </summary>
        public void LaunchEffect()
        {
            if (launchEffectAction != null)
            {
                launchEffectAction(this);
            }
            switch (cardType)
            {
                case CardType.Monster:
                    MonsterLaunchEffectCallback();
                    break;
                case CardType.Magic:
                    MagicLaunchEffectCallback();
                    break;
                case CardType.Trap:
                    TrapLaunchEffectCallback();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 添加卡牌效果
        /// </summary>
        public void AddCardEffect(CardBase cardBase, CardEffectType cardEffectType, int value)
        {
            CardEffect cardEffect = new CardEffect(cardEffectType, value);
            if(!cardEffectMap.ContainsKey(cardBase))
            {
                cardEffectMap[cardBase] = new List<CardEffect>();
            }
            cardEffectMap[cardBase].Add(cardEffect);
            RecalculationCardByCardEffect();
        }

        /// <summary>
        /// 移除指定卡牌的效果
        /// </summary>
        public void RemoveCardEffect(CardBase cardBase)
        {
            if (cardEffectMap.ContainsKey(cardBase))
            {
                cardEffectMap.Remove(cardBase);
            }
            RecalculationCardByCardEffect();
        }

        /// <summary>
        /// 移除所有卡牌效果
        /// </summary>
        public void RemoveAllCardEffect()
        {
            cardEffectMap.Clear();
            RecalculationCardByCardEffect();
        }

        /// <summary>
        /// 根据卡牌效果重新计算卡牌
        /// </summary>
        public void RecalculationCardByCardEffect()
        {
            //计算攻击力和守备力
            CardBase cardBase = GameManager.GetSingleInstance().GetAllCardInfoList()[cardNo];
            if(GetCardType()==CardType.Monster)
            {
                SetAttackNumber(cardBase.GetAttackNumber());
                SetDefenseNumber(cardBase.GetDefenseNumber());
            }
            foreach (var effectCard in cardEffectMap)
            {
                foreach (var cardEffect in effectCard.Value)
                {
                    switch (cardEffect.GetCardEffectType())
                    {
                        case CardEffectType.Attack:
                            SetAttackNumber(GetAttackNumber() + cardEffect.GetValue());
                            break;
                        case CardEffectType.Defense:
                            SetDefenseNumber(GetDefenseNumber() + cardEffect.GetValue());
                            break;
                        default:
                            Debug.LogError("未知卡牌效果类型：" + cardEffect.GetCardEffectType());
                            break;
                    }
                }
            }
        }
    }
}
