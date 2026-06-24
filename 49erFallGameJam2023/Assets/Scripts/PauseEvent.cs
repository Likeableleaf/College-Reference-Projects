using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseEvent : MonoBehaviour
{
    public static event Action<bool> OnPause;

    public static void SetPaused(bool p)
    {
        OnPause?.Invoke(p);
        Debug.Log("Pause");
    }
}
