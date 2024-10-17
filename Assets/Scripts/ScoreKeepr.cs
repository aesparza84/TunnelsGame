using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeepr : MonoBehaviour
{
    [Header("Increase difficulty after 'x' levels complete")]
    [SerializeField] private int DificultyLevelThreshold;

    [Header("Hard reference to 2nd enemy")]
    [SerializeField] private GameObject SecondEnemy;

    private int CurrentScore;
    private int HighestScore;

    public static event EventHandler DifficultyIncrease;
    private void Start()
    {
        HighestScore = -1;

        SecondEnemy.SetActive(false);

        LevelMessanger.MapReady += NextLevel;
        LevelMessanger.LevelExitCompleted += IncreaseScore;
        PlayerController.OnDeath += OnPlayerDeath;
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
            SecondEnemy.SetActive(true);
            DifficultyIncrease?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnDisable()
    {
        LevelMessanger.MapReady -= NextLevel;
        PlayerController.OnDeath -= OnPlayerDeath;
    }
}
