using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Assets.Script.Net
{
    abstract class Protocol
    {
        string name;
        string content;
        protected Socket socket;
        protected Dictionary<string, string> contentMap = new Dictionary<string, string>();

        public Protocol(string name)
        {
            this.name = name;
        }

        public string GetName()
        {
            return name;
        }

        public void SetSocket(Socket socket)
        {
            this.socket = socket;
        }

        public Socket GetSocket()
        {
            return socket;
        }

        public void LoadContentFromWString(string content)
        {
            this.content = content;
            string[] contentVector = content.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (contentVector.Length % 2 != 1)
            {
                return;
            }
            for (int i = 1; i < contentVector.Length; i += 2)
            {
                contentMap[contentVector[i]] = contentVector[i + 1];
            }
        }

        public string ExportContentToWString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(name + " ");
            foreach (var item in contentMap)
            {
                stringBuilder.Append(item.Key + " " + item.Value + " ");
            }
            content = stringBuilder.ToString();
            return content;
        }
    }
}
