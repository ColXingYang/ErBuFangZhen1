using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public delegate void OnRecevieGoodPos(float pos);
public delegate void OnRecevieJXBData(JXBMove data);
public delegate void OnRecevieCaputerScreenShotFrame(CaptureShotFrameRange data);
public delegate void OnRecevieInitData(ReceiveInitData data);
public delegate void OnRecevieStepData(ReceiveStepData data);
public delegate void OnRecevieResetCommand();

public class ServerDemo : MonoSingleton<ServerDemo>
{
    public SZHSocket server;
    public OnRecevieGoodPos OnRecevieGoodPos;
    public OnRecevieJXBData OnRecevieJXBData;
    public OnRecevieCaputerScreenShotFrame OnRecevieCaputerScreenShotFrame;
    public OnRecevieInitData OnRecevieInitData;
    public OnRecevieStepData OnRecevieStepData;
    public OnRecevieResetCommand OnRecevieResetCommand;

    public override void Init()
    {
        server = new SZHSocket();
        //执行初始化服务器方法，传入委托函数  
        server.InitServer(/*ShowMsg*/);
        server.OnServerReceiveData += OnReceive;

        //ReceiveInitData data = new ReceiveInitData();
        //Debug.Log(JsonUtility.ToJson(data));
        //ReceiveStepData data = new ReceiveStepData();
        //Debug.Log(JsonUtility.ToJson(data));
    }

    void OnReceive(ReceiveMessage message)
    {
        Debug.Log("OnReceive00000");
        switch (message.type)
        {
            case DataSendType.Init:
                if (OnRecevieInitData != null)
                {
                    string data = Encoding.UTF8.GetString(message.data);
                    Debug.Log(data);
                    OnRecevieInitData(JsonUtility.FromJson<ReceiveInitData>(data));
                }
                break;
            case DataSendType.Step:
                if (OnRecevieStepData != null)
                {
                    string data = Encoding.UTF8.GetString(message.data);
                    Debug.Log(data);
                    OnRecevieStepData(JsonUtility.FromJson<ReceiveStepData>(data));
                }
                break;
            case DataSendType.Reset:
                if (OnRecevieResetCommand != null)
                {
                    OnRecevieResetCommand();
                }
                break;
            //case DataSendType.CAMERA:
            //    break;
            //case DataSendType.JXBDATA:
            //    if (OnRecevieJXBData != null)
            //    {
            //        string data = Encoding.UTF8.GetString(message.data);
            //        Debug.Log(data);
            //        OnRecevieJXBData(JsonUtility.FromJson<JXBMove>(data));
            //    }
            //    break;
            //case DataSendType.GOODPOS:
            //    if (OnRecevieGoodPos != null)
            //    {
            //        string data = Encoding.UTF8.GetString(message.data);
            //        Debug.Log(data);
            //        OnRecevieGoodPos(JsonUtility.FromJson<Good>(data).pos);
            //    }
            //    break;
            //case DataSendType.CAPUTERSCREENSHOTFRAME:
            //    if (OnRecevieCaputerScreenShotFrame != null)
            //    {
            //        string data = Encoding.UTF8.GetString(message.data);
            //        Debug.Log(data);
            //        OnRecevieCaputerScreenShotFrame(JsonUtility.FromJson<CaptureShotFrameRange>(data));
            //    }
            //    break;
            //case DataSendType.NONE:
            //    break;
            default:
                break;
        }
    }



    private void OnDestroy()
    {
        if (server == null) return;
        server.OnServerReceiveData -= OnReceive;
        server.DisConnect();
    }



}