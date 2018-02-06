﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace BridgeUI.Common
{
    public class TreeNodeCreater
    {
        private int deepth;
        private Transform parent;
        private GameObjectPool pool;
        private TreeSelectItem[] created;
        private TreeOption option;
        private ToggleGroup group;
        public ToggleGroup Group { get { return group; } }
        public TreeSelectItem[] CreatedItems { get { return created; } }

        public TreeNodeCreater(int deepth, Transform parent, TreeOption option)
        {
            this.deepth = deepth;
            this.parent = parent;
            this.option = option;
            pool = UIFacade.PanelPool;
        }

        public TreeSelectItem[] CreateTreeSelectItems(TreeNode[] childNodes)
        {
            var prefab = option.prefab;
            var rule = option.ruleGetter(deepth + 1);

            if(rule.makeGroup) {
                group = InitGroup();
            }

            created = new TreeSelectItem[childNodes.Length];
            for (int i = 0; i < childNodes.Length; i++)
            {
                var item = pool.GetPoolObject(prefab.gameObject, parent, false);
                TreeSelectItem tsi = item.GetComponent<TreeSelectItem>();
                tsi.InitTreeSelecter(deepth + 1, childNodes[i],option);
                if (rule.makeGroup) tsi.SetGroup(group);
                created[i] = tsi;
            }
            return created;
        }

        internal void SetChildActive(List<int> list)
        {
            if (list == null || list.Count == 0) return;

            var key = list[0];

            if (created.Length <= key) return;

            var item = created[key];

            if (item == null) return;
            item.SetToggle(true);

            if (item.Creater == null) return;
            list.RemoveAt(0);
            item.Creater.SetChildActive(list);
        }

        private ToggleGroup InitGroup()
        {
            group = parent.GetComponent<ToggleGroup>();
            if (group == null)
            {
                group = parent.gameObject.AddComponent<ToggleGroup>();
            }
            //group.allowSwitchOff = true;
            return group;
        }
    }

}