using UnityEditor;
using UnityEngine;
using static Spectral.Editor.EditorUtils;

namespace Spectral.Editor
{
	[CustomEditor(typeof(ScriptableObject), true)]
	public class ObjectEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			if (!ShouldHideBaseInspector())
			{
				DrawDefaultInspector();
			}

			CustomInspector();
			if (IsDirty)
			{
				ShouldBeDirty(false);
				EditorUtility.SetDirty(target);
			}
		}

		protected virtual bool ShouldHideBaseInspector()
		{
			return false;
		}

		protected virtual void CustomInspector() { }
	}
}