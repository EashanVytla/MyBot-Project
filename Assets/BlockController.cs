//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using SimpleTCP;
//using System.Text;
//using System.Net.Sockets;
//using System.Net;
//using Assets;
//using System.Threading;

//public class BlockController : MonoBehaviour
//{
//    float brakeFactor;
//    Vector3 prevPosRight = new Vector3(0, 0, 0);
//    Vector3 prevPosLeft = new Vector3(0, 0, 0);
//    Vector3 prevPosStrafe = new Vector3(0, 0, 0);
//    public static double encoderCountLeft = 0.0;
//    public static double encoderCountRight = 0.0;
//    public static double encoderCountStrafe = 0.0;

//    public static double heading;
//    public static double globalheading;
//    public static double startheading;

//    public float angDrag;
//    public float maxAngVel;
//    public static float signalScale = 2 * (float)(1.8/2.065);
//    public static float newsignalScale = 2 * (float)(1.8/2.065);

//    Rigidbody rb;

//    public Transform LeftOdoWheel;
//    public Transform RightOdoWheel;
//    public Transform StrafeOdoWheel;

//    public static Vector3 signalForce = new Vector3(0, 0);
//    public static float signaltorque = 0;
//    public static SimpleTcpServer server;
//    Listener list;
//    DTWheels dtw;
//    ForceCalculator forceCalc;

//    // Start is called before the first frame update
//    void Start()
//    {
//        dtw = new DTWheels();
//        forceCalc = new ForceCalculator();

//        server = new SimpleTcpServer();
//        list = new Listener();

//        rb = GetComponent<Rigidbody>();

//        //rb.centerOfMass = new Vector3(0, 0.3255587f, 0);
//        list.StartListener();
//    }

//    public float torque;

//    // Update is called once per frame
//    void Update()
//    {
//        if(OptionsInterface.Mass != 0)
//        {
//            rb.mass = OptionsInterface.Mass / 2.205f;
//        }

//        //Odometry Delta Update
//        Vector3 deltaPosRight = RightOdoWheel.position - prevPosRight;
//        Vector3 deltaPosLeft = LeftOdoWheel.position - prevPosLeft;
//        Vector3 deltaPosStrafe = StrafeOdoWheel.position - prevPosStrafe;

//        //Getting Different Heading for encoder translation.
//        heading = (transform.localRotation.eulerAngles.y - 180) - startheading;
//        globalheading = transform.rotation.eulerAngles.y;
//        ///Debug.Log(heading);

//        //Debug.Log(globalheading);

//        prevPosRight = RightOdoWheel.position;
//        prevPosLeft = LeftOdoWheel.position;
//        prevPosStrafe = StrafeOdoWheel.position;

//        encoderCountRight += ((rotated(deltaPosRight, heading * (Math.PI / 180)).z) / 0.05) / 10;
//        encoderCountLeft += ((rotated(deltaPosLeft, heading * (Math.PI / 180)).z) / 0.05) / 10;
//        encoderCountStrafe += ((rotated(deltaPosStrafe, heading * (Math.PI / 180)).x) / 0.05) / 10;
//        if (GameButtons.reset)
//        {
//            encoderCountRight = 0.0;
//            encoderCountLeft = 0.0;
//            encoderCountStrafe = 0.0;
//            GameButtons.reset = false;
//        }

//        if (Input.GetKey(KeyCode.Q))
//        {
//            if (server.IsStarted)
//            {
//                server.Stop();
//            }
//            Application.Quit();
//        }
//    }

//    void FixedUpdate()
//    {
//        /*Vector3 force = forceCalc.getForce(forceCalc.update((float)list.input_ul, dtw.getULVelo()), 
//                                            forceCalc.update((float)list.input_bl, dtw.getBLVelo()), 
//                                            forceCalc.update((float)list.input_ur, dtw.getURVelo()), 
//                                            forceCalc.update((float)list.input_br, dtw.getBRVelo()));*/

//        Vector3 force = forceCalc.getForce(forceCalc.update((float)list.input_ul, rb.GetPointVelocity(new Vector3(0.173f, 0.0276f, -0.175f)).magnitude),
//                                            forceCalc.update((float)list.input_bl, rb.GetPointVelocity(new Vector3(-0.175f, 0.0276f, -0.1718f)).magnitude),
//                                            forceCalc.update((float)list.input_ur, rb.GetPointVelocity(new Vector3(-0.176f, 0.0276f, -0.174f)).magnitude),
//                                            forceCalc.update((float)list.input_br, rb.GetPointVelocity(new Vector3(0.176f, 0.0276f, 0.1718f)).magnitude));

//        Debug.Log("Force" + force);

//        rb.AddForce(new Vector3(force.x, 0, force.z), ForceMode.VelocityChange);

//        double magnitude = rb.velocity.magnitude;

//        //Rotational:
//        rb.SetMaxAngularVelocity(maxAngVel);
//        rb.angularDrag = angDrag;
//        //rb.AddRelativeTorque(0, torque * ((force.y + (tele.getTorque()))), 0, ForceMode.VelocityChange);
//    }

//    void OnApplicationQuit()
//    {
//        Debug.Log("Application ending after " + Time.time + " seconds");
//        list.runningstring = "stop";
//        Thread.Sleep(10);
//        list.stopper = true;
//    }

//    private Vector3 rotated(Vector3 input, double angle)
//    {
//        double newX = input.x * Math.Cos(angle) - input.z * Math.Sin(angle);
//        double newY = input.x * Math.Sin(angle) + input.z * Math.Cos(angle);
//        return new Vector3((float)(newX), input.y, (float)(newY));
//    }
//}
