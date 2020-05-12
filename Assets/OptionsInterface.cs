using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class OptionsInterface : MonoBehaviour
{
    public static int MotorType = 0;

    public static float Ratio = 20;

    public static float Mass = 30;

    public static float WheelDiameter = 4;

    public static bool FC = true;
    public static bool AB = true;

    public TMP_Dropdown MotorTypeIN;
    public TMP_InputField RatioInput;
    public TMP_InputField MassInput;
    public TMP_InputField WheelDiameterInput;
    public Toggle FCToggle;
    public Toggle ABToggle;

    public void startOptions()
    {
        MotorTypeIN.SetValueWithoutNotify(0);
        RatioInput.SetTextWithoutNotify(Ratio.ToString());
        MassInput.SetTextWithoutNotify(Mass.ToString());
        WheelDiameterInput.SetTextWithoutNotify(WheelDiameter.ToString());
        FCToggle.SetIsOnWithoutNotify(FC);
        ABToggle.SetIsOnWithoutNotify(AB);
    }

    public void update_Type(int type)
    {
        try
        {
            MotorType = type;
        }
        catch
        {
            //EditorUtility.DisplayDialog("ERROR", "Please input a numerical value in this box.", "ok");
        }
    }

    public void update_Ratio(string drivingtxt)
    {
        try
        {
            Ratio = float.Parse(drivingtxt);
        }
        catch
        {
            //EditorUtility.DisplayDialog("ERROR", "Please input a numerical value in this box.", "ok");
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
            //EditorUtility.DisplayDialog("ERROR", "Please input a numerical value in this box.", "ok");
        }
    }

    public void update_wheel_diameter(string wdtxt)
    {
        try
        {
            WheelDiameter = float.Parse(wdtxt);
        }
        catch
        {
            //EditorUtility.DisplayDialog("ERROR", "Please input a numerical value in this box.", "ok");
        }
    }

    public void update_FC(bool FeildCentric)
    {
        FC = FeildCentric;
    }

    public void update_AB(bool AutoBreak)
    {
        AB = AutoBreak;
    }
}
