using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Spectral.Runtime;
using Spectral.Runtime.Behaviours;
using Spectral.Runtime.DataStorage;
using Spectral.Runtime.DataStorage.FX;
using UnityEditor;
using UnityEngine;
using static Spectral.Editor.EditorUtils;


namespace Spectral.Editor
{
	public class ObjectCreatorWindow : EditorWindow
	{
		private const string SETTINGS_FOLDER = "Settings";
		private const string FX_FOLDER = "FX";
		private const string ENTITIES_FOLDER = "Entities";

		private static readonly Dictionary<Type, string> DefinedPathLookup = new Dictionary<Type, string>
		{
			//General
			{typeof(FXObject), $"{SETTINGS_FOLDER}/{FX_FOLDER}"},
			{typeof(EntitySettings), $"{SETTINGS_FOLDER}/{ENTITIES_FOLDER}"},
			{typeof(EntityBodyPart), $"{SETTINGS_FOLDER}/{ENTITIES_FOLDER}/BodyParts"},
			{typeof(LevelSettings), $"{SETTINGS_FOLDER}/Levels"},
		};

		private Type baseType;
		private Type[] inheritedTypes;
		private string currentTargetName = "";
		private string currentTargetPath = "";

		private void Initialise(Type baseType)
		{
			this.baseType = baseType;
			inheritedTypes = baseType.Assembly.GetTypes().Where(type => type.IsSubclassOf(baseType) || (type == baseType)).ToArray();
			Array.Sort(inheritedTypes, (a, b) => GetInheritLevel(a, baseType).CompareTo(GetInheritLevel(b, baseType)));
			currentTargetName = $"New{this.baseType.Name}";
			if (!TryGetPredictedPath())
			{
				currentTargetPath = $"Assets/{SETTINGS_FOLDER}";
			}
		}

		private bool TryGetPredictedPath()
		{
			Type inspectedType = baseType;
			while (inspectedType != null)
			{
				if (DefinedPathLookup.TryGetValue(inspectedType, out string retrievedPath))
				{
					currentTargetPath = $"{Application.dataPath}/{retrievedPath}";

					return true;
				}

				inspectedType = inspectedType.BaseType;
			}

			return false;
		}

		private static int GetInheritLevel(Type targetType, Type baseType)
		{
			int inheritLevel = 0;
			Type inspectedType = targetType;
			while ((targetType != baseType) && (inspectedType != null))
			{
				inspectedType = inspectedType.BaseType;
				inheritLevel++;
			}

			return inheritLevel;
		}

		private void OnGUI()
		{
			if (inheritedTypes == null)
			{
				if (baseType == null)
				{
					Close();

					return;
				}
				else
				{
					Initialise(baseType);
				}
			}

			bool lastWasAbstract = false;
			for (int i = 0; i < inheritedTypes.Length; i++)
			{
				bool isAbstract = inheritedTypes[i].IsAbstract;
				if (lastWasAbstract != isAbstract)
				{
					GUILayout.Space(6);
				}

				EditorGUI.BeginDisabledGroup(isAbstract);
				if (GUILayout.Button(ObjectNames.NicifyVariableName(inheritedTypes[i].Name)))
				{
					bool invalidName = string.IsNullOrEmpty(currentTargetName);
					bool invalidPath = !currentTargetPath.Contains(Application.dataPath);
					if (invalidName)
					{
						Debug.LogError("Cannot create an asset without a given name.");
						currentTargetName = "INVALID NAME";
					}

					if (invalidPath)
					{
						Debug.LogError("Invalid path given.");
						currentTargetPath = "INVALID PATH";
					}

					if (!invalidName && !invalidPath)
					{
						AssetCreator.CreateAsset(inheritedTypes[i], currentTargetName, currentTargetPath);
					}
				}

				EditorGUI.EndDisabledGroup();
				lastWasAbstract = isAbstract;
			}

			GUILayout.Space(10);
			StringField(ref currentTargetName, "Name");
			GUILayout.BeginHorizontal();
			EditorGUI.BeginDisabledGroup(true);
			BeginIndentSpaces();
			EditorGUILayout.TextField("Path", currentTargetPath.Substring(Math.Max(0, currentTargetPath.LastIndexOf("Assets"))));
			EndIndentSpaces();
			EditorGUI.EndDisabledGroup();
			if (GUILayout.Button("...", GUILayout.Width(25)))
			{
				string newPath;
				if (Directory.Exists(currentTargetPath))
				{
					newPath = EditorUtility.OpenFolderPanel("Asset Save Folder", currentTargetPath.Substring(Math.Max(0, currentTargetPath.LastIndexOf("Assets"))), "");
				}
				else
				{
					newPath = EditorUtility.OpenFolderPanel("Asset Save Folder", $"Assets/{SETTINGS_FOLDER}", "");
				}

				if (!string.IsNullOrEmpty(newPath))
				{
					currentTargetPath = newPath;
				}
			}

			GUILayout.EndHorizontal();
		}

		public static void InitialiseWindow(Type baseType)
		{
			if (!baseType.IsSubclassOf(typeof(ScriptableObject)))
			{
				throw new ArgumentException($"Cannot initiate Object Creation window with a type not inheriting from {nameof(ScriptableObject)}. (Given: {baseType.Name})");
			}

			ObjectCreatorWindow creatorWindow = GetWindow<ObjectCreatorWindow>("Object Creation", true, mouseOverWindow.GetType());
			creatorWindow.titleContent.image = EditorGUIUtility.IconContent("Toolbar Plus").image;
			creatorWindow.Initialise(baseType);
			creatorWindow.Show();
		}
	}
}