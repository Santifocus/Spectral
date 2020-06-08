using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Spectral.Runtime;
using UnityEditor;
using UnityEngine;

namespace Spectral.Editor
{
	public class ScriptCreationKeywordReplacer : UnityEditor.AssetModificationProcessor
	{
		public static void OnWillCreateAsset(string path)
		{
			path = path.Replace(".meta", "");
			int index = path.LastIndexOf(".");
			if (index == -1)
			{
				return;
			}

			string fileType = path.Substring(index);
			if (fileType != ".cs")
			{
				return;
			}

			index = Application.dataPath.LastIndexOf("Assets");
			string fullPath = Application.dataPath.Substring(0, index) + path;
			fileType = System.IO.File.ReadAllText(fullPath);
			fileType = fileType.Replace("#NAMESPACE#", BuildNameSpace(path));
			fileType = fileType.Replace("#PROJECTNAMESPACES#", AssemblyFullyQualifiedNamespaces());
			System.IO.File.WriteAllText(path, fileType);
			AssetDatabase.Refresh();
		}

		private static string BuildNameSpace(string path)
		{
			path = path.Substring(0, path.LastIndexOf("/"));
			path = path.Replace("Assets/Scripts", EditorSettings.projectGenerationRootNamespace);
			path = path.Replace("/", ".");

			return path;
		}

		private static string AssemblyFullyQualifiedNamespaces()
		{
			Type[] assemblyTypes = Assembly.GetAssembly(typeof(StaticData)).GetTypes();
			string[] namespaceNames = assemblyTypes
									.Select(type => type.Namespace)
									.Distinct()
									.Where(namespaceName => !string.IsNullOrEmpty(namespaceName))
									.ToArray();

			Array.Sort(namespaceNames);
			StringBuilder fullString = new StringBuilder();
			for (int i = 0; i < namespaceNames.Length; i++)
			{
				fullString.Append($"using {namespaceNames[i]};\n");
			}

			return fullString.ToString();
		}
	}
}