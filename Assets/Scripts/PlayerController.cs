using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Mapped Inputs
    private PlayerControls _mappedInputs;

    //Input references
    private InputAction LeftArm;
    private InputAction RightArm;
    private InputAction TurnLeft;
    private InputAction TurnRight;

    private bool OnRightArm;

    private void Start()
    {
        _mappedInputs = new PlayerControls();
    }
    private void OnEnable()
    {
        LeftArm = _mappedInputs.Crawl.LeftArm;
        LeftArm.Enable();
        LeftArm.started += OnLeftCrawl;

        RightArm = _mappedInputs.Crawl.RightArm;    
        RightArm.Enable();
        RightArm.started += OnRightCrawl;

        TurnLeft = _mappedInputs.Crawl.CamLeft;
        TurnLeft.Enable();
        TurnLeft.started += OnTurnLeft;

        TurnRight = _mappedInputs.Crawl.CamRight;
        TurnRight.Enable();
        TurnRight.started += OnTurnRight;

    }

    private void OnRightCrawl(InputAction.CallbackContext obj)
    {
        throw new System.NotImplementedException();
    }
    private void OnLeftCrawl(InputAction.CallbackContext obj)
    {
        throw new System.NotImplementedException();
    }
    private void OnTurnLeft(InputAction.CallbackContext obj)
    {
        throw new System.NotImplementedException();
    }
    private void OnTurnRight(InputAction.CallbackContext obj)
    {
        throw new System.NotImplementedException();
    }    

    private void OnDisable()
    {
        LeftArm.Disable();
        RightArm.Disable();
        TurnLeft.Disable();
        TurnRight.Disable();
    }
}
