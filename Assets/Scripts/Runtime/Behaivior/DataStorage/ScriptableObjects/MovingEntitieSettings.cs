using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spectral.Data
{
	public class MovingEntitieSettings : SpectralScriptableObject
	{
		public float MoveSpeed = 15;

		public float Acceleration = 10;
		public float Deceleration = 20;
		public float AccelerationAngle = 90;

		public float TurnSpeed = 280;
		public float TurnAcceleration = 0.5f;
		public float TurnSmoothAngle = 40;
		public float TurnSmoothMultiplier = 0.025f;

		[Space(10)]
		[Header("Body Settings")]
		public int MinParts = 3;
		public float PartMinimumScale = 0.8f;
		public float ScaleChangePerPart = 0.1f;
		public EntitieBodyPartSettings EntitieHead;
		public EntitieBodyPartSettings EntitieTorso;
		public EntitieBodyPartSettings EntitieTail;
	}
}