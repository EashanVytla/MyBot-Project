using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rings : MonoBehaviour
{
    public Transform[] rings = new Transform[2];
    public Vector3[] startpos = new Vector3[2];

    public void reset()
    {
        for (int i = 0; i < 2; i++)
        {
            rings[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            rings[i].transform.localPosition = startpos[i];
        }
    }
}
