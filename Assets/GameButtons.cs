using ICSharpCode.SharpZipLib.Zip;
using SFB;
using System.IO;
using System.Net;
using System.Resources;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameButtons : MonoBehaviour
{
    public Transform robotPos;
    //public Transform Blocks;
    public static Vector3 startpos = new Vector3(0, 0.3255587f, 0);
    public static float startheading = 0;
    public static bool reset = false;
    public Rigidbody rb;
    public Rigidbody wheel1;
    public Rigidbody wheel2;
    public Rigidbody wheel3;
    public Rigidbody wheel4;
    public static int menuChoice = -1;

    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;
    public TMP_Text Version;
    public TMP_Text currentVersionTXT;
    public readonly string CURRENT_VERSION = "1.1.5";
    public GameObject MiniUpdateNotification;


    public TMP_InputField x;
    public TMP_InputField y;
    public TMP_InputField z;

    public void ResetClick()
    {
        reset = true;
        wheel1.position = (Quaternion.Euler(0, startheading, 0) * new Vector3(0.2f, 0, 0.2f)) + (startpos / 39.37f);
        wheel2.position = (Quaternion.Euler(0, startheading, 0) * new Vector3(-0.2f, 0, 0.2f)) + (startpos / 39.37f);
        wheel3.position = (Quaternion.Euler(0, startheading, 0) * new Vector3(-0.2f, 0, -0.2f)) + (startpos / 39.37f);
        wheel4.position = (Quaternion.Euler(0, startheading, 0) * new Vector3(0.2f, 0, -0.2f)) + (startpos / 39.37f);

        WheelController.initWheel1 = (Quaternion.Euler(0, startheading, 0) * new Vector3(0.2f, 0, 0.2f)) + (startpos / 39.37f);
        WheelController.initWheel2 = (Quaternion.Euler(0, startheading, 0) * new Vector3(-0.2f, 0, 0.2f)) + (startpos / 39.37f);
        WheelController.initWheel3 = (Quaternion.Euler(0, startheading, 0) * new Vector3(-0.2f, 0, -0.2f)) + (startpos / 39.37f);
        WheelController.initWheel4 = (Quaternion.Euler(0, startheading, 0) * new Vector3(0.2f, 0, -0.2f)) + (startpos / 39.37f);

        robotPos.SetPositionAndRotation(startpos / 39.37f, Quaternion.Euler(0, startheading, 0));
        rb.velocity = new Vector3(0, 0, 0);
        WheelController.encoderCountLeft = 0;
        WheelController.encoderCountRight = 0;
        WheelController.encoderCountStrafe = 0;
        WheelController.startheading = startheading;
    }

    public void LateUpdate()
    {
        switch (menuChoice)
        {
            case 0:
                button1.Select();
                break;
            case 1:
                button2.Select();
                break;
            case 2:
                button3.Select();
                break;
            case 3:
                button4.Select();
                break;
        }
    }

    public void Start()
    {
        if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork || Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            RemoteSettings.Updated += onRemoteSettingsUpdated;
            onRemoteSettingsUpdated();
        }
        else
        {
            onRemoteSettingsUpdated();
        }
    }

    public Button createBtn;
    public TMP_Text text;
    public TMP_Text title;
    public GameObject canvas;

    public void CreateProj()
    {
        createBtn.interactable = false;
        if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork || Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            string path = "";
            path = StandaloneFileBrowser.SaveFilePanel("Select Project location", "", "MyBotProject", "");

            if (path.Length != 0)
            {
                WebClient webClient = new WebClient();
                webClient.DownloadFile("https://github.com/EashanVytla/MyBotSDK-Java/archive/master.zip", path + ".zip");

                var zipFileName = path + ".zip";
                string[] pathname = path.Split(Path.DirectorySeparatorChar);

                string desiredName = pathname[pathname.Length - 1];
                string targetDir = path.Remove(path.Length - desiredName.Length, desiredName.Length);
                FastZip fastZip = new FastZip();
                string fileFilter = null;

                // Will always overwrite if target filenames already exist
                fastZip.ExtractZip(zipFileName, targetDir, fileFilter);

                //Delete the zip file once we are done with it
                File.Delete(path + ".zip");

                //Rename the file to the user's desired file name
                File.Move(targetDir + "MyBotSDK-Java-master", targetDir + desiredName);
                File.Delete(targetDir + "MyBotSDK-Java-master");

                canvas.SetActive(true);
                title.text = "Project Status";
                text.text = "You project has succesfully been created at:\n" + targetDir + desiredName + ".\nPlease open it as a project in IntelliJ IDEA.";
            }
        }
        else
        {
            canvas.SetActive(true);
            title.text = "Error";
            text.text = "Please connect to internet to compelete this process.";
        }
        createBtn.interactable = true;
    }

    public void Update()
    {
        currentVersionTXT.text = "v" + CURRENT_VERSION;
    }

    public GameObject SkystoneField;
    public GameObject UlitmateGoalField;
    public GameObject FreightFrenzyField;
    int fieldNum = 2;

    public void changeField(int fieldNum)
    {
        this.fieldNum = fieldNum;
        if (fieldNum == 0)
        {
            startpos.x = 36;
            SkystoneField.active = true;
            UlitmateGoalField.active = false;
            FreightFrenzyField.active = false;
        }
        else if (fieldNum == 1)
        {
            startpos.x = 50;
            SkystoneField.active = false;
            UlitmateGoalField.active = true;
            FreightFrenzyField.active = false;
        }
        else
        {
            startpos.x = -32.8f;
            SkystoneField.active = false;
            UlitmateGoalField.active = false;
            FreightFrenzyField.active = true;
        }
        ResetClick();
    }

    private void onRemoteSettingsUpdated()
    {

        string[] currentVersionF = CURRENT_VERSION.Split('.');
        string[] latestVersionF = RemoteSettings.GetString("AppVersion", "1.0.0").Split('.');
        float currentversionvalue = float.Parse(currentVersionF[0]) + (float.Parse(currentVersionF[1]) / 10) + (float.Parse(currentVersionF[2]) / 100);
        float latestversionvalue = float.Parse(latestVersionF[0]) + (float.Parse(latestVersionF[1]) / 10) + (float.Parse(latestVersionF[2]) / 100);
        Debug.Log(currentversionvalue + ", " + latestversionvalue);
        if (latestversionvalue > currentversionvalue)
        {
            MiniUpdateNotification.SetActive(true);
        }
        else
        {
            MiniUpdateNotification.SetActive(false);
        }
        Version.text = "MyBot Update Available: " + "v" + RemoteSettings.GetString("AppVersion", "1.0.0");
    }

    public void updateMenuButton(int view)
    {
        menuChoice = view;
    }

    public void goToSite()
    {
        Application.OpenURL("http://www.robotstudiosimulator.com/");
    }

    public void goToDownload()
    {
        Application.OpenURL("https://www.robotstudiosimulator.com/download.html");
    }

    public void goToDoc()
    {
        Application.OpenURL("https://eashan-vytla.gitbook.io/debuggerappmaster/");
    }

    public void goToDiscord()
    {
        Application.OpenURL("https://discord.gg/Vx94FxV");
    }


    public void exit()
    {
        ResetClick();
        SceneManager.LoadScene("Menu 3D");
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


    public void updateStartPosTXT()
    {
        if (startpos.x != 0)
        {
            x.SetTextWithoutNotify(startpos.x.ToString());
        }

        if (startpos.z != 0)
        {
            y.SetTextWithoutNotify(startpos.z.ToString());
        }

        if (startheading != 0)
        {
            z.SetTextWithoutNotify(startheading.ToString());
        }
    }

    public void startFeild()
    {
        Debug.Log("Sup");
        if (fieldNum == 0)
        {
            startpos.x = 36;
        }
        else if (fieldNum == 1)
        {
            startpos.x = 50;
        }
        else
        {
            startpos.x = -32.8f;
        }
        startpos.z = -52;
        startheading = 0;
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
