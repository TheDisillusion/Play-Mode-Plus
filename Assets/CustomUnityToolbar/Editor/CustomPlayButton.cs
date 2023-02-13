using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.SceneManagement;
using System.Reflection;
using ASze.CustomPlayButton;
using PopupWindow = UnityEditor.PopupWindow;


namespace CustomUnityToolbar
{
    [InitializeOnLoad]
    public class CustomPlayButton
    {
        const string FolderPath = "Assets/CustomUnityToolbar/Editor";
        const string SettingPath = FolderPath + "BookmarkSetting.asset";
        const string IconsPath = "Assets/CustomUnityToolbar/Editor/Icons/";

        // private static SceneBookmark bookmark = null;
        private static SceneAsset _selectedScene;

        private static readonly GUIContent PlaySceneContent;
        private static readonly GUIContent SceneSelectContent;
        private static readonly GUIContent PlaySettingsContent;
        private static readonly GUIContent BuildContent;

        static Rect buttonRect;
        static VisualElement _toolbarElement;
        static SceneAsset _lastScene;
        

        internal static SceneAsset SelectedScene
        {
            get => _selectedScene;
            set
            {
                _selectedScene = value;
                _toolbarElement?.MarkDirtyRepaint();

                if (value != null)
                {
                    var path = AssetDatabase.GetAssetPath(value);
                    EditorPrefs.SetString(GetEditorPrefKey(), path);
                }
                else
                {
                    EditorPrefs.DeleteKey(GetEditorPrefKey());
                }
            }
        }

        static class ToolbarStyles
        {
            public static readonly GUIStyle CommandButtonStyle;

            static ToolbarStyles()
            {
                EditorApplication.playModeStateChanged += HandleOnPlayModeChanged;
                CommandButtonStyle = new GUIStyle("Command")
                {
                    fontSize = 16,
                    alignment = TextAnchor.MiddleCenter,
                    imagePosition = ImagePosition.ImageAbove,
                    fontStyle = FontStyle.Bold,
                    fixedWidth = 32,
                    fixedHeight = 20
                };
            }
        }

        static CustomPlayButton()
        {
            CustomUnityToolbar.RightToolbarGUI.Add(OnToolbarRightGUI);
            EditorApplication.update += OnUpdate;

            if (_selectedScene == null)
            {
                _selectedScene = _lastScene;
            }
            /*if (bookmark == null)
            {
                bookmark = AssetDatabase.LoadAssetAtPath<SceneBookmark>(SETTING_PATH);
                Bookmark?.RemoveNullValue();
            }*/

            var savedScenePath = EditorPrefs.GetString(GetEditorPrefKey(), "");
            _selectedScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(savedScenePath);
            if (_selectedScene == null && EditorBuildSettings.scenes.Length > 0)
            {
                var scenePath = EditorBuildSettings.scenes[0].path;
                SelectedScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            }
            
            PlaySceneContent = CreateIconContent("CustomPlayButton.png", "d_UnityEditor.Timeline.TimelineWindow@2x",
                "Play Custom Scene");
            SceneSelectContent = CreateIconContent("CustomSceneColorButton.png", "d_UnityEditor.GameView@2x", "Play Game Scene");
            PlaySettingsContent = CreateIconContent("CustomSettingsButton.png", "d_UnityEditor.GameView@2x", "Play Game Scene");
            BuildContent = CreateIconContent("CustomBuildButton.png", "d_UnityEditor.GameView@2x", "Play Game Scene");
        }

