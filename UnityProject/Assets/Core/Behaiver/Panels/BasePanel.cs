﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Sprites;
using UnityEngine.Scripting;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Assertions.Must;
using UnityEngine.Assertions.Comparers;
using System.Collections;
using System;
using System.Collections.Generic;

public class PanelBase :MonoBehaviour, IPanelBase
{
    public int InstenceID
    {
        get
        {
            return GetInstanceID();
        }
    }
    public string Name { get { return name; } }
    public IPanelGroup Group { get; set; }
    [SerializeField]
    private Transform content;
    public Transform Content { get { return content == null ? transform:content; } }

    protected UIFacade selfFacade;

    protected BridgeObj bridge;

    public event UnityAction<IPanelBase> onDelete;

    public void CallBack(object data)
    {
        if(bridge != null)
        {
            bridge.CallBack(data);
        }
    }

    public void HandleData(BridgeObj bridge)
    {
        this.bridge = bridge;
        if (bridge)
        {
            HandleData(bridge.dataQueue);
            bridge.onGet = HandleData;
        }
    }

    protected virtual void HandleData(Queue<object> dataQueue)
    {
        if(dataQueue != null)
        {
            while (dataQueue.Count > 0)
            {
                var data = dataQueue.Dequeue();
                HandleData(data);
            }
        }
    }

    protected virtual void HandleData(object data)
    {
        Debug.Log(data);
    }

    protected virtual void Awake()
    {
        selfFacade = UIFacade.CreatePanelFacade(this);
    }

    protected virtual void OnDestroy()
    {
        if(bridge){
            bridge.Release();
        }
        if(onDelete != null){
            onDelete.Invoke(this);
        }
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    public virtual void Close()
    {
        Destroy(gameObject);
    }

}
