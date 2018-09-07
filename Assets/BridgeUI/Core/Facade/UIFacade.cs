﻿using UnityEngine;
using System.Collections.Generic;
using System;
using BridgeUI.Model;
using System.Reflection;
using UnityEngine.Events;

namespace BridgeUI
{
    /// <summary>
    /// 界面操作接口
    /// </summary>
    public sealed class UIFacade : IUIFacade
    {
        private static UIFacade _instence;
        public static UIFacade Instence
        {
            get
            {
                if (_instence == null)
                {
                    _instence = new UIFacade();
                }
                return _instence;
            }
        }
        private static GameObjectPool _panelPool;
        public static GameObjectPool PanelPool
        {
            get
            {
                if (_panelPool == null || !_panelPool.transform)
                {
                    var gameObjectPool = new GameObject("PanelPool");
                    _panelPool = new GameObjectPool(gameObjectPool.transform);
                }
                return _panelPool;
            }
        }
        // Facade实例
        // 面板组
        private static List<IPanelGroup> groupList = new List<IPanelGroup>();
        //handle池
        private static UIHandlePool handlePool = new UIHandlePool();

        public static void RegistGroup(IPanelGroup group)
        {
            if (!groupList.Contains(group))
            {
                groupList.Add(group);
            }
        }

        public static void UnRegistGroup(IPanelGroup group)
        {
            if (groupList.Contains(group))
            {
                groupList.Remove(group);
            }
        }

        public IUIHandle Open(string panelName, object data = null)
        {
            return Open(null, panelName,  data);
        }

        public IUIHandle Open(IUIPanel parent, string panelName, object data = null)
        {
            var handle = handlePool.Allocate(panelName);

            var currentGroup = parent == null ? null : parent.Group;

            if (currentGroup != null)//限制性打开
            {
                InternalOpen(parent, currentGroup, handle, panelName,-1);
            }
            else
            {
                foreach (var group in groupList)
                {
                    InternalOpen(parent, group, handle, panelName,-1);
                }
            }

            if (data != null)
            {
                handle.Send(data);
            }

            return handle;
        }

        public IUIHandle Open(IUIPanel parent, string panelName, int index,  object data = null)
        {
            var handle = handlePool.Allocate(panelName);

            var currentGroup = parent == null ? null : parent.Group;

            if (currentGroup != null)//限制性打开
            {
                InternalOpen(parent, currentGroup, handle, panelName, index);
            }
            else
            {
                foreach (var group in groupList)
                {
                    InternalOpen(parent, group, handle, panelName, index);
                }
            }

            if (data != null)
            {
                handle.Send(data);
            }

            return handle;
        }
        private void InternalOpen(IUIPanel parentPanel, IPanelGroup group, IUIHandleInternal handle,string panelName,int index)
        {
            var Content = parentPanel == null ? null : parentPanel.Content;
            Bridge bridgeObj = group.InstencePanel(parentPanel, panelName,index, Content);
            if (bridgeObj != null)
            {
                handle.RegistBridge(bridgeObj);
            }
        }

        public void Hide(string panelName)
        {
            Hide(null, panelName);
        }

        public void Hide(IPanelGroup currentGroup, string panelName)
        {
            if (currentGroup != null)//限制性打开
            {
                InternalHide(currentGroup, panelName);
            }
            else
            {
                foreach (var group in groupList)
                {
                    InternalHide(group, panelName);
                }
            }
        }

        private void InternalHide(IPanelGroup group, string panelName)
        {
            var panels = group.RetrivePanels(panelName);
            if (panels != null)
            {
                foreach (var panel in panels)
                {
                    panel.Hide();
                }
            }
        }

        public void Close(IPanelGroup currentGroup, string panelName)
        {
            if (currentGroup != null)
            {
                InteralClose(currentGroup, panelName);
            }
            else
            {
                foreach (var group in groupList)
                {
                    InteralClose(group, panelName);
                }
            }
        }
        public void Close(string panelName)
        {
            Close(null, panelName);
        }

        private void InteralClose(IPanelGroup group, string panelName)
        {
            var panels = group.RetrivePanels(panelName);
            if (panels != null)
            {
                foreach (var panel in panels)
                {
                    panel.Close();
                }
            }
            group.CansaleInstencePanel(panelName);
        }

        public bool IsPanelOpen(string panelName)
        {
            bool globleHave = false;
            foreach (var item in groupList)
            {
                globleHave |= IsPanelOpen(item, panelName);
            }
            return globleHave;
        }

        public bool IsPanelOpen(IPanelGroup parentPanel, string panelName)
        {
            var panels = parentPanel.RetrivePanels(panelName);
            return (panels != null && panels.Count > 0);
        }
    }
}