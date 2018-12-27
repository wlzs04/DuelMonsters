using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Duel.Rule
{
    /// <summary>
    /// 卡组规则
    /// </summary>
    class CardGroupRule
    {
        static int groupNumberUpperLimit = 80;//卡组数量上限
        static int groupNumberLowerLimit = 40;//卡组数量下限
        static int sameCardMaxNumber = 3;//相同卡片数量上限

        public static bool IsLegal(UserCardGroup cardGroup)
        {
            if (cardGroup.userCardList.Count > groupNumberUpperLimit)
            {
                return false;
            }
            if (cardGroup.userCardList.Count < groupNumberLowerLimit)
            {
                return false;
            }
            foreach (var item in cardGroup.userCardList)
            {
                if(item.number> sameCardMaxNumber)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
