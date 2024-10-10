using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMessanger : MonoBehaviour
{
    //Static events to send out
    public static event EventHandler LevelStart;
    public static event EventHandler MapReady;
    public static event EventHandler LevelFinished;

    private void Awake()
    {
        Application.targetFrameRate = 240;

        //Static
        AdjustRoom.ExitReached += LevelFinish;
        PathGenDFS.OnNewMapGenerated += MapCompleted;
        MapManeuver.PlayerReset += PlayerPosReset;
    }


    //We Reset Player position, clear & update all pathfinding data to new MAP
    private void MapCompleted(object sender, EventArgs e)
    {
        Debug.Log("Map Finished");
        MapReady?.Invoke(this, EventArgs.Empty);
    }

    //After new level init, send the Start Level message to activate components
    private void PlayerPosReset(object sender, EventArgs e)
    {
        LevelStart?.Invoke(this, EventArgs.Empty);
    }

    
    //Disable subscribed components and clear all pathfinding data
    private void LevelFinish(object sender, System.EventArgs e)
    {
        Debug.Log("Exit Reached");
        LevelFinished?.Invoke(this, EventArgs.Empty);
    }

    private void OnDisable()
    {
        AdjustRoom.ExitReached -= LevelFinish;
        PathGenDFS.OnNewMapGenerated -= MapCompleted;
        MapManeuver.PlayerReset -= PlayerPosReset;
    }
}
