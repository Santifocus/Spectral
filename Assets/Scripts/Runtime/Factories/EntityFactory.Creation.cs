using System.Collections.Generic;
using Spectral.Runtime.Behaviours;
using Spectral.Runtime.Behaviours.Entities;
using Spectral.Runtime.DataStorage;
using UnityEngine;

namespace Spectral.Runtime.Factories
{
	public static partial class EntityFactory
	{
		public static EntityMover CreateEntity(EntitySettings settings, int bodyPartCount, Vector2 position, int levelIndex, int rotation = default)
		{
			//Setup core object
			Transform entityObjectCore = new GameObject("Entity").transform;
			entityObjectCore.SetParent(LevelLoader.GameLevelPlanes[levelIndex].CoreObject.TargetStorage.EntityStorage);
			entityObjectCore.localPosition = position.XZtoXYZ();

			//Add entityCore
			EntityMover entityCore = settings.EnableAI ? entityObjectCore.gameObject.AddComponent<AIMover>() : entityObjectCore.gameObject.AddComponent<EntityMover>();
			BuildEntityBody(entityCore, settings, bodyPartCount, rotation);

			return entityCore;
		}

		public static void CreatePlayerEntity()
		{
			GameObject playerObjectCore = new GameObject("Player");
			playerObjectCore.transform.SetParent(LevelLoader.CoreStorage.EntityStorage);
			PlayerMover playerCore = playerObjectCore.AddComponent<PlayerMover>();
			playerCore.transform.position = Vector3.zero;
			BuildEntityBody(playerCore, LevelLoaderSettings.Current.PlayerSettings, LevelLoaderSettings.Current.PlayerSettings.SpawnPartCount, Random.Range(0, 360));
		}

