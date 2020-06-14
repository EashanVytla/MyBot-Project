using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

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

    public Transform wheel1t;
    public Transform wheel2t;
    public Transform wheel3t;
    public Transform wheel4t;

    public ConfigurableJoint wheel1;

    public ConfigurableJoint wheel2;

    public ConfigurableJoint wheel3;

    public ConfigurableJoint wheel4;

    ForceCalculator forceCalc;

    float x, y, z;

    int prevQual = 0;

    Listener list;

    TeleOp tele;
    public GameObject ErrorScreenVersion;

    private void Awake()
    {
        GameButtons.startheading = 0;
        GameButtons.startpos = Vector3.zero;
        startheading = 0;
        tele = new TeleOp();
    }

    // Start is called before the first frame update
    void Start()
    {
        forceCalc = new ForceCalculator();

        list = new Listener();

        list.StartListener();
    }

    Vector3 forceov = Vector3.zero;


    void Update()
    {
        if (list.wrongversion)
        {
            ErrorScreenVersion.SetActive(true);
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

            if ((connectedbody.transform.position - (GameButtons.startpos / 10)).magnitude == 0/* && connectedbody.rotation.eulerAngles.y - GameButtons.startheading == 0.0*/)
            {
                GameButtons.reset = false;
                encoderCountRight = 0.0;
                encoderCountLeft = 0.0;
                encoderCountStrafe = 0.0;
            }
        }

        graphicWheel1.Rotate(new Vector3((getProjected(wheel1.targetAngularVelocity, 45) * 180 / Mathf.PI) * 10 * Time.deltaTime, 0, 0));
        graphicWheel2.Rotate(new Vector3((getProjected(wheel2.targetAngularVelocity, -45) * 180 / Mathf.PI) * 10 * Time.deltaTime, 0, 0));
        graphicWheel3.Rotate(new Vector3((getProjected(wheel3.targetAngularVelocity, 45) * 180 / Mathf.PI) * 10 * Time.deltaTime, 0, 0));
        graphicWheel4.Rotate(new Vector3((getProjected(wheel4.targetAngularVelocity, -45) * 180 / Mathf.PI) * 10 * Time.deltaTime, 0, 0));

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

    // Update is called once per frame
    void FixedUpdate()
    {
        //Odometry Delta Update
        Vector3 deltaPosRight = RightOdoWheel.position - prevPosRight;
        Vector3 deltaPosLeft = LeftOdoWheel.position - prevPosLeft;
        Vector3 deltaPosStrafe = StrafeOdoWheel.position - prevPosStrafe;

        prevPosRight = RightOdoWheel.position;
        prevPosLeft = LeftOdoWheel.position;
        prevPosStrafe = StrafeOdoWheel.position;

        //Getting Different Heading for encoder translation.
        heading = connectedbody.transform.rotation.eulerAngles.y - startheading;
        globalheading = connectedbody.transform.rotation.eulerAngles.y;
        //Debug.Log(heading);

        //Debug.Log(globalheading);
        encoderCountRight += ((rotated(deltaPosRight, heading * (Math.PI / 180)).z) / 0.05);
        encoderCountLeft += ((rotated(deltaPosLeft, heading * (Math.PI / 180)).z) / 0.05);
        encoderCountStrafe += ((rotated(deltaPosStrafe, heading * (Math.PI / 180)).x) / 0.05);
        list.setPose(connectedbody.position * 39.37f);


        //Forces
        if (!GameButtons.reset && !list.clientDisconnecting)
        {
            float[] codeForces = { forceCalc.update((float)(list.input_ur), getVelo(wheel1rb, 45)),
                                forceCalc.update((float)(list.input_ul), getVelo(wheel2rb, -45)),
                                forceCalc.update((float)(list.input_bl), getVelo(wheel3rb, 45)),
                                forceCalc.update((float)(list.input_br), getVelo(wheel4rb, -45)) };

            float[] teleForces = { forceCalc.update(setPower(-tele.getOverallVec().x, tele.getOverallVec().z, tele.getOverallVec().y)[0], getVelo(wheel1rb, 45)),
                                forceCalc.update(setPower(-tele.getOverallVec().x, tele.getOverallVec().z, tele.getOverallVec().y)[1], getVelo(wheel2rb, -45)),
                                forceCalc.update(setPower(-tele.getOverallVec().x, tele.getOverallVec().z, tele.getOverallVec().y)[2], getVelo(wheel3rb, 45)),
                                forceCalc.update(setPower(-tele.getOverallVec().x, tele.getOverallVec().z, tele.getOverallVec().y)[3], getVelo(wheel4rb, -45)) };


            forceov = forceCalc.getForce(codeForces[1], codeForces[2], codeForces[0], codeForces[3]) + forceCalc.getForce(teleForces[1], teleForces[2], teleForces[0], teleForces[3]);

            wheel1.targetAngularVelocity = (new Vector3(forceov.z + (forceov.y * (forceCalc.getTrackWidth() + forceCalc.getWheelBase())), 0, forceov.x) * (OptionsInterface.WheelDiameter / 2)); //+
            wheel2.targetAngularVelocity = (new Vector3(forceov.z - (forceov.y * (forceCalc.getTrackWidth() + forceCalc.getWheelBase())), 0, forceov.x) * (OptionsInterface.WheelDiameter / 2)); //-
            wheel3.targetAngularVelocity = (new Vector3(forceov.z - (forceov.y * (forceCalc.getTrackWidth() + forceCalc.getWheelBase())), 0, forceov.x) * (OptionsInterface.WheelDiameter / 2)); //-
            wheel4.targetAngularVelocity = (new Vector3(forceov.z + (forceov.y * (forceCalc.getTrackWidth() + forceCalc.getWheelBase())), 0, forceov.x) * (OptionsInterface.WheelDiameter / 2)); //+
        }
        else if(GameButtons.reset)
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

            forceov = Vector3.zero;
        }else if (list.clientDisconnecting)
        {
            float[] teleForces = { forceCalc.update(setPower(-tele.getOverallVec().x, tele.getOverallVec().z, tele.getOverallVec().y)[0], getVelo(wheel1rb, 45)),
                                forceCalc.update(setPower(-tele.getOverallVec().x, tele.getOverallVec().z, tele.getOverallVec().y)[1], getVelo(wheel2rb, -45)),
                                forceCalc.update(setPower(-tele.getOverallVec().x, tele.getOverallVec().z, tele.getOverallVec().y)[2], getVelo(wheel3rb, 45)),
                                forceCalc.update(setPower(-tele.getOverallVec().x, tele.getOverallVec().z, tele.getOverallVec().y)[3], getVelo(wheel4rb, -45)) };

            forceov = forceCalc.getForce(teleForces[1], teleForces[2], teleForces[0], teleForces[3]);

            wheel1.targetAngularVelocity = (new Vector3(forceov.z + (forceov.y * (forceCalc.getTrackWidth() + forceCalc.getWheelBase())), 0, forceov.x) * (OptionsInterface.WheelDiameter / 2)); //+
            wheel2.targetAngularVelocity = (new Vector3(forceov.z - (forceov.y * (forceCalc.getTrackWidth() + forceCalc.getWheelBase())), 0, forceov.x) * (OptionsInterface.WheelDiameter / 2)); //-
            wheel3.targetAngularVelocity = (new Vector3(forceov.z - (forceov.y * (forceCalc.getTrackWidth() + forceCalc.getWheelBase())), 0, forceov.x) * (OptionsInterface.WheelDiameter / 2)); //-
            wheel4.targetAngularVelocity = (new Vector3(forceov.z + (forceov.y * (forceCalc.getTrackWidth() + forceCalc.getWheelBase())), 0, forceov.x) * (OptionsInterface.WheelDiameter / 2)); //+
        }
    }

    float getVelo(Rigidbody og, float angle)
    {
        Vector3 dir = Quaternion.Euler(0, (float)heading + angle, 0) * transform.forward;

        return Vector3.Dot(og.velocity, dir);
    }

    float getProjected(Vector3 val, float angle)
    {
        Vector3 dir = Quaternion.Euler(0, (float)heading + angle, 0) * transform.forward;

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
