using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform target;
    public float smoothspeed = 0.125f;

    public Vector3 offset;

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothspeed);
        transform.position = smoothedPosition;

        transform.LookAt(target);
    }
}
