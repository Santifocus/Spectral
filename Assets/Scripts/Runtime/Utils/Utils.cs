using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Spectral.Runtime
{
	public enum TimeType
	{
		ScaledDeltaTime = 1,
		UnscaledDeltaTime = 2,
		FixedDeltaTime = 3,
	}

	public static class Utils
	{
		public static bool ShiftBetweenLists<T>(T target, List<T> lhd, List<T> rhd)
		{
			if (lhd.Remove(target))
			{
				rhd.Add(target);

				return true;
			}
			else if (rhd.Remove(target))
			{
				lhd.Add(target);

				return true;
			}

			return false;
		}

		public static string ReplaceBetween(this string input, char firstMarking, char secondMarking, string replaceWith, bool removeMarkings = true)
		{
			for (int i = 0; i < input.Length; i++)
			{
				if (input[i] != firstMarking)
				{
					continue;
				}

				for (int j = i + 1; j < input.Length; j++)
				{
					if (input[j] != secondMarking)
					{
						continue;
					}

					int substringStart = i        + (removeMarkings ? 0 : 1);
					int substringLength = (j - i) + (removeMarkings ? 1 : -1);
					input = input.Replace(input.Substring(substringStart, substringLength), replaceWith);
					i = j + (replaceWith.Length - substringLength);

					break;
				}
			}

			return input;
		}

		public static float GetTimeValue(this TimeType timeType)
		{
			return timeType == TimeType.ScaledDeltaTime ? Time.deltaTime : timeType == TimeType.FixedDeltaTime ? Time.fixedDeltaTime : Time.unscaledDeltaTime;
		}

		public static string ToRichTextColorTag(this Color color)
		{
			return "<color=#" + ColorUtility.ToHtmlStringRGBA(color) + ">";
		}

		public static string EnumerableString(this IEnumerable enumerable)
		{
			StringBuilder totalString = enumerable is ICollection collection ? new StringBuilder(collection.Count) : new StringBuilder();
			foreach (object o in enumerable)
			{
				totalString.Append(o);
			}

			return totalString.ToString();
		}
	}
}