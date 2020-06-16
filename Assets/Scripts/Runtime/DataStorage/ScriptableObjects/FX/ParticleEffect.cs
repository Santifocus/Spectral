using UnityEngine;

namespace Spectral.Runtime.DataStorage.FX
{
	public class ParticleEffect : FXObject
	{
		public GameObject ParticleMainObject;
		public Vector3 OffsetToTarget = Vector3.zero;
		public Vector3 RotationOffset = Vector3.zero;
		public bool FollowTarget;
		public bool InheritRotationOfTarget;
		public float RotationInheritLerpSpeed = 4;
	}
}