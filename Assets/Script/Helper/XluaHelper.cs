using Assets.Script.Card;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Helper
{
    /// <summary>
    /// xlua帮助类
    /// </summary>
    class XLuaHelper
    {
        public static IList CreateListInstance(Type type)
        {
            return (IList)Activator.CreateInstance(type);
        }

        public static List<CardBase> CreateCardBaseList()
        {
            return new List<CardBase>();
        }
    }
}