		public static void BuildEntityBody(EntityMover entityCore, EntitySettings settings, int bodyPartCount, int rotation)
		{
			float scaleChangePerPart = settings.ScaleChangePerPart;
			float startScale = settings.PartMinimumScale;

			//Create the head
			GameObject headObject = new GameObject("Entity Head");
			GameObject turnCore = new GameObject("Head Turn Core");
			turnCore.transform.SetParent(headObject.transform);
			headObject.transform.SetParent(entityCore.transform);
			float headScale = GetCurrentScale(bodyPartCount, 0, scaleChangePerPart, startScale);
			headObject.transform.position = entityCore.transform.position;
			headObject.transform.localScale = Vector3.one * headScale;
			headObject.transform.forward = new Vector3(Mathf.Cos(rotation * Mathf.Deg2Rad), 0, Mathf.Sin(rotation * Mathf.Deg2Rad));

			//Setup Rigidbody
			Rigidbody currentJointAttachment = headObject.AddComponent<Rigidbody>();
			currentJointAttachment.isKinematic = true;
			currentJointAttachment.useGravity = false;
			float lastSize = settings.EntityHead.Lenght * headScale;

			//Add the EntityBodyPart
			entityCore.Head = headObject.AddComponent<EntityBodyPart>();
			entityCore.Head.Config = settings.EntityHead;
			entityCore.Head.Body = currentJointAttachment;
			entityCore.Head.Model = Object.Instantiate(settings.EntityHead.PartPrefab, turnCore.transform).transform;
			entityCore.Head.TurnCore = turnCore.transform;
			entityCore.Head.CalculatedLength = lastSize;

			//Create torso parts
			List<EntityBodyPart> torsoParts = new List<EntityBodyPart>(bodyPartCount + 1 + (settings.EntityTail ? 0 : 1));
			if (((bodyPartCount > 2) || ((bodyPartCount == 2) && (settings.EntityTail == null))) && (settings.EntityTorso.Length == 0))
			{
#if SPECTRAL_DEBUG
				entityCore.gameObject.SetActive(false);

				throw new
					System.Exception("The Entity Factory was given the instructions to spawn an Entity which has no set Torso Parts but was instructed to spawn it with more then one Torso... Disabling");
#else
				bodyPartCount = 0;
#endif
			}

			for (int i = 1; i < bodyPartCount; i++)
			{
				if ((i < (bodyPartCount - 1)) || (settings.EntityTail == null))
				{
					int chosenIndex = settings.TorsoChooseStyle == ChooseStyle.Random ? Random.Range(0, settings.EntityTorso.Length) : i % settings.EntityTorso.Length;
					EntityBodyPart newTorsoPart = CreateEntityBodyPart(entityCore,
																		settings.EntityTorso[chosenIndex],
																		GetCurrentScale(bodyPartCount, i, scaleChangePerPart, startScale),
																		ref currentJointAttachment,
																		ref lastSize);

					newTorsoPart.TorsoPrefabIndex = chosenIndex;
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
			entityCore.Initialise(settings);
		}

		public static void IncreaseEntitySize(EntityMover entityCore, EntitySettings settings)
		{
			int currentEntitySize = GetTotalEntityBodyPartCount(entityCore);
			float scaleChangePerPart = settings.ScaleChangePerPart;
			float startScale = settings.PartMinimumScale;

			//Update the scale of the head and all minus one body part
			int toResize = currentEntitySize - (entityCore.Tail ? 1 : 0);
			EntityBodyPart currentBodyPart = null;
			Rigidbody jointAttachment = null;
			float lastSize = 0;
			for (int i = 0; i < toResize; i++)
			{
				float scale = GetCurrentScale(currentEntitySize + 1, i, scaleChangePerPart, startScale);
				currentBodyPart = GetBodyPartFromIndex(entityCore, i);
				currentBodyPart.transform.localScale = Vector3.one               * scale;
				currentBodyPart.CalculatedLength = currentBodyPart.Config.Lenght * scale;
				if (i > 0)
				{
					float offset = ((currentBodyPart.CalculatedLength + lastSize) * 0.5f) + currentBodyPart.Config.PartOffset;
					currentBodyPart.Joint.anchor = new Vector3(0, 0, (offset / scale) * 0.5f);

					//currentBodyPart.transform.localPosition = jointAttachment.transform.localPosition + (jointAttachment.transform.forward * (offset * -0.5f) + currentBodyPart.transform.forward * (offset * -0.5f));
				}

				lastSize = currentBodyPart.CalculatedLength;
				jointAttachment = currentBodyPart.Body;
			}

			//Create the new BodyPart
			if ((currentEntitySize == 1) && settings.EntityTail)
			{
				entityCore.Tail = CreateEntityBodyPart(entityCore, settings.EntityTail, GetCurrentScale(currentEntitySize + 1, toResize, scaleChangePerPart, startScale),
														ref jointAttachment, ref lastSize);

				return;
			}
			else
			{
				int chosenIndex = settings.TorsoChooseStyle == ChooseStyle.Random ? Random.Range(0, settings.EntityTorso.Length) :
								currentEntitySize           > 1                   ? (currentBodyPart.TorsoPrefabIndex + 1) % settings.EntityTorso.Length : 0;

				EntityBodyPart newBodyPart = CreateEntityBodyPart(entityCore, settings.EntityTorso[chosenIndex],
																GetCurrentScale(currentEntitySize + 1, toResize, scaleChangePerPart, startScale), ref jointAttachment,
																ref lastSize);

				newBodyPart.TorsoPrefabIndex = chosenIndex;
				entityCore.TorsoParts.Add(newBodyPart);
				if (!settings.EntityTail)
				{
					return;
				}
			}

			//If we are here it means the entity had a tail before adding a new body part
			//Therefore we have to update its position & joints
			ReAttachJoint(entityCore.Tail,
						GetBodyPartFromIndex(entityCore, currentEntitySize - 1),
						(entityCore.Tail.transform.position - GetBodyPartFromIndex(entityCore, currentEntitySize - 2).transform.position).normalized);
		}

		public static void DecreaseEntitySize(EntityMover entityCore, EntitySettings settings)
		{
			int currentEntitySize = GetTotalEntityBodyPartCount(entityCore);
			if (((currentEntitySize - 1) < settings.MinParts) || (currentEntitySize == 1))
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
			float newHeadScale = ((currentEntitySize - 1) * settings.ScaleChangePerPart) + settings.PartMinimumScale;
			entityCore.Head.transform.localScale = Vector3.one               * newHeadScale;
			entityCore.Head.CalculatedLength = entityCore.Head.Config.Lenght * newHeadScale;

			//Update the body part offset
			ReAttachJoint(GetBodyPartFromIndex(entityCore, 1), entityCore.Head, offsetDirection);
		}

		private static EntityBodyPart CreateEntityBodyPart(EntityMover entityCore, EntityBodyPartConfiguration config, float scale, ref Rigidbody jointAttachment,
															ref float lastSize)
		{
			GameObject bodyObject = new GameObject("Body Part");
			EntityBodyPart bodyPart = bodyObject.AddComponent<EntityBodyPart>();
			bodyObject.transform.SetParent(entityCore.transform);
			float selfLength = config.Lenght * scale;

			//Setup the EntityBodyPart
			bodyPart.Initialise(config, jointAttachment);
			bodyPart.CalculatedLength = selfLength;
			SetupSpringAndLimits(bodyPart.Joint, config);

			//Setup BodyPart transform
			Vector3 forward = jointAttachment.transform.forward;
			float offset = ((selfLength + lastSize) * 0.5f) + config.PartOffset;
			bodyObject.transform.localPosition = jointAttachment.transform.localPosition - (offset * forward);
			bodyObject.transform.localScale = Vector3.one * scale;
			bodyObject.transform.forward = forward;

			//Setup the joint anchors
			bodyPart.Joint.autoConfigureConnectedAnchor = false;

			//bodyPart.Joint.anchor = new Vector3(0, 0, 0.5f + (config.PartOffset / 5.75f));
			bodyPart.Joint.anchor = new Vector3(0, 0, (offset / scale) * 0.5f);
			bodyPart.Joint.connectedAnchor = (-bodyObject.transform.InverseTransformPoint(jointAttachment.transform.position) * bodyPart.Joint.anchor.z).ZeroY();

			//Update the reference values
			jointAttachment = bodyPart.Body;
			lastSize = selfLength;

			return bodyPart;
		}

		private static void ReAttachJoint(EntityBodyPart targetPart, EntityBodyPart newParentPart, Vector3 offsetDirection)
		{
			//Offset locally
			float newOffset = ((targetPart.CalculatedLength + newParentPart.CalculatedLength) * 0.5f) + targetPart.Config.PartOffset;
			targetPart.transform.localPosition = newParentPart.transform.localPosition + (newOffset * offsetDirection);

			//Update the hinge joint of the body part
			targetPart.Joint.connectedBody = newParentPart.Body;
			RebuildSpringAndLimits(targetPart, newParentPart.transform);
		}

		public static int GetTotalEntityBodyPartCount(EntityMover entityCore)
		{
			int totalSize = entityCore.Head ? 1 : 0;
			totalSize += entityCore.TorsoParts.Count;
			totalSize += entityCore.Tail ? 1 : 0;

			return totalSize;
		}

		public static EntityBodyPart GetBodyPartFromIndex(EntityMover entityCore, int index)
		{
			if (index == 0)
			{
				return entityCore.Head;
			}

			index--;
			if ((index >= 0) && (index <= entityCore.TorsoParts.Count))
			{
				return index < entityCore.TorsoParts.Count ? entityCore.TorsoParts[index] : entityCore.Tail;
			}

#if SPECTRAL_DEBUG
			throw new System.IndexOutOfRangeException("The given body part index was not able to be parsed to a EntityBodyPart because it was out of Range. Given: " + (index + 1) +
													", Max Possible index: " + (GetTotalEntityBodyPartCount(entityCore) - 1) + ".");
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
			limits.max = targetPart.Config.AngleLimiter  - angleDelta;
			targetPart.Joint.limits = limits;
		}

		private static float GetCurrentScale(int totalBodyParts, int partIndex, in float scaleChangePerPart, in float startScale)
		{
			return ((totalBodyParts - partIndex - 1) * scaleChangePerPart) + startScale;
		}
	}
}