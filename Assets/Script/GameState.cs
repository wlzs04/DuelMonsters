using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script
{
    /// <summary>
    /// 游戏状态
    /// </summary>
    enum GameState
    {
        MainScene,//主场景
        SeleteDuelModeScene,//选择决斗模式场景
        GuessFirstScene,//选择决斗模式场景
        DuelScene,//决斗场景
        CardGroupScene,//卡组场景
        CardGroupEditScene,//卡组编辑场景
        SettingScene,//设置场景
    }
}
