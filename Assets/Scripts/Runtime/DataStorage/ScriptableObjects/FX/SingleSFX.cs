using Spectral.Sounds;
using UnityEngine;

namespace Spectral.Runtime.DataStorage.FX
{
	public class SingleSFX : FXObject
	{
		public Sound TargetSound = null;
		public bool FollowTarget = false;
		public Vector3 OffsetToTarget = Vector3.zero;
	}
}