        static void OnToolbarRightGUI()
        {
            GUILayout.BeginHorizontal();

            var sceneName = _selectedScene != null ? _selectedScene.name : "Play Mode Scene...";
            var selected =
                EditorGUILayout.DropdownButton(new GUIContent(sceneName), FocusType.Passive, GUILayout.Width(100.0f));
            if (Event.current.type == EventType.Repaint)
            {
                buttonRect = GUILayoutUtility.GetLastRect();
            }

            if (selected)
            {
                
                PopupWindow.Show(buttonRect, new CustomSceneSelect());
            }

            if (GUILayout.Button(PlaySceneContent, ToolbarStyles.CommandButtonStyle))
            {
                if (_selectedScene != null)
                {
                    StartScene(_selectedScene);
                }
                else
                {
                    EditorUtility.DisplayDialog(
                        "Cannot play custom scene",
                        "No scene is selected to play. Please select a scene from the dropdown list.",
                        "Ok");
                }
            }

            if (GUILayout.Button(SceneSelectContent, ToolbarStyles.CommandButtonStyle))
            {
                PopupWindow.Show(buttonRect, new CustomSceneSelect());
                // if (EditorBuildSettings.scenes.Length > 0)
                // {
                //     var scenePath = EditorBuildSettings.scenes[0].path;
                //     var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                //     StartScene(scene);
                // }
                // else
                // {
                //     if (!EditorUtility.DisplayDialog(
                //             "Cannot play the game",
                //             "Please add the first scene in build setting in order to play the game.",
                //             "Ok", "Open build setting"))
                //     {
                //         EditorWindow.GetWindow(System.Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
                //     }
                //
                //     // Avoid error from GUILayout.EndHorizontal()
                //     GUILayout.BeginHorizontal();
                // }
            }
            
            if (GUILayout.Button(PlaySettingsContent, ToolbarStyles.CommandButtonStyle))
            {
                if (EditorBuildSettings.scenes.Length > 0)
                {
                    var scenePath = EditorBuildSettings.scenes[0].path;
                    var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                    StartScene(scene);
                }
                else
                {
                    if (!EditorUtility.DisplayDialog(
                            "Cannot play the game",
                            "Please add the first scene in build setting in order to play the game.",
                            "Ok", "Open build setting"))
                    {
                        EditorWindow.GetWindow(System.Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
                    }

                    // Avoid error from GUILayout.EndHorizontal()
                    GUILayout.BeginHorizontal();
                }
            }
            
            if (GUILayout.Button(BuildContent, ToolbarStyles.CommandButtonStyle))
            {
                if (EditorBuildSettings.scenes.Length > 0)
                {
                    var scenePath = EditorBuildSettings.scenes[0].path;
                    var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                    StartScene(scene);
                }
                else
                {
                    if (!EditorUtility.DisplayDialog(
                            "Cannot play the game",
                            "Please add the first scene in build setting in order to play the game.",
                            "Ok", "Open build setting"))
                    {
                        EditorWindow.GetWindow(System.Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
                    }

                    // Avoid error from GUILayout.EndHorizontal()
                    GUILayout.BeginHorizontal();
                }
            }
        }

        static void StartScene(SceneAsset scene)
        {
            if (EditorApplication.isPlaying)
            {
                _lastScene = scene;
                EditorApplication.isPlaying = false;
            }
            else
            {
                ChangeScene(scene);
            }
        }

        static void OnUpdate()
        {
            // Get toolbar element for repainting
            if (_toolbarElement == null)
            {
                var toolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
                var toolbars = Resources.FindObjectsOfTypeAll(toolbarType);
                var currentToolbar = toolbars.Length > 0 ? (ScriptableObject) toolbars[0] : null;
                if (currentToolbar != null)
                {
                    var guiViewType = typeof(Editor).Assembly.GetType("UnityEditor.GUIView");

                    var iWindowBackendType = typeof(Editor).Assembly.GetType("UnityEditor.IWindowBackend");
                    var guiBackend = guiViewType.GetProperty("windowBackend",
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    var viewVisualTree = iWindowBackendType.GetProperty("visualTree",
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    var windowBackend = guiBackend.GetValue(currentToolbar);
                    _toolbarElement = (VisualElement) viewVisualTree.GetValue(windowBackend, null);
                }
            }

            if (_lastScene == null ||
                EditorApplication.isPlaying || EditorApplication.isPaused ||
                EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            ChangeScene(_lastScene);
            _lastScene = null;
        }

        static void ChangeScene(SceneAsset scene)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.playModeStartScene = scene;
                EditorApplication.isPlaying = true;
            }
        }

        static void HandleOnPlayModeChanged(PlayModeStateChange playMode)
        {
            if (playMode == PlayModeStateChange.ExitingPlayMode)
            {
                EditorSceneManager.playModeStartScene = null;
            }
        }

        public static string GetEditorPrefKey()
        {
            var projectPrefix = PlayerSettings.companyName + "." + PlayerSettings.productName;
            return projectPrefix + "_CustomPlayButton_SelectedScenePath";
        }

        public static GUIContent CreateIconContent(string localTex, string builtInTex, string tooltip)
        {
            var tex = LoadTexture(localTex);
            return tex != null ? new GUIContent(tex, tooltip) : EditorGUIUtility.IconContent(builtInTex, tooltip);
        }

        private static Texture2D LoadTexture(string path)
        {
            return (Texture2D) EditorGUIUtility.Load(IconsPath + path);
        }
    }
}