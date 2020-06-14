using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocks : MonoBehaviour
{
    public Transform[] block = new Transform[12];
    public Vector3[] startpos = new Vector3[12];
    public Transform platform1;
    public Transform platform2;

    public void reset()
    {
       for(int i = 0; i < 12; i++)
       {
            block[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            block[i].transform.localPosition = startpos[i];
       }

       platform1.localPosition = new Vector3(-0.002f, 0.01f, -0.012f);
       platform1.localRotation = Quaternion.Euler(0, 0, 0);

       platform2.localPosition = new Vector3(0, 0, 0);
       platform2.localRotation = Quaternion.Euler(0, 0, 0);
    }
}
