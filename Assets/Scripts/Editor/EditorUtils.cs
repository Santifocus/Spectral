using UnityEditor;
using UnityEngine;

namespace Spectral.EditorInspector
{
	public static class EditorUtils
	{
		#region Fields
		private const int LINE_HEIGHT = 2;
		private const float STANDARD_OFFSET = 15;
		private static readonly Color StandardLineColor = new Color(0.25f, 0.25f, 0.65f, 1);

		public static int Indent { get; private set; }
		public static bool IsDirty { get; private set; }
		#endregion
		#region Standard Drawing
		public static bool FloatField(string content, ref float curValue)
		{
			BeginIndentSpaces();
			float newValue = EditorGUILayout.FloatField(content, curValue);
			EndIndentSpaces();

			if (newValue != curValue)
			{
				ShouldBeDirty();
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool FloatSliderField(string content, ref float curValue, float minValue = 0, float maxValue = 1)
		{
			BeginIndentSpaces();
			float newValue = EditorGUILayout.Slider(content, curValue, minValue, maxValue);
			EndIndentSpaces();

			if (newValue != curValue)
			{
				ShouldBeDirty();
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool IntField(string content, ref int curValue)
		{
			BeginIndentSpaces();
			int newValue = EditorGUILayout.IntField(content, curValue);
			EndIndentSpaces();

			if (newValue != curValue)
			{
				ShouldBeDirty();
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool IntSliderField(string content, ref int curValue, int minValue, int maxValue)
		{
			BeginIndentSpaces();
			int newValue = EditorGUILayout.IntSlider(content, curValue, minValue, maxValue);
			EndIndentSpaces();

			if (newValue != curValue)
			{
				ShouldBeDirty();
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool DelayedIntField(string content, ref int curValue)
		{
			BeginIndentSpaces();
			int newValue = EditorGUILayout.DelayedIntField(content, curValue);
			EndIndentSpaces();

			if (newValue != curValue)
			{
				ShouldBeDirty();
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool DoubleField(string content, ref double curValue)
		{
			BeginIndentSpaces();
			double newValue = EditorGUILayout.DoubleField(content, curValue);
			EndIndentSpaces();

			if (newValue != curValue)
			{
				ShouldBeDirty();
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool StringField(string content, ref string curValue)
		{
			BeginIndentSpaces();
			string newValue = EditorGUILayout.TextField(content, curValue);
			EndIndentSpaces();

			if (newValue != curValue)
			{
				ShouldBeDirty();
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool BoolField(string content, ref bool curValue)
		{
			BeginIndentSpaces();
			bool newValue = EditorGUILayout.Toggle(content, curValue);
			EndIndentSpaces();

			if (newValue != curValue)
			{
				ShouldBeDirty();
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool Vector2Field(string content, ref Vector2 curValue)
		{
			BeginIndentSpaces();
			Vector2 newValue = EditorGUILayout.Vector2Field(content, curValue);
			EndIndentSpaces();

			if (newValue != curValue)
			{
				ShouldBeDirty();
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool Vector3Field(string content, ref Vector3 curValue)
		{
			BeginIndentSpaces();
			Vector3 newValue = EditorGUILayout.Vector3Field(content, curValue);
			EndIndentSpaces();

			if (newValue != curValue)
			{
				ShouldBeDirty();
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool Vector4Field(string content, ref Vector4 curValue)
		{
			bool changed = false;
			Vector4 newValue = curValue;

			float preFieldWidth = EditorGUIUtility.fieldWidth;
			EditorGUIUtility.fieldWidth = STANDARD_OFFSET * 4;

			IncreaseIndent(3);
			BeginIndentSpaces();
			newValue.y = EditorGUILayout.FloatField(" ", newValue.y);
			GUILayout.FlexibleSpace();
			EndIndentSpaces();
			DecreaseIndent(3);

			BeginIndentSpaces();
			newValue.x = EditorGUILayout.FloatField(content, newValue.x);

			GUILayout.Space(STANDARD_OFFSET * 1.3f);
			newValue.z = EditorGUILayout.FloatField("", newValue.z, GUILayout.MaxWidth(EditorGUIUtility.fieldWidth));
			GUILayout.FlexibleSpace();
			EndIndentSpaces();

			IncreaseIndent(3);
			BeginIndentSpaces();
			newValue.w = EditorGUILayout.FloatField(" ", newValue.w);
			GUILayout.FlexibleSpace();
			EndIndentSpaces();

			for (int i = 0; i < 4; i++)
			{
				if (curValue[i] != newValue[i])
				{
					changed = true;
					curValue[i] = newValue[i];
				}
			}

			EditorGUIUtility.fieldWidth = preFieldWidth;

			if (changed)
			{
				ShouldBeDirty();
			}
			return changed;
		}
		public static bool ColorField(string content, ref Color curValue)
		{
			BeginIndentSpaces();
			Color newValue = EditorGUILayout.ColorField(content, curValue);
			EndIndentSpaces();

			if (newValue != curValue)
			{
				ShouldBeDirty();
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool GradientField(string content, ref Gradient curValue)
		{
			Gradient newValue = new Gradient
			{
				colorKeys = curValue.colorKeys,
				alphaKeys = curValue.alphaKeys
			};
			BeginIndentSpaces();
			newValue = EditorGUILayout.GradientField(content, newValue);
			EndIndentSpaces();

			if (!newValue.Equals(curValue))
			{
				ShouldBeDirty();
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool AnimationCurveField(string content, ref AnimationCurve curValue)
		{
			BeginIndentSpaces();
			AnimationCurve newValue = new AnimationCurve(curValue.keys);
			newValue = EditorGUILayout.CurveField(content, newValue);
			EndIndentSpaces();

			if (!newValue.Equals(curValue))
			{
				ShouldBeDirty();
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool EnumField<T>(string content, ref T curValue) where T : System.Enum
		{
			BeginIndentSpaces();
			T newValue = (T)EditorGUILayout.EnumPopup(content, curValue);
			EndIndentSpaces();

			if (newValue.GetHashCode() != curValue.GetHashCode())
			{
				ShouldBeDirty();
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool PopupField(string content, ref int curValue, string[] options)
		{
			BeginIndentSpaces();
			int newValue = EditorGUILayout.Popup(content, curValue, options);
			EndIndentSpaces();

			if (newValue != curValue)
			{
				ShouldBeDirty();
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool PopupField(string content, ref int curValue, string[] options, int[] optionValues)
		{
			GUIContent[] displayedOptions = new GUIContent[options.Length];
			for (int i = 0; i < options.Length; i++)
			{
				displayedOptions[i] = new GUIContent(options[i]);
			}
			BeginIndentSpaces();
			int newValue = EditorGUILayout.IntPopup(new GUIContent(content), curValue, displayedOptions, optionValues);
			EndIndentSpaces();

			if (newValue != curValue)
			{
				ShouldBeDirty();
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool PopupMaskField(string content, ref int curValue, string[] options)
		{
			BeginIndentSpaces();
			int newValue = EditorGUILayout.MaskField(content, curValue, options);
			EndIndentSpaces();

			if (newValue != curValue)
			{
				ShouldBeDirty();
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool EnumFlagField<T>(string content, ref T curValue) where T : System.Enum
		{
			BeginIndentSpaces();
			T newValue = (T)EditorGUILayout.EnumFlagsField(content, curValue);
			EndIndentSpaces();

			if (newValue.GetHashCode() != curValue.GetHashCode())
			{
				ShouldBeDirty();
				curValue = (T)newValue;
				return true;
			}
			return false;
		}
		public static bool UnityObjectField<T>(string content, ref T curValue) where T : Object
		{
			BeginIndentSpaces();
			T newValue = (T)EditorGUILayout.ObjectField(content, curValue, typeof(T), false);
			EndIndentSpaces();

			if (newValue != curValue)
			{
				ShouldBeDirty();
				curValue = newValue;
				return true;
			}
			return false;
		}
		#endregion
		#region Array Drawing
		public static bool DrawArray<T>(string content,
										ref T[] curValue,
										SerializedProperty arrayProperty,
										System.Action<string, T, SerializedProperty> elementBinding,
										string elementContent = null,
										bool allowTypeSpecificFieldContent = true,
										bool asHeader = false) where T : class
		{
			return DrawArray<T>(content,
								ref curValue,
								arrayProperty,
								elementBinding,
								(elementContent != null) ? (new System.Func<int, string>((int index) => elementContent)) : null,
								allowTypeSpecificFieldContent,
								asHeader);
		}
		public static bool DrawArray<T>(string content,
										ref T[] curValue,
										SerializedProperty arrayProperty,
										System.Action<string, T, SerializedProperty> elementBinding,
										System.Func<int, string> fieldContentGetter,
										bool allowTypeSpecificFieldContent = true,
										bool asHeader = false) where T : class
		{
			if (typeof(T) == typeof(Object))
			{
				Object[] objectArray = curValue as Object[];
				return DrawUnityObjectArray<Object>(content, ref objectArray, arrayProperty, fieldContentGetter, allowTypeSpecificFieldContent, asHeader);
			}
			bool changed = false;
			if (curValue == null)
			{
				curValue = new T[0];
				changed = true;
			}

			changed |= Foldout(content, arrayProperty, asHeader);

			if (arrayProperty.isExpanded)
			{
				IncreaseIndent();
				int newSize = curValue.Length;
				DelayedIntField("Size", ref newSize);

				string failedGetterContent = allowTypeSpecificFieldContent ? ObjectNames.NicifyVariableName(typeof(T).Name) : "Element";
				failedGetterContent = ". " + failedGetterContent;

				for (int i = 0; i < curValue.Length; i++)
				{
					string elementContent = fieldContentGetter?.Invoke(i) ?? failedGetterContent;
					elementBinding?.Invoke(i + elementContent, curValue[i], arrayProperty.GetArrayElementAtIndex(i));
				}

				if (curValue.Length != newSize)
				{
					changed = true;
					T[] newArray = new T[newSize];
					for (int i = 0; i < newSize; i++)
					{
						if (i < curValue.Length)
						{
							newArray[i] = curValue[i];
						}
						else
						{
							break;
						}
					}
					curValue = newArray;
					arrayProperty.arraySize = newSize;
				}
				DecreaseIndent();
			}
			if (asHeader)
			{
				EndFoldoutHeader();
			}

			if (changed)
			{
				ShouldBeDirty();
			}
			return changed;
		}
		public static bool DrawUnityObjectArray<T>(string content,
													ref T[] curValue,
													SerializedProperty arrayProperty,
													string elementContent = null,
													bool allowTypeSpecificFieldContent = true,
													bool asHeader = false) where T : Object
		{
			return DrawUnityObjectArray<T>(content,
											ref curValue,
											arrayProperty,
											(elementContent != null) ? (new System.Func<int, string>((int index) => elementContent)) : null,
											allowTypeSpecificFieldContent,
											asHeader);
		}
		public static bool DrawUnityObjectArray<T>(string content,
													ref T[] curValue,
													SerializedProperty arrayProperty,
													System.Func<int, string> fieldContentGetter,
													bool allowTypeSpecificFieldContent = true,
													bool asHeader = false) where T : Object
		{
			bool changed = false;
			if (curValue == null)
			{
				curValue = new T[0];
				changed = true;
			}
			changed |= Foldout(content, arrayProperty, asHeader);

			if (arrayProperty.isExpanded)
			{
				int newSize = curValue.Length;
				DelayedIntField("Size", ref newSize);

				string failedGetterContent = allowTypeSpecificFieldContent ? ObjectNames.NicifyVariableName(typeof(T).Name) : "Element";
				failedGetterContent = ". " + failedGetterContent;

				for (int i = 0; i < curValue.Length; i++)
				{
					string elementContent = fieldContentGetter?.Invoke(i) ?? failedGetterContent;
					changed |= UnityObjectField(i + elementContent, ref curValue[i]);
				}

				if (newSize != curValue.Length)
				{
					changed = true;
					T[] newArray = new T[newSize];
					for (int i = 0; i < newSize; i++)
					{
						if (i < curValue.Length)
						{
							newArray[i] = curValue[i];
						}
						else
						{
							break;
						}
					}

					curValue = newArray;
				}
			}

			if (asHeader)
			{
				EndFoldoutHeader();
			}

			if (changed)
			{
				ShouldBeDirty();
			}

			return changed;
		}
		#endregion
		#region Layout Drawer
		public static void BeginIndentSpaces()
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(Indent * STANDARD_OFFSET);
		}
		public static void EndIndentSpaces()
		{
			EditorGUILayout.EndHorizontal();
		}
		public static void Header(string content, bool spaces = true, bool bold = true)
		{
			if (spaces)
			{
				GUILayout.Space(8);
			}

			BeginIndentSpaces();
			if (bold)
			{
				GUILayout.Label(content, EditorStyles.boldLabel);
			}
			else
			{
				GUILayout.Label(content);
			}
			EndIndentSpaces();

			if (spaces)
			{
				GUILayout.Space(4);
			}
		}
		public static void LineBreak(Color? lineColor = null, bool spaces = true)
		{
			if (spaces)
			{
				GUILayout.Space(3);
			}

			Rect rect = EditorGUILayout.GetControlRect(false, LINE_HEIGHT);
			rect.height = LINE_HEIGHT;
			rect.x /= 2;
			EditorGUI.DrawRect(rect, lineColor ?? StandardLineColor);

			if (spaces)
			{
				GUILayout.Space(3);
			}
		}
		public static bool Foldout(string content, SerializedProperty property, bool asHeader = false)
		{
			bool curOpen = property.isExpanded;
			if (Foldout(content, ref curOpen, asHeader))
			{
				property.isExpanded = curOpen;
				return true;
			}
			return false;
		}
		public static bool Foldout(string content, ref bool curValue, bool asHeader = false)
		{
			bool newValue;
			if (asHeader)
			{
				newValue = EditorGUILayout.BeginFoldoutHeaderGroup(curValue, content);
			}
			else
			{
				BeginIndentSpaces();
				newValue = EditorGUILayout.Foldout(curValue, content, true);
				EndIndentSpaces();
			}

			if (newValue != curValue)
			{
				ShouldBeDirty();
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool DrawInFoldoutHeader(string content, SerializedProperty property, System.Action drawFunction)
		{
			bool curOpen = property.isExpanded;
			if (DrawInFoldoutHeader(content, ref curOpen, drawFunction))
			{
				property.isExpanded = curOpen;
				return true;
			}
			return false;
		}
		public static bool DrawInFoldoutHeader(string content, ref bool curValue, System.Action drawFunction)
		{
			bool changed = Foldout(content, ref curValue, true);
			if (curValue)
			{
				drawFunction?.Invoke();
			}
			EndFoldoutHeader();
			return changed;
		}
		public static bool DrawButton(string content, System.Action activateFunction)
		{
			if (GUILayout.Button(content))
			{
				activateFunction?.Invoke();
				return true;
			}
			return false;
		}
		public static void EndFoldoutHeader()
		{
			EditorGUILayout.EndFoldoutHeaderGroup();
		}
		#endregion
		#region Utils
		public static void IncreaseIndent(int amount = 1)
		{
			Indent += amount;
		}

		public static void DecreaseIndent(int amount = 1)
		{
			Indent -= amount;
		}

		public static void ShouldBeDirty(bool state = true)
		{
			IsDirty = state;
		}
		#endregion
	}
}