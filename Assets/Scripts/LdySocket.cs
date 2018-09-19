using UnityEngine;
using System.Collections.Generic;

//关于网络  

//关于套接字  

//关于文本  
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System;
using System.Linq;

public enum DataSendType
{
    Init,
    Step,
    Reset,

    Result,

    IMAGE,
    COLLIDER,
    GOODPOS,

    CAMERA,
    JXBDATA,
    CAPUTERSCREENSHOTFRAME,

    NONE,
}

//声明一个委托  
public delegate void ldyReceiveCallBack(string content);
public class LdySocket
{
    #region 服务器端  

    //声明一个服务器端的套接字  
    Socket serverSocket; 
    //声明一个委托对象  
    ldyReceiveCallBack serverCallBake;
    //生成一个比特缓存  
    byte[] serverBuffer = new byte[1024];
    //初始化服务器  
    public void InitServer(ldyReceiveCallBack rcb)
    {
        //传入委托对象  
        serverCallBake = rcb;
        //初始化服务器端的套接字  
        serverSocket = new Socket(AddressFamily.InterNetwork/*IPV4*/, SocketType.Stream/*双向读写流（服务端可以发给客户 客户也可以发服务）*/,
            ProtocolType.Tcp/*TCP协议*/);
        //实例一个网络端点  传入地址和端口  
        IPEndPoint serverEP = new IPEndPoint(IPAddress.Any, 6065);
        //绑定网络端点  
        serverSocket.Bind(serverEP);
        //设置最大监听数量  
        serverSocket.Listen(10);
        //异步接受客户端的连接(CallBack)  
        serverSocket.BeginAccept(new System.AsyncCallback(ServerAccept), serverSocket);
        //发送一个消息 表示服务器已经创建  
        serverCallBake("Server Has Init");

    }

    public void Send(byte[] value)
    {
        AsyncSend(client, value);
    }

