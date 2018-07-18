using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Net
{
    abstract class ServerProtocol: Protocol
    {
        public ServerProtocol(string name) : base(name)
        {
        }

        public abstract ServerProtocol GetInstance();

        public void AddContent(string key, string value)
        {
            contentMap[key] = value;
        }

        public string GetContent(string key)
        {
            return contentMap[key];
        }

        public abstract void Process();
    }
}
