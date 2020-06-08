using Spectral.Runtime;
using UnityEditor;
using UnityEngine;

namespace Spectral.Editor
{
	public static class EditorGameSettingsLoader
	{
		private enum FailMessageType
		{
			NoneFound = 1,
			MultipleFound = 2,
		}

		[InitializeOnLoadMethod]
		public static void LoadGameSettings()
		{
			string[] allFoundGUIDs = AssetDatabase.FindAssets("t:" + nameof(GameSettings));
			if (allFoundGUIDs.Length == 0)
			{
				PrintGameSettingsLoadFailMessage(FailMessageType.NoneFound);
			}
			else if (allFoundGUIDs.Length == 1)
			{
				GameSettings target = AssetDatabase.LoadAssetAtPath<GameSettings>(AssetDatabase.GUIDToAssetPath(allFoundGUIDs[0]));
				if (target.ChooseAsEditorReference)
				{
					GameSettings.EditorReference = target;
				}
				else
				{
					PrintGameSettingsLoadFailMessage(FailMessageType.NoneFound);
				}
			}
			else
			{
				int found = 0;
				for (int i = 0; i < allFoundGUIDs.Length; i++)
				{
					GameSettings target = AssetDatabase.LoadAssetAtPath<GameSettings>(AssetDatabase.GUIDToAssetPath(allFoundGUIDs[i]));
					if (target.ChooseAsEditorReference)
					{
						found++;
						if (!GameSettings.Current)
						{
							GameSettings.EditorReference = target;
						}
					}
				}

				if (found == 0)
				{
					PrintGameSettingsLoadFailMessage(FailMessageType.NoneFound);
				}
				else if (found > 1)
				{
					PrintGameSettingsLoadFailMessage(FailMessageType.MultipleFound);
				}
			}
		}

		private static void PrintGameSettingsLoadFailMessage(FailMessageType messageType)
		{
			switch (messageType)
			{
				case FailMessageType.NoneFound:
					Debug.LogError("Could not find any GameSettings which could be used for Editor referencing, this means either none were found at all or the ones that were found have " +
									nameof(GameSettings.ChooseAsEditorReference) + " disabled. This will cause errors for all Scripts that rely on an existing GameSetting.");

					break;
				case FailMessageType.MultipleFound:
					Debug.LogError("Found multiple GameSettings which have " + nameof(GameSettings.ChooseAsEditorReference) +
									" enabled. Therefore the first found was used instead.");

					break;
			}
		}
	}
}