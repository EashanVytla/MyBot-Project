using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleTCP;
using System.Text;
using System.Net.Sockets;
using System.Net;
using Assets;

public class BlockController : MonoBehaviour
{
    Vector3 velocityInSeconds;
    Vector3 prevPosRight = new Vector3(0, 0, 0);
    Vector3 prevPosLeft = new Vector3(0, 0, 0);
    Vector3 prevPosStrafe = new Vector3(0, 0, 0);
    public static double encoderCountLeft = 0.0;
    public static double encoderCountRight = 0.0;
    public static double encoderCountStrafe = 0.0;
    bool first = true;

    public static double heading;
    public static double startheading;

    public float angDrag;
    public float spinForce;
    public float maxAngVel;
    public static float signalScale = 6;


    Vector3 targetPosition = new Vector3();
    Vector3 lookAtTarget = new Vector3();
    Quaternion playerRot = new Quaternion();

    float rotSpeed = 5;
    float speed = 10;
    bool moving = false;
    Rigidbody rb;

    public Transform LeftOdoWheel;
    public Transform RightOdoWheel;
    public Transform StrafeOdoWheel;

    float strafePForce;
    float strafeNForce;
    float forwardForce;
    float backwardForce;

    float maxspeed = 0.3f;
    float rotationalForce;
    float Speed;
    float AngularSpeed;
    public static Vector3 signalForce = new Vector3(0, 0);
    public static float signaltorque = 0;
    public static SimpleTcpServer server = new SimpleTcpServer();
    string status;
    Vector3 prevvel;
    Listener list = new Listener();

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        list.StartListener();
    }
    
    int monitor;

    int counter = 0;

    // Update is called once per frame
    void Update()
    {
        Vector3 deltaPosRight = RightOdoWheel.position - prevPosRight;
        Vector3 deltaPosLeft = LeftOdoWheel.position - prevPosLeft;
        Vector3 deltaPosStrafe = StrafeOdoWheel.position - prevPosStrafe;

        heading = transform.rotation.eulerAngles.y - startheading;

        prevPosRight = RightOdoWheel.position;
        prevPosLeft = LeftOdoWheel.position;
        prevPosStrafe = StrafeOdoWheel.position;

        encoderCountRight += ((rotated(deltaPosRight, heading * (Math.PI / 180)).z) / 0.05) / 10;
        encoderCountLeft += ((rotated(deltaPosLeft, heading * (Math.PI / 180)).z) / 0.05) / 10;
        encoderCountStrafe += ((rotated(deltaPosStrafe, heading * (Math.PI / 180)).x) / 0.05) / 10;
        if (first)
        {
            encoderCountRight = 0.0;
            encoderCountLeft = 0.0;
            encoderCountStrafe = 0.0;
            first = false;
        }
        //Debug.Log("Right: " + encoderCountRight);
        //Debug.Log("Left: " + encoderCountLeft);
        //Debug.Log("Strafe: " + encoderCountStrafe);

        if (Input.GetKey(KeyCode.Q))
        {
            if (server.IsStarted)
            {
                server.Stop();
            }
            Application.Quit();
        }
    }

    void FixedUpdate()
    {

        if (Input.GetKey(KeyCode.UpArrow))
        {
            strafePForce = 3f;
        }
        
        if (Input.GetKey(KeyCode.DownArrow))
        {
            strafeNForce = -3f;
        }
        
        if (Input.GetKey(KeyCode.RightArrow))
        {
            forwardForce = 3f;
        }
        
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            backwardForce = -3f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            rotationalForce = -spinForce;
        }

        if (Input.GetKey(KeyCode.D))
        {
            rotationalForce = spinForce;
        }

        if (!Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            strafePForce = 0.0f;
            strafeNForce = 0.0f;
            forwardForce = 0.0f;
            backwardForce = 0.0f;
            rotationalForce = 0.0f;
        }

        Speed = rb.velocity.magnitude;
        AngularSpeed = rb.angularVelocity.magnitude / (float)(signaltorque * (Math.PI * 2));

        rb.angularVelocity = new Vector3(0, (float)(signaltorque * (Math.PI * 2)), 0);

        rb.SetMaxAngularVelocity(maxAngVel);

        //Debug.Log(signalForce);
        //Debug.Log(forwardForce);
        //Debug.Log(backwardForce);
        //Debug.Log(strafeNForce);
        //Debug.Log(strafePForce);
        //Debug.Log(signalForce.magnitude);
        if (rb.velocity.magnitude < signalForce.magnitude + 2)
        {
            rb.AddForce(signalForce.x, 0, signalForce.y, ForceMode.VelocityChange);
        }
        //Debug.Log(signaltorque);

        rb.angularDrag = angDrag;
        rb.AddTorque(0, 10 * signaltorque, 0);
        //rb.AddTorque(0, 1000000, 0, ForceMode.VelocityChange);
        //if (rb.velocity.magnitude < 4)
        //{
        //    rb.AddForce(forwardForce + backwardForce, 0, strafePForce + strafeNForce, ForceMode.VelocityChange);
        //}
        

    }

    void SetTargetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, 1000))
        {
            targetPosition = hit.point;
            lookAtTarget = new Vector3(targetPosition.x - transform.position.x,
                transform.position.y,
                targetPosition.z - transform.position.z);
            //this.transform.LookAt(targetPosition);
            playerRot = Quaternion.LookRotation(lookAtTarget);
            moving = true;
        }
    }

    void Move()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, playerRot, rotSpeed * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        if(distTo(targetPosition, transform.position) <= 0.1)
        {
            moving = false;
        }
    }

    private double distTo(Vector3 target, Vector3 current)
    {
        return Math.Sqrt(Math.Pow(target.x - current.x, 2.0) + Math.Pow(target.x - current.x, 2.0));
    }

    public Vector3 rotated(Vector3 input, double angle)
    {
        double newX = input.x * Math.Cos(angle) - input.z * Math.Sin(angle);
        double newY = input.x * Math.Sin(angle) + input.z * Math.Cos(angle);
        return new Vector3((float)(newX), input.y, (float)(newY));
    }
}
