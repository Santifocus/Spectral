using Spectral.Runtime.DataStorage;
using UnityEngine;

namespace Spectral.Runtime.Behaviours
{
	public class EntityBodyPart : MonoBehaviour
	{
		private const float ENTITY_PART_BODY_DRAG = 2.5f;
		public EntityBodyPartConfiguration Config { get; set; }
		public Rigidbody Body { get; set; }
		public Transform Model { get; set; }
		public Transform TurnCore { get; set; }
		public HingeJoint Joint { get; set; }

		public float CalculatedLength { get; set; }
		public int TorsoPrefabIndex { get; set; }

		public void Initialise(EntityBodyPartConfiguration config, Rigidbody jointAttachment)
		{
			Config = config;

			//Create turn core
			TurnCore = new GameObject("Body Turn Core").transform;
			TurnCore.SetParent(transform);

			//Create the model
			Model = Instantiate(config.PartPrefab, TurnCore).transform;

			//Setup components
			//Rigidbody
			Body = gameObject.AddComponent<Rigidbody>();
			Body.useGravity = false;
			Body.drag = ENTITY_PART_BODY_DRAG;

			//HingeJoint
			Joint = gameObject.AddComponent<HingeJoint>();
			Joint.axis = Vector3Int.up;
			Joint.connectedBody = jointAttachment;
		}
	}
}