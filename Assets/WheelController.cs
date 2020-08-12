using Assets;
using System;
using System.Threading;
using TMPro;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    public Transform graphicWheel1;
    public Transform graphicWheel2;
    public Transform graphicWheel3;
    public Transform graphicWheel4;

    public Transform LeftOdoWheel;
    public Transform RightOdoWheel;
    public Transform StrafeOdoWheel;

    Vector3 prevPosRight = new Vector3(0.1833609f, 0.0223f, 0.2974554f);
    Vector3 prevPosLeft = new Vector3(-0.1552391f, 0.023f, 0.2963554f);
    Vector3 prevPosStrafe = new Vector3(-0.1666391f, 0.0221f, 0.1974554f);

    public static double encoderCountLeft = 0.0;
    public static double encoderCountRight = 0.0;
    public static double encoderCountStrafe = 0.0;

    public static double heading;
    public static double globalheading;
    public static double startheading;

    public Rigidbody connectedbody;

    public Rigidbody wheel1rb;
    public Rigidbody wheel2rb;
    public Rigidbody wheel3rb;
    public Rigidbody wheel4rb;


    public float friction;

    private const float l = 5.8245f;
    private const float b = 4.811f;

    public ConfigurableJoint wheel1;

    public ConfigurableJoint wheel2;

    public ConfigurableJoint wheel3;

    public ConfigurableJoint wheel4;


    ForceCalculator forceCalc;


    int prevQual = 0;

    Listener list;

    TeleOp tele;
    public GameObject ErrorScreenVersion;
    public static string os;

    private void Awake()
    {
        GameButtons.startheading = 0;
        GameButtons.startpos = Vector3.zero;
        startheading = 0;
        tele = new TeleOp();
        os = SystemInfo.operatingSystem;
    }

    // Start is called before the first frame update
    void Start()
    {
        forceCalc = new ForceCalculator();

        list = new Listener();


        list.StartListener();
    }

    Vector3 forceov = Vector3.zero;
    int prevRate;
    public TMP_Text poseUpdateTxt;

    void Update()
    {
        list.setPose(connectedbody.position * 39.37f);

        poseUpdateTxt.text = "(" + Math.Round((connectedbody.position * 39.37f).x, 1) + ", " + Math.Round((connectedbody.position * 39.37f).z, 1) + ", " + Math.Round(globalheading, 1) + ")";
        if (OptionsInterface.framerate != prevRate)
        {
            if (OptionsInterface.framerate == 0)
            {
                Application.targetFrameRate = 30;
            }
            else if (OptionsInterface.framerate == 1)
            {
                Application.targetFrameRate = 60;
            }
        }
        prevRate = OptionsInterface.framerate;

        if (list.wrongversion)
        {
            ErrorScreenVersion.SetActive(true);
            list.wrongversion = false;
        }

        if (OptionsInterface.Mass != 0)
        {
            connectedbody.mass = OptionsInterface.Mass / 2.205f;
        }

        if (GameButtons.reset)
        {
            list.input_bl = 0;
            list.input_ul = 0;
            list.input_br = 0;
            list.input_ur = 0;

            if ((connectedbody.transform.position - (GameButtons.startpos / 39.37f)).magnitude == 0/* && connectedbody.rotation.eulerAngles.y - GameButtons.startheading == 0.0*/)
            {
                GameButtons.reset = false;
                encoderCountRight = 0.0;
                encoderCountLeft = 0.0;
                encoderCountStrafe = 0.0;
            }
        }

        graphicWheel1.Rotate(new Vector3(getProjected(-wheel1.targetAngularVelocity, -45), 0, 0));
        graphicWheel2.Rotate(new Vector3(getProjected(wheel2.targetAngularVelocity, 45), 0, 0));
        graphicWheel3.Rotate(new Vector3(getProjected(wheel3.targetAngularVelocity, 45), 0, 0));
        graphicWheel4.Rotate(new Vector3(getProjected(-wheel4.targetAngularVelocity, -45), 0, 0));

        if (!OptionsInterface.QualityOverride)
        {
            OptionsInterface.Quality = (int)Math.Round(SystemInfo.batteryLevel * 5);
            if (OptionsInterface.Quality != prevQual)
            {
                QualitySettings.SetQualityLevel((int)Math.Round(SystemInfo.batteryLevel * 5));
            }
            prevQual = OptionsInterface.Quality;
        }
    }

    private void OnEnable()
    {
        tele.Enable();
    }

    private void OnDisable()
    {
        tele.Disable();
    }

    bool prevcld = false;
    public static Vector3 initWheel1 = new Vector3(0, 0, 0);
    public static Vector3 initWheel2 = new Vector3(0, 0, 0);
    public static Vector3 initWheel3 = new Vector3(0, 0, 0);
    public static Vector3 initWheel4 = new Vector3(0, 0, 0);

    // Update is called once per frame
    void FixedUpdate()
    {
        double Wheel1Velo = getVelo(wheel1rb, 45);
        double Wheel2Velo = getVelo(wheel2rb, -45);
        double Wheel3Velo = getVelo(wheel3rb, -45);
        double Wheel4Velo = getVelo(wheel4rb, 45);

        //Getting Different Heading for encoder translation.
        heading = connectedbody.transform.rotation.eulerAngles.y - startheading;
        globalheading = connectedbody.transform.rotation.eulerAngles.y;
        //Debug.Log(heading);

        //Debug.Log(globalheading);
        encoderCountRight += ((rotated(RightOdoWheel.position - prevPosRight, heading * (Math.PI / 180)).z) / 0.05);
        encoderCountLeft += ((rotated(LeftOdoWheel.position - prevPosLeft, heading * (Math.PI / 180)).z) / 0.05);
        encoderCountStrafe += ((rotated(StrafeOdoWheel.position - prevPosStrafe, heading * (Math.PI / 180)).x) / 0.05);

        prevPosRight = RightOdoWheel.position;
        prevPosLeft = LeftOdoWheel.position;
        prevPosStrafe = StrafeOdoWheel.position;

        //Forces
        if (!GameButtons.reset && !list.clientDisconnecting)
        {
            float[] codeForces = { forceCalc.update((float)(list.input_ul), getVelo(wheel1rb, 45)),
                                forceCalc.update((float)(list.input_ur), getVelo(wheel2rb, -45)),
                                forceCalc.update((float)(list.input_bl), getVelo(wheel3rb, -45)),
                                forceCalc.update((float)(list.input_br), getVelo(wheel4rb, 45)) };

            float[] teleForces = { forceCalc.update(setPower(-tele.getOverallVec().x, tele.getOverallVec().z, tele.getOverallVec().y)[1], getVelo(wheel1rb, 45)),
                                forceCalc.update(setPower(-tele.getOverallVec().x, tele.getOverallVec().z, tele.getOverallVec().y)[0], getVelo(wheel2rb, -45)),
                                forceCalc.update(setPower(-tele.getOverallVec().x, tele.getOverallVec().z, tele.getOverallVec().y)[2], getVelo(wheel3rb, -45)),
                                forceCalc.update(setPower(-tele.getOverallVec().x, tele.getOverallVec().z, tele.getOverallVec().y)[3], getVelo(wheel4rb, 45)) };


            forceov = forceCalc.getForce(codeForces[0], codeForces[2], codeForces[1], codeForces[3]) + forceCalc.getForce(teleForces[0], teleForces[2], teleForces[1], teleForces[3]);

            wheel1.targetAngularVelocity = (new Vector3(forceov.z + (forceov.y * (forceCalc.getTrackWidth() + forceCalc.getWheelBase())), 0, forceov.x) * (OptionsInterface.WheelDiameter / 2)); //+
            wheel2.targetAngularVelocity = (new Vector3(forceov.z - (forceov.y * (forceCalc.getTrackWidth() + forceCalc.getWheelBase())), 0, forceov.x) * (OptionsInterface.WheelDiameter / 2)); //-
            wheel3.targetAngularVelocity = (new Vector3(forceov.z - (forceov.y * (forceCalc.getTrackWidth() + forceCalc.getWheelBase())), 0, forceov.x) * (OptionsInterface.WheelDiameter / 2)); //-
            wheel4.targetAngularVelocity = (new Vector3(forceov.z + (forceov.y * (forceCalc.getTrackWidth() + forceCalc.getWheelBase())), 0, forceov.x) * (OptionsInterface.WheelDiameter / 2)); //+
        }
        else if (GameButtons.reset)
        {
            wheel1.targetAngularVelocity = (new Vector3(0, 0, 0));
            wheel2.targetAngularVelocity = (new Vector3(0, 0, 0));
            wheel3.targetAngularVelocity = (new Vector3(0, 0, 0));
            wheel4.targetAngularVelocity = (new Vector3(0, 0, 0));


            connectedbody.velocity = new Vector3(0, 0, 0);
            connectedbody.angularVelocity = new Vector3(0, 0, 0);
            wheel1rb.angularVelocity = (new Vector3(0, 0, 0));
            wheel2rb.angularVelocity = (new Vector3(0, 0, 0));
            wheel3rb.angularVelocity = (new Vector3(0, 0, 0));
            wheel4rb.angularVelocity = (new Vector3(0, 0, 0));

            encoderCountLeft = 0;
            encoderCountRight = 0;
            encoderCountStrafe = 0;

            forceov = Vector3.zero;

            initWheel1 = wheel1rb.position;
            initWheel2 = wheel2rb.position;
            initWheel3 = wheel3rb.position;
            initWheel4 = wheel4rb.position;
        } else if (list.clientDisconnecting)
        {
            float freespeed = (((6600 * (OptionsInterface.WheelDiameter/1.968504f)) / OptionsInterface.Ratio) * 2 * (float)Math.PI) / 60;
            JointDrive drive = new JointDrive();
            drive.positionDamper = (8.75f * OptionsInterface.Ratio) / (OptionsInterface.WheelDiameter/1.968504f) * freespeed;
            drive.maximumForce = 3.402823e+38f;

            float wheel1power = setPower(-tele.getOverallVec().x, tele.getOverallVec().z, tele.getOverallVec().y)[1] * freespeed;
            float wheel2power = setPower(-tele.getOverallVec().x, tele.getOverallVec().z, tele.getOverallVec().y)[0] * freespeed;
            float wheel3power = setPower(-tele.getOverallVec().x, tele.getOverallVec().z, tele.getOverallVec().y)[2] * freespeed;
            float wheel4power = setPower(-tele.getOverallVec().x, tele.getOverallVec().z, tele.getOverallVec().y)[3] * freespeed;

            forceov = getForce(wheel1power, wheel2power, wheel3power, wheel4power);

            wheel1.angularYZDrive = drive;
            wheel2.angularYZDrive = drive;
            wheel3.angularYZDrive = drive;
            wheel4.angularYZDrive = drive;

            wheel1.angularXDrive = drive;
            wheel2.angularXDrive = drive;
            wheel3.angularXDrive = drive;
            wheel4.angularXDrive = drive;

            wheel1.targetAngularVelocity = new Vector3(forceov.z + (forceov.y * (l + b)), 0, forceov.x); //+
            wheel2.targetAngularVelocity = new Vector3(forceov.z - (forceov.y * (l + b)), 0, forceov.x); //-
            wheel3.targetAngularVelocity = new Vector3(forceov.z - (forceov.y * (l + b)), 0, forceov.x); //-
            wheel4.targetAngularVelocity = new Vector3(forceov.z + (forceov.y * (l + b)), 0, forceov.x); //+
        }
    }

    bool first = true;

    public Vector3 getForce(float ul, float bl, float ur, float br)
    {
        Vector3 upleft = new Vector3(0, 0, ul);
        Vector3 backleft = new Vector3(0, 0, bl);
        Vector3 upright = new Vector3(0, 0, ur);
        Vector3 backright = new Vector3(0, 0, br);

        upleft = Quaternion.Euler(0, -45, 0) * upleft;
        backleft = Quaternion.Euler(0, 45, 0) * backleft;
        upright = Quaternion.Euler(0, 45, 0) * upright;
        backright = Quaternion.Euler(0, -45, 0) * backright;

        Vector3 overall = upleft + backleft + upright + backright;
        Vector3 rot = (upleft * (-1 / (l + b))) + (backleft * (-1 / (l + b))) + (upright * (1 / (l + b))) + (backright * (1 / (l + b)));
        Vector3 velo = new Vector3(overall.x, -rot.x, overall.z)/4;

        return velo;
    }

    float getVelo(Rigidbody og, float angle)
    {
        Vector3 dir = Quaternion.Euler(0, (float)heading + angle, 0) * transform.forward;

        return Vector3.Dot(og.velocity, dir);
    }

    float getProjected(Vector3 val, float angle)
    {
        Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.forward;

        return Vector3.Dot(val, dir);
    }

    void OnApplicationQuit()
    {
        Debug.Log("Application ending after " + Time.time + " seconds");
        list.runningstring = "stop";
        Thread.Sleep(10);
        list.stopper = true;
        list.stopListener();
    }

    private Vector3 rotated(Vector3 input, double angle)
    {
        double newX = input.x * Math.Cos(angle) - input.z * Math.Sin(angle);
        double newY = input.x * Math.Sin(angle) + input.z * Math.Cos(angle);
        return new Vector3((float)(newX), input.y, (float)(newY));
    }

    float[] setPower(float x, float y, float rightX)
    {
        float FrontLeftVal = y - x + rightX;
        float FrontRightVal = y + x - rightX;
        float BackLeftVal = y + x + rightX;
        float BackRightVal = y - x - rightX;

        float[] powers = { FrontRightVal, FrontLeftVal, BackLeftVal, BackRightVal };

        return powers;
    }
}
