using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

namespace ASze.CustomPlayButton
{
    public class CustomSceneSelect : PopupWindowContent
    {
        const float COLLUMN_WIDTH = 200.0f;
        readonly GUILayoutOption[] ICON_LAYOUT = new GUILayoutOption[] {
            GUILayout.Width(20.0f), GUILayout.Height(20.0f)
        };


        GUIStyle titleButtonStyle;
        GUIStyle buttonStyle;
        GUIStyle selectedButtonStyle;
        GUIContent bookmarkContent;
        SceneAsset[] buildScenes;
        SceneAsset currentScene;

        Vector2 scrollPosBuild;
        Vector2 scrollPosBookmark;

        public CustomSceneSelect() : base()
        {
            InitStyles();

            bookmarkContent = EditorGUIUtility.IconContent("blendKeySelected", "Bookmark ScriptableObject");

            GetBuildScenes();
            currentScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorSceneManager.GetActiveScene().path);
            // CustomPlayButton.Bookmark.RemoveNullValue();
        }

        void InitStyles()
        {
            var blankTex = MakeTex(new Color(0f, 0f, 0f, 0f));
            var selectedTex = MakeTex(new Color(0f, 0f, 0f, 0.3f));

            var hoverState = new GUIStyleState()
            {
                background = selectedTex,
                textColor = GUI.skin.button.onHover.textColor,
            };
            buttonStyle = new GUIStyle(GUI.skin.label)
            {
                onHover = hoverState,
                hover = hoverState,
            };
            buttonStyle.normal.background = blankTex;

            selectedButtonStyle = new GUIStyle(buttonStyle);
            selectedButtonStyle.normal.background = selectedTex;

            titleButtonStyle = new GUIStyle(EditorStyles.boldLabel);
            titleButtonStyle.onHover = buttonStyle.onHover;
            titleButtonStyle.hover = buttonStyle.hover;
            titleButtonStyle.normal.background = blankTex;
        }

        public static Texture2D MakeTex(Color col)
        {
            var texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            texture.SetPixel(0, 0, col);
            texture.Apply();
            return texture;
        }

        public override Vector2 GetWindowSize()
        {
            var width = COLLUMN_WIDTH;
            var maxRow = Mathf.Max(buildScenes.Length);
            var height = Mathf.Min(22 * maxRow + 26, Screen.currentResolution.height * 0.5f);
            return new Vector2(width, height);
        }

        public override void OnGUI(Rect rect)
        {
            EditorGUILayout.BeginHorizontal();
            DrawBuildScenes();
            EditorGUILayout.EndHorizontal();

            if (Event.current.type == EventType.MouseMove && EditorWindow.mouseOverWindow == editorWindow)
                editorWindow?.Repaint();
        }

        void DrawBuildScenes()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Scenes in Project", EditorStyles.boldLabel, GUILayout.Height(20.0f));
            // if (!CustomPlayButton.Bookmark.HasBookmark())
            // {
            //     GUILayout.FlexibleSpace();
            //     if (GUILayout.Button(bookmarkContent, buttonStyle, ICON_LAYOUT))
            //     {
            //         Selection.activeObject = CustomPlayButton.Bookmark;
            //     }
            // }
            EditorGUILayout.EndHorizontal();

            if (buildScenes.Length > 0)
            {
                scrollPosBuild = EditorGUILayout.BeginScrollView(scrollPosBuild);
                for (int i = 0; i < buildScenes.Length; i++)
                {
                    DrawSelection(buildScenes[i], i);
                }
                EditorGUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Label("No scenes available");
            }
            EditorGUILayout.EndVertical();
        }
        
        void DrawSelection(SceneAsset scene, int index = -1)
        {
            if (scene == null) return;

            GUILayout.BeginHorizontal();
            var style = CustomUnityToolbar.CustomPlayButton.SelectedScene == scene ? selectedButtonStyle : buttonStyle;
            if (GUILayout.Button(index >= 0 ? $"{index}\t{scene.name}" : scene.name, style))
            {
                SelectScene(scene);
            }
        }

        void SelectScene(SceneAsset scene)
        {
            CustomUnityToolbar.CustomPlayButton.SelectedScene = scene;
            editorWindow.Close();
        }

        void OpenScene(SceneAsset scene)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                var scenePath = AssetDatabase.GetAssetPath(scene);
                EditorSceneManager.OpenScene(scenePath);
                currentScene = scene;
                // Recreate textures which are destoryed by OpenScene
                InitStyles();
            }
        }
        
        private static SceneAsset[] GetAllScenes()
        {
            string[] guids = AssetDatabase.FindAssets("t:SceneAsset");
            SceneAsset[] sceneAssets = new SceneAsset[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                sceneAssets[i] = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                if (string.IsNullOrEmpty(sceneAssets[i].name))
                {
                    sceneAssets[i].name = guids[i];
                }
            }
            return sceneAssets;
        }

        void GetBuildScenes()
        {
            List<SceneAsset> buildSceneList = new List<SceneAsset>();
            var settingScenes = EditorBuildSettings.scenes;
            foreach (var settingScene in settingScenes)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(settingScene.guid.ToString());
                var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                if (scene != null) buildSceneList.Add(scene);
            }

            buildScenes = buildSceneList.ToArray();
        }
    }
}