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
        CardBase effectCard;//发动效果的卡牌
        CardBase effectedCard;//受效果影响的卡牌
        CardEffectType cardEffectType;
        int value;//作用值
        int speed = 1;//咒文速度(1 2 3)

        int effectActivityTurnNumber = int.MaxValue;//作用持续回合数
        PhaseType removeEffectPhaseType = PhaseType.End;//移除当前效果的流程
        bool removeWhenExitArea = true;//离场是否移除

        public CardEffect(CardBase effectCard,CardBase effectedCard,CardEffectType cardEffectType, int value)
        {
            this.effectCard = effectCard;
            this.effectedCard = effectedCard;
            this.cardEffectType = cardEffectType;
            this.value = value;
        }

        public CardBase GetEffectCard()
        {
            return effectCard;
        }

        public CardBase GetEffectedCard()
        {
            return effectedCard;
        }

        public CardEffectType GetCardEffectType()
        {
            return cardEffectType;
        }

        public int GetValue()
        {
            return value;
        }

        public bool GetRemoveWhenExitArea()
        {
            return removeWhenExitArea;
        }

        /// <summary>
        /// 设置效果限制，如持续时间等
        /// </summary>
        public void SetEffectLimit(int effectActivityTurnNumber, PhaseType removeEffectPhaseType)
        {
            this.effectActivityTurnNumber = effectActivityTurnNumber;
            this.removeEffectPhaseType = removeEffectPhaseType;
        }

        /// <summary>
        /// 检查当前效果，判断是否进行移除等操作
        /// </summary>
        public void CheckEffect()
        {
            if(effectActivityTurnNumber<=0 && effectedCard.GetDuelScene().GetCurrentPhaseType() == removeEffectPhaseType)
            {
                effectedCard.RemoveCardEffect(this);
            }
        }

        public void SetSpeed(int speed)
        {
            this.speed = speed;
        }

        public int GetSpeed()
        {
            return speed;
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
        CardType cardType = CardType.Unknown;//卡片类型
        CardGameState cardGameState = CardGameState.Group;
        Sprite image;
        string name = "未命名";//名称
        int cardNo = 0;//唯一编号，0代表为假卡。
        string effectInfo = "没有效果。";//卡片效果信息

        int cardID = 0;//代表在决斗过程中的唯一标志

        int bePlacedAreaTurnNumber = 0;//被放置到场上的第回合数

        GameObject cardObject=null;
        CardScript cardScript = null;
        DuelCardScript duelCardScript = null;
        DuelScene duelScene = null;

        //标记map，用来放置一些受效果而产生的标记物
        Dictionary<string, object> contentMap = new Dictionary<string, object>();

        //效果map，表示当前卡牌受到其他卡牌的作用
        List<CardEffect> cardEffectedMap = new List<CardEffect>();

        //lua部分
        private LuaTable scriptEnv;
        private Action<CardBase> initInfoAction = null;
        private ActionJudge canLaunchEffectAction = null;
        private Action<CardBase> costAction = null;
        private Action<CardBase> launchEffectAction = null;
        private ActionChangeCardGameState changeCardGameState = null;
        private string luaPath;

        bool infoDirty = false;//卡牌的信息是否被修改并且没有刷新
        bool inLaunch = false;

        int areaIndex = 0;

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
            costAction = scriptEnv.GetInPath<Action<CardBase>>("C" + cardNo + ".Cost");
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

        public void Update()
        {
            if(infoDirty)
            {
                CheckAndShowCardInfo();
            }
        }

        /// <summary>
        /// 为卡牌设置场景内依附的物体
        /// </summary>
        /// <param name="gameObject"></param>
        public void SetCardObject(GameObject gameObject)
        {
            cardObject = gameObject;
            cardScript = cardObject.GetComponent<CardScript>();
            duelCardScript = cardObject.GetComponent<DuelCardScript>();
            if (duelCardScript!=null)
            {
                duelScene = GameManager.GetDuelScene();
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
            infoDirty = true;
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

        public CardScript GetCardScript()
        {
            return cardScript;
        }

        /// <summary>
        /// 获得拥有此卡的玩家
        /// </summary>
        /// <returns></returns>
        public Player GetOwner()
        {
            return GetDuelCardScript().GetOwner();
        }

        /// <summary>
        /// 设置当前卡牌状态，如果是在怪兽魔法陷阱区时，根据设置index位置
        /// </summary>
        /// <param name="cardGameState"></param>
        public void SetCardGameState(CardGameState cardGameState,int index = -1)
        {
            CardGameState oldCardGameState = this.cardGameState;
            //卡牌刚进入场上开始计被放置到场上的第回合数
            if (!IsInArea(this.cardGameState) && IsInArea(cardGameState))
            {
                bePlacedAreaTurnNumber = duelScene.GetCurrentTurnNumber();
            }
            //离场
            else if(IsInArea(this.cardGameState) && !IsInArea(cardGameState))
            {
                ExitAreaCallBack();
            }

            this.cardGameState = cardGameState;

            GetDuelCardScript().SetCardGameState(cardGameState,index);

            changeCardGameState?.Invoke(this, oldCardGameState);
        }

        public CardGameState GetCardGameState()
        {
            return cardGameState;
        }

        //检查卡牌信息面板显示的是否时此卡，是的话重新显示卡牌信息
        public void CheckAndShowCardInfo()
        {
            if (duelScene!=null && duelScene.GetCurrentInfoCard() == this)
            {
                duelScene.SetCurrentInfoCard(this);
            }
        }

        /// <summary>
        /// 离场时的回调
        /// </summary>
        public void ExitAreaCallBack()
        {
            switch (cardType)
            {
                case CardType.Monster:
                    MonsterExitAreaCallBack();
                    break;
                case CardType.Magic:
                    MagicExitAreaCallBack();
                    break;
                case CardType.Trap:
                    TrapExitAreaCallBack();
                    break;
                default:
                    break;
            }
            for (int i = cardEffectedMap.Count - 1; i >= 0; i--)
            {
                if(cardEffectedMap[i].GetRemoveWhenExitArea())
                {
                    RemoveCardEffect(cardEffectedMap[i]);
                }
            }
        }

        public void SetEffectInfo(string effectInfo)
        {
            this.effectInfo = effectInfo;
            infoDirty = true;
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
            infoDirty = true;
        }

        public void SetCardTypeByString(string value)
        {
            SetCardType( GetCardTypeByString(value));
        }

        public string GetCardTypeString()
        {
            return GetStringByCardType(cardType);
        }

        public bool IsInArea()
        {
            return cardGameState == CardGameState.FrontAttack ||
                cardGameState == CardGameState.FrontDefense ||
                cardGameState == CardGameState.Front ||
                cardGameState == CardGameState.Back;
        }

        /// <summary>
        /// 判断当前是否在场上
        /// </summary>
        /// <returns></returns>
        public static bool IsInArea(CardGameState cardGameState)
        {
            return cardGameState==CardGameState.FrontAttack ||
                cardGameState == CardGameState.FrontDefense ||
                cardGameState == CardGameState.Front ||
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
            //在发动效果期间的卡片不能再次发动效果。
            if(inLaunch)
            {
                return false;
            }
            switch (cardType)
            {
                case CardType.Monster:
                    //MonsterCanLaunchEffect();
                    break;
                case CardType.Magic:
                    return MagicCanLaunchEffect();
                case CardType.Trap:
                    return TrapCanLaunchEffect();
                default:
                    break;
            }
            return false;
        }

        /// <summary>
        /// 在发动效果前的操作
        /// </summary>
        public void BeforeLaunchEffect(Action costFinishAction)
        {
            inLaunch = true;
            if(costAction!=null)
            {
                costAction(this);
            }
            else
            {
                costFinishAction();
            }
        }

        /// <summary>
        /// 发动效果
        /// </summary>
        public void LaunchEffect()
        {
            launchEffectAction?.Invoke(this);
        }

        /// <summary>
        /// 发动效果后的回调
        /// </summary>
        public void LaunchEffectFinishCallBack()
        {
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
            inLaunch = false;
        }

        /// <summary>
        /// 添加卡牌效果
        /// </summary>
        public CardEffect AddCardEffect(CardBase cardBase, CardEffectType cardEffectType, int value)
        {
            CardEffect cardEffect = new CardEffect(cardBase,this, cardEffectType, value);
            cardEffectedMap.Add(cardEffect);
            RecalculationCardByCardEffect();
            return cardEffect;
        }

        /// <summary>
        /// 移除指定效果
        /// </summary>
        /// <param name="effectCard"></param>
        /// <param name="cardEffect"></param>
        public void RemoveCardEffect(CardEffect cardEffect)
        {
            cardEffectedMap.Remove(cardEffect);
            RecalculationCardByCardEffect();
        }

        /// <summary>
        /// 移除指定卡牌的效果
        /// </summary>
        public void RemoveCardEffect(CardBase effectCard)
        {
            for (int i = cardEffectedMap.Count - 1; i >= 0; i--)
            {
                if(cardEffectedMap[i].GetEffectCard()== effectCard)
                {
                    cardEffectedMap.RemoveAt(i);
                }
            }
            RecalculationCardByCardEffect();
        }

        /// <summary>
        /// 移除所有卡牌效果
        /// </summary>
        public void RemoveAllCardEffect()
        {
            cardEffectedMap.Clear();
            RecalculationCardByCardEffect();
        }

        public void CheckAllCardEffect()
        {
            for (int i = cardEffectedMap.Count - 1; i >= 0; i--)
            {
                cardEffectedMap[i].CheckEffect();
            }
        }

        /// <summary>
        /// 根据卡牌效果重新计算卡牌
        /// </summary>
        public void RecalculationCardByCardEffect()
        {
            //计算攻击力和守备力
            if(GetCardType()==CardType.Monster)
            {
                SetAttackValue(GetOriginalAttackValue());
                SetDefenseValue(GetOriginalDefenseValue());
            }
            foreach (var cardEffect in cardEffectedMap)
            {
                switch (cardEffect.GetCardEffectType())
                {
                    case CardEffectType.Attack:
                        SetAttackValue(GetAttackValue() + cardEffect.GetValue());
                        break;
                    case CardEffectType.Defense:
                        SetDefenseValue(GetDefenseValue() + cardEffect.GetValue());
                        break;
                    default:
                        Debug.LogError("未知卡牌效果类型：" + cardEffect.GetCardEffectType());
                        break;
                }
            }
        }

        /// <summary>
        /// 判断是否在怪兽区
        /// </summary>
        /// <returns></returns>
        public bool InMonsterArea()
        {
            if(cardGameState==CardGameState.FrontAttack ||
                cardGameState == CardGameState.FrontDefense ||
                (cardGameState == CardGameState.Back && cardType==CardType.Monster))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断是否在魔法陷阱区
        /// </summary>
        /// <returns></returns>
        public bool InMagicTrapArea()
        {
            if (cardGameState == CardGameState.Front ||
                (cardGameState == CardGameState.Back && (cardType == CardType.Magic || cardType == CardType.Trap)))
            {
                return true;
            }
            return false;
        }

        public DuelScene GetDuelScene()
        {
            return duelScene;
        }

        public void SetAreaIndex(int areaIndex)
        {
            this.areaIndex = areaIndex;
        }

        public int GetAreaIndex()
        {
            return areaIndex;
        }
    }
}
