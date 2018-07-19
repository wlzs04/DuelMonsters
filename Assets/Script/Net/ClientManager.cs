using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Assets.Script.Net
{
    class ClientManager
    {
        static ClientManager instance = new ClientManager();
        Socket clientSocket = null;

        Dictionary<string, ClientProtocol> legalProtocolMap = new Dictionary<string, ClientProtocol>();

        public delegate void SocketEvent(Socket socket);
        public delegate void ProtocolEvent(ClientProtocol protocol);
        public SocketEvent ConnectSuccessEvent; 
        public SocketEvent ConnectFailEvent;
        public SocketEvent DisconnectEvent;
        public ProtocolEvent ProcessProtocolEvent;

        private ClientManager()
        {
        }
        
        public static ClientManager GetSingleInstance()
        {
            return instance;
        }

        public void StartConnect(string ip,int port)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                clientSocket.Connect(ip, port);
                if(ConnectSuccessEvent!=null)
                {
                    ConnectSuccessEvent.Invoke(clientSocket);
                }
                StartReceive();
            }
            catch (Exception)
            {
                if (ConnectFailEvent != null)
                {
                    ConnectFailEvent.Invoke(clientSocket);
                    clientSocket.Close();
                }
            }
        }

        public void GetClientFromServer(Socket clientSocket)
        {
            if (ConnectSuccessEvent != null)
            {
                ConnectSuccessEvent.Invoke(clientSocket);
            }
            this.clientSocket = clientSocket;
            StartReceive();
        }

        void StartReceive()
        {
            Thread receiveThread = new Thread(new ThreadStart(ReceiveFromServer));
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }

        void ReceiveFromServer()
        {
            try
            {
                byte[] buf4 = new byte[4];
                while (true)
                {
                    clientSocket.Receive(buf4, 4, SocketFlags.None);
                    int protocolLength = (buf4[0] - '0') * 1000 + (buf4[1] - '0') * 100 + (buf4[2] - '0') * 10 + (buf4[3] - '0') * 1;
                    byte[] buf = new byte[protocolLength];
                    int byteNumber = clientSocket.Receive(buf, protocolLength, SocketFlags.None);

                    string s = Encoding.UTF8.GetString(buf, 0, byteNumber);
                    string[] ss = s.Split(' ');

                    if (legalProtocolMap.ContainsKey(ss[0]))
                    {
                        ClientProtocol protocol = legalProtocolMap[ss[0]].GetInstance();
                        protocol.LoadContentFromWString(s);
                        if(ProcessProtocolEvent!=null)
                        {
                            ProcessProtocolEvent(protocol);
                        }
                    }
                    else
                    {
                        Debug.LogError("非法协议！" + ss[0]);
                    }
                }
            }
            catch (Exception)
            {
                if (DisconnectEvent != null)
                {
                    DisconnectEvent.Invoke(clientSocket);
                }
            }
            finally
            {
                clientSocket.Close();
            }
        }

        public void StopConnect()
        {
            clientSocket.Close();
            if (DisconnectEvent != null)
            {
                DisconnectEvent.Invoke(clientSocket);
            }
        }

        public bool SendProtocol(Protocol protocol)
        {
            byte[] msg = Encoding.UTF8.GetBytes(protocol.ExportContentToWString());

            List<byte> byteSource = new List<byte>();
            byteSource.AddRange(Encoding.UTF8.GetBytes(String.Format("{0:0000}", msg.Length)));
            byteSource.AddRange(msg);
            clientSocket.Send(byteSource.ToArray());
            return true;
        }

        public void AddLegalProtocol(ClientProtocol protocol)
        {
            legalProtocolMap.Add(protocol.GetName(),protocol);
        }
    }
}
