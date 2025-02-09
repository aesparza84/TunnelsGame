using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CurrentRuntTime
{
    public int minutes {  get; private set; }
    public int seconds { get; private set; }
    public int miliseconds {  get; private set; }

    public CurrentRuntTime(int min, int sec, int mili)
    {
        minutes = min; 
        seconds = sec;
        miliseconds = mili;
    }
}
public class VHS_UI : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private TextMeshProUGUI _playbackText;

    [Header("Playback Text")]
    [SerializeField] private string PlayText;
    [SerializeField] private string PauseText;

    [SerializeField] private GameObject DeathStatsObj;
    [SerializeField] private TextMeshProUGUI FinalTime;
    [SerializeField] private TextMeshProUGUI FinalCheeseCount;
    [SerializeField] private TextMeshProUGUI EngagementText;

    //values
    private int minutes;
    private int seconds;
    private int mililseconds;

    //Elapsed time
    private float totalTime;
    private bool TickTime;
    private bool initStart;

    public static event EventHandler<CurrentRuntTime> SendTime;
    private void Start()
    {
        LevelMessanger.GameLoopStopped += DeathEvents;
        LevelMessanger.GameLoopStopped += StopEngagment;

        PauseMenu.OnPause += OnPause;
        PauseMenu.OnResume += OnResume;


        SceneManager.activeSceneChanged += ThisSceneInit;
        LevelMessanger.LevelStart += BeginTImer;


        PlayerController.OnAttackStatic += Engagement;
        PlayerController.OnAttackRelease += StopEngagment;
        ScoreKeepr.DeathStats += GetDeathStats;


    }

    private void BeginTImer(object sender, EventArgs e)
    {
        if (!initStart)
        {
            DeathStatsObj.SetActive(false);

            ResetValues();
            TickTime = true;

            initStart = true;
        }
        
    }

    private void StopEngagment(object sender, EventArgs e)
    {
        EngagementText.gameObject.SetActive(false);
    }

    private void Engagement(object sender, bool e)
    {
        if (e)
        {
            EngagementText.text = "INPUT: F";
        }
        else
        {
            EngagementText.text = "INPUT: SPACE";
        }

        EngagementText.gameObject.SetActive(true);
    }

    private void GetDeathStats(object sender, Tuple<CurrentRuntTime, int> e)
    {
        FinalCheeseCount.text = $"Cheese collected {e.Item2}";
    }

    private void ThisSceneInit(Scene arg0, Scene arg1)
    {
        ResetValues();
        DeathStatsObj.SetActive(false);

        ResetValues();
        TickTime = true;
    }
    private void DeathEvents(object sender, System.EventArgs e)
    {
        TickTime = false;
        CurrentRuntTime recentTime = new CurrentRuntTime(minutes, seconds, mililseconds);
        SendTime?.Invoke(this, recentTime);

        DeathStatsObj.SetActive(true);
    }

    private void OnResume(object sender, System.EventArgs e)
    {
        _playbackText.text = PlayText;
    }

    private void OnPause(object sender, System.EventArgs e)
    {
        _playbackText.text = PauseText;
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
        LevelMessanger.GameLoopStopped -= DeathEvents;
        LevelMessanger.GameLoopStopped -= StopEngagment;

        PauseMenu.OnPause -= OnPause;
        PauseMenu.OnResume -= OnResume;


        SceneManager.activeSceneChanged -= ThisSceneInit;
        LevelMessanger.LevelStart -= BeginTImer;

        PlayerController.OnAttackStatic -= Engagement;
        PlayerController.OnAttackRelease -= StopEngagment;
        ScoreKeepr.DeathStats -= GetDeathStats;
    }

}
