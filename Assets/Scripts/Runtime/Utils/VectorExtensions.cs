using System.Globalization;
using UnityEngine;

namespace Spectral.Runtime
{
	public static class VectorExtensions
	{
		//Helper Method's
		public static Vector2 ClampIntoLevelBounds(this Vector2 self, LevelSettings targetLevelSettings)
		{
			float levelWidth = targetLevelSettings.LevelWidth;
			float levelHeight = targetLevelSettings.LevelHeight;

			return new Vector2(Mathf.Clamp(self.x, -levelWidth / 2, levelWidth / 2), Mathf.Clamp(self.y, -levelHeight / 2, levelHeight / 2));
		}

		public static bool RequiresLevelBoundsClamping(this Vector2 self, LevelSettings targetLevelSettings)
		{
			float levelWidth = targetLevelSettings.LevelWidth;
			float levelHeight = targetLevelSettings.LevelHeight;

			return (self.x < (-levelWidth / 2)) || (self.x > (levelWidth / 2)) || (self.y < (-levelHeight / 2)) || (self.y > (levelHeight / 2));
		}

		public static Vector2 NormalizeFromLevelBounds(this Vector2 self, LevelSettings targetLevelSettings, bool clamped)
		{
			float levelWidth = targetLevelSettings.LevelWidth;
			float levelHeight = targetLevelSettings.LevelHeight;
			if (clamped)
			{
				return new Vector2(Mathf.Clamp(self.x / (levelWidth / 2), -1, 1), Mathf.Clamp(self.y / (levelHeight / 2), -1, 1));
			}
			else
			{
				return new Vector2(self.x / (levelWidth / 2), self.y / (levelHeight / 2));
			}
		}

		public static Vector3 XZtoXYZ(this Vector2 self)
		{
			return new Vector3(self.x, 0, self.y);
		}

		public static Vector2 XYZtoXZ(this Vector3 self)
		{
			return new Vector2(self.x, self.z);
		}

		public static Vector3 ZeroY(this Vector3 self)
		{
			return new Vector3(self.x, 0, self.z);
		}

		public static float Sum(this Vector2 self)
		{
			return self.x + self.y;
		}

		public static float Sum(this Vector3 self)
		{
			return self.x + self.y + self.z;
		}

		public static float Average(this Vector2 self)
		{
			return (self.x + self.y) / 2;
		}

		public static float Average(this Vector3 self)
		{
			return (self.x + self.y + self.z) / 3;
		}

		public static bool ShorterThan(this Vector2 self, float distance)
		{
			return self.sqrMagnitude < (distance * distance);
		}

		public static bool ShorterThan(this Vector3 self, float distance)
		{
			return self.sqrMagnitude < (distance * distance);
		}

		public static string ToLongString(this Vector2 self)
		{
			return $"({self.x.ToString(CultureInfo.InvariantCulture)}, {self.y.ToString(CultureInfo.InvariantCulture)})";
		}

		public static string ToLongString(this Vector3 self)
		{
			return "("                                            + self.x.ToString(CultureInfo.InvariantCulture) + ", " + self.y.ToString(CultureInfo.InvariantCulture) + ", " +
					self.z.ToString(CultureInfo.InvariantCulture) + ")";
		}
	}
}