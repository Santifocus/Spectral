using UnityEngine;

namespace Spectral.Runtime.DataStorage.FX
{
	public class ScreenShake : FXObject
	{
		public float AxisIntensity = 0.125f;
		public float AngleIntensity = 0.05f;
		public Vector3 CustomAxisMultiplier = Vector3.one;
		public Vector3 CustomAngleMultiplier = Vector3.one;
	}
}