using Assets;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameButtons : MonoBehaviour
{
    public Transform robotPos;
    //public Transform Blocks;
    Vector3 startpos = new Vector3(0, 0.3255587f, 0);
    float startheading = 0;
    public static bool reset = false;
    bool first = true;
    public Rigidbody rb;



    public void ResetClick()
    {
        robotPos.SetPositionAndRotation(startpos, Quaternion.Euler(0, startheading, 0));
        reset = true;
        rb.velocity = new Vector3(0, 0, 0);
        BlockController.encoderCountLeft = 0;
        BlockController.encoderCountRight = 0;
        BlockController.encoderCountStrafe = 0;
        BlockController.startheading = startheading;
    }

    public void exit()
    {
        ResetClick();
        SceneManager.LoadScene("MainMenuScene");
    }

    //1.75970711674 is the scale factor from encoder counts to inches

    public void ChangeStartPosx(string x)
    {
        startpos.x = float.Parse(x);
        ResetClick();
    }

    public void ChangeStartPosy(string y)
    {
        startpos.z = float.Parse(y);
        ResetClick();
    }

    public void ChangeStartPosz(string rot)
    {
        startheading = float.Parse(rot);
        ResetClick();
    }

    public void startFeild()
    {
        startpos.x = -10;
        startpos.z = 15;
        startheading = 180;
        ResetClick();
    }

    public void killFeild()
    {
        startpos.x = 0;
        startpos.z = 0;
        startheading = 0;
        ResetClick();
    }
}
