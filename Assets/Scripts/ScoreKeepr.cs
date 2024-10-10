using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeepr : MonoBehaviour
{
    private int CurrentScore;
    private int HighestScore;
    private void Start()
    {
        HighestScore = -1;

        AdjustRoom.ExitReached += OnExitReached;
        HealthComponent.OnPlayerDeath += OnPlayerDeath;
    }

    private void OnPlayerDeath(object sender, System.EventArgs e)
    {
        CurrentScore = 0;
    }

    private void OnExitReached(object sender, System.EventArgs e)
    {
        CurrentScore++;

        if (CurrentScore > HighestScore)
            HighestScore = CurrentScore;
    }

    private void OnDisable()
    {
        AdjustRoom.ExitReached -= OnExitReached;
        HealthComponent.OnPlayerDeath -= OnPlayerDeath;
    }
}
