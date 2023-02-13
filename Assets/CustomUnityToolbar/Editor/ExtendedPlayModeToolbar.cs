using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ExtendedPlayModeToolbar : VisualElement
{
    public ExtendedPlayModeToolbar()
    {
        var visualTree =
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/CustomUnityToolbar/Editor/ExtendedPlayModeToolbar.uxml");
        visualTree.CloneTree(this);
        style.flexGrow = 1;
        styleSheets.Add((StyleSheet) EditorGUIUtility.Load("ToolbarDark_inter.uss"));
        styleSheets.Add((StyleSheet) EditorGUIUtility.Load("MainToolbarDark"));
        styleSheets.Add((StyleSheet) EditorGUIUtility.Load("EditorToolbarCommon"));
    }
}
