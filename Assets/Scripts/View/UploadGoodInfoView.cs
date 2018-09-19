using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UploadGoodInfoView : MonoBehaviour {

    public GoodView view;
    public ServerDemo server;

    public Button uploadBtn;
    public Button cancelBtn;

    private void Start()
    {
        uploadBtn.onClick.AddListener(UploadGoodInfo);
        cancelBtn.onClick.AddListener(Cancel);
    }

    private void OnDestroy()
    {
        uploadBtn.onClick.RemoveListener(UploadGoodInfo);
        cancelBtn.onClick.RemoveListener(Cancel);
    }

    private void Cancel()
    {
        this.transform.gameObject.SetActive(false);
        view.GetComponent<BoxCollider>().enabled = true;
    }

    private void UploadGoodInfo()
    {
        string goodInfo = JsonUtility.ToJson(view.good);
        
        Debug.Log(goodInfo);
        Cancel();
        //server.Send(goodInfo);
    }
}
