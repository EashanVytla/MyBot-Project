using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freight : MonoBehaviour
{
    ArrayList initialPositions = new ArrayList();
    public GameObject field;

    void Start()
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            initialPositions.Add(child.localPosition);
        }
    }

    public void reset()
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        int counter = 0;
        foreach (Transform child in allChildren)
        {
            child.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            child.transform.localPosition = (Vector3)initialPositions[counter];
            Debug.Log(initialPositions[counter]);
            counter++;
        }


        transform.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        field.transform.rotation = Quaternion.Euler(new Vector3(0f, 270f, 0f));
    }
}
