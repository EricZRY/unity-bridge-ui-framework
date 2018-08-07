﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using BridgeUI;
using BridgeUI.Model;
using System;
using UnityEngine.EventSystems;

namespace BridgeUI
{

    public sealed class PanelCore : MonoBehaviour, IUIPanel
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
        public IUIPanel Parent { get; set; }
        public Transform Content { get { return transform; } }
        public Transform Root { get { return transform.parent.parent; } }
        public UIType UType { get; set; }
        public List<IUIPanel> ChildPanels
        {
            get
            {
                return childPanels;
            }
        }
        public bool IsShowing
        {
            get
            {
                return _isShowing;
            }
        }
        public bool IsAlive
        {
            get
            {
                return _isAlive;
            }
        }
        private AnimPlayer _enterAnim;
        private AnimPlayer enterAnim
        {
            get
            {
                if (_enterAnim == null)
                {
                    _enterAnim = UType.enterAnim;
                    _enterAnim.SetContext(this);
                }
                return _enterAnim;
            }
        }
        private AnimPlayer _quitAnim;
        private AnimPlayer quitAnim
        {
            get
            {
                if (_quitAnim == null)
                {
                    _quitAnim = UType.quitAnim;
                    _quitAnim.SetContext(this);
                }
                return _quitAnim;

            }
        }

        private Bridge bridge;
        private List<IUIPanel> childPanels;
        public event PanelCloseEvent onDelete;
        private event UnityAction<object> onReceive;
        private bool _isShowing = true;
        private bool _isAlive = true;

        void Start()
        {
            if (bridge != null){
                bridge.OnCreatePanel(this);
            }
            AppendComponentsByType();
            OnOpenInternal();
        }

        void OnDestroy()
        {
            _isAlive = false;

            _isShowing = false;

            if (bridge != null)
            {
                bridge.Release();
            }

            if (onDelete != null)
            {
                onDelete.Invoke(this, true);
            }

        }

        public void OnRegistOnRecevie(UnityAction<object> onReceive)
        {
            this.onReceive += onReceive;
        }

        public void SetParent(Transform Trans)
        {
            Utility.SetTranform(transform, UType.layer, UType.layerIndex, Trans);
        }
        public void CallBack(object data)
        {
            if (bridge != null)
            {
                bridge.CallBack(this, data);
            }
        }
        public void HandleData(Bridge bridge)
        {
            this.bridge = bridge;
            if (bridge != null)
            {
                HandleData(bridge.dataQueue);
                bridge.onGet = HandleData;
            }
        }
        private void HandleData(Queue<object> dataQueue)
        {
            if (dataQueue != null)
            {
                while (dataQueue.Count > 0)
                {
                    var data = dataQueue.Dequeue();
                    if (data != null)
                    {
                        HandleData(data);
                    }
                }
            }
        }

        private void HandleData(object data)
        {
            if(this.onReceive != null)
            {
                onReceive.Invoke(data);
            }
        }
 
        public void Hide()
        {
            _isShowing = false;
            switch (UType.hideRule)
            {
                case HideRule.AlaphGameObject:
                    AlaphGameObject(true);
                    break;
                case HideRule.HideGameObject:
                    gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
        }

        public void OnDestroyHide()
        {
            _isShowing = false;
            gameObject.SetActive(false);
            if (onDelete != null)
            {
                onDelete.Invoke(this, false);
            }
        }

        public void UnHide()
        {
            gameObject.SetActive(true);
            if (UType.hideRule == HideRule.AlaphGameObject)
            {
                AlaphGameObject(false);
            }
            _isShowing = true;
            OnOpenInternal();
        }
        public void Close()
        {
            if (IsShowing && UType.quitAnim != null)
            {
                quitAnim.PlayAnim( CloseInternal);
            }
            else
            {
                CloseInternal();
            }
        }
        private void CloseInternal()
        {
            _isShowing = false;

            switch (UType.closeRule)
            {
                case CloseRule.DestroyImmediate:
                    DestroyImmediate(gameObject);
                    break;
                case CloseRule.DestroyDely:
                    Destroy(gameObject, 0.02f);
                    break;
                case CloseRule.DestroyNoraml:
                    Destroy(gameObject);
                    break;
                case CloseRule.HideGameObject:
                    OnDestroyHide();
                    break;
                default:
                    break;
            }
        }
        public void RecordChild(IUIPanel childPanel)
        {
            if (childPanels == null)
            {
                childPanels = new List<IUIPanel>();
            }
            if (!childPanels.Contains(childPanel))
            {
                childPanel.onDelete += OnRemoveChild;
                childPanels.Add(childPanel);
            }
            childPanel.Parent = this;
        }
        private void AppendComponentsByType()
        {
            if (UType.form == UIFormType.DragAble)
            {
                if (gameObject.GetComponent<DragPanel>() == null)
                {
                    gameObject.AddComponent<DragPanel>();
                }
            }
        }
        public void Cover()
        {
            var covername = Name + "_Cover";
            var rectt = new GameObject(covername, typeof(RectTransform)).GetComponent<RectTransform>();
            rectt.gameObject.layer = 5;
            rectt.SetParent(transform, false);
            rectt.SetSiblingIndex(0);
            rectt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 10000);
            rectt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 10000);
            var img = rectt.gameObject.AddComponent<Image>();
            img.color = new Color(0, 0, 0, 0.01f);
            img.raycastTarget = true;
        }
        private void OnRemoveChild(IUIPanel childPanel, bool remove)
        {
            if (childPanels != null && childPanels.Contains(childPanel) && remove)
            {
                childPanels.Remove(childPanel);
            }
        }
        private void OnOpenInternal()
        {
            if (UType.enterAnim != null)
            {
                enterAnim.PlayAnim( null);
            }
        }
        private void AlaphGameObject(bool hide)
        {
            var canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            if (hide)
            {
                canvasGroup.alpha = UType.hideAlaph;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
            else
            {
                canvasGroup.alpha = 1;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
        }
    }
}