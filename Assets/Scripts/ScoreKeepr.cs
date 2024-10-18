using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeepr : MonoBehaviour
{
    [Header("Increase difficulty after 'x' levels complete")]
    [SerializeField] private int DificultyLevelThreshold;

    private int CurrentScore;
    private int HighestScore;

    private CurrentRuntTime BestTime;
    private CurrentRuntTime CurrentTime;

    public static event EventHandler DifficultyIncrease;
    private void Start()
    {
        HighestScore = -1;

        LevelMessanger.MapReady += NextLevel;
        LevelMessanger.LevelExitCompleted += IncreaseScore;
        VHS_UI.SendTime += OnRetrieveTime;
        PlayerController.OnDeath += OnPlayerDeath;
    }

    private void OnRetrieveTime(object sender, CurrentRuntTime e)
    {
        CurrentTime = e;

        if (BestTime != null)
        {
            if (CurrentTimeisLower())
            {
                BestTime = CurrentTime;
            }
        }
        else
        {
            BestTime = CurrentTime;
        }
    }

    private bool CurrentTimeisLower()
    {
        if (CurrentTime.minutes <= BestTime.minutes)
        {
            if (CurrentTime.seconds <= BestTime.seconds)
            {
                if (CurrentTime.miliseconds < BestTime.seconds)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void IncreaseScore(object sender, EventArgs e)
    {
        CurrentScore++;
        if (CurrentScore > HighestScore)
            HighestScore = CurrentScore;
    }

    private void OnPlayerDeath(object sender, System.EventArgs e)
    {
        CurrentScore = 0;
    }

    private void NextLevel(object sender, System.EventArgs e)
    {
        if (CurrentScore > DificultyLevelThreshold)
        {
            DifficultyIncrease?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnDisable()
    {
        LevelMessanger.MapReady -= NextLevel;
        LevelMessanger.LevelExitCompleted -= IncreaseScore;
        VHS_UI.SendTime -= OnRetrieveTime;
        PlayerController.OnDeath -= OnPlayerDeath;
    }
}