    private void AsyncSend(Socket client, byte[] value)
    {
        if (client == null || value.Length.Equals(0)) return;

        try
        {
            //开始发送消息  
            client.BeginSend(value, 0, value.Length, SocketFlags.None, asyncResult =>
            {
                //完成消息发送  
                client.EndSend(asyncResult);
                //输出消息  
                Debug.Log("服务器发出消息");
            }, null);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    Socket client;
    //服务器接受  
    void ServerAccept(System.IAsyncResult ar)
    {
        //接受结果状态  
        serverSocket = ar.AsyncState as Socket;
        //接收结果  
        client = serverSocket.EndAccept(ar);

        client.BeginReceive(serverBuffer/*消息缓存*/,
            0/*接受消息的偏移量 就是从第几个开始*/,
            this.serverBuffer.Length/*设置接受字节数*/,
            SocketFlags.None/*Socket标志位*/,
            new System.AsyncCallback(ServerReceive)/*接受回调*/,
            client/*最后的状态*/);

        //继续接受客户端的请求  
        client.BeginAccept(new System.AsyncCallback(ServerAccept), client);

    }

    void ServerReceive(System.IAsyncResult ar)
    {
        //获取正在工作的Socket对象（用来接受数据的 ）  
        Socket workingSocket = ar.AsyncState as Socket;
        //接受到得数据字节   
        int byteCount = 0;
        //接收到的数据字符串  
        string content = "";
        try
        {
            byteCount = workingSocket.EndReceive(ar);

        }
        catch (SocketException ex)
        {
            //如果接受失败 返回详细异常  
            serverCallBake(ex.ToString());
        }
        if (byteCount > 0)
        {
            //转换byte数组为字符串（支持中文）  
            content = UTF8Encoding.UTF8.GetString(serverBuffer);


        }
        if (string.IsNullOrEmpty(content))
        {
            workingSocket.Shutdown(SocketShutdown.Both);
            workingSocket.Close();
            serverSocket.BeginAccept(new System.AsyncCallback(ServerAccept), serverSocket);
        }
        else
        {
            //发送接收到的消息
            serverCallBake(content);
            AsyncSend(workingSocket, content);
            //继续接受消息
            workingSocket.BeginReceive(serverBuffer/*消息缓存*/,
                0/*接受消息的偏移量 就是从第几个开始*/,
                this.serverBuffer.Length/*设置接受字节数*/,
                SocketFlags.None/*Socket标志位*/,
                new System.AsyncCallback(ServerReceive)/*接受回调*/,
                workingSocket/*最后的状态*/);
        }
    }



    public void Send(string arg0)
    {
        AsyncSend(client, arg0);
    }

    public void SendGoodPos(string p = "", byte[] value = null)
    {
        AsyncSend(client, DataSendType.GOODPOS, p, value);
    }


   


    private void AsyncSend(Socket client, DataSendType type, string p = "",byte[] value = null)
    {
        //if (client == null || p == string.Empty) return;
        //数据转码  
        byte[] data = new byte[1024];

        //MemoryStream ms = new MemoryStream();
        //BinaryWriter bw = new BinaryWriter(ms, new UTF8Encoding());
        //bw.Write()

        byte sendType = (byte)type;
        byte[] length = BitConverter.GetBytes(value.Length+1+4);
        //byte[] message = Encoding.UTF8.GetBytes(p);

        List<byte> allMessage = new List<byte>();
        allMessage.Add(sendType);
        allMessage.AddRange(length);
        allMessage.AddRange(value);
        byte[] allMessageBytes = allMessage.ToArray();

        Debug.Log("ServiceSendType:"+(DataSendType)((int)allMessage[0]));
        byte[] lengthRecive = new byte[4];
        Array.Copy(allMessageBytes, 1, lengthRecive, 0, 4);
        int messageLength = BitConverter.ToInt32(lengthRecive, 0);
        Debug.Log("messageLength:" + messageLength);
        byte[] messageRecive = new byte[messageLength];
        Array.Copy(allMessageBytes, 5, messageRecive, 0, messageLength - 5);


        string filename = Application.streamingAssetsPath + "/Screenshot.jpg";
        System.IO.File.WriteAllBytes(filename, messageRecive);
        Debug.Log(string.Format("截屏了一张图片: {0}", filename));


        Debug.Log(Encoding.UTF8.GetString(messageRecive));

        data = Encoding.UTF8.GetBytes(p);
        try
        {
            //开始发送消息  
            client.BeginSend(data, 0, data.Length, SocketFlags.None, asyncResult =>
            {
                //完成消息发送  
                client.EndSend(asyncResult);
                //输出消息  
                Debug.Log(string.Format("服务器发出消息:{0}", p));
            }, null);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }


    public void DisConnect()
    {
        //serverSocket.Shutdown(SocketShutdown.Both);
        serverSocket.Close();
        if (client == null || client.Connected == false)
            return;
        client.Shutdown(SocketShutdown.Both);
        client.Close();
        
    }


    

    private void AsyncSend(Socket client, string p)
    {
        if (client == null || p == string.Empty) return;
        //数据转码  
        byte[] data = new byte[1024];
        data = Encoding.UTF8.GetBytes(p);
        try
        {
            //开始发送消息  
            client.BeginSend(data, 0, data.Length, SocketFlags.None, asyncResult =>
            {
                //完成消息发送  
                client.EndSend(asyncResult);
                //输出消息  
                Debug.Log(string.Format("服务器发出消息:{0}", p));
            }, null);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

   


    #endregion

    #region  

    //声明客户端的套接字  
    Socket clientSocket;
    //声明客户端的委托对象  
    ldyReceiveCallBack clientReceiveCallBack;
    //声明客户端的缓存1KB  
    byte[] clientBuffer = new byte[1024];
    //1.ip地址 2.端口3.委托对象  
    public void InitClient(string ip, int port, ldyReceiveCallBack rcb)
    {
        //接受委托对象  
        clientReceiveCallBack = rcb;
        //实例客户端的Socket 参数（IPV4 ，双向读写流，TCP协议）  
        clientSocket = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);
        //实例化一个客户端的网络端点        IPAddress.Parse (ip)：将IP地址字符串转换为Ip地址实例  
        IPEndPoint clientEP = new IPEndPoint(IPAddress.Parse(ip), port);
        //连接服务器  
        clientSocket.Connect(clientEP);
        //第一个是缓存  第二个 是从第几个开始接受 第三个 接受多少个字节  第四个 需不需要特殊的服务 第五个回调函数 第六个当前对象  
        clientSocket.BeginReceive(clientBuffer, 0, this.clientBuffer.Length, SocketFlags.None,
            new System.AsyncCallback(clientReceive), this.clientSocket);
    }

    void clientReceive(System.IAsyncResult ar)
    {
        //获取一个客户端正在接受数据的对象  
        Socket workingSocket = ar.AsyncState as Socket;
        int byteCount = 0;
        string content = "";
        try
        {
            //结束接受数据 完成储存  
            byteCount = workingSocket.EndReceive(ar);

        }
        catch (SocketException ex)
        {
            //如果接受消息失败  
            clientReceiveCallBack(ex.ToString());
        }
        if (byteCount > 0)
        {
            //转换已经接受到得Byte数据为字符串  
            content = UTF8Encoding.UTF8.GetString(clientBuffer);
        }
        //发送数据  
        clientReceiveCallBack(content);
        //接受下一波数据  
        clientSocket.BeginReceive(clientBuffer, 0, this.clientBuffer.Length, SocketFlags.None,
            new System.AsyncCallback(clientReceive), this.clientSocket);

    }

    public void ClientSendMessage(string msg)
    {
        if (msg != "")
        {
            //将要发送的字符串消息转换成BYTE数组  
            clientBuffer = UTF8Encoding.UTF8.GetBytes(msg);
        }
        clientSocket.BeginSend(clientBuffer, 0, this.clientBuffer.Length, SocketFlags.None,
            new System.AsyncCallback(SendMsg),
            this.clientSocket);
    }

    void SendMsg(System.IAsyncResult ar)
    {
        Socket workingSocket = ar.AsyncState as Socket;
        workingSocket.EndSend(ar);
    }
    //public void DisConnect()
    //{
    //    clientSocket.Disconnect(false);
    //    clientSocket.Close();
    //}
    #endregion
}