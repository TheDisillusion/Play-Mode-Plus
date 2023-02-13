using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CustomUnityToolbar
{
    [InitializeOnLoad]
    public static class CustomUnityToolbar
    {
        static int m_toolCount;
        static GUIStyle m_commandStyle = null;
        
        public static readonly List<Action> LeftToolbarGUI = new List<Action>();
        public static readonly List<Action> RightToolbarGUI = new List<Action>();
        
        static CustomUnityToolbar()
        {
            Type toolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
            
            string fieldName = "k_ToolCount";
            
            FieldInfo toolIcons = toolbarType.GetField(fieldName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            
            m_toolCount = toolIcons != null ? ((int) toolIcons.GetValue(null)) : 8;
            
            CustomUnityToolbarCallback.OnToolbarGUI = OnGUI;
            CustomUnityToolbarCallback.OnToolbarGUILeft = GUILeft;
            CustomUnityToolbarCallback.OnToolbarGUIRight = GUIRight;
        }
        
        public const float space = 4;
        public const float largeSpace = 20;
        public const float buttonWidth = 24;
        public const float dropdownWidth = 80;
        public const float playPauseStopWidth = 140;

        private static void OnGUI()
        {
            // Create two containers, left and right
			// Screen is whole toolbar

			m_commandStyle ??= new GUIStyle("CommandLeft");

			var screenWidth = EditorGUIUtility.currentViewWidth;

			// Following calculations match code reflected from Toolbar.OldOnGUI()
			float playButtonsPosition = Mathf.RoundToInt ((screenWidth - playPauseStopWidth) / 2);

			Rect rightRect = new Rect(0, 0, screenWidth, Screen.height);
			rightRect.xMin = playButtonsPosition;
			rightRect.xMin += m_commandStyle.fixedWidth * 3; // Play buttons
			rightRect.xMax = screenWidth;
			rightRect.xMax -= space; // Spacing right
			rightRect.xMax -= dropdownWidth; // Layout
			rightRect.xMax -= space; // Spacing between layout and layers
			rightRect.xMax -= dropdownWidth; // Layers

			rightRect.xMax -= dropdownWidth; // Account
			rightRect.xMax -= space; // Spacing between account and cloud
			rightRect.xMax -= buttonWidth; // Cloud
			rightRect.xMax -= space; // Spacing between cloud and collab
			rightRect.xMax -= 78; // Colab

			// Add spacing around existing controls
			rightRect.xMin += space;
			rightRect.xMax -= space;

			// Add top and bottom margins
			rightRect.y = 4;
			rightRect.height = 22;

			if (!(rightRect.width > 0)) return;
			GUILayout.BeginArea(rightRect);
			GUILayout.BeginHorizontal();
			foreach (var handler in RightToolbarGUI)
			{
				handler();
			}

			GUILayout.EndHorizontal();
			GUILayout.EndArea();
        }
		
		public static void GUILeft() {
			GUILayout.BeginHorizontal();
			foreach (var handler in LeftToolbarGUI)
			{
				handler();
			}
			GUILayout.EndHorizontal();
		}
		
		public static void GUIRight() {
			GUILayout.BeginHorizontal();
			foreach (var handler in RightToolbarGUI)
			{
				handler();
			}
			GUILayout.EndHorizontal();
		}
		
		
    }
}
