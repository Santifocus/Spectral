using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spectral.Runtime;
using Spectral.Runtime.Behaviours;
using Spectral.Runtime.Behaviours.Entities;
using Spectral.Runtime.Behaviours.UI;
using Spectral.Runtime.DataStorage;
using Spectral.Runtime.DataStorage.FX;
using Spectral.Runtime.DataStorage.Logic;
using Spectral.Runtime.Factories;
using Spectral.Runtime.FX.Handling;
using Spectral.Runtime.Interfaces;


namespace Spectral.Runtime
{
	public class PersistentObjectManager : MonoBehaviour
	{
		private static bool Initiated;
		private void Awake()
		{
			if (Initiated)
			{
				gameObject.SetActive(false);
				Destroy(gameObject);
			}
			else
			{
				Initialise();
			}
		}

		private void Initialise()
		{
			Initiated = true;
			PersistentSettingsManager.ReadOrCreate();
			DontDestroyOnLoad(this);
		}
	}
}