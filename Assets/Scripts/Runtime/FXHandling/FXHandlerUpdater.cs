using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Spectral.Runtime.FX.Handling
{
	public class FXHandlerUpdater : MonoBehaviour
	{
		private readonly List<FXHandler> scaledTimeFXHandlers = new List<FXHandler>();
		private readonly List<FXHandler> physicsTimeFXHandlers = new List<FXHandler>();
		private readonly List<FXHandler> realTimeTimeFXHandlers = new List<FXHandler>();

		#region MonoBehaviour

		private void Start()
		{
			SetupFXHandlers();
		}

		private void Update()
		{
			if (!StaticData.GameIsPaused)
			{
				foreach (FXHandler handler in scaledTimeFXHandlers)
				{
					handler.HandleFX(Time.deltaTime);
				}
			}

			foreach (FXHandler handler in realTimeTimeFXHandlers)
			{
				handler.HandleFX(Time.unscaledDeltaTime);
			}
		}

		private void FixedUpdate()
		{
			foreach (FXHandler handler in physicsTimeFXHandlers)
			{
				handler.HandleFX(Time.fixedDeltaTime);
			}
		}

		#endregion

		#region Management

		private void SetupFXHandlers()
		{
			IEnumerable<Type> fxHandlerTypes = typeof(FXHandler)
												.Assembly
												.GetTypes()
												.Where(type => type.IsSubclassOf(typeof(FXHandler)));

			foreach (Type type in fxHandlerTypes)
			{
				FXHandler handler = (FXHandler) Activator.CreateInstance(type);
				handler.OnHandlerInitiated();
				AddFXHandler(handler);
			}
		}

		private void AddFXHandler(FXHandler handler)
		{
			FXInstanceUtils.AddHandler(handler);
			switch (handler.UpdateStyle)
			{
				case TimeType.ScaledDeltaTime:
				{
					scaledTimeFXHandlers.Add(handler);

					break;
				}
				case TimeType.FixedDeltaTime:
				{
					physicsTimeFXHandlers.Add(handler);

					break;
				}
				case TimeType.UnscaledDeltaTime:
				{
					realTimeTimeFXHandlers.Add(handler);

					break;
				}
#if SPECTRAL_DEBUG
				default:
					throw new NotImplementedException("Not implemented Time Update style on " + handler.GetType().Name);
#endif
			}
		}

		private void ResetFX()
		{
			foreach (FXHandler handler in ForAllHandlers())
			{
				handler.Reset();
			}
		}
#if UNITY_EDITOR
		private void OnApplicationQuit()
		{
			ResetFX();
		}
#endif

		private IEnumerable<FXHandler> ForAllHandlers()
		{
			foreach (FXHandler handler in scaledTimeFXHandlers)
			{
				yield return handler;
			}

			foreach (FXHandler handler in physicsTimeFXHandlers)
			{
				yield return handler;
			}

			foreach (FXHandler handler in realTimeTimeFXHandlers)
			{
				yield return handler;
			}
		}

		#endregion
	}
}