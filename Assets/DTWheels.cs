using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DTWheels : MonoBehaviour
{
    private GameObject upleft;
    private GameObject upright;
    private GameObject backleft;
    private GameObject backright;
    Vector3 lastPosUl;
    Vector3 lastPosBl;
    Vector3 lastPosBR;
    Vector3 lastPosUR;
    const float EPILSON = 0.0001f;


    public DTWheels()
    {
        upleft = GameObject.Find("UpLeft");
        backleft = GameObject.Find("BackLeft");
        upright = GameObject.Find("UpRight");
        backright = GameObject.Find("BackRight");
        lastPosUl = upleft.transform.position;
        lastPosBl = backleft.transform.position;
        lastPosBR = backright.transform.position;
        lastPosUR = upright.transform.position;
    }

    Vector3 FrameVelocityUL = new Vector3(0, 0);
    public float getULVelo()
    {
        Vector3 currFrameVelocity = (upleft.transform.position - lastPosUl) / Time.deltaTime;
        FrameVelocityUL = Vector3.Lerp(FrameVelocityUL, currFrameVelocity, 0.1f);
        lastPosUl = upleft.transform.position;

        return FrameVelocityUL.magnitude/10;
    }

    Vector3 FrameVelocityBL = new Vector3(0, 0);
    public float getBLVelo()
    {
        Vector3 currFrameVelocity = (backleft.transform.position - lastPosBl) / Time.deltaTime;
        FrameVelocityBL = Vector3.Lerp(FrameVelocityBL, currFrameVelocity, 0.1f);
        lastPosBl = backleft.transform.position;

        return FrameVelocityBL.magnitude/10;
    }

    Vector3 FrameVelocityUR = new Vector3(0, 0);
    public float getURVelo()
    {
        Vector3 currFrameVelocity = (upright.transform.position - lastPosUR) / Time.deltaTime;
        FrameVelocityUR = Vector3.Lerp(FrameVelocityUR, currFrameVelocity, 0.1f);
        lastPosUR = upright.transform.position;

        return FrameVelocityUR.magnitude/10;
    }

    Vector3 FrameVelocityBR = new Vector3(0, 0);
    public float getBRVelo()
    {
        Vector3 currFrameVelocity = (backright.transform.position - lastPosBR) / Time.deltaTime;
        FrameVelocityBR = Vector3.Lerp(FrameVelocityBR, currFrameVelocity, 0.1f);
        lastPosBR = backright.transform.position;

        return FrameVelocityBR.magnitude/10;
    }
}
