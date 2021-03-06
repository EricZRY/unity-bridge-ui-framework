﻿#region statement
/*************************************************************************************   
    * 作    者：       zouhunter
    * 时    间：       2018-02-06 01:13:36
    * 说    明：       1.用户选择面板
                       2.反正true或false
* ************************************************************************************/
#endregion
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace BridgeUI.Common
{
    /// <summary>
    /// 添加了事件的popupPanel
    /// </summary>
    public class FunctionPopupPanel : PopupPanel
    {
        public Button deniBtn;
        protected override void Awake()
        {
            base.Awake();
            deniBtn.onClick.AddListener(OnButtonAction);
        }
        public override void Close()
        {
            callBack = true;
            base.Close();
        }

        void OnButtonAction()
        {
            callBack = false;
            base.Close();
        }
    }
}