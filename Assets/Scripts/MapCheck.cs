using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCheck : MonoBehaviour
{
    public static int PieceCount = 0;
    public static int MaxPieceCount = 15;

    public static bool CanAddPiece()
    {
        return PieceCount < MaxPieceCount;  
    }

    public static void AddedPiece()
    {
        PieceCount++;
    }
}
