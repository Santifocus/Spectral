using Spectral.Runtime.Behaviours;
using Spectral.Runtime.DataStorage;
using UnityEngine;

namespace Spectral.Runtime.Factories
{
	public static class TransitionGateFactory
	{
		public static TransitionGate CreateTransitionGate(int transitionDirection, int levelPlaneIndex)
		{
			//Setup the core transform
			Transform gateCoreObject = new GameObject("Transition Gate").transform;
			gateCoreObject.SetParent(LevelLoader.GameLevelPlanes[levelPlaneIndex].CoreObject.TargetStorage.FoodObjectStorage);
			float planeWidth = LevelLoader.GameLevelPlanes[levelPlaneIndex].PlaneSettings.LevelWidth   - GameSettings.Current.LevelBorderForceFieldWidth;
			float planeHeight = LevelLoader.GameLevelPlanes[levelPlaneIndex].PlaneSettings.LevelHeight - GameSettings.Current.LevelBorderForceFieldWidth;
			gateCoreObject.localPosition = new Vector3((Random.value - 0.5f) * planeWidth, 0, (Random.value - 0.5f) * planeHeight);

			//Setup the model
			switch (transitionDirection)
			{
				case 1:
				{
					Object.Instantiate(LevelLoaderSettings.Current.TransitionGateModelDown, gateCoreObject);

					break;
				}
				case -1:
				{
					Object.Instantiate(LevelLoaderSettings.Current.TransitionGateModelUp, gateCoreObject);

					break;
				}
				default:
				{
#if SPECTRAL_DEBUG
					throw new System.ArgumentException($"Cannot create a Transition gate with {nameof(transitionDirection)}: '{transitionDirection}'.");
#else
					Object.Destroy(gateCoreObject.gameObject);
					return;
#endif
				}
			}

			//Setup the Gate
			TransitionGate gate = gateCoreObject.gameObject.AddComponent<TransitionGate>();
			gate.Initialise(transitionDirection);

			return gate;
		}
	}
}