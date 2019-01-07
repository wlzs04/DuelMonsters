using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;

namespace Assets.Script
{
    /// <summary>
    /// 定时任务
    /// </summary>
    class TimerFunction
    {
        UnityAction action = null;
        float time = 0;
        float remainTime = 0;

        public void Update(float deltaTime)
        {
            remainTime -= deltaTime;
        }

        /// <summary>
        /// 设置时间和方法
        /// </summary>
        /// <param name="time"></param>
        /// <param name="action"></param>
        public void SetFunction(float time,UnityAction action)
        {
            this.time = time;
            remainTime = time;
            this.action = action;
        }

        public float GetRemainTime()
        {
            return remainTime;
        }

        public void DoFunction()
        {
            if(action!=null)
            {
                action();
            }
        }
    }
}
