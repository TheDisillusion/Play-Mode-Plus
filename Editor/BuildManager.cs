using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

namespace PlayModePlus.Editor
{
    public class BuildManager
    {
        public Preset SelectedBuildPreset;
        public Preset[] PlayerSettingsPresetsInProject;

        public void OpenBuildWindow()
        {
            var buildPlayerWindow = EditorWindow.GetWindow<BuildPlayerWindow>("Build Settings");
            buildPlayerWindow.Show();
        }

        public static void ApplyPreset(Preset preset)
        {
            var projectSettings = Resources.FindObjectsOfTypeAll<PlayerSettings>();

            foreach (var settings in projectSettings)
            {
                preset.ApplyTo(settings);
            }
        }

        public List<string> GenerateBuildSettingsList()
        {
            PlayerSettingsPresetsInProject = AssetDatabase.FindAssets("t:Preset")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => path.Contains("P_"))
                .Select(AssetDatabase.LoadAssetAtPath<Preset>)
                .ToArray();

            var buildPresetsList = new List<string>(PlayerSettingsPresetsInProject.Select(preset => preset.name));

            return buildPresetsList;
        }
    }
}