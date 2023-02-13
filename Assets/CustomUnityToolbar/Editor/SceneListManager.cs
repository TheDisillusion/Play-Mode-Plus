using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CustomUnityToolbar
{
	[ExecuteAlways]
	[CreateAssetMenu(fileName = "SceneList", menuName = "Assets/SceneList")]
	public class SceneListManager : ScriptableObject
	{
		public List<SceneAsset> sceneList = new List<SceneAsset>();
	    
#if UNITY_EDITOR
		void OnEnable()
		{
			RefreshList();
		}

		private void RefreshList()
		{
			sceneList?.RemoveAll(sceneAsset => sceneAsset == null);
			Debug.Log("Test2");
			
			sceneList = new List<SceneAsset>(GetAllScenes());
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
#endif

		private void OnValidate()
		{
			RefreshList();
			Debug.Log("Test");
		}
	}
}

