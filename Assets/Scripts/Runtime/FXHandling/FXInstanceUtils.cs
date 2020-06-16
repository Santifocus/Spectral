using System;
using System.Collections.Generic;
using Spectral.Runtime.DataStorage.FX;
using UnityEngine;

namespace Spectral.Runtime.FX.Handling
{
	public static class FXInstanceUtils
	{
		#region Initiation

		private static readonly Dictionary<Type, FXHandler> AffiliatedHandler = new Dictionary<Type, FXHandler>();

		public static void AddHandler(FXHandler handler)
		{
			AffiliatedHandler.Add(handler.FXTargetType, handler);
		}

		#endregion

		#region FX Instance Creation Input

		/// <summary>
		///     Non-allocating Execution of a FXObjectData Array.
		/// </summary>
		public static void ExecuteFX(IEnumerable<FXObjectData> effectDataObjects, Transform parent, bool fromCombat, float multiplier = 1)
		{
			if (effectDataObjects == null)
			{
				return;
			}

			foreach (FXObjectData effectDataObject in effectDataObjects)
			{
				ExecuteFX(effectDataObject, parent, fromCombat, multiplier);
			}
		}

		/// <summary>
		///     Returns an array of FXInstances that were created via the given FXObjectData Array.
		/// </summary>
		public static void ExecuteFX(FXObjectData[] effectDataObjects, out FXInstance[] createdEffects, Transform parent, bool fromCombat, float multiplier = 1)
		{
			if (effectDataObjects == null)
			{
				createdEffects = new FXInstance[0];

				return;
			}

			createdEffects = new FXInstance[effectDataObjects.Length];
			for (int i = 0; i < effectDataObjects.Length; i++)
			{
				createdEffects[i] = ExecuteFX(effectDataObjects[i], parent, fromCombat, multiplier);
			}
		}

		/// <summary>
		///     Allocates an array of FXInstances based on the given FXObjectData array.
		///     Allows the List to be null.
		/// </summary>
		public static void ExecuteFX(FXObjectData[] effectDataObjects, ref List<FXInstance> referencedInstanceList, Transform parent, bool fromCombat, float multiplier = 1)
		{
			if (effectDataObjects == null)
			{
				return;
			}

			if (referencedInstanceList == null)
			{
				referencedInstanceList = new List<FXInstance>(effectDataObjects.Length);
			}

			//Make sure the list does not have to resize while adding the new instances
			referencedInstanceList.Capacity = Math.Max(referencedInstanceList.Capacity, referencedInstanceList.Count + effectDataObjects.Length);
			for (int i = 0; i < effectDataObjects.Length; i++)
			{
				referencedInstanceList.Add(ExecuteFX(effectDataObjects[i], parent, fromCombat, multiplier));
			}
		}

		#endregion

		#region FX Creation Core

		private static FXInstance ExecuteFX(FXObjectData effectData, Transform parent, bool fromCombat, float multiplier)
		{
			float? initiationDelay = HasFlag(effectData.EnabledOverwrites, FXDataOverwrite.InitiationDelay) ? (float?) effectData.InitiationDelay : null;
			Vector3? customPositionOffset = HasFlag(effectData.EnabledOverwrites, FXDataOverwrite.Position) ? (Vector3?) effectData.PositionOffset : null;
			Vector3? customRotationOffset = HasFlag(effectData.EnabledOverwrites, FXDataOverwrite.Rotation) ? (Vector3?) effectData.RotationOffset : null;
			Vector3? customScale = HasFlag(effectData.EnabledOverwrites, FXDataOverwrite.Scale) ? (Vector3?) effectData.NewScale : null;
			multiplier = effectData.MultiplierClamps.Clamped(multiplier);

			return AffiliatedHandler[effectData.BaseFX.GetType()].InitiateFX(effectData.BaseFX,
																			new FXInstanceData(parent, fromCombat, multiplier, initiationDelay, customPositionOffset,
																								customRotationOffset, customScale));
		}

		private static bool HasFlag(FXDataOverwrite mask, FXDataOverwrite flag)
		{
			return (flag | mask) == mask;
		}

		#endregion

		#region FX Finish

		/// <summary>
		///     Will request all instances in the IEnumerable to finish their executions.
		/// </summary>
		public static void FinishFX(this IEnumerable<FXInstance> toFinish)
		{
			foreach (FXInstance instance in toFinish)
			{
				instance.RequestFinishFX();
			}
		}

		#endregion
	}
}