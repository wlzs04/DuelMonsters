using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Assets.Script.Net
{
    class ServerManager
    {
        static ServerManager instance = new ServerManager();
        Socket serverSocket = null;
        IPAddress ip = null;
        int maxLinkNumber = 1;//服务器最大监听数量。
        Socket clientSocket = null;
        //Dictionary<string, Socket> ipSocketMap = new Dictionary<string, Socket>();
        //Queue<ClientProtocol> protocolQueue = new Queue<ClientProtocol>();
        //Dictionary<string, ClientProtocol> legalProtocolMap = new Dictionary<string, ClientProtocol>();

        public delegate void SocketEvent(Socket socket);
        //public delegate void ProtocolEvent(ClientProtocol protocol);
        public SocketEvent AcceptNewSocketEvent;
        public SocketEvent SocketDisconnectEvent;
        public ClientManager.ProtocolEvent ProcessProtocolEvent;

        private ServerManager()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ip = GetHostIPV4();
        }

        /// <summary>
        /// 获得服务器管理单一实例
        /// </summary>
        /// <returns></returns>
        public static ServerManager GetInstance()
        {
            return instance;
        }

        /// <summary>
        /// 开始监听指定端口
        /// </summary>
        /// <param name="port"></param>
        public bool StartListen(int port)
        {
            IPEndPoint point = new IPEndPoint(ip, port);
            try
            {
                serverSocket.Bind(point);
                serverSocket.Listen(maxLinkNumber);

                Thread threadwatch = new Thread(AcceptClient);
                threadwatch.IsBackground = true;
                threadwatch.Start();
            }
            catch (Exception)
            {
                serverSocket.Close();
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获得本机IPV4地址
        /// </summary>
        /// <returns></returns>
        public IPAddress GetHostIPV4()
        {
            IPAddress ip = null;
            IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (var item in ips)
            {
                if (item.AddressFamily == AddressFamily.InterNetwork)
                {
                    ip = item;
                    break;
                }
            }
            return ip;
        }

        /// <summary>
        /// 服务器接收客户端
        /// </summary>
        void AcceptClient()
        {
            try
            {
                clientSocket = serverSocket.Accept();
                string ipPort = (clientSocket.RemoteEndPoint as IPEndPoint).ToString();
                
                if(AcceptNewSocketEvent!=null)
                {
                    AcceptNewSocketEvent.Invoke(clientSocket);
                }

                ClientManager.GetInstance().GetClientFromServer(clientSocket);
                ClientManager.GetInstance().ProcessProtocolEvent += ProcessProtocolEvent;
            }
            catch (SocketException)
            {
                if (SocketDisconnectEvent != null)
                {
                    SocketDisconnectEvent.Invoke(serverSocket);
                    serverSocket.Close();
                }
            }
        }

        /// <summary>
        /// 服务器停止监听，并断开所有链接
        /// </summary>
        public void StopListen()
        {
            serverSocket.Close();
        }
    }
}
