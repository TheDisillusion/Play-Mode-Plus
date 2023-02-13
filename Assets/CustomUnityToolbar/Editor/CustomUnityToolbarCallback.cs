using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace CustomUnityToolbar
{
    public static class CustomUnityToolbarCallback
    {
        private static readonly Type MToolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
        private static readonly Type MGUIViewType = typeof(Editor).Assembly.GetType("UnityEditor.GUIView");

        private static readonly Type MiWindowBackendType = typeof(Editor).Assembly.GetType("UnityEditor.IWindowBackend");
        static PropertyInfo m_windowBackend = MGUIViewType.GetProperty("windowBackend",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        static PropertyInfo m_viewVisualTree = MiWindowBackendType.GetProperty("visualTree",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        
        static FieldInfo m_imguiContainerOnGui = typeof(IMGUIContainer).GetField("m_OnGUIHandler",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        static ScriptableObject m_currentToolbar;
        
        public static Action OnToolbarGUI;
        public static Action OnToolbarGUILeft;
        public static Action OnToolbarGUIRight;
		
        static CustomUnityToolbarCallback()
        {
            EditorApplication.update -= OnUpdate;
            EditorApplication.update += OnUpdate;
        }

        private static void OnUpdate()
        {
            if (m_currentToolbar == null)
            {
                var toolbars = Resources.FindObjectsOfTypeAll(MToolbarType);
                m_currentToolbar = toolbars.Length > 0 ? (ScriptableObject) toolbars[0] : null;

                if (m_currentToolbar != null)
                {
                    var root = m_currentToolbar.GetType()
                        .GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
                    var rawRoot = root?.GetValue(m_currentToolbar);
                    var mRoot = rawRoot as VisualElement;
                    var playButton = mRoot.Q("Play");
                    var extendedPlayModeToolbar = new ExtendedPlayModeToolbar();
                    mRoot.Q("ToolbarZoneRightAlign").Add(extendedPlayModeToolbar);
                    RegisterCallback("ToolbarZoneLeftAlign", OnToolbarGUILeft);
                    // RegisterCallback("ToolbarZoneRightAlign", OnToolbarGUIRight);
                    

                    void RegisterCallback(string root, Action cb)
                    {
                        var toolbarZone = mRoot.Q(root);

                        var parent = new VisualElement()
                        {
                            style =
                            {
                                flexGrow = 1,
                                flexDirection = FlexDirection.Row,
                            }
                        };
                        var container = new IMGUIContainer();
                        container.style.flexGrow = 1;
                        container.onGUIHandler += () => { cb?.Invoke(); };
                        parent.Add(container);
                        toolbarZone.Add(parent);
                    }
                }
            }
        }
        
        static void OnGUI()
        {
            var handler = OnToolbarGUI;
            handler?.Invoke();
        }
    }
}

