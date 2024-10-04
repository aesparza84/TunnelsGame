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

    [Header("Desired Layout")]
    [SerializeField] private OpeningSide EntranceSide;
    [SerializeField] private OpeningSide[] ExitSides;
    private int ExitCount;

    //Side that this room is branching from
    public OpeningSide BranchingFrom;

    private bool HasHappened;
    public bool BadRoom { get; private set; }
    private void Awake()
    {
        EntranceSide = OpeningSide.NONE;
        ExitSides = new OpeningSide[] { OpeningSide.NONE };
        ExitCount = 0;
    }
    //Public Methods
    public void SetEntrance(OpeningSide side)
    {
        EntranceSide = side;

        switch (EntranceSide)
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

            if (!HasHappened)
            {
                HasHappened = true;
            }
            else
            {

            }

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
    public bool HasZeroExits()
    {
        return ExitCount < 1;
    }
}
