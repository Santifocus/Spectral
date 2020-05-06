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
		public static bool FloatField(string label, ref float curValue)
		{
			BeginIndentSpaces();
			float newValue = EditorGUILayout.FloatField(label, curValue);
			EndIndentSpaces();

			if (newValue != curValue)
			{
				ShouldBeDirty();
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool FloatSliderField(string label, ref float curValue, float minValue = 0, float maxValue = 1)
		{
			BeginIndentSpaces();
			float newValue = EditorGUILayout.Slider(label, curValue, minValue, maxValue);
			EndIndentSpaces();

			if (newValue != curValue)
			{
				ShouldBeDirty();
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool IntField(string label, ref int curValue)
		{
			BeginIndentSpaces();
			int newValue = EditorGUILayout.IntField(label, curValue);
			EndIndentSpaces();

			if (newValue != curValue)
			{
				ShouldBeDirty();
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool IntSliderField(string label, ref int curValue, int minValue, int maxValue)
		{
			BeginIndentSpaces();
			int newValue = EditorGUILayout.IntSlider(label, curValue, minValue, maxValue);
			EndIndentSpaces();

			if (newValue != curValue)
			{
				ShouldBeDirty();
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool DelayedIntField(string label, ref int curValue)
		{
			BeginIndentSpaces();
			int newValue = EditorGUILayout.DelayedIntField(label, curValue);
			EndIndentSpaces();

			if (newValue != curValue)
			{
				ShouldBeDirty();
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool DoubleField(string label, ref double curValue)
		{
			BeginIndentSpaces();
			double newValue = EditorGUILayout.DoubleField(label, curValue);
			EndIndentSpaces();

			if (newValue != curValue)
			{
				ShouldBeDirty();
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool StringField(string label, ref string curValue)
		{
			BeginIndentSpaces();
			string newValue = EditorGUILayout.TextField(label, curValue);
			EndIndentSpaces();

			if (newValue != curValue)
			{
				ShouldBeDirty();
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool BoolField(string label, ref bool curValue)
		{
			BeginIndentSpaces();
			bool newValue = EditorGUILayout.Toggle(label, curValue);
			EndIndentSpaces();

			if (newValue != curValue)
			{
				ShouldBeDirty();
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool Vector2Field(string label, ref Vector2 curValue)
		{
			BeginIndentSpaces();
			Vector2 newValue = EditorGUILayout.Vector2Field(label, curValue);
			EndIndentSpaces();

			if (newValue != curValue)
			{
				ShouldBeDirty();
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool Vector3Field(string label, ref Vector3 curValue)
		{
			BeginIndentSpaces();
			Vector3 newValue = EditorGUILayout.Vector3Field(label, curValue);
			EndIndentSpaces();

			if (newValue != curValue)
			{
				ShouldBeDirty();
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool Vector4Field(string label, ref Vector4 curValue)
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
			newValue.x = EditorGUILayout.FloatField(label, newValue.x);

			GUILayout.Space(STANDARD_OFFSET * 1.3f);
			newValue.z = EditorGUILayout.FloatField("", newValue.z, GUILayout.MaxWidth(EditorGUIUtility.fieldWidth));
			GUILayout.FlexibleSpace();
			EndIndentSpaces();

			IncreaseIndent(3);
			BeginIndentSpaces();
			newValue.w = EditorGUILayout.FloatField(" ", newValue.w);
			GUILayout.FlexibleSpace();
			EndIndentSpaces();
			DecreaseIndent(3);

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
		public static bool ColorField(string label, ref Color curValue)
		{
			BeginIndentSpaces();
			Color newValue = EditorGUILayout.ColorField(label, curValue);
			EndIndentSpaces();

			if (newValue != curValue)
			{
				ShouldBeDirty();
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool GradientField(string label, ref Gradient curValue)
		{
			Gradient newValue = new Gradient
			{
				colorKeys = curValue.colorKeys,
				alphaKeys = curValue.alphaKeys
			};
			BeginIndentSpaces();
			newValue = EditorGUILayout.GradientField(label, newValue);
			EndIndentSpaces();

			if (!newValue.Equals(curValue))
			{
				ShouldBeDirty();
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool AnimationCurveField(string label, ref AnimationCurve curValue)
		{
			BeginIndentSpaces();
			AnimationCurve newValue = new AnimationCurve(curValue.keys);
			newValue = EditorGUILayout.CurveField(label, newValue);
			EndIndentSpaces();

			if (!newValue.Equals(curValue))
			{
				ShouldBeDirty();
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool EnumField<T>(string label, ref T curValue) where T : System.Enum
		{
			BeginIndentSpaces();
			T newValue = (T)EditorGUILayout.EnumPopup(label, curValue);
			EndIndentSpaces();

			if (newValue.GetHashCode() != curValue.GetHashCode())
			{
				ShouldBeDirty();
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool PopupField(string label, ref int curValue, string[] options)
		{
			BeginIndentSpaces();
			int newValue = EditorGUILayout.Popup(label, curValue, options);
			EndIndentSpaces();

			if (newValue != curValue)
			{
				ShouldBeDirty();
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool PopupField(string label, ref int curValue, string[] options, int[] optionValues)
		{
			GUIContent[] displayedOptions = new GUIContent[options.Length];
			for (int i = 0; i < options.Length; i++)
			{
				displayedOptions[i] = new GUIContent(options[i]);
			}
			BeginIndentSpaces();
			int newValue = EditorGUILayout.IntPopup(new GUIContent(label), curValue, displayedOptions, optionValues);
			EndIndentSpaces();

			if (newValue != curValue)
			{
				ShouldBeDirty();
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool PopupMaskField(string label, ref int curValue, string[] options)
		{
			BeginIndentSpaces();
			int newValue = EditorGUILayout.MaskField(label, curValue, options);
			EndIndentSpaces();

			if (newValue != curValue)
			{
				ShouldBeDirty();
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool EnumFlagField<T>(string label, ref T curValue) where T : System.Enum
		{
			BeginIndentSpaces();
			T newValue = (T)EditorGUILayout.EnumFlagsField(label, curValue);
			EndIndentSpaces();

			if (newValue.GetHashCode() != curValue.GetHashCode())
			{
				ShouldBeDirty();
				curValue = (T)newValue;
				return true;
			}
			return false;
		}
		public static bool UnityObjectField<T>(string label, ref T curValue) where T : Object
		{
			BeginIndentSpaces();
			T newValue = (T)EditorGUILayout.ObjectField(label, curValue, typeof(T), false);
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
		public static bool DrawArray<T>(string label,
										ref T[] curValue,
										SerializedProperty arrayProperty,
										System.Action<string, T, SerializedProperty> drawerFunction,
										string elementlabel = null,
										bool allowTypeSpecificFieldlabel = true,
										bool asHeader = false) where T : class
		{
			return DrawArray<T>(label,
								ref curValue,
								arrayProperty,
								drawerFunction,
								(elementlabel != null) ? (new System.Func<int, string>((int index) => elementlabel)) : null,
								allowTypeSpecificFieldlabel,
								asHeader);
		}
		public static bool DrawArray<T>(string label,
										ref T[] curValue,
										SerializedProperty arrayProperty,
										System.Action<string, T, SerializedProperty> drawerFunction,
										System.Func<int, string> fieldlabelGetter,
										bool allowTypeSpecificFieldlabel = true,
										bool asHeader = false) where T : class
		{
			if (typeof(T) == typeof(Object))
			{
				Object[] objectArray = curValue as Object[];
				return DrawUnityObjectArray<Object>(label, ref objectArray, arrayProperty, fieldlabelGetter, allowTypeSpecificFieldlabel, asHeader);
			}
			bool changed = false;
			if (curValue == null)
			{
				curValue = new T[0];
				changed = true;
			}

			changed |= Foldout(label, arrayProperty, asHeader);

			if (arrayProperty.isExpanded)
			{
				IncreaseIndent();
				int newSize = curValue.Length;
				DelayedIntField("Size", ref newSize);

				string failedGetterlabel = allowTypeSpecificFieldlabel ? ObjectNames.NicifyVariableName(typeof(T).Name) : "Element";
				failedGetterlabel = ". " + failedGetterlabel;

				for (int i = 0; i < curValue.Length; i++)
				{
					string elementlabel = fieldlabelGetter?.Invoke(i) ?? failedGetterlabel;
					drawerFunction?.Invoke(i + elementlabel, curValue[i], arrayProperty.GetArrayElementAtIndex(i));
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
		public static bool DrawArray<T>(string label,
										ref T[] curValue,
										SerializedProperty arrayProperty,
										System.Func<string, T, SerializedProperty, T> drawerFunction,
										string elementlabel = null,
										bool allowTypeSpecificFieldlabel = true,
										bool asHeader = false) where T : struct
		{
			return DrawArray<T>(label,
								ref curValue,
								arrayProperty,
								drawerFunction,
								(elementlabel != null) ? (new System.Func<int, string>((int index) => elementlabel)) : null,
								allowTypeSpecificFieldlabel,
								asHeader);
		}
		public static bool DrawArray<T>(string label,
										ref T[] curValue,
										SerializedProperty arrayProperty,
										System.Func<string, T, SerializedProperty, T> drawerFunction,
										System.Func<int, string> fieldlabelGetter,
										bool allowTypeSpecificFieldlabel = true,
										bool asHeader = false) where T : struct
		{
			if (typeof(T) == typeof(Object))
			{
				Object[] objectArray = curValue as Object[];
				return DrawUnityObjectArray<Object>(label, ref objectArray, arrayProperty, fieldlabelGetter, allowTypeSpecificFieldlabel, asHeader);
			}
			bool changed = false;
			if (curValue == null)
			{
				curValue = new T[0];
				changed = true;
			}

			changed |= Foldout(label, arrayProperty, asHeader);

			if (arrayProperty.isExpanded)
			{
				IncreaseIndent();
				int newSize = curValue.Length;
				DelayedIntField("Size", ref newSize);

				string failedGetterlabel = allowTypeSpecificFieldlabel ? ObjectNames.NicifyVariableName(typeof(T).Name) : "Element";
				failedGetterlabel = ". " + failedGetterlabel;

				for (int i = 0; i < curValue.Length; i++)
				{
					string elementlabel = fieldlabelGetter?.Invoke(i) ?? failedGetterlabel;
					curValue[i] = drawerFunction?.Invoke(i + elementlabel, curValue[i], arrayProperty.GetArrayElementAtIndex(i)) ?? default;
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
		public static bool DrawUnityObjectArray<T>(string label,
													ref T[] curValue,
													SerializedProperty arrayProperty,
													string elementlabel = null,
													bool allowTypeSpecificFieldlabel = true,
													bool asHeader = false) where T : Object
		{
			return DrawUnityObjectArray<T>(label,
											ref curValue,
											arrayProperty,
											(elementlabel != null) ? (new System.Func<int, string>((int index) => elementlabel)) : null,
											allowTypeSpecificFieldlabel,
											asHeader);
		}
		public static bool DrawUnityObjectArray<T>(string label,
													ref T[] curValue,
													SerializedProperty arrayProperty,
													System.Func<int, string> fieldlabelGetter,
													bool allowTypeSpecificFieldlabel = true,
													bool asHeader = false) where T : Object
		{
			bool changed = false;
			if (curValue == null)
			{
				curValue = new T[0];
				changed = true;
			}
			changed |= Foldout(label, arrayProperty, asHeader);

			if (arrayProperty.isExpanded)
			{
				IncreaseIndent();
				int newSize = curValue.Length;
				DelayedIntField("Size", ref newSize);

				string failedGetterlabel = allowTypeSpecificFieldlabel ? ObjectNames.NicifyVariableName(typeof(T).Name) : "Element";
				failedGetterlabel = ". " + failedGetterlabel;

				for (int i = 0; i < curValue.Length; i++)
				{
					string elementlabel = fieldlabelGetter?.Invoke(i) ?? failedGetterlabel;
					changed |= UnityObjectField(i + elementlabel, ref curValue[i]);
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
		public static void Header(string label, bool spaces = true, bool bold = true)
		{
			if (spaces)
			{
				GUILayout.Space(8);
			}

			BeginIndentSpaces();
			if (bold)
			{
				GUILayout.Label(label, EditorStyles.boldLabel);
			}
			else
			{
				GUILayout.Label(label);
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
		public static bool Foldout(string label, SerializedProperty property, bool asHeader = false)
		{
			bool curOpen = property.isExpanded;
			if (Foldout(label, ref curOpen, asHeader))
			{
				property.isExpanded = curOpen;
				return true;
			}
			return false;
		}
		public static bool Foldout(string label, ref bool curValue, bool asHeader = false)
		{
			bool newValue;
			if (asHeader)
			{
				newValue = EditorGUILayout.BeginFoldoutHeaderGroup(curValue, label);
			}
			else
			{
				BeginIndentSpaces();
				newValue = EditorGUILayout.Foldout(curValue, label, true);
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
		public static bool DrawInFoldoutHeader(string label, SerializedProperty property, System.Action drawFunction)
		{
			bool curOpen = property.isExpanded;
			if (DrawInFoldoutHeader(label, ref curOpen, drawFunction))
			{
				property.isExpanded = curOpen;
				return true;
			}
			return false;
		}
		public static bool DrawInFoldoutHeader(string label, ref bool curValue, System.Action drawFunction)
		{
			bool changed = Foldout(label, ref curValue, true);
			if (curValue)
			{
				drawFunction?.Invoke();
			}
			EndFoldoutHeader();
			return changed;
		}
		public static bool DrawButtonWithFunction(string label, System.Action activateFunction)
		{
			if (GUILayout.Button(label))
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