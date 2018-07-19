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

        public static bool IsLegal(UserData userData)
        {
            if (userData.userCardGroupList.Count > groupNumberUpperLimit)
            {
                return false;
            }
            if (userData.userCardGroupList.Count < groupNumberLowerLimit)
            {
                return false;
            }
            foreach (var item in userData.userCardGroupList)
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
