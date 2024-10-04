using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;
public enum RoomLayout { STR, LEFT, RIGHT, T, FOUR, DEADEND}
public class AdjustRoom : MonoBehaviour
{
    [Header("Walls")]
    [SerializeField] private GameObject N_Wall;
    [SerializeField] private GameObject E_Wall;
    [SerializeField] private GameObject S_Wall;
    [SerializeField] private GameObject W_Wall;

    [Header("Hide Walls")]
    [SerializeField] private GameObject Hide_N_Wall;
    [SerializeField] private GameObject Hide_E_Wall;
    [SerializeField] private GameObject Hide_S_Wall;
    [SerializeField] private GameObject Hide_W_Wall;

    [Header("Desired Layout")]
    [SerializeField] private OpeningSide entranceSide;
    [SerializeField] private OpeningSide[] ExitSides;
    private int ExitCount;
    public OpeningSide EntranceSide { get { return entranceSide;} }

    //Side that this room is branching from
    public OpeningSide BranchingFrom;

    private bool HasHappened;
    public bool BadRoom { get; private set; }
    private void Awake()
    {
        entranceSide = OpeningSide.NONE;
        ExitSides = new OpeningSide[] { OpeningSide.NONE };
        ExitCount = 0;
    }
    //Public Methods
    public void SetEntrance(OpeningSide side)
    {
        entranceSide = side;

        switch (entranceSide)
        {
            case OpeningSide.N:
                N_Wall.SetActive(false);

                break;
            case OpeningSide.E:
                E_Wall.SetActive(false);

                break;
            case OpeningSide.S:
                S_Wall.SetActive(false);

                break;
            case OpeningSide.W:
                W_Wall.SetActive(false);

                break;
            case OpeningSide.NONE:

                break;
            default:
                break;
        }
    }

    public void SetExits(OpeningSide[] Exits)
    {
        ExitSides = Exits;

        foreach (OpeningSide side in ExitSides)
        {
            switch (side)
            {
                case OpeningSide.N:

                    if (N_Wall.activeInHierarchy)
                    {
                        N_Wall.SetActive(false);
                        ExitCount++;
                    }

                    break;
                case OpeningSide.E:
                    if (E_Wall.activeInHierarchy)
                    {
                        E_Wall.SetActive(false);
                        ExitCount++;
                    }


                    break;
                case OpeningSide.S:
                    if (S_Wall.activeInHierarchy)
                    {
                        S_Wall.SetActive(false);
                        ExitCount++;
                    }



                    break;
                case OpeningSide.W:
                    if (W_Wall.activeInHierarchy)
                    {
                        W_Wall.SetActive(false);
                        ExitCount++;
                    }

                    break;
                case OpeningSide.NONE:

                    break;
                default:
                    break;
            }
        }
    }

    public void SetHideWall(OpeningSide hideSide)
    {
        switch (hideSide)
        {
            case OpeningSide.N:
                N_Wall.SetActive(false);
                Hide_N_Wall.SetActive(true);

                break;
            case OpeningSide.E:
                E_Wall.SetActive(false);
                Hide_E_Wall.SetActive(true);

                break;
            case OpeningSide.S:
                S_Wall.SetActive(false);
                Hide_S_Wall.SetActive(true);

                break;
            case OpeningSide.W:
                W_Wall.SetActive(false);
                Hide_W_Wall.SetActive(true);

                break;
            case OpeningSide.NONE:
                break;
            default:
                break;
        }
    }
    public bool HasZeroExits()
    {
        return ExitCount < 1;
    }
}
