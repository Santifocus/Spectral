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
		private const string BODY_PART_NAME = "Body Part";

		public static void CreateEntity(EntitySettings settings, int bodyPartCount, Vector2 position, int rotation)
		{
			EntityMover entityCore;
			if (settings.OverwritePrefab)
			{
				entityCore = Object.Instantiate(settings.OverwritePrefab, position.XZtoXYZ(), Quaternion.identity, Storage.EntityStorage);
#if SPECTRAL_DEBUG
				if (settings.EnableAI && !(entityCore is AIMover))
				{
					throw new System.Exception("The Entitiy Factory was given the instructions to spawn an Entitie which has AIEnabled active but has an Overwrite-Prefab set which is not of the type AIMover.");
				}
#endif
			}
			else
			{
				GameObject entityObjectCore = new GameObject("Spawned Entity");
				entityObjectCore.transform.SetParent(Storage.EntityStorage);
				if (settings.EnableAI)
				{
					entityCore = entityObjectCore.AddComponent<AIMover>();
				}
				else
				{
					entityCore = entityObjectCore.AddComponent<EntityMover>();
				}
				entityCore.transform.position = position.XZtoXYZ();
			}

			BuildEntityBody(entityCore, settings, bodyPartCount, rotation);
		}
		public static void BuildEntityBody(EntityMover entityCore, EntitySettings settings, int bodyPartCount, int rotation)
		{
			float scaleChangePerPart = settings.ScaleChangePerPart.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.EntityScaleChangePerPart);
			float startScale = settings.PartMinimumScale.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.EntityPartMinimumScale);

			Rigidbody currentJointAttachment;
			float lastSize;

			//Create the head
			GameObject headObject = Object.Instantiate(settings.EntityHead.PartPrefab, entityCore.transform);
			float headScale = GetCurrentScale(bodyPartCount, 0, scaleChangePerPart, startScale);
			headObject.transform.localScale = Vector3.one * headScale;
			headObject.transform.forward = new Vector3(Mathf.Cos(rotation * Mathf.Deg2Rad), 0, Mathf.Sin(rotation * Mathf.Deg2Rad));

			currentJointAttachment = headObject.AddComponent<Rigidbody>();
			currentJointAttachment.isKinematic = true;
			currentJointAttachment.useGravity = false;
			lastSize = settings.EntityHead.Lenght * headScale;

			//Add the bodypart data
			entityCore.Head = headObject.AddComponent<EntityBodyPart>();

			entityCore.Head.Config = settings.EntityHead;
			entityCore.Head.Body = currentJointAttachment;
			entityCore.Head.CalculatedLenght = lastSize;

			//Create torso parts
			List<EntityBodyPart> torsoParts = new List<EntityBodyPart>(bodyPartCount + 1 + (settings.EntityTail ? 0 : 1));

			if (((bodyPartCount > 2) || (bodyPartCount == 2 && settings.EntityTail == null)) && settings.EntityTorso.Length == 0)
			{
#if SPECTRAL_DEBUG
				entityCore.gameObject.SetActive(false);
				throw new System.Exception("The Entitiy Factory was given the instructions to spawn an Entitie which has no set Torso Parts but was instructed to spawn it with more then one Torso... Disabling");
#else
				bodyPartCount = 0;
#endif
			}

			for (int i = 1; i < bodyPartCount; i++)
			{
				if (i < (bodyPartCount - 1) || settings.EntityTail == null)
				{
					int choosenIndex = (settings.TorsoChooseStyle == ChooseStyle.Random) ? Random.Range(0, settings.EntityTorso.Length) : i % settings.EntityTorso.Length;
					EntityBodyPart newTorsoPart = CreateEntityBodyPart(entityCore, 
																		settings.EntityTorso[choosenIndex], 
																		GetCurrentScale(bodyPartCount, i, scaleChangePerPart, startScale), 
																		ref currentJointAttachment, 
																		ref lastSize);
					newTorsoPart.TorsoPrefabIndex = choosenIndex;
					torsoParts.Add(newTorsoPart);
				}
				else
				{
					//Create the tail
					entityCore.Tail = CreateEntityBodyPart(entityCore,
															settings.EntityTail,
															GetCurrentScale(bodyPartCount, i, scaleChangePerPart, startScale),
															ref currentJointAttachment,
															ref lastSize);
				}
			}

			//Setup the core
			entityCore.TorsoParts = torsoParts;
			entityCore.SetupEntity(settings);
		}
		public static void IncreaseEntitySize(EntityMover entityCore, EntitySettings settings)
		{
			int currentEntitySize = GetTotalEntityBodyPartCount(entityCore);

			float scaleChangePerPart = settings.ScaleChangePerPart.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.EntityScaleChangePerPart);
			float startScale = settings.PartMinimumScale.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.EntityPartMinimumScale);

			//Update the scale of the head and all minus one bodyparts
			int toResize = currentEntitySize - (entityCore.Tail ? 1 : 0);

			EntityBodyPart currentBodyPart = null;
			Rigidbody jointAttachment = null;
			float lastSize = 0;

			for (int i = 0; i < toResize; i++)
			{
				float scale = GetCurrentScale(currentEntitySize + 1, i, scaleChangePerPart, startScale);
				currentBodyPart = GetBodyPartFromIndex(entityCore, i);

				currentBodyPart.transform.localScale = Vector3.one * scale;
				currentBodyPart.CalculatedLenght = scale * currentBodyPart.Config.Lenght;

				if (i > 0 && false)
				{
					float offset = ((currentBodyPart.CalculatedLenght + lastSize) * 0.5f) + currentBodyPart.Config.PartOffset;
					currentBodyPart.Joint.connectedAnchor = -currentBodyPart.transform.InverseTransformPoint(jointAttachment.transform.position) * currentBodyPart.Joint.anchor.z;
					currentBodyPart.transform.localPosition = jointAttachment.transform.localPosition - (offset * jointAttachment.transform.forward);
				}
				lastSize = currentBodyPart.CalculatedLenght;
				jointAttachment = currentBodyPart.Body;
			}

			//Create the new BodyPart
			if(currentEntitySize == 1 && settings.EntityTail)
			{
				entityCore.Tail = CreateEntityBodyPart(entityCore, settings.EntityTail, GetCurrentScale(currentEntitySize + 1, toResize, scaleChangePerPart, startScale), ref jointAttachment, ref lastSize);
				return;
			}
			else
			{
				int choosenIndex = (settings.TorsoChooseStyle == ChooseStyle.Random) ? Random.Range(0, settings.EntityTorso.Length) : (currentEntitySize > 1 ? ((currentBodyPart.TorsoPrefabIndex + 1) % settings.EntityTorso.Length) : 0);
				EntityBodyPart newBodyPart = CreateEntityBodyPart(entityCore, settings.EntityTorso[choosenIndex], GetCurrentScale(currentEntitySize + 1, toResize, scaleChangePerPart, startScale), ref jointAttachment, ref lastSize);
				newBodyPart.TorsoPrefabIndex = choosenIndex;
				entityCore.TorsoParts.Add(newBodyPart);

				if (!settings.EntityTail)
				{
					return;
				}
			}
			//If we are here it means the entity had a tail before adding a new body part
			//Therefore we have to update its position & joints
			ReAttachJoint(entityCore.Tail, GetBodyPartFromIndex(entityCore, currentEntitySize - 1), (entityCore.Tail.transform.position - GetBodyPartFromIndex(entityCore, currentEntitySize - 2).transform.position).normalized);
		}
		public static void DecreaseEntitySize(EntityMover entityCore, EntitySettings settings)
		{
			int currentEntitySize = GetTotalEntityBodyPartCount(entityCore);
			if ((currentEntitySize - 1) < settings.MinParts || (currentEntitySize == 1))
			{
				entityCore.Death();
				return;
			}
			Vector3 offsetDirection;
			if ((currentEntitySize > 2) || !entityCore.Tail)
			{
				offsetDirection = (entityCore.TorsoParts[0].transform.position - entityCore.Head.transform.position).normalized;
				Object.Destroy(entityCore.TorsoParts[0].gameObject);
				entityCore.TorsoParts.RemoveAt(0);
				if (currentEntitySize <= 2)
				{
					return;
				}
			}
			else
			{
				Object.Destroy(entityCore.Tail.gameObject);
				return;
			}
			currentEntitySize--;

			//Update the Head part
			float newHeadScale = ((currentEntitySize - 1) 
									* settings.ScaleChangePerPart.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.EntityScaleChangePerPart)) 
									+ settings.PartMinimumScale.GetDefaultedValue(GameManager.CurrentGameSettings.DefaultEntitySettings.EntityPartMinimumScale);

			entityCore.Head.transform.localScale = Vector3.one * newHeadScale;
			entityCore.Head.CalculatedLenght = entityCore.Head.Config.Lenght * newHeadScale;

			//Update the body part offset
			ReAttachJoint(GetBodyPartFromIndex(entityCore, 1), entityCore.Head, offsetDirection);
		}
		private static EntityBodyPart CreateEntityBodyPart(EntityMover entityCore, EntityBodyPartConfiguration config, float scale, ref Rigidbody jointAttachment, ref float lastSize)
		{
			GameObject bodyObject = new GameObject(BODY_PART_NAME);
			bodyObject.transform.SetParent(entityCore.transform);
			Object.Instantiate(config.PartPrefab, bodyObject.transform);

			float selfLenght = config.Lenght * scale;

			//Setup the transform
			Vector3 forward = jointAttachment.transform.forward;
			float offset = ((selfLenght + lastSize) * 0.5f) + config.PartOffset;
			bodyObject.transform.localPosition = jointAttachment.transform.localPosition - (offset * forward);
			bodyObject.transform.localScale = Vector3.one * scale;
			bodyObject.transform.forward = forward;

			lastSize = selfLenght;

			//Setup the hinge joint
			EntityBodyPart bodyPart = bodyObject.AddComponent<EntityBodyPart>();
			bodyPart.Initialise(config);

			bodyPart.CalculatedLenght = selfLenght;

			bodyPart.Joint.axis = Vector3Int.up;
			bodyPart.Joint.connectedBody = jointAttachment;

			SetupSpringAndLimits(bodyPart.Joint, config);

			//Setup the anchors
			bodyPart.Joint.autoConfigureConnectedAnchor = false;
			bodyPart.Joint.anchor = new Vector3(0, 0, 0.5f + (config.PartOffset / 5.75f));
			bodyPart.Joint.connectedAnchor = (-bodyObject.transform.InverseTransformPoint(jointAttachment.transform.position) * bodyPart.Joint.anchor.z).ZeroY();

			jointAttachment = bodyPart.Body;

			return bodyPart;
		}
		private static void ReAttachJoint(EntityBodyPart targetPart, EntityBodyPart newParentPart, Vector3 offsetDirection)
		{
			//Offset locally
			float newOffset = ((targetPart.CalculatedLenght + newParentPart.CalculatedLenght) * 0.5f) + targetPart.Config.PartOffset;
			targetPart.transform.localPosition = newParentPart.transform.localPosition + (newOffset * offsetDirection);

			//Update the hinge joint of the body part
			targetPart.Joint.connectedBody = newParentPart.Body;
			RebuildSpringAndLimits(targetPart, newParentPart.transform);
		}
		private static int GetTotalEntityBodyPartCount(EntityMover entityCore)
		{
			int totalSize = entityCore.Head ? 1 : 0;
			totalSize += entityCore.TorsoParts.Count;
			totalSize += entityCore.Tail ? 1 : 0;
			return totalSize;
		}
		private static EntityBodyPart GetBodyPartFromIndex(EntityMover entityCore, int index)
		{
			if(index == 0)
			{
				return entityCore.Head;
			}

			index--;
			if ((index >= 0) && (index <= entityCore.TorsoParts.Count))
			{
				return index < entityCore.TorsoParts.Count ? entityCore.TorsoParts[index] : entityCore.Tail;
			}

#if SPECTRAL_DEBUG
			throw new System.IndexOutOfRangeException("The given body part index was not able to be parsed to a EntityBodyPart because it was out of Range. Given: " + index + ", Max Possible index: " + (1 + entityCore.TorsoParts.Count + (entityCore.Tail ? 1 : 0)) + ".");
#else
			return null;
#endif
		}
		private static void SetupSpringAndLimits(HingeJoint hingeJoint, EntityBodyPartConfiguration settings)
		{
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
		}
		private static void RebuildSpringAndLimits(EntityBodyPart targetPart, Transform parent)
		{
			float angleDelta = Mathf.DeltaAngle(parent.localEulerAngles.y, targetPart.transform.localEulerAngles.y);

			JointSpring spring = targetPart.Joint.spring;
			spring.targetPosition = -angleDelta;
			targetPart.Joint.spring = spring;

			JointLimits limits = targetPart.Joint.limits;
			limits.min = -targetPart.Config.AngleLimiter - angleDelta;
			limits.max = targetPart.Config.AngleLimiter - angleDelta;
			targetPart.Joint.limits = limits;
		}
		private static float GetCurrentScale(int totalBodyParts, int partIndex, in float scaleChangePerPart, in float startScale)
		{
			return ((totalBodyParts - partIndex - 1) * scaleChangePerPart) + startScale;
		}
	}
}