using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPathFinder 
{
    public void SetNewDestination(Point p);
    public void TraversePath();
    public void StopTraverse();
}
