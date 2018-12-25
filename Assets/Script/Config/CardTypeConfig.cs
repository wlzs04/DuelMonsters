using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Config
{
    /// <summary>
    /// 卡牌类型配置表
    /// </summary>
    class CardTypeConfig
    {

        [System.Xml.Serialization.XmlAttribute]
        int id = 0;

        [System.Xml.Serialization.XmlAttribute]
        string value = "";
    }
}
