using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
    class ForceCalculator
    {
        public float radius = (float)((4) / 78.74);

        private const float l = 5.8245f;
        private const float b = 4.811f;

        float stallT; //Stall torque of the motor
        float stallI; //Rtall current of the motor
        float Kt; //Torque constant of the motor
        float resistance; //Resistance of the motor
        float freespeed; //No-Load RPM
        float freecurrent; //No-Load Current
        float Kv; //Angular-Velocity Constant under no-load
        float gearatio;

        public ForceCalculator()
        {
            if (OptionsInterface.MotorType == 0)
            {
                gearatio = 1 / OptionsInterface.Ratio;
                stallT = (float)(OptionsInterface.Ratio * 8.75); //Stall torque of the motor
                stallI = 11.5f; //Rtall current of the motor
                Kt = stallT / stallI; //Torque constant of the motor
                resistance = 12 / stallI; //Resistance of the motor
                freespeed = ((6600 / OptionsInterface.Ratio) * 2 * (float)Math.PI) / 60; //No-Load Radians per Second
                freecurrent = 0.5f; //No-Load Current
                Kv = freespeed / (12 - (freecurrent * resistance)); //Angular-Velocity Constant under no-load
            }
        }

        public float getTrackWidth()
        {
            return l;
        }

        public float getWheelBase()
        {
            return b;
        }

        public float getFreeSpeed()
        {
            return freespeed;
        }

        public float update(float inputvoltage, float currentvelo)
        {
            double BackEMF = 0;
            float forwardforce = 0;

            BackEMF = -((Math.Pow(gearatio, 2) * Kt) / (Kv * resistance * Math.Pow(radius, 2)) * (currentvelo / 10));
            //Debug.Log("Current Velo: " + currentvelo);
            forwardforce = ((gearatio * Kt) / (resistance * radius)) * inputvoltage;

            return (float)BackEMF + forwardforce;
        }



        public Vector3 getForce(float ul, float bl, float ur, float br)
        {
            Vector3 upleft = new Vector3(0, 0, ul);
            Vector3 backleft = new Vector3(0, 0, bl);
            Vector3 upright = new Vector3(0, 0, ur);
            Vector3 backright = new Vector3(0, 0, br);

            upleft = Quaternion.Euler(0, -45, 0) * upleft;
            backleft = Quaternion.Euler(0, 45, 0) * backleft;
            upright = Quaternion.Euler(0, 45, 0) * upright;
            backright = Quaternion.Euler(0, -45, 0) * backright;

            //Debug.Log("upleft: " + upleft);
            //Debug.Log("backleft: " + backleft);
            //Debug.Log("upright: " + upright);
            //Debug.Log("backright: " + backright);

            //Vector3 x = upleft + backleft + upright + backright;
            //Vector3 y = -upleft + backleft + upright - backright;
            //Vector3 rot = (upleft * (-1/(l+b))) + (backleft * (-1 / (l + b))) + (upright * (1 / (l + b))) + (backright * (1 / (l + b)));

            Vector3 overall = upleft + backleft + upright + backright;
            Vector3 rot = (upleft * (-1 / (l + b))) + (backleft * (-1 / (l + b))) + (upright * (1 / (l + b))) + (backright * (1 / (l + b)));

            // Debug.Log("overall" + rot);

            return new Vector3(overall.x, rot.z, overall.z);
        }
    }
}
