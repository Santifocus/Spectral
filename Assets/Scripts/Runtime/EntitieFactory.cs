using Spectral.Behaiviors;
using Spectral.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spectral
{
	public static class EntitieFactory
	{
		private const float ENTITIE_PART_BODY_DRAG = 2.5f;
		public static void BuildEntitie(MovingEntitie entitieCore, MovingEntitieSettings data, int torsoCount, int rotation)
		{
			Vector3 direction = new Vector3(Mathf.Cos(rotation * Mathf.Deg2Rad), 0, Mathf.Sin(rotation * Mathf.Deg2Rad));
			int totalBodyParts = torsoCount + 1 + (data.EntitieTail ? 1 : 0);

			Rigidbody currentJointAttachment;
			float lastSize;

			//Create the head
			GameObject head = Object.Instantiate(data.EntitieHead.PartPrefab);
			head.transform.SetParent(entitieCore.transform);
			head.transform.localScale *= GetCurrentScale(0);
			head.transform.forward = -direction;

			currentJointAttachment = head.AddComponent<Rigidbody>();
			currentJointAttachment.isKinematic = true;
			currentJointAttachment.useGravity = false;
			lastSize = data.EntitieHead.Lenght * GetCurrentScale(0);

			//Create torso parts
			float torsoSize = 0;
			List<GameObject> torsoParts = new List<GameObject>(torsoCount);
			for (int i = 0; i < torsoCount; i++)
			{
				torsoParts.Add(CreateBodyHingePart(data.EntitieTorso, i));
			}

			//Create the tail
			GameObject tail = data.EntitieTail ? CreateBodyHingePart(data.EntitieTail, torsoCount) : null;

			//Inform the core about its body parts
			entitieCore.SetEntitieBodyInfo(head, torsoParts, tail);

			//Local Methodes
			GameObject CreateBodyHingePart(EntitieBodyPartSettings settings, int partIndex)
			{
				GameObject bodyHingePart = Object.Instantiate(settings.PartPrefab);
				float selfLenght = settings.Lenght * GetCurrentScale(partIndex);

				//Setup the transform
				bodyHingePart.transform.SetParent(entitieCore.transform);
				float offset = ((selfLenght + lastSize) * 0.5f) + settings.PartOffset;
				bodyHingePart.transform.localPosition = (offset + torsoSize) * direction;
				bodyHingePart.transform.localScale *= GetCurrentScale(partIndex);
				bodyHingePart.transform.forward = direction;

				lastSize = selfLenght;
				torsoSize += offset;

				//Setup the hinge joint
				HingeJoint hingeJoint = bodyHingePart.AddComponent<HingeJoint>();
				hingeJoint.axis = Vector3Int.up;
				hingeJoint.connectedBody = currentJointAttachment;

				hingeJoint.useSpring = true;
				JointSpring spring = default;
				spring.spring = settings.Springiness;
				spring.damper = settings.SpringDamping;
				hingeJoint.spring = spring;

				hingeJoint.useLimits = true;
				JointLimits limits = default;
				limits.min = -settings.AngleLimiter;
				limits.max = settings.AngleLimiter;
				hingeJoint.limits = limits;

				//This will put the anchor in the middle of the offset between the last and current torso part, why it has to be divided by exactly 5.75f is unknown
				//probably some hard coded value in the Unity API that had to be neutralized from here
				hingeJoint.anchor = new Vector3(0, 0, -0.5f - (settings.PartOffset / 5.75f));

				//Setup the rigidbody, the component was already added from HingeJoint
				currentJointAttachment = bodyHingePart.GetComponent<Rigidbody>();
				currentJointAttachment.useGravity = false;
				currentJointAttachment.drag = ENTITIE_PART_BODY_DRAG;

				return bodyHingePart;
			}
			float GetCurrentScale(int partIndex)
			{
				return ((totalBodyParts - partIndex - 1) * data.ScaleChangePerPart) + data.PartMinimumScale;
			}
		}
	}
}