using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class OptionsInterface : MonoBehaviour
{
    public float Driven = 0;
    public GameObject DrivenInput;

    public float Driving = 0;
    public GameObject DrivingInput;
    
    public float Mass = 0;
    public GameObject MassInput;
    
    public float RPM = 0;
    public GameObject RPMInput;

    public void update_Driven(string driventxt)
    {
        try
        {
            Driven = float.Parse(driventxt);
        }
        catch
        {
            EditorUtility.DisplayDialog("ERROR", "Please input a numerical value in this box.", "ok");
        }
    }

    public void update_Driving(string drivingtxt)
    {
        try
        {
            Driving = float.Parse(drivingtxt);
        }
        catch
        {
            EditorUtility.DisplayDialog("ERROR", "Please input a numerical value in this box.", "ok");
        }
    }

    public void update_RPM(string RPMtxt)
    {
        try
        {
            RPM = float.Parse(RPMtxt);
        }
        catch
        {
            EditorUtility.DisplayDialog("ERROR", "Please input a numerical value in this box.", "ok");
        }
    }

    public void update_Mass(string masstxt)
    {
        try
        {
            Mass = float.Parse(masstxt);
        }
        catch
        {
            EditorUtility.DisplayDialog("ERROR", "Please input a numerical value in this box.", "ok");
        }
    }
}
