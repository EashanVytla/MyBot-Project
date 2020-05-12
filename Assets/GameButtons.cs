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
    public static Vector3 startpos = new Vector3(0, 0.3255587f, 0);
    float startheading = 0;
    public static bool reset = false;
    bool first = true;
    public Rigidbody rb;
    public Rigidbody wheel1;
    public Rigidbody wheel2;
    public Rigidbody wheel3;
    public Rigidbody wheel4;

    public void ResetClick()
    {
        reset = true;
        robotPos.SetPositionAndRotation(startpos, Quaternion.Euler(0, startheading, 0));
        rb.velocity = new Vector3(0, 0, 0);
        wheel1.velocity = new Vector3(0, 0, 0);
        wheel2.velocity = new Vector3(0, 0, 0);
        wheel3.velocity = new Vector3(0, 0, 0);
        wheel4.velocity = new Vector3(0, 0, 0);
        WheelController.encoderCountLeft = 0;
        WheelController.encoderCountRight = 0;
        WheelController.encoderCountStrafe = 0;
        WheelController.startheading = startheading;
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
