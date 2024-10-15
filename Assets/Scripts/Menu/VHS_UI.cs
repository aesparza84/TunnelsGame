using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VHS_UI : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private TextMeshProUGUI _playbackText;

    [Header("Playback Text")]
    [SerializeField] private string PlayText;
    [SerializeField] private string PauseText;

    //values
    private int minutes;
    private int seconds;
    private int mililseconds;

    //Elapsed time
    private float totalTime;
    private bool TickTime;

    private void Start()
    {
        LevelMessanger.LevelStart += BeginTimer;
    }

    private void BeginTimer(object sender, System.EventArgs e)
    {
        ResetValues();
        TickTime = true;
    }

    private void Update()
    {
        if (TickTime)
        {
            UpdateTimer();
        }
    }

    private void UpdateTimer()
    {
        totalTime += Time.deltaTime;
        minutes = Mathf.FloorToInt(totalTime/60);
        seconds = Mathf.FloorToInt(totalTime % 60);
        mililseconds = Mathf.FloorToInt((totalTime * 100) % 100);
        _timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, mililseconds);
    }

    private void ResetValues()
    {
        totalTime = 0.0f;
        minutes = 0;
        seconds = 0;
        mililseconds = 0;
    }

    private void OnDisable()
    {
        LevelMessanger.LevelStart -= BeginTimer;
    }
}
