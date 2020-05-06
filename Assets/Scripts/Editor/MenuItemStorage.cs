using Spectral.DataStorage;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Spectral.EditorInspector
{
	public static class MenuItemStorage
	{
		//[MenuItem("Spectral/Objects/Object")] public static void CreateObject() => AssetCreator<Object>("Settings", "Objects");

		[MenuItem("Spectral/Settings/GameSettings")]
		public static void CreateGameSettings()
		{
			AssetCreator<GameSettings>("Settings");
		}

		[MenuItem("Spectral/Settings/DefaultEntitySettings")]
		public static void CreateDefaultEntitySettings()
		{
			AssetCreator<DefaultEntitySettings>("Settings");
		}

		[MenuItem("Spectral/Settings/LevelSettings")]
		public static void CreateLevelSettings()
		{
			AssetCreator<LevelSettings>("Settings", "Levels");
		}

		//EntitySettings
		[MenuItem("Spectral/Settings/Entities/Settings")]
		public static void CreateEntitySettings()
		{
			AssetCreator<EntitySettings>("Settings", "Entities");
		}

		[MenuItem("Spectral/Settings/Entities/BodyPart")]
		public static void CreateEntityBodyPartSettings()
		{
			AssetCreator<EntityBodyPartSettings>("Settings", "Entities", "BodyParts");
		}

		#region Asset Creator
		public static T AssetCreator<T>(params string[] pathParts) where T : ScriptableObject
		{
			T asset = ScriptableObject.CreateInstance<T>();
			string name = "/New " + typeof(T).Name;
			string path = "";

			for (int i = 0; i < pathParts.Length; i++)
			{
				path += "/" + pathParts[i];
				if (!Directory.Exists(Application.dataPath + path))
				{
					Directory.CreateDirectory(Application.dataPath + path);
				}
			}
			AssetDatabase.CreateAsset(asset, "Assets" + path + name + ".asset");

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			EditorUtility.FocusProjectWindow();

			Selection.activeObject = null;
			Selection.activeObject = asset;
			EditorGUIUtility.PingObject(asset);

			Debug.Log("Created: '" + name.Substring(1) + "' at: Assets" + path + "/..");
			return asset;
		}
		#endregion
	}
}