using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    //Side that this room is branching from
    public OpeningSide BranchingFrom;
    private void Start()
    {
        EntranceSide = OpeningSide.NONE;
        ExitSides = new OpeningSide[] { OpeningSide.NONE };
    }
    //Public Methods
    public void SetEntrance(OpeningSide side)
    {
        EntranceSide = side;
    }

    public void SetExits(OpeningSide[] Exits)
    {
        ExitSides = Exits;
    }

    public void CutOutDoors()
    {
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

        foreach (OpeningSide side in ExitSides)
        {
            switch (side)
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
    }
}
