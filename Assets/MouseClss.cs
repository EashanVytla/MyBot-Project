using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;
using UnityEngine;

namespace Assets
{
    class MouseClss
    {
        private static UnityEngine.Vector2 MousePoint;

        public static UnityEngine.Vector2 getPose()
        {
            Mouse mouse = Mouse.current;
            MousePoint = mouse.position.ReadValue();

            return MousePoint;
        } 
    }
}
