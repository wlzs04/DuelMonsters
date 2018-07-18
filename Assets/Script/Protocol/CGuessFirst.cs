using Assets.Script.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Protocol
{
    /// <summary>
    /// 猜先的选择
    /// key：guess
    /// value：GuessEnum 
    /// </summary>
    class CGuessFirst : ClientProtocol
    {
        public CGuessFirst() : base("CGuessFirst") { }

        public override ClientProtocol GetInstance()
        {
            return new CGuessFirst();
        }

        public override void Process()
        {
            GuessEnum OpponentGuess =(GuessEnum)Enum.Parse(typeof(GuessEnum),GetContent("guess"));
            GameManager.GetSingleInstance().SetOpponentGuess(OpponentGuess);
        }
    }
}
