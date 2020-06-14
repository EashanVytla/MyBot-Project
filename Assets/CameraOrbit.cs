using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraOrbit : MonoBehaviour
{
    private Transform XForm_Camera;
    private Transform XForm_Parent;

    private Vector3 LocalRotation;
    private float CameraDistance;

    public float MouseSensitivity = 4f;
    public float PanSensitivity = 4f;
    public float ScrollSensitivity = 2f;
    public float OrbitDampening = 10f;
    public float ScrollDampening = 6f;

    float ScrollWheel = 0;
    Vector2 MousePoint = Vector2.zero;
    private bool first = true;

    void Start()
    {
        this.XForm_Camera = this.transform;
        this.XForm_Parent = this.transform.parent;
    }

    void LateUpdate()
    {
        Mouse mouse = Mouse.current;

        MousePoint = mouse.delta.ReadValue();

        if (mouse.rightButton.ReadValue() == 1)
        {
            //Rotation of the camera
            if (MousePoint != Vector2.zero)
            {
                //Append the mouse input ot the rotation
                LocalRotation.x += MousePoint.x * MouseSensitivity;
                LocalRotation.y -= MousePoint.y * MouseSensitivity;

                //Restrict Camera orientation so it does not flip or go under the ground plane
                LocalRotation.y = Mathf.Clamp(LocalRotation.y, 0 - 36.471f, 90 - 36.471f);
            }

            

            //Actual Camera Rig Tranformations
            Quaternion QT = Quaternion.Euler(LocalRotation.y, LocalRotation.x, 0);

            this.XForm_Parent.rotation = Quaternion.Lerp(this.XForm_Parent.rotation, QT, Time.deltaTime * OrbitDampening);
        }

        ScrollWheel = mouse.scroll.ReadValue().y;

        if (ScrollWheel != 0)
        {
            //Getting Scroll Amount
            float ScrollAmount = ScrollWheel * ScrollSensitivity;

            //Allowing the scrolling to be faster as we get further away from the object
            ScrollAmount *= (this.CameraDistance * 0.3f);

            //Append camera distance
            this.CameraDistance += ScrollAmount * -1f;

            //This makes camera go no closer than .15 meters from target and no further than 10 meters

            this.CameraDistance = Mathf.Clamp(this.CameraDistance, .15f, 10);

            if (this.XForm_Parent.localPosition.z != this.CameraDistance * -1f)
            {
                this.XForm_Camera.localPosition = new Vector3(this.XForm_Camera.localPosition.x, this.XForm_Camera.localPosition.y, Mathf.Lerp(this.XForm_Camera.localPosition.z, this.CameraDistance * -1, Time.deltaTime * ScrollDampening));
            }
        }

        if (mouse.middleButton.ReadValue() == 1 || mouse.leftButton.ReadValue() == 1)
        {
            this.XForm_Camera.localPosition += new Vector3(MousePoint.x * -PanSensitivity, MousePoint.y * -PanSensitivity, 0);
        }
    }

    public void reset()
    {
        this.XForm_Camera.localPosition = new Vector3(0.02f, 2.05f, -2.5f);
        this.XForm_Parent.rotation = new Quaternion(0, 0, 0, 1);
    }
}
