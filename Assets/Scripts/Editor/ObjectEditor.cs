using UnityEditor;
using static Spectral.Editor.EditorUtils;

namespace Spectral.Editor
{
	public abstract class ObjectEditor : UnityEditor.Editor
	{
		protected abstract bool ShouldHideBaseInspector { get; }
		private bool initialised;
		protected virtual void OnInitialize() { }

		public sealed override void OnInspectorGUI()
		{
			//If this is the first frame of being selected call the initialise method
			if (!initialised)
			{
				initialised = true;
				OnInitialize();
			}

			//Reset the indent from last frame to 0
			DecreaseIndent(Indent);

			//Draw default inspector if requested
			if (!ShouldHideBaseInspector)
			{
				DrawDefaultInspector();
			}

			//Draw the custom inspector
			CustomInspector();

			//Check for changes
			if (IsDirty)
			{
				ShouldBeDirty(false);
				EditorUtility.SetDirty(target);
			}
		}

		protected abstract void CustomInspector();
	}
}