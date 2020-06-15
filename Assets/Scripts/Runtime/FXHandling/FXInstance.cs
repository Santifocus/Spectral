using Spectral.Runtime.DataStorage.FX;

namespace Spectral.Runtime.FX.Handling
{
	public enum FXState
	{
		Starting = 1,
		Staying = 2,
		Ending = 3,
	}

	public abstract class FXInstance
	{
		//Public readonly fields
		public readonly FXObject BaseInfo;
		public readonly FXInstanceData InstanceData;

		//Public readonly properties, private setter
		public bool ShouldBeDestroyed { get; private set; }
		public bool IsDestroyed { get; private set; }

		//Protected Properties
		protected FXState CurrentFXState { get; private set; }
		protected bool IsDisabled => !active || IsDestroyed;
		protected bool AllowBaseUpdate = true;

		//Private Fields
		private bool active;
		private float passedTime;
		private float stateMultiplier;

		protected FXInstance(FXObject baseInfo, FXInstanceData instanceData)
		{
			BaseInfo = baseInfo;
			InstanceData = instanceData;
			if (instanceData.InitiationDelay.HasValue)
			{
				passedTime = -instanceData.InitiationDelay.Value;
			}
			else
			{
				Initialise();
			}
		}

		private void Initialise()
		{
			passedTime = 0;
			CurrentFXState = FXState.Starting;
			SetActiveState(true);
			OnInitiate();
		}

		protected abstract void OnInitiate();

		public void SetActiveState(bool state)
		{
			active = state;
			OnSetActiveState(state);
		}

		public void Update(float timeStep)
		{
			if (passedTime < 0)
			{
				passedTime += timeStep;
				if (passedTime >= 0)
				{
					Initialise();
				}

				return;
			}

			if (IsDisabled)
			{
				return;
			}

			if (AllowBaseUpdate)
			{
				if (!ShouldBeDestroyed)
				{
					passedTime += timeStep;
				}

				if (CheckForFinishCondition())
				{
					RequestFinishFX();
				}

				UpdateCurrentState();
				UpdateMultiplier();
			}

			InternalUpdate(timeStep);
		}

		private bool CheckForFinishCondition()
		{
			if (CurrentFXState != FXState.Ending)
			{
				if (BaseInfo.FinishConditions.OnParentDeath && (!InstanceData.Parent || !InstanceData.Parent.gameObject.activeSelf))
				{
					return true;
				}

				if (BaseInfo.FinishConditions.OnConditionMet && BaseInfo.FinishConditions.ConditionMet())
				{
					return true;
				}
			}

			return false;
		}

		private void UpdateCurrentState()
		{
			if ((CurrentFXState == FXState.Starting) && (passedTime > BaseInfo.TimeIn))
			{
				passedTime -= BaseInfo.TimeIn;
				CurrentFXState = FXState.Staying;
			}

			if ((CurrentFXState == FXState.Staying) && BaseInfo.FinishConditions.OnTimeout && (passedTime > BaseInfo.FinishConditions.TimeStay))
			{
				passedTime -= BaseInfo.FinishConditions.TimeStay;
				CurrentFXState = FXState.Ending;
			}

			if ((CurrentFXState == FXState.Ending) && !ShouldBeDestroyed)
			{
				if (passedTime > BaseInfo.TimeOut)
				{
					ShouldBeDestroyed = true;
					passedTime = BaseInfo.TimeOut;
				}
			}
		}

		private void UpdateMultiplier()
		{
			if (CurrentFXState == FXState.Starting)
			{
				stateMultiplier = BaseInfo.TimeIn > 0 ? passedTime / BaseInfo.TimeIn : 1;
			}
			else if (CurrentFXState == FXState.Staying)
			{
				stateMultiplier = 1;
			}
			else //currentState == FXState.Ending
			{
				stateMultiplier = BaseInfo.TimeOut > 0 ? 1 - (passedTime / BaseInfo.TimeOut) : 0;
			}
		}

		protected float GetCurrentMultiplier()
		{
			return IsDisabled ? 0 : stateMultiplier * InstanceData.Multiplier;
		}

		public virtual void RequestFinishFX()
		{
			if (CurrentFXState != FXState.Ending)
			{
				passedTime = 0;
				CurrentFXState = FXState.Ending;
			}
		}

		public bool WillBeDestroyed()
		{
			if (!IsDestroyed && ShouldBeDestroyed && AllowedToDestroy())
			{
				Destroy();
			}

			return IsDestroyed;
		}

		public void Destroy()
		{
			IsDestroyed = true;
			OnDestroy();
		}

		protected virtual void InternalUpdate(float timeStep) { }

		protected virtual bool AllowedToDestroy()
		{
			return true;
		}

		protected virtual void OnDestroy() { }
		protected virtual void OnSetActiveState(bool state) { }
	}

	public abstract class FXInstance<T> : FXInstance where T : FXObject
	{
		protected T FXData { get; private set; }
		protected FXInstance(FXObject baseInfo, FXInstanceData instanceData) : base(baseInfo, instanceData) { }

		protected override void OnInitiate()
		{
			FXData = BaseInfo as T;
		}
	}
}