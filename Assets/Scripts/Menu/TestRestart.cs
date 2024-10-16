using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestRestart : MonoBehaviour
{
    public void ReloadScene()
    {
        SceneManager.LoadScene(1);
    }
}
