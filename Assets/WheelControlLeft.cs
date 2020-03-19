using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelControlLeft : MonoBehaviour
{
    Rigidbody rb;
    public float forwardForce = 2000f;
    public float sideForce = 200f;
    // Start is called before the first frame update
    void Start()
    {
        rb = new Rigidbody();
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(0, 0, forwardForce * Time.deltaTime);

        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(10,0,0,ForceMode.Force);
        }
        else
        {
            rb.AddForce(0, 0, 0, 0);
        }
    }
}
