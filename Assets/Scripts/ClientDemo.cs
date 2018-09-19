using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ClientDemo : MonoSingleton<ClientDemo>
{
    private SZHSocket szhSocket;

    public override void Init()
    {
        szhSocket = new SZHSocket();
        szhSocket.InitClient("127.0.0.1", 6065, null);
        szhSocket.OnClientReceiveData += OnClientReceiveData;
    }

    public void SendGoodPos(string value)
    {
        float goodPos = float.Parse(value);
        Good temp = new Good(goodPos);
        string sendMessage = JsonUtility.ToJson(temp);

        byte[] bytes = Encoding.UTF8.GetBytes(sendMessage);
        byte[] sendType = BitConverter.GetBytes((int)DataSendType.GOODPOS);
        byte[] length = BitConverter.GetBytes(bytes.Length + 4 + 4);
        List<byte> allMessage = new List<byte>();
        allMessage.AddRange(sendType);
        allMessage.AddRange(length);
        allMessage.AddRange(bytes);
        byte[] allMessageBytes = allMessage.ToArray();

        //Debug.Log("sendMessage:" + sendMessage);
        szhSocket.ClientSend(allMessageBytes);
    }

    public void SendJXBData(string value)
    {
        CaptureShotFrameRange range = new CaptureShotFrameRange();
        range.start = 10;
        range.end = 110;

        string sendMessage01 = JsonUtility.ToJson(range);

        byte[] bytes01 = Encoding.UTF8.GetBytes(sendMessage01);
        byte[] sendType01 = BitConverter.GetBytes((int)DataSendType.CAPUTERSCREENSHOTFRAME);
        byte[] length01 = BitConverter.GetBytes(bytes01.Length + 4 + 4);
        List<byte> allMessage01 = new List<byte>();
        allMessage01.AddRange(sendType01);
        allMessage01.AddRange(length01);
        allMessage01.AddRange(bytes01);
        byte[] allMessageBytes01 = allMessage01.ToArray();

        //Debug.Log("sendMessage01:" + sendMessage01);
        szhSocket.ClientSend(allMessageBytes01);


        float angle = float.Parse(value);

        JXBMove data = new JXBMove();
        data.datas = new List<JXBMoveData>(5);
        data.datas.Add(new JXBMoveData(0, 10));
        data.datas.Add(new JXBMoveData(-50, 10));
        data.datas.Add(new JXBMoveData(100, 10));
        data.datas.Add(new JXBMoveData(0, 10));
        data.datas.Add(new JXBMoveData(0, 10));
        
        string sendMessage = JsonUtility.ToJson(data);

        byte[] bytes = Encoding.UTF8.GetBytes(sendMessage);
        byte[] sendType = BitConverter.GetBytes((int)DataSendType.JXBDATA);
        byte[] length = BitConverter.GetBytes(bytes.Length + 4 + 4);
        List<byte> allMessage = new List<byte>();
        allMessage.AddRange(sendType);
        allMessage.AddRange(length);
        allMessage.AddRange(bytes);
        byte[] allMessageBytes = allMessage.ToArray();

        //Debug.Log("sendMessage:" + sendMessage);
        szhSocket.ClientSend(allMessageBytes);
    }

    int i = 0;
    void OnClientReceiveData(ReceiveMessage data)
    {
        switch (data.type)
        {
            case DataSendType.IMAGE:
                string filename = "C:/Users/007/Desktop/Image/Screenshot " + i++.ToString()+".jpg";
                System.IO.File.WriteAllBytes(filename, data.data);
                //Debug.Log(string.Format("截屏了一张图片: {0}", filename));
                break;
            case DataSendType.COLLIDER:
                break;
            case DataSendType.GOODPOS:
                break;
            case DataSendType.NONE:
                break;
            default:
                break;
        }
    }

    private void OnDestroy()
    {
        if (szhSocket == null) return;
        szhSocket.DisConnect();
    }


}