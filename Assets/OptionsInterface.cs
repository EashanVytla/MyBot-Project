using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class OptionsInterface : MonoBehaviour
{
    public static int MotorType = 0;

    public static float Ratio = 20;

    public static float Mass = 40;

    public static float WheelDiameter = 4;

    public static bool FC = true;
    public static float turningspeed = 1.0f;
    public static float movementspeed = 1.0f;

    public static int Quality;
    public static bool QualityOverride = false;
    public static bool fullscreenmode = false;

    public TMP_Dropdown MotorTypeIN;
    public TMP_InputField RatioInput;
    public TMP_InputField MassInput;
    public TMP_InputField WheelDiameterInput;

    public Toggle FCToggle;
    public Slider MovementSpeed;
    public Slider TurningSpeed;

    public Slider QualitySlider;
    public Toggle FullScreen;

    public void Awake()
    {
        Quality = (int)Math.Round(SystemInfo.batteryLevel * 5);
    }

    public void startDrive()
    {
        MotorTypeIN.SetValueWithoutNotify(MotorType);
        RatioInput.SetTextWithoutNotify(Ratio.ToString());
        MassInput.SetTextWithoutNotify(Mass.ToString());
        WheelDiameterInput.SetTextWithoutNotify(WheelDiameter.ToString());
    }

    public void startControls()
    {
        FCToggle.SetIsOnWithoutNotify(FC);
        MovementSpeed.SetValueWithoutNotify(movementspeed);
        TurningSpeed.SetValueWithoutNotify(turningspeed);
    }

    public void startVideo()
    {
        FullScreen.SetIsOnWithoutNotify(fullscreenmode);
        QualitySlider.SetValueWithoutNotify(Quality);
    }

    public void save()
    {
        string[] lines = { MotorType.ToString(), Ratio.ToString(), Mass.ToString(), WheelDiameter.ToString(), FC.ToString(), turningspeed.ToString(), movementspeed.ToString(), Quality.ToString(), fullscreenmode.ToString(), QualityOverride.ToString() };
        File.WriteAllLines(@"C:\Users\Public\RobotStudio_Preferences.txt", lines);
    }

    public void retrieve()
    {
        if (File.Exists(@"C:\Users\Public\RobotStudio_Preferences.txt"))
        {
            string[] lines = File.ReadAllLines(@"C:\Users\Public\RobotStudio_Preferences.txt");
            MotorType = int.Parse(lines[0]);
            Ratio = float.Parse(lines[1]);
            Mass = float.Parse(lines[2]);
            WheelDiameter = float.Parse(lines[3]);
            FC = Boolean.Parse(lines[4]);
            turningspeed = float.Parse(lines[5]);
            movementspeed = float.Parse(lines[6]);
            Quality = int.Parse(lines[7]);
            fullscreenmode = Boolean.Parse(lines[8]);
            QualityOverride = Boolean.Parse(lines[9]);
        }
    }

    public void update_Type(int type)
    {
        try
        {
            MotorType = type;
        }
        catch
        {
            MotorType = 0;
            startDrive();
        }
    }

    public void update_Quality(Single quality)
    {
        QualityOverride = true;
        Quality = (int)quality;
        QualitySettings.SetQualityLevel(Quality, true);
    }

    public void update_Ratio(string drivingtxt)
    {
        try
        {
            Ratio = float.Parse(drivingtxt);
        }
        catch
        {
            Ratio = 20;
            startDrive();
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
            Mass = 38;
            startDrive();
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
            WheelDiameter = 4;
            startDrive();
        }
    }

    public void update_FC(bool FeildCentric)
    {
        FC = FeildCentric;
    }

    public void update_TS(float speed)
    {
        turningspeed = speed;
    }

    public void update_MS(float speed)
    {
        movementspeed = speed;
    }

    public void update_FS(bool fullscreen)
    {
        fullscreenmode = fullscreen;
        Screen.fullScreen = fullscreenmode;
    }
}
