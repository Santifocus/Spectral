using Spectral.Behaviours.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spectral.DataStorage
{
	public enum ChooseStyle { Random = 1, InOrder = 2 }
	public class EntitySettings : SpectralScriptableObject
	{
		//Movement Settings
		public DefaultableFloat MoveSpeed = new DefaultableFloat(null);
		public DefaultableFloat VelocityDamping = new DefaultableFloat(null);

		public DefaultableFloat Acceleration = new DefaultableFloat(null);
		public DefaultableFloat Deceleration = new DefaultableFloat(null);
		public DefaultableFloat AccelerationAngle = new DefaultableFloat(null);

		public DefaultableFloat TurnSpeed = new DefaultableFloat(null);
		public DefaultableFloat TurnAcceleration = new DefaultableFloat(null);
		public DefaultableFloat TurnSmoothAngle = new DefaultableFloat(null);
		public DefaultableFloat TurnSmoothMultiplier = new DefaultableFloat(null);

		//Body Settings
		public int MinParts = 1;

		public DefaultableFloat PartMinimumScale = new DefaultableFloat(null);
		public DefaultableFloat ScaleChangePerPart = new DefaultableFloat(null);

		public EntityBodyPartSettings EntityHead;
		public ChooseStyle TorsoChooseStyle = ChooseStyle.Random;
		public EntityBodyPartSettings[] EntityTorso;
		public EntityBodyPartSettings EntityTail;

		//Other
		public EntityMover OverwritePrefab = default;
		public bool EnableAI = false;
		public EntityAIConfiguration AIConfiguration;
	}
}