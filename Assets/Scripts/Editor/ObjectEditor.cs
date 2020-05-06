using UnityEditor;
using UnityEngine;
using static Spectral.EditorInspector.EditorUtils;

namespace Spectral.EditorInspector
{
	[CustomEditor(typeof(ScriptableObject), true)]
	public class ObjectEditor : Editor
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