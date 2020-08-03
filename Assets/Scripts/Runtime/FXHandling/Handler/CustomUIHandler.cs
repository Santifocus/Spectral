using System.Collections.Generic;
using Spectral.Runtime.DataStorage.FX;
using UnityEngine;
using UnityEngine.UI;

namespace Spectral.Runtime.FX.Handling
{
	public class CustomUIHandler : FXHandler
	{
		public override TimeType UpdateStyle => TimeType.UnscaledDeltaTime;
		public override System.Type FXTargetType => typeof(CustomUI);
		private readonly List<CustomUIInstance> effectInstances = new List<CustomUIInstance>();

		public override FXInstance InitialiseFX(FXObject baseData, FXInstanceData instanceData)
		{
			CustomUIInstance newInstance = new CustomUIInstance(baseData, instanceData);
			effectInstances.Add(newInstance);

			return newInstance;
		}

		public override void HandleFX(float timeStep)
		{
			for (int i = 0; i < effectInstances.Count; i++)
			{
				effectInstances[i].Update(timeStep);
				if (effectInstances[i].WillBeDestroyed())
				{
					effectInstances.RemoveAt(i);
					i--;
				}
			}
		}

		protected override IEnumerable<FXInstance> GetFXInstances()
		{
			foreach (CustomUIInstance instance in effectInstances)
			{
				yield return instance;
			}
		}

		public override void Reset() { }

		private class CustomUIInstance : FXInstance<CustomUI>
		{
			private CustomUIInstanceData effectInstanceData;
			private float curMultiplier;

			public CustomUIInstance(FXObject baseInfo, FXInstanceData instanceData) : base(baseInfo, instanceData) { }

			protected override void OnInitiate()
			{
				base.OnInitiate();

				//Create the prefab
				GameObject mainObject = Object.Instantiate(FXData.UIPrefabObject, null);
				mainObject.transform.SetSiblingIndex(FXData.CustomChildIndex);

				//Get all GraphicComponents within the prefab
				Graphic[] containedGraphics = mainObject.GetComponentsInChildren<Graphic>();

				//Extract all base alpha values so we can use them to multiply
				float[] containedBaseAlphas = new float[containedGraphics.Length];
				for (int i = 0; i < containedBaseAlphas.Length; i++)
				{
					containedBaseAlphas[i] = containedGraphics[i].color.a;
				}

				effectInstanceData = new CustomUIInstanceData(mainObject, containedGraphics, containedBaseAlphas);

				//Update the alpha multiplier to 0
				UpdateAlphaMultiplier(0);
			}

			protected override void InternalUpdate(float timeStep)
			{
				float newMultiplier = GetCurrentMultiplier();
				if (System.Math.Abs(curMultiplier - newMultiplier) > 0.01f)
				{
					UpdateAlphaMultiplier(newMultiplier);
				}
			}

			private void UpdateAlphaMultiplier(float newMultiplier)
			{
				curMultiplier = newMultiplier;
				for (int i = 0; i < effectInstanceData.ContainedGraphics.Length; i++)
				{
					effectInstanceData.ContainedGraphics[i].color = new Color(effectInstanceData.ContainedGraphics[i].color.r, effectInstanceData.ContainedGraphics[i].color.g,
																			effectInstanceData.ContainedGraphics[i].color.b,
																			effectInstanceData.ContainedBaseAlphas[i] * newMultiplier);
				}
			}

			protected override void OnDestroy()
			{
				Object.Destroy(effectInstanceData.MainObject);
			}

			private readonly struct CustomUIInstanceData
			{
				public readonly GameObject MainObject;
				public readonly Graphic[] ContainedGraphics;
				public readonly float[] ContainedBaseAlphas;

				public CustomUIInstanceData(GameObject mainObject, Graphic[] containedGraphics, float[] containedBaseAlphas)
				{
					MainObject = mainObject;
					ContainedGraphics = containedGraphics;
					ContainedBaseAlphas = containedBaseAlphas;
				}
			}
		}
	}
}