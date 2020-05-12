using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class WheelController : MonoBehaviour
{
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
    Vector3 prevresetpos = Vector3.zero;

    float x, y, z;

    Listener list;

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

            if (prevresetpos == connectedbody.transform.position)
            {
                encoderCountRight = 0.0;
                encoderCountLeft = 0.0;
                encoderCountStrafe = 0.0;
                connectedbody.velocity = new Vector3(0, 0, 0);
                connectedbody.angularVelocity = new Vector3(0, 0, 0);
                wheel1rb.velocity = (new Vector3(0, 0, 0));
                wheel2rb.velocity = (new Vector3(0, 0, 0));
                wheel3rb.velocity = (new Vector3(0, 0, 0));
                wheel4rb.velocity = (new Vector3(0, 0, 0));
                GameButtons.reset = false;
            }
        }
        prevresetpos = connectedbody.transform.position;
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
        Debug.Log(encoderCountLeft);

        //Forces
        if (!GameButtons.reset)
        {
            float force1 = forceCalc.update((float)(list.input_ur), getVelo(wheel1rb, 45));
            float force2 = forceCalc.update((float)(list.input_ul), getVelo(wheel2rb, -45));
            float force3 = forceCalc.update((float)(list.input_bl), getVelo(wheel3rb, 45));
            float force4 = forceCalc.update((float)(list.input_br), getVelo(wheel4rb, -45));

            forceov = forceCalc.getForce(force2, force3, force1, force4);

            //Debug.Log(forceov);

            wheel1.targetAngularVelocity = (new Vector3(forceov.z + (forceov.y * (forceCalc.getTrackWidth() + forceCalc.getWheelBase())), 0, forceov.x) * (OptionsInterface.WheelDiameter / 2)); //+
            wheel2.targetAngularVelocity = (new Vector3(forceov.z - (forceov.y * (forceCalc.getTrackWidth() + forceCalc.getWheelBase())), 0, forceov.x) * (OptionsInterface.WheelDiameter / 2)); //-
            wheel3.targetAngularVelocity = (new Vector3(forceov.z - (forceov.y * (forceCalc.getTrackWidth() + forceCalc.getWheelBase())), 0, forceov.x) * (OptionsInterface.WheelDiameter / 2)); //-
            wheel4.targetAngularVelocity = (new Vector3(forceov.z + (forceov.y * (forceCalc.getTrackWidth() + forceCalc.getWheelBase())), 0, forceov.x) * (OptionsInterface.WheelDiameter / 2)); //+
        }
        else
        {
            wheel1rb.velocity = (new Vector3(0, 0, 0));
            wheel2rb.velocity = (new Vector3(0, 0, 0));
            wheel3rb.velocity = (new Vector3(0, 0, 0));
            wheel4rb.velocity = (new Vector3(0, 0, 0));

            connectedbody.velocity = new Vector3(0, 0, 0);
            connectedbody.angularVelocity = new Vector3(0, 0, 0);

            forceov = Vector3.zero;
        }
    }

    float getVelo(Rigidbody og, float angle)
    {
        Vector3 dir = Quaternion.Euler(0, (float)heading + angle, 0) * transform.forward;

        return Vector3.Dot(og.velocity, dir);
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
}
