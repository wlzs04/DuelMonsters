using Assets.Script.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Duel.Rule
{
    /// <summary>
    /// 回合中的流程阶段
    /// </summary>
    public enum DuelProcess
    {
        Unknown,//未知
        Draw,//抽卡
        Prepare,//准备
        Main,//主要
        Battle,//战斗
        Second,//第二主要
        End//结束
    }

    /// <summary>
    /// 决斗结束原因
    /// </summary>
    public enum DuelEndReason
    {
        Unknown,//未知
        Stop,//中途停止
        Life,//生命值为0
        Draw,//抽卡时卡牌数量为0
        Special,//特殊效果
        Surrender,//投降
    }

    /// <summary>
    /// 决斗规则管理器
    /// </summary>
    class DuelRuleManager
    {
        static RuleConfig ruleConfig;

        /// <summary>
        /// 初始化决斗规则
        /// </summary>
        public static void InitDuelRule()
        {
            ruleConfig = ConfigManager.GetConfigByName("Rule") as RuleConfig;
        }

        /// <summary>
        /// 主卡组内最小卡牌数量
        /// </summary>
        /// <returns></returns>
        public static int GetMainCardGroupNumberLowerLimit()
        {
            return ruleConfig.GetRecordById(0).value;
        }

        /// <summary>
        /// 主卡组内最大卡牌数量
        /// </summary>
        /// <returns></returns>
        public static int GetMainCardGroupNumberUpperLimit()
        {
            return ruleConfig.GetRecordById(1).value;
        }

        /// <summary>
        /// 额外卡组内最大卡牌数量
        /// </summary>
        /// <returns></returns>
        public static int GetExtraCardGroupNumberUpperLimit()
        {
            return ruleConfig.GetRecordById(2).value;
        }

        /// <summary>
        /// 副卡组内最大卡牌数量
        /// </summary>
        /// <returns></returns>
        public static int GetDeputyCardGroupNumberUpperLimit()
        {
            return ruleConfig.GetRecordById(3).value;
        }

        /// <summary>
        /// 卡组内同名卡牌的最大数量
        /// </summary>
        /// <returns></returns>
        public static int GetSameCardNumberUpperLimit()
        {
            return ruleConfig.GetRecordById(4).value;
        }

        /// <summary>
        /// 玩家初始手卡数量
        /// </summary>
        /// <returns></returns>
        public static int GetDrawNumberOnStartDuel()
        {
            return ruleConfig.GetRecordById(5).value;
        }

        /// <summary>
        /// 每到自己回合抽卡数量
        /// </summary>
        /// <returns></returns>
        public static int GetDrawNumberEveryTurn() 
        {
            return ruleConfig.GetRecordById(6).value;
        }

        /// <summary>
        /// 最大手卡数量
        /// </summary>
        /// <returns></returns>
        public static int GetHandCardNumberUpperLimit()
        {
            return ruleConfig.GetRecordById(7).value;
        }

        /// <summary>
        /// 决斗初始血量
        /// </summary>
        /// <returns></returns>
        public static int GetPlayerStartLife()
        {
            return ruleConfig.GetRecordById(8).value;
        }

        /// <summary>
        /// 每个怪兽在战斗流程可以进行攻击的次数
        /// </summary>
        /// <returns></returns>
        public static int GetMonsterAttackNumberEveryTurn()
        {
            return ruleConfig.GetRecordById(9).value;
        }

        /// <summary>
        /// 自己回合每个怪兽可以进行攻守转换的次数
        /// </summary>
        /// <returns></returns>
        public static int GetMonsterChangeAttackOrDefenseNumberEveryTurn()
        {
            return ruleConfig.GetRecordById(10).value;
        }

        /// <summary>
        /// 怪兽不需要祭品召唤的最大等级
        /// </summary>
        /// <returns></returns>
        public static int GetCallMonsterWithoutSacrificeLevelUpperLimit()
        {
            return ruleConfig.GetRecordById(11).value;
        }

        /// <summary>
        /// 怪兽需要一只祭品召唤的最大等级
        /// </summary>
        /// <returns></returns>
        public static int GetCallMonsterWithOneSacrificeLevelUpperLimit()
        {
            return ruleConfig.GetRecordById(12).value;
        }

        /// <summary>
        /// 玩家怪兽区的数量
        /// </summary>
        /// <returns></returns>
        public static int GetMonsterAreaNumber()
        {
            return ruleConfig.GetRecordById(13).value;
        }
    }
}
