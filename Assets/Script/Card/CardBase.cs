using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Card
{
    enum CardType
    {
        Monster,
        Magic,
        trap
    }

    enum CardGameState
    {
        Group,//在卡组中
        Hand,//手牌中
        Front,//表侧表示
        Back,//覆盖表示
        Tomb,//在墓地中
        Exclusion//被排除在游戏外
    }

    class CardBase
    {
        CardType cardType = CardType.Monster;//卡片类型
        CardGameState cardGameState = CardGameState.Group;
        string name = "未命名";//名称
        int cardNO = 0;//唯一编号，0代表为假卡。
    }
}
