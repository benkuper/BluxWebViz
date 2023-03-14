using OSCQuery;
using System;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using Kilosoft.Tools;

public class BluxController : MonoBehaviour
{
    WebSocket ws;

    float timeAtLastConnect;

    ObjectManager om;

    Dictionary<string, BluxObject> addressObjectMap;

    void Start()
    {
        om = FindObjectOfType<ObjectManager>();
        addressObjectMap = new Dictionary<string, BluxObject>();

        initWS();
        connect();

    }

    void initWS()
    {
        ws = new WebSocket("ws://127.0.0.1:6060");
        ws.OnOpen += OnOpen;
        ws.OnClose += OnClose;
        ws.OnError += OnError;
        ws.OnMessage += OnMessage;
    }

    // Update is called once per frame
    void Update()
    {
        if (ws == null) initWS();

        if (ws.State != WebSocketState.Open && Time.time > timeAtLastConnect + 1) connect();

#if !UNITY_WEBGL || UNITY_EDITOR
        ws.DispatchMessageQueue();
#endif
    }


    void parseMessage(string message)
    {
        JSONObject o = new JSONObject(message);
        switch (o["type"].str)
        {
            case "setup":
                if (addressObjectMap == null) addressObjectMap = new Dictionary<string, BluxObject>();
                addressObjectMap.Clear();
                if (om == null) om = FindObjectOfType<ObjectManager>();
                if (om != null) om.rebuildFromJSON(o["data"]["objects"]);
                break;

            case "controllable":
                string address = o["data"]["controlAddress"].str;
                string[] addSplit = address.Split("/");

                foreach (KeyValuePair<string, BluxObject> so in addressObjectMap)
                {
                    if (address.StartsWith(so.Key)) so.Value.updateFromJSONData(addSplit[addSplit.Length - 1], o["data"]);
                }
                break;
        }
    }

    //WS
    [EditorButton("Connect")]
    public void connect()
    {
        if (ws.State == WebSocketState.Open) ws.Close();
        ws.Connect();
        timeAtLastConnect = Time.time;
    }

    //Web socket handling
    public void sendMessage(string message)
    {
        ws.SendText(message);
    }

    public void sendData(byte[] data)
    {
        ws.Send(data);


    }

    void OnOpen()
    {
        Debug.Log("Socket Opened ");

    }
    void OnClose(WebSocketCloseCode code)
    {
        Debug.Log("Socket closed " + code);
    }
    void OnError(string msg)
    {
        Debug.LogWarning("Socket error " + msg);
    }

    void OnMessage(byte[] data)
    {
        string msg = System.Text.Encoding.Default.GetString(data);
        parseMessage(msg);

    }


    //Address map
    public void registerAddress(string address, BluxObject o)
    {
        addressObjectMap.Add(address, o);
    }
}
