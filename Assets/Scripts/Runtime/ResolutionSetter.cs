using UnityEngine;

public class ResolutionSetter : MonoBehaviour
{
	private void Start()
	{
		Screen.SetResolution(Mathf.RoundToInt(Screen.height * (9f / 16f)), Screen.height, true);
	}
}