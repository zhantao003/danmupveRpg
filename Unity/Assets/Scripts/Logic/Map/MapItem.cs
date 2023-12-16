using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MapItem 
{
    public int x;
    public int y;
    public bool isObstacle;//false可通行，true不可通行

    public bool CanThrough()
    {
        return isObstacle == false;
    }


}