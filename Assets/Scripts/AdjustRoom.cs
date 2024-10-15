using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;
public enum RoomLayout { STR, LEFT, RIGHT, T, FOUR, DEADEND}
public class AdjustRoom : MonoBehaviour
{
    [Header("Roof")]
    [SerializeField] private GameObject Roof;
    [SerializeField] private GameObject Floor;
    [SerializeField] private GameObject ShaftDown;
    [SerializeField] private GameObject ShaftUp;
    [SerializeField] private Light _exitLight;
    [SerializeField] private GameObject ExitIcon;

    [Header("Walls")]
    [SerializeField] private GameObject N_Wall;
    [SerializeField] private GameObject E_Wall;
    [SerializeField] private GameObject S_Wall;
    [SerializeField] private GameObject W_Wall;

    [Header("Unlit Walls (Map)")]
    [SerializeField] private GameObject N_UnlitWall;
    [SerializeField] private GameObject E_UnlitWall;
    [SerializeField] private GameObject S_UnlitWall;
    [SerializeField] private GameObject W_UnlitWall;

    [Header("Hide Walls")]
    [SerializeField] private GameObject Hide_N_Wall;
    [SerializeField] private GameObject Hide_E_Wall;
    [SerializeField] private GameObject Hide_S_Wall;
    [SerializeField] private GameObject Hide_W_Wall;

    [Header("Desired Layout")]
    [SerializeField] private OpeningSide entranceSide;
    [SerializeField] private OpeningSide[] ExitSides;
    public List<OpeningSide> ExitList { get; private set; }
    private int ExitCount;
    public OpeningSide EntranceSide { get { return entranceSide;} }

    //Side that this room is branching from
    public OpeningSide BranchingFrom;

    [Header("Room Trigger")]
    [SerializeField] private Collider RoomTrigger;
    public bool HideRoom { get; private set; }

    //Maze Exit bool
    public bool MazeExit { get; private set; }

    public bool ExitActivated { get; private set; }

    public static event EventHandler ExitReached;
    private void Awake()
    {
        entranceSide = OpeningSide.NONE;
        ExitSides = new OpeningSide[] { OpeningSide.NONE };
        ExitList = new List<OpeningSide>();
        ExitCount = 0;

        _exitLight.enabled = false;

        if (RoomTrigger != null)
            RoomTrigger.enabled = false;

        ExitActivated = false;
    }
    //Public Methods
    public void SetEntrance(OpeningSide side)
    {
        entranceSide = side;

        switch (entranceSide)
        {
            case OpeningSide.N:
                N_Wall.SetActive(false);
                N_UnlitWall.SetActive(false);
                ExitList.Add(OpeningSide.N);

                break;
            case OpeningSide.E:
                E_Wall.SetActive(false);
                E_UnlitWall.SetActive(false);
                ExitList.Add(OpeningSide.E);

                break;
            case OpeningSide.S:
                S_Wall.SetActive(false);
                S_UnlitWall.SetActive(false);
                ExitList.Add(OpeningSide.S);

                break;
            case OpeningSide.W:
                W_Wall.SetActive(false);
                W_UnlitWall.SetActive(false);
                ExitList.Add(OpeningSide.W);

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
                        N_UnlitWall.SetActive(false);
                        ExitCount++;

                        ExitList.Add(OpeningSide.N);
                    }

                    break;
                case OpeningSide.E:
                    if (E_Wall.activeInHierarchy)
                    {
                        E_Wall.SetActive(false);
                        E_UnlitWall.SetActive(false);
                        ExitCount++;

                        ExitList.Add(OpeningSide.E);
                    }


                    break;
                case OpeningSide.S:
                    if (S_Wall.activeInHierarchy)
                    {
                        S_Wall.SetActive(false);
                        S_UnlitWall.SetActive(false);
                        ExitCount++;

                        ExitList.Add(OpeningSide.S);
                    }



                    break;
                case OpeningSide.W:
                    if (W_Wall.activeInHierarchy)
                    {
                        W_Wall.SetActive(false);
                        W_UnlitWall.SetActive(false);
                        ExitCount++;
                        ExitList.Add(OpeningSide.W);
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
        HideRoom = true;

        if (RoomTrigger != null)
            RoomTrigger.enabled = true;

        switch (hideSide)
        {
            case OpeningSide.N:
                Hide_N_Wall.SetActive(true);

                break;
            case OpeningSide.E:
                Hide_E_Wall.SetActive(true);

                break;
            case OpeningSide.S:
                Hide_S_Wall.SetActive(true);

                break;
            case OpeningSide.W:
                Hide_W_Wall.SetActive(true);

                break;
            case OpeningSide.NONE:
                break;
            default:
                break;
        }

        Roof.SetActive(false);
        ShaftUp.SetActive(true);
    }

    public void SetMazeExit()
    {
        MazeExit = true;

        if (RoomTrigger != null)
            RoomTrigger.enabled = true;

        Floor.SetActive(false);
        ShaftDown.SetActive(true);
        ExitIcon.SetActive(true);

        _exitLight.enabled = true;

    }
    public bool HasZeroExits()
    {
        return ExitCount < 1;
    }
    public bool ContainsOpenings(OpeningSide entranceSide, OpeningSide exitSide)
    {
        if (EntranceSide == entranceSide || ExitList.Contains(exitSide))
        {
            return true;
        }

        return false;
    }
    private void OnTriggerEnter(Collider other)
    {
        //Hide 'hideable' if this is marked 'Hide Room'
        if (HideRoom)
        {
            if (other.TryGetComponent<IHideable>(out IHideable h))
            {
                if (h.IsVisible())
                {
                    h.Hide();
                }
            }
        }

        //End of level
        if (MazeExit)
        {
            if (ExitActivated)
            {
                ExitReached?.Invoke(this, EventArgs.Empty);
                //UnityEngine.Debug.Log("Player escaped, make a new level");
            }
            
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //Hide 'hideable' if this is marked 'Hide Room'
        if (HideRoom)
        {
            if (other.TryGetComponent<IHideable>(out IHideable h))
            {
                if (h.IsVisible())
                {
                    h.Hide();
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        // Reveal 'hideable' if this is marked 'Hide Room'
        if (HideRoom)
        {
            if (other.TryGetComponent<IHideable>(out IHideable h))
            {
                h.Reveal();
            }
        }
    }

    public void ActivateExit()
    {
        ExitActivated = true;
    }
}
