using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsInterface : MonoBehaviour
{
    public static int MotorType = 0;

    public static float Ratio = 20;

    public static float Mass = 40;

    public static float WheelDiameter = 2;

    public static bool FC = false;
    public static float turningspeed = 1.0f;
    public static float movementspeed = 1.0f;
    public static int DriveType = 0;

    public static int Quality = 0;
    public static bool QualityOverride = false;
    public bool fullscreenmode = false;
    public static int framerate = 0;

    public TMP_Dropdown MotorTypeIN;
    //public TMP_Dropdown DriveTypeIN;
    public TMP_InputField RatioInput;
    public TMP_InputField MassInput;
    public TMP_InputField WheelDiameterInput;
    public TMP_InputField freespeedInput;
    public TMP_InputField stalltorqueInput;
    public TMP_InputField dtWidthInput;

    public Toggle FCToggle;
    public Slider MovementSpeed;
    public Slider TurningSpeed;

    public Slider QualitySlider;
    public Toggle FullScreen;
    public TMP_Dropdown FrameRateDrp;
    public GameObject customMotorPopup;
    public static float freespeed = 5475.764f;
    public static float stallTorque = 24.49886428410438f;
    public static float dtWidth = 1;

    public void Awake()
    {
        Quality = 0;
        retrieve();
    }

    public void startDrive()
    {
        //    DriveTypeIN.SetValueWithoutNotify(DriveType);
        MotorTypeIN.SetValueWithoutNotify(MotorType);
        RatioInput.SetTextWithoutNotify(Ratio.ToString());
        MassInput.SetTextWithoutNotify(Mass.ToString());
        WheelDiameterInput.SetTextWithoutNotify(WheelDiameter.ToString());
        freespeedInput.SetTextWithoutNotify(freespeed.ToString());
        stalltorqueInput.SetTextWithoutNotify(stallTorque.ToString());
        dtWidthInput.SetTextWithoutNotify((dtWidth * 18.0).ToString());

        if (MotorType == 3)
        {
            customMotorPopup.SetActive(true);
        }
        else
        {
            customMotorPopup.SetActive(false);
        }
    }

    public void startControls()
    {
        FrameRateDrp.SetValueWithoutNotify(framerate);
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
        string[] lines = { MotorType.ToString(), Ratio.ToString(), Mass.ToString(), WheelDiameter.ToString(), FC.ToString(), turningspeed.ToString(), movementspeed.ToString(), Quality.ToString(), fullscreenmode.ToString(), QualityOverride.ToString(), DriveType.ToString(), framerate.ToString(), freespeed.ToString(), stallTorque.ToString() };
        File.WriteAllLines(Application.persistentDataPath + "/RobotStudio_Preferences.txt", lines);
    }

    public void retrieve()
    {
        Debug.Log(Application.persistentDataPath + "/RobotStudio_Preferences.txt");
        if (File.Exists(Application.persistentDataPath + "/RobotStudio_Preferences.txt"))
        {
            try
            {
                string[] lines = File.ReadAllLines(Application.persistentDataPath + "/RobotStudio_Preferences.txt");
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
                DriveType = int.Parse(lines[10]);
                framerate = int.Parse(lines[11]);
                freespeed = float.Parse(lines[12]);
                stallTorque = float.Parse(lines[13]);
            }
            catch
            {
                Debug.Log("Could not retrieve all data from file.");
                save();
            }
        }
    }

    public void update_framerate(int rate)
    {
        framerate = rate;
    }

    public void update_dtwidth(string width)
    {
        if (float.Parse(width) >= 11 && float.Parse(width) <= 18)
        {
            dtWidth = float.Parse(width) / 18;
        }
        else
        {
            dtWidth = 1;
        }
    }

    public void update_Type(int type)
    {
        MotorType = type;
        if (type == 3)
        {
            customMotorPopup.SetActive(true);
        }
        else
        {
            customMotorPopup.SetActive(false);
            switch (type)
            {
                case 0:
                    freespeed = 5475.764f;
                    stallTorque = 24.49886428410438f;
                    break;
                case 1:
                    freespeed = 5994;
                    stallTorque = 20.2680776774f;
                    break;
                case 2:
                    freespeed = 6000;
                    stallTorque = 14.8692528891963f;
                    break;
            }
        }
    }


    public void update_stall_torque(string mystalltorque)
    {
        if (float.Parse(mystalltorque) <= 1000 && float.Parse(mystalltorque) >= 0)
        {
            stallTorque = float.Parse(mystalltorque);
        }
        else
        {
            stallTorque = 8.75f;
            startDrive();
        }
    }

    public void update_free_speed(string myfreespeed)
    {
        if (float.Parse(myfreespeed) <= 100000 && float.Parse(myfreespeed) >= 0)
        {
            freespeed = float.Parse(myfreespeed);
        }
        else
        {
            freespeed = 5475.764f;
            startDrive();
        }
    }

    public void update_DType(int type)
    {
        DriveType = type;
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
            if (Ratio <= 1000)
            {
                Ratio = float.Parse(drivingtxt);
            }
            else
            {
                Ratio = 20;
                startDrive();
            }
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
