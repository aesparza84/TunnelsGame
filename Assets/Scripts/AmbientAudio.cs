using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientAudio : MonoBehaviour
{
    private static AmbientAudio _instance;

    private void Start()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            DontDestroyOnLoad(this);
            _instance = this;
        }
    }
}
