using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spectral
{
	public static class SpectralUtils
	{
		//Helper methodes
		public static Vector2 ClampIntoLevelBounds(this Vector2 self)
		{
			float levelWidht = GameManager.CurrentLevelSettings.LevelWidht.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultLevelWidht);
			float levelHeight = GameManager.CurrentLevelSettings.LevelHeight.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultLevelHeight);

			return new Vector2(Mathf.Clamp(self.x, -levelWidht / 2, levelWidht / 2), Mathf.Clamp(self.y, -levelHeight / 2, levelHeight / 2));
		}
		public static bool RequiresLevelBoundsClamping(this Vector2 self)
		{
			float levelWidht = GameManager.CurrentLevelSettings.LevelWidht.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultLevelWidht);
			float levelHeight = GameManager.CurrentLevelSettings.LevelHeight.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultLevelHeight);

			return (self.x < -levelWidht / 2) || (self.x > levelWidht / 2) || (self.y < -levelHeight / 2) || (self.y > levelHeight / 2);
		}
		public static Vector2 NormalizeFromLevelBounds(this Vector2 self, bool clamped)
		{
			float levelWidht = GameManager.CurrentLevelSettings.LevelWidht.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultLevelWidht);
			float levelHeight = GameManager.CurrentLevelSettings.LevelHeight.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultLevelHeight);

			if (clamped)
			{
				return new Vector2(Mathf.Clamp(self.x / (levelWidht / 2), -1, 1), Mathf.Clamp(self.y / (levelHeight / 2), -1, 1));
			}
			else
			{
				return new Vector2(self.x / (levelWidht / 2), self.y / (levelHeight / 2));
			}
		}
		public static Vector2 XYZtoXZ(this Vector3 self)
		{
			return new Vector2(self.x, self.z);
		}
		public static Vector3 ZeroY(this Vector3 self)
		{
			return new Vector3(self.x, 0, self.z);
		}
		public static Vector3 XZtoXYZ(this Vector2 self)
		{
			return new Vector3(self.x, 0, self.y);
		}
		public static float Sum(this Vector2 self)
		{
			return self.x + self.y;
		}
		public static float Sum(this Vector3 self)
		{
			return self.x + self.y + self.z;
		}
		public static string ToLongString(this Vector2 self)
		{
			return "(" + self.x.ToString() + ", " + self.y.ToString() + ")";
		}
		public static string ToLongString(this Vector3 self)
		{
			return "(" + self.x.ToString() + ", " + self.y.ToString() + ", " + self.z.ToString() + ")";
		}
	}
}