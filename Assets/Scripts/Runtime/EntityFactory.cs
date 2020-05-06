using Spectral.Behaviours;
using Spectral.Behaviours.Entities;
using Spectral.DataStorage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spectral
{
	public static class EntityFactory
	{
		private const float ENTITY_PART_BODY_DRAG = 2.5f;
		public static void CreateEntity(EntitySettings data, int bodyPartCount, Vector2 position, int rotation)
		{
			EntityMover entityCore = null;
			if (data.OverwritePrefab)
			{
				entityCore = Object.Instantiate(data.OverwritePrefab, position.XZtoXYZ(), Quaternion.identity, Storage.EntityStorage);
				if (data.EnableAI && !(entityCore is AIMover))
				{
					throw new System.Exception("The Entitiy Factory was given the instructions to spawn an Entitie which has AIEnabled active but has an Overwrite-Prefab set which is not of the type AIMover.");
				}
			}
			else
			{
				GameObject entityObjectCore = new GameObject("Spawned Entity");
				entityObjectCore.transform.SetParent(Storage.EntityStorage);
				if (data.EnableAI)
				{
					entityCore = entityObjectCore.AddComponent<AIMover>();
				}
				else
				{
					entityCore = entityObjectCore.AddComponent<EntityMover>();
				}
				entityCore.transform.position = position.XZtoXYZ();
			}

			BuildEntityBody(entityCore, data, bodyPartCount, rotation);
		}
		public static void BuildEntityBody(EntityMover entityCore, EntitySettings data, int bodyPartCount, int rotation)
		{
			Vector3 direction = new Vector3(Mathf.Cos(rotation * Mathf.Deg2Rad), 0, Mathf.Sin(rotation * Mathf.Deg2Rad));
			int totalBodyParts = bodyPartCount + 1 + (data.EntityTail ? 1 : 0);

			float minimumPartScale = data.PartMinimumScale.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.EntityPartMinimumScale);
			float scaleChangePerPart = data.ScaleChangePerPart.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.EntityScaleChangePerPart);

			Rigidbody currentJointAttachment;
			float lastSize;

			//Create the head
			GameObject head = Object.Instantiate(data.EntityHead.PartPrefab, entityCore.transform);
			float headScale = GetCurrentScale(0);
			head.transform.localScale *= headScale;
			head.transform.forward = -direction;

			currentJointAttachment = head.AddComponent<Rigidbody>();
			currentJointAttachment.isKinematic = true;
			currentJointAttachment.useGravity = false;
			lastSize = data.EntityHead.Lenght * headScale;

			//Add the bodypart data
			EntityBodyPart headPart = head.AddComponent<EntityBodyPart>();

			headPart.Data = data.EntityHead;
			headPart.CalculatedLenght = lastSize;

			//Create torso parts
			float torsoSize = 0;
			List<GameObject> torsoParts = new List<GameObject>(bodyPartCount);
			GameObject tail = null;

			if (((bodyPartCount > 1) || (bodyPartCount == 1 && data.EntityTail == null)) && data.EntityTorso.Length == 0)
			{
				entityCore.gameObject.SetActive(false);
#if SPECTRAL_DEBUG
				throw new System.Exception("The Entitiy Factory was given the instructions to spawn an Entitie which has no set Torso Parts but was instructed to spawn it with more then one Torso... Disabling");
#else
				return;
#endif
			}

			for (int i = 0; i < bodyPartCount; i++)
			{
				if (i < (bodyPartCount - 1) || data.EntityTail == null)
				{
					int choosenIndex = data.TorsoChooseStyle == ChooseStyle.Random ? Random.Range(0, data.EntityTorso.Length) : i % data.EntityTorso.Length;
					torsoParts.Add(CreateBodyHingePart(data.EntityTorso[choosenIndex], i + 1));
				}
				else
				{
					//Create the tail
					tail = CreateBodyHingePart(data.EntityTail, i + 1);
				}
			}

			//Inform the core about its body parts
			entityCore.Head = head;
			entityCore.TorsoParts = torsoParts;
			entityCore.Tail = tail;
			entityCore.SetupEntity(data);

			//Local Methodes
			GameObject CreateBodyHingePart(EntityBodyPartSettings settings, int partIndex)
			{
				GameObject bodyHingePart = Object.Instantiate(settings.PartPrefab, entityCore.transform);
				float selfLenght = settings.Lenght * GetCurrentScale(partIndex);

				//Setup the transform
				float offset = ((selfLenght + lastSize) * 0.5f) + settings.PartOffset;
				bodyHingePart.transform.localPosition = (offset + torsoSize) * direction;
				bodyHingePart.transform.localScale *= GetCurrentScale(partIndex);
				bodyHingePart.transform.forward = -direction;

				lastSize = selfLenght;
				torsoSize += offset;

				//Setup the hinge joint
				HingeJoint hingeJoint = bodyHingePart.AddComponent<HingeJoint>();
				EntityBodyPart bodyPart = bodyHingePart.AddComponent<EntityBodyPart>();

				bodyPart.Data = settings;
				bodyPart.CalculatedLenght = selfLenght;

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

				//Setup the anchors
				hingeJoint.autoConfigureConnectedAnchor = false;
				hingeJoint.anchor = new Vector3(0, 0, 0.5f + (settings.PartOffset / 5.75f));
				hingeJoint.connectedAnchor = -bodyHingePart.transform.InverseTransformPoint(currentJointAttachment.transform.position) * hingeJoint.anchor.z;

				//Setup the rigidbody which was already added from HingeJoint
				currentJointAttachment = bodyHingePart.GetComponent<Rigidbody>();
				currentJointAttachment.useGravity = false;
				currentJointAttachment.drag = ENTITY_PART_BODY_DRAG;

				return bodyHingePart;
			}
			float GetCurrentScale(int partIndex)
			{
				return ((totalBodyParts - partIndex - 1) * scaleChangePerPart) + minimumPartScale;
			}
		}
		public static void IncreaseEntitySize(EntityMover entityCore, EntitySettings data)
		{

		}
		public static void DecreaseEntitySize(EntityMover entityCore, EntitySettings data)
		{
			int currentEntitySize = GetEntitySize(entityCore);
			if ((currentEntitySize - 1) < data.MinParts || (currentEntitySize == 1))
			{
				entityCore.Death();
				return;
			}
			Vector3 offsetDirection;
			if ((currentEntitySize > 2) || !entityCore.Tail)
			{
				offsetDirection = (entityCore.Head.transform.position - entityCore.TorsoParts[0].transform.position).normalized;
				Object.Destroy(entityCore.TorsoParts[0]);
				entityCore.TorsoParts.RemoveAt(0);
				if (currentEntitySize <= 2)
				{
					return;
				}
			}
			else
			{
				Object.Destroy(entityCore.Tail);
				return;
			}
			currentEntitySize--;

			GameObject targetPart = (currentEntitySize > 2) || (!entityCore.Tail) ? entityCore.TorsoParts[0] : entityCore.Tail;

			//Update the Head part
			EntityBodyPart headPartData = entityCore.Head.GetComponent<EntityBodyPart>();
			float newHeadScale = ((currentEntitySize - 1) 
				* data.ScaleChangePerPart.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.EntityScaleChangePerPart)) 
				+ data.PartMinimumScale.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.EntityPartMinimumScale);

			entityCore.Head.transform.localScale = headPartData.Data.PartPrefab.transform.localScale * newHeadScale;
			headPartData.CalculatedLenght = headPartData.Data.Lenght * newHeadScale;

			//Update the body part offset
			EntityBodyPart bodyPartData = targetPart.GetComponent<EntityBodyPart>();
			float newOffset = ((bodyPartData.CalculatedLenght + headPartData.CalculatedLenght) * 0.5f) + bodyPartData.Data.PartOffset;
			targetPart.transform.localPosition = entityCore.Head.transform.localPosition + (newOffset * offsetDirection * -1);

			//Update the hinge joint of the body part
			HingeJoint hingeJoint = targetPart.GetComponent<HingeJoint>();
			hingeJoint.connectedBody = entityCore.Head.GetComponent<Rigidbody>();

			float angleDelta = Mathf.DeltaAngle(entityCore.Head.transform.localEulerAngles.y, targetPart.transform.localEulerAngles.y);

			JointSpring spring = hingeJoint.spring;
			spring.targetPosition = -angleDelta;
			hingeJoint.spring = spring;

			JointLimits limits = hingeJoint.limits;
			limits.min = -bodyPartData.Data.AngleLimiter - angleDelta;
			limits.max = bodyPartData.Data.AngleLimiter - angleDelta;
			hingeJoint.limits = limits;
		}
		private static int GetEntitySize(EntityMover entityCore)
		{
			int totalSize = entityCore.Head ? 1 : 0;
			totalSize += entityCore.TorsoParts.Count;
			totalSize += entityCore.Tail ? 1 : 0;
			return totalSize;
		}
	}
}