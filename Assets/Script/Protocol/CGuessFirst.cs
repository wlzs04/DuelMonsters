using Assets.Script.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.Protocol
{
    /// <summary>
    /// 猜先的选择
    /// key：guess
    /// value：GuessEnum 
    /// </summary>
    class CGuessFirst : ClientProtocol
    {
        public override void Process()
        {
            GuessEnum opponentGuess =(GuessEnum)Enum.Parse(typeof(GuessEnum),GetContent("guess"));
            GameObject.Find("Main Camera").GetComponent<GuessFirstSceneScript>().SetOpponentGuess(opponentGuess);
        }
    }
}
