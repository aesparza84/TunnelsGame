using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMessanger : MonoBehaviour
{
    [Header("Camera Transition Component")]
    [SerializeField] private CameraTransitioner _camTransitioner;


    //Static events to send out
    public static event EventHandler LevelStart;
    public static event EventHandler MapReady;
    public static event EventHandler PlayerReset;
    //Siginify that player reached end: Camera down, fade to black, Invoke next level event...
    public static event EventHandler LevelFinished; 
    
    //Load next level layout, reset map data, enemy & player etc.
    public static event EventHandler LevelExitCompleted;

    public static event EventHandler GameLoopStopped;

    private void Awake()
    {
        Application.targetFrameRate = 240;

        //Camera transition events
        _camTransitioner.OnEntranceTransitionFinished += OnEntranaceCamFinished;
        _camTransitioner.OnExitTransitionFinished += OnExitCamFinished;

        //Static
        AdjustRoom.ExitReached += LevelFinish;
        PathGenDFS.OnNewMapGenerated += MapCompleted;
        MapManeuver.PlayerReset += PlayerPosReset;
        PlayerController.OnDeath += StopGameLoop;
    }

    private void StopGameLoop(object sender, EventArgs e)
    {
        GameLoopStopped?.Invoke(this, EventArgs.Empty);
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
        //TRIGGER entrance camera blend
        PlayerReset?.Invoke(this, EventArgs.Empty);
    }
    
    //Disable subscribed components and clear all pathfinding data
    private void LevelFinish(object sender, System.EventArgs e)
    {
        Debug.Log("Exit Reached");

        //TRIGGER exit camera blend

        LevelFinished?.Invoke(this, EventArgs.Empty);
    }

    //Camera events
    private void OnExitCamFinished(object sender, EventArgs e)
    {
        //Start level after transition
        LevelExitCompleted?.Invoke(this, EventArgs.Empty);
    }

    private void OnEntranaceCamFinished(object sender, EventArgs e)
    {
        //Start level after entrance-transition
        LevelStart?.Invoke(this, EventArgs.Empty);
    }

    private void OnDisable()
    {
        _camTransitioner.OnEntranceTransitionFinished += OnEntranaceCamFinished;
        _camTransitioner.OnExitTransitionFinished += OnExitCamFinished;

        AdjustRoom.ExitReached -= LevelFinish;
        PathGenDFS.OnNewMapGenerated -= MapCompleted;
        MapManeuver.PlayerReset -= PlayerPosReset;
        PlayerController.OnDeath -= StopGameLoop;
    }
}
