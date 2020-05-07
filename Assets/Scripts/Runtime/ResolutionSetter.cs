using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionSetter : MonoBehaviour
{
    void Start()
    {
        Screen.SetResolution(Mathf.RoundToInt(Screen.height * (9f / 16f)), Screen.height, true);
    }
}
