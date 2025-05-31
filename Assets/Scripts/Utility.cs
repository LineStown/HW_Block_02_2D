using UnityEngine;

public class Utility
{
    static public float GetMinX()
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
    }

    static public float GetMaxX()
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
    }
}
