using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets
{
    class TeleOp
    {
        TeleOpActions teleControls;
        private Vector2 move;
        private Vector2 rotation;

        public TeleOp(){
            teleControls = new TeleOpActions();

            teleControls.Gamepad.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
            teleControls.Gamepad.Move.canceled += ctx => move = Vector2.zero;

            teleControls.Gamepad.Rotate.performed += ctx => rotation = ctx.ReadValue<Vector2>();
            teleControls.Gamepad.Rotate.canceled += ctx => rotation = Vector2.zero;
        }

        public Vector3 getOverallVec()
        {
            if (!OptionsInterface.FC)
            {
                return new Vector3(move.x * OptionsInterface.movementspeed, rotation.x * OptionsInterface.turningspeed, move.y * OptionsInterface.movementspeed);
            }
            else
            {
                return Quaternion.Euler(0, -(float)WheelController.heading, 0) * new Vector3(move.x * OptionsInterface.movementspeed, rotation.x * OptionsInterface.turningspeed, move.y * OptionsInterface.movementspeed);
            }
        }

        public void Enable()
        {
            teleControls.Gamepad.Enable();
        }

        public void Disable()
        {
            teleControls.Gamepad.Disable();
        }
    }
}
