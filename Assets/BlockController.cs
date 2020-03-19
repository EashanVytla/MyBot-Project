using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleTCP;
using System.Text;

public class BlockController : MonoBehaviour
{
    Vector3 targetPosition = new Vector3();
    Vector3 lookAtTarget = new Vector3();
    Quaternion playerRot = new Quaternion();
    float rotSpeed = 5;
    float speed = 10;
    bool moving = false;
    Rigidbody rb;
    float strafePForce;
    float strafeNForce;
    float forwardForce;
    float backwardForce;
    float maxspeed = 0.3f;
    float rotationalForce;
    float Speed;
    float AngularSpeed;
    Vector3 signalForce = new Vector3(0, 0);
    float signaltorque = 0;
    SimpleTcpServer server = new SimpleTcpServer();
    string status;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        server.Delimiter = 0x13;//enter
        server.StringEncoder = Encoding.UTF8;

        System.Net.IPAddress ip = System.Net.IPAddress.Parse("192.168.68.120");
        server.Start(ip, Convert.ToInt32(8719));
    }

    char[] message;
    string messageFull;
    int monitor;

    private void Server_DataRecieved(object sender, SimpleTCP.Message e)
    {
        //Debug.Log("Status: ");
        status = e.MessageString;
        Debug.Log(status);
        message = status.ToCharArray();
        e.ReplyLine(string.Format("You Said: {0}", e.MessageString));
        messageFull = "";
        for (int i = 1; i < (status.Length - 1); i++)
        {
            messageFull += message[i];
        }

        if (message[0] == 'p')
        {
            signalForce.x = float.Parse(message[1].ToString() + message[2].ToString() + message[3].ToString());
            signalForce.y = float.Parse(message[4].ToString() + message[5].ToString() + message[6].ToString());
            signaltorque = float.Parse(message[7].ToString() + message[8].ToString() + message[9].ToString());
        }
        else if(message[0] == 'y')
        {
            signalForce.y = float.Parse(messageFull);
        }else if(message[0] == 'z')
        {
            signaltorque = float.Parse(messageFull);
        }else if(message[0] == 'm')
        {
            Debug.Log(messageFull);
        }else if(message[0] == 'c')
        {
            Debug.ClearDeveloperConsole();
        }else if(message[0] == 'd')
        {
            Vector3 start = new Vector3(message[1], message[2]);
            Vector3 end = new Vector3(message[3], message[4]);
            char colorc = message[4];
            Color color = new Color();
            switch (colorc)
            {
                case 'g':
                    color = Color.green;
                    break;
                case 'w':
                    color = Color.white;
                    break;
                case 'r':
                    color = Color.red;
                    break;
                case 'b':
                    if (message[6] == 'a')
                    {
                        color = Color.black;
                    }
                    else if(message[6] == 'u')
                    {
                        color = Color.blue;
                    }
                    break;

            }
            Debug.DrawLine(start, end, color);
        }
        //Debug.Log(signalForce);
    }

    // Update is called once per frame
    void Update()
    {
        server.DataReceived += Server_DataRecieved;

        //if (Input.GetMouseButton(0))
        //{
        //    SetTargetPosition();
        //}
        //if (moving)
        //{
        //    //Move();
        //}

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
            strafePForce = 0.5f;
        }
        
        if (Input.GetKey(KeyCode.DownArrow))
        {
            strafeNForce = -0.5f;
        }
        
        if (Input.GetKey(KeyCode.RightArrow))
        {
            forwardForce = 0.5f;
        }
        
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            backwardForce = -0.5f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            rotationalForce = -0.5f;
        }

        if (Input.GetKey(KeyCode.D))
        {
            rotationalForce = 0.5f;
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
        AngularSpeed = rb.angularVelocity.magnitude / (float)(rotationalForce * (Math.PI * 2));

        rb.angularVelocity = new Vector3(0, (float)(rotationalForce * (Math.PI * 2)), 0);

        rb.SetMaxAngularVelocity(0.5f);

        //Debug.Log(signalForce);
        //Debug.Log(forwardForce);
        //Debug.Log(backwardForce);
        //Debug.Log(strafeNForce);
        //Debug.Log(strafePForce);
        if(rb.velocity.magnitude <= signalForce.magnitude)
        {
            rb.AddTorque(0.0f, rotationalForce + signaltorque, 0.0f, ForceMode.VelocityChange);
            //rb.AddForce(forwardForce + backwardForce + signalForce.x, 0, strafePForce + strafeNForce + signalForce.y, ForceMode.VelocityChange);
            rb.AddForce(signalForce.x, 0, signalForce.y, ForceMode.VelocityChange);
        }
        else if(rb.velocity.magnitude > signalForce.magnitude)
        {
            rb.velocity = rb.velocity.normalized * signalForce.magnitude;
        }
        else
        {
            rb.AddTorque(0, 0, 0, ForceMode.VelocityChange);
            rb.AddForce(0,0,0, ForceMode.VelocityChange);
        }
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
}
