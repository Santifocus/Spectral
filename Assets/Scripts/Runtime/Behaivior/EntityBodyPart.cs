using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spectral.DataStorage;

namespace Spectral.Behaviours
{
	public class EntityBodyPart : MonoBehaviour
	{
		private const float ENTITY_PART_BODY_DRAG = 2.5f;
		public EntityBodyPartConfiguration Config { get; set; }
		public Rigidbody Body { get; set; }
		public HingeJoint Joint { get; private set; }

		public float CalculatedLenght { get; set; }
		public int TorsoPrefabIndex { get; set; }

		public void Initialise(EntityBodyPartConfiguration Data)
		{
			this.Config = Data;
			Body = gameObject.AddComponent<Rigidbody>();
			Joint = gameObject.AddComponent<HingeJoint>();

			Body.useGravity = false;
			Body.drag = ENTITY_PART_BODY_DRAG;
		}
	}
}