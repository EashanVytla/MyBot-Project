using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameButtons : MonoBehaviour
{
    public Transform robotPos;
    Vector3 startpos = new Vector3(0, 0.3255587f, 0);
    float startheading = 0;

    public void ResetClick()
    {
        robotPos.SetPositionAndRotation(startpos, new Quaternion(0, startheading, 0, 0));
        BlockController.encoderCountLeft = 0;
        BlockController.encoderCountRight = 0;
        BlockController.encoderCountStrafe = 0;
        BlockController.startheading = BlockController.heading;
        Listener.stopper = false;
    }

    public void ChangeStartPosx(string x)
    {
        startpos.z = float.Parse(x);
    }

    public void ChangeStartPosy(string y)
    {
        startpos.z = float.Parse(y);
    }

    public void ChangeStartPosz(string rot)
    {
        startheading = float.Parse(rot);
    }
}
