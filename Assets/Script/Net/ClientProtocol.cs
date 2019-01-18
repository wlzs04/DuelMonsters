using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Assets.Script.Net
{
    abstract class ClientProtocol:Protocol
    {
        public ClientProtocol GetInstance()
        {
            Type chindType = GetType();
            return (ClientProtocol)chindType.Assembly.CreateInstance(chindType.FullName);
        }

        public void AddContent(string key, object value)
        {
            contentMap[key] = value.ToString();
        }

        public string GetContent(string key)
        {
            return contentMap[key];
        }

        public abstract void Process();
    }
}
