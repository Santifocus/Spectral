using System;
using UnityEngine;

namespace Spectral.Runtime.Behaviours
{
	public class EntityFeedbackPlayer : MonoBehaviour
	{
		[SerializeField] private EffectInformation eatEffect = default;
		[SerializeField] private EffectInformation damageEffect = default;

		private int eatEffectNameID;
		private int damageEffectNameID;

		private bool playingEatEffect;
		private bool playingDamageEffect;

		private float eatEffectTimer;
		private float damageEffectTimer;

		private void Start()
		{
			eatEffectNameID = Shader.PropertyToID(eatEffect.EffectName);
			damageEffectNameID = Shader.PropertyToID(damageEffect.EffectName);
		}

		private void Update()
		{
			UpdateEatEffect();
			UpdateDamageEffect();
		}

		public void PlayEatEffect()
		{
			if (eatEffect.ShouldPlayEffect)
			{
				InitialiseEatEffect();
			}
		}

		public void PlayDamageEffect()
		{
			if (damageEffect.ShouldPlayEffect)
			{
				InitialiseDamageEffect();
			}
		}

		private void InitialiseEatEffect()
		{
			playingEatEffect = true;
			eatEffectTimer = 0;
			SetEatEffectValue(eatEffect.EffectValueMin);
		}

		private void InitialiseDamageEffect()
		{
			playingDamageEffect = true;
			damageEffectTimer = 0;
			SetDamageEffectValue(damageEffect.EffectValueMin);
		}

		private void UpdateEatEffect()
		{
			if (!playingEatEffect)
			{
				return;
			}

			eatEffectTimer += Time.deltaTime;
			float lerpPoint = Mathf.Sin((eatEffectTimer / eatEffect.EffectTime) * Mathf.PI);
			SetEatEffectValue(Mathf.Lerp(eatEffect.EffectValueMin, eatEffect.EffectValueMax, lerpPoint));
			if (eatEffectTimer > eatEffect.EffectTime)
			{
				FinaliseEatEffect();
			}
		}

		private void UpdateDamageEffect()
		{
			if (!playingDamageEffect)
			{
				return;
			}

			damageEffectTimer += Time.deltaTime;
			float lerpPoint = Mathf.Sin((damageEffectTimer / damageEffect.EffectTime) * Mathf.PI);
			SetDamageEffectValue(Mathf.Lerp(damageEffect.EffectValueMin, damageEffect.EffectValueMax, lerpPoint));
			if (damageEffectTimer > damageEffect.EffectTime)
			{
				FinaliseDamageEffect();
			}
		}

		private void FinaliseEatEffect()
		{
			playingEatEffect = false;
			SetEatEffectValue(eatEffect.EffectValueMin);
		}

		private void FinaliseDamageEffect()
		{
			playingDamageEffect = false;
			SetDamageEffectValue(damageEffect.EffectValueMin);
		}

		private void SetEatEffectValue(float value)
		{
			eatEffect.EffectRenderer.material.SetFloat(eatEffectNameID, value);
		}

		private void SetDamageEffectValue(float value)
		{
			damageEffect.EffectRenderer.material.SetFloat(damageEffectNameID, value);
		}


		[Serializable]
		private class EffectInformation
		{
			public Renderer EffectRenderer = default;
			public bool ShouldPlayEffect = default;
			public string EffectName = default;
			public float EffectTime = 2;
			public float EffectValueMin = 0;
			public float EffectValueMax = 1;
		}
	}
}