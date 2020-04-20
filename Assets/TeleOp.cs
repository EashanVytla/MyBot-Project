using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
    class TeleOp
    {
        public float getXForce()
        {
            double strafePForce = 0;
            double strafeNForce = 0;

            //Keyboard Input Update:
            if (Input.GetKey(KeyCode.W))
            {
                strafePForce = BlockController.newsignalScale;
            }
            else
            {
                strafePForce = 0;
            }

            if (Input.GetKey(KeyCode.S))
            {
                strafeNForce = -BlockController.newsignalScale;
            }
            else
            {
                strafeNForce = 0;
            }

            return (float)(strafePForce + strafeNForce);
        }

        public float getYForce()
        {
            double forwardForce;
            double backwardForce;
            if (Input.GetKey(KeyCode.D))
            {
                forwardForce = -BlockController.newsignalScale;
            }
            else
            {
                forwardForce = 0;
            }
        
            if (Input.GetKey(KeyCode.A))
            {
                backwardForce = BlockController.newsignalScale;
            }
            else
            {
                backwardForce = 0;
            }

            return (float)(forwardForce + backwardForce);
        }

        public float getTorque()
        {
            double rotationalForceN;
            double rotationalForceP;

            if (Input.GetKey(KeyCode.J))
            {
                rotationalForceN = -0.5f;
            }
            else
            {
                rotationalForceN = 0;
            }

            if (Input.GetKey(KeyCode.L))
            {
                rotationalForceP = 0.5f;
            }
            else
            {
                rotationalForceP = 0;
            }

            return (float)(rotationalForceN + rotationalForceP);
        }
    }
}
