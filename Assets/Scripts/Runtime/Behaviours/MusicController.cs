using System.Collections.Generic;
using Spectral.Sounds;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Spectral.Runtime.Behaviours
{
	public class MusicController : MonoBehaviour
	{
		private const float REMOVE_THRESHOLD = 0.01f;
		public static MusicController Instance { get; private set; }

		public Sound[] MusicList = default;
		[SerializeField] private float musicFadeInTime = 2;
		[SerializeField] private float musicFadeOutTime = 2;

		private SoundPlayer[] musicPlayer;
		private readonly List<MusicInstance> activeMusicInstances = new List<MusicInstance>();

		private void Start()
		{
			Instance = this;
			SceneManager.sceneLoaded += OnSceneLoaded;

			//Setup Music player
			musicPlayer = new SoundPlayer[MusicList.Length];
			for (int i = 0; i < MusicList.Length; i++)
			{
				SoundPlayer soundPlayer = new GameObject(MusicList[i].soundName + " Sound Player").AddComponent<SoundPlayer>();
				soundPlayer.transform.SetParent(transform);
				soundPlayer.Setup(MusicList[i]);
				soundPlayer.FadePoint = 0;
				musicPlayer[i] = soundPlayer;
			}
		}

		public void AddMusicInstance(MusicInstance newInstance)
		{
			newInstance.IsAdded = true;
			activeMusicInstances.Add(newInstance);
		}

		private void RemoveMusicInstance(MusicInstance instance)
		{
			instance.IsAdded = false;
			activeMusicInstances.Remove(instance);
		}

		private void Update()
		{
			//Find the music instance with the highest index that wants to be played
			int targetInstanceIndex = -1;
			int highestPriority = -1;
			for (int i = 0; i < activeMusicInstances.Count; i++)
			{
				if (!activeMusicInstances[i].WantsToPlay)
				{
					if (activeMusicInstances[i].Volume < REMOVE_THRESHOLD)
					{
						RemoveMusicInstance(activeMusicInstances[i]);
						i--;
					}

					continue;
				}

				if (activeMusicInstances[i].Priority > highestPriority)
				{
					highestPriority = activeMusicInstances[i].Priority;
					targetInstanceIndex = i;
				}
			}

			//Now update the volumes of all instances, the target instance will be increased to 1
			//All other will be decreased to 0
			float volumeIncrease = musicFadeInTime  > 0 ? Time.unscaledDeltaTime / musicFadeInTime : 1;
			float volumeDecrease = musicFadeOutTime > 0 ? Time.unscaledDeltaTime / musicFadeOutTime : 1;
			for (int i = 0; i < activeMusicInstances.Count; i++)
			{
				if (i == targetInstanceIndex)
				{
					activeMusicInstances[i].Volume = Mathf.Min(1, activeMusicInstances[i].Volume + volumeIncrease);
				}
				else
				{
					activeMusicInstances[i].Volume = Mathf.Max(0, activeMusicInstances[i].Volume - volumeDecrease);
				}
			}

			//Now we apply the highest volume of all instances
			//To do that we first set all volumes to 0
			for (int i = 0; i < musicPlayer.Length; i++)
			{
				if (musicPlayer[i].FadePoint > 0)
				{
					musicPlayer[i].FadePoint = 0;
				}
			}

			//Then we go througth all instances and apply their volume if it is higher then the current volume
			for (int i = 0; i < activeMusicInstances.Count; i++)
			{
				if (musicPlayer[activeMusicInstances[i].MusicIndex].FullyStopped)
				{
					if (activeMusicInstances[i].AllowedToRestart)
					{
						activeMusicInstances[i].TotalRestarts++;
					}
					else
					{
						continue;
					}
				}

				if (activeMusicInstances[i].Volume > musicPlayer[activeMusicInstances[i].MusicIndex].FadePoint)
				{
					musicPlayer[activeMusicInstances[i].MusicIndex].FadePoint = activeMusicInstances[i].Volume;
				}
			}

			//Set the state of the music players based on if there volume is above zero
			for (int i = 0; i < musicPlayer.Length; i++)
			{
				if (musicPlayer[i].FadePoint > 0)
				{
					if (musicPlayer[i].FullyStopped)
					{
						musicPlayer[i].Play();
					}
				}
				else
				{
					musicPlayer[i].Stop();
				}
			}
		}

		public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			for (int i = 0; i < activeMusicInstances.Count; i++)
			{
				activeMusicInstances[i].WantsToPlay = false;
			}
		}
	}

	public class MusicInstance
	{
		public bool WantsToPlay;
		public bool IsAdded;
		public float Volume;
		public int Priority;
		public int MusicIndex;

		public bool AllowedToRestart => (maxRestarts == -1) || (TotalRestarts < maxRestarts);
		private readonly int maxRestarts;
		public int TotalRestarts;

		public MusicInstance(float volume, int priority, int musicIndex, int maxRestarts = -1)
		{
			WantsToPlay = false;
			Volume = volume;
			Priority = priority;
			MusicIndex = musicIndex;
			this.maxRestarts = maxRestarts;
		}
	}
}