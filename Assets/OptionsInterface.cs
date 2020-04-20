using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class OptionsInterface : MonoBehaviour
{
    public static float Driven = 2;

    public static float Driving = 3;
    
    public static float Mass = 0;
    
    public static float RPM = 340;

    public static float WheelDiameter = 4;

    public static bool FC = true;
    public static bool AB = true;

    public TMP_InputField DrivenInput;
    public TMP_InputField DrivingInput;
    public TMP_InputField MassInput;
    public TMP_InputField RPMInput;
    public TMP_InputField WheelDiameterInput;
    public Toggle FCToggle;
    public Toggle ABToggle;

    public void startOptions()
    {
        DrivenInput.SetTextWithoutNotify(Driven.ToString());
        DrivingInput.SetTextWithoutNotify(Driving.ToString());
        MassInput.SetTextWithoutNotify(Mass.ToString());
        RPMInput.SetTextWithoutNotify(RPM.ToString());
        WheelDiameterInput.SetTextWithoutNotify(WheelDiameter.ToString());
        FCToggle.SetIsOnWithoutNotify(FC);
        ABToggle.SetIsOnWithoutNotify(AB);
    }

    public void update_Driven(string driventxt)
    {
        try
        {
            Driven = float.Parse(driventxt);
            if(Driven != 0)
            {
                changescaler();
            }
        }
        catch
        {
            //EditorUtility.DisplayDialog("ERROR", "Please input a numerical value in this box.", "ok");
        }
    }

    public void update_Driving(string drivingtxt)
    {
        try
        {
            Driving = float.Parse(drivingtxt);
            if(Driving != 0)
            {
                changescaler();
            }
        }
        catch
        {
            //EditorUtility.DisplayDialog("ERROR", "Please input a numerical value in this box.", "ok");
        }
    }

    public void update_RPM(string RPMtxt)
    {
        try
        {
            RPM = float.Parse(RPMtxt);
            if(RPM != 0)
            {
                changescaler();
            }
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
            if(WheelDiameter != 0)
            {
                changescaler();
            }
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

    public void changescaler()
    {
        BlockController.newsignalScale = (float)(((RPM * (WheelDiameter * Math.PI) * (Driving / Driven)) / 12) / 60) *
            (BlockController.signalScale / (float)8.9011791851710808423108229192919);
    }
}
