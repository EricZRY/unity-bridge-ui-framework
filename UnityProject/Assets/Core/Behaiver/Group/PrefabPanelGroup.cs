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
using System.Collections.Generic;

public class PrefabPanelGroup : PanelGroup {
    public List<PrefabUIInfo> nodes;
    public List<PrefabPanelGroupObj> subGroups;
    public override List<UIInfoBase> Nodes { get { return nodes.ConvertAll<UIInfoBase>(x => x); } }
    public override List<PanelGroupObj> SubGroups { get { return subGroups.ConvertAll<PanelGroupObj>(x => x); } }
}
