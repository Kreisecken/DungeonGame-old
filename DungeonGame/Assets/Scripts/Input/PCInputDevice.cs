using System;
using System.Collections;
using UnityEngine;

namespace DungeonGame.InputSystem
{
    public class PCInputDevice : InputDevice
    {
        public override Vector2 Movement()
        {
            return Input.GetAxisRaw("Horizontal") * Vector2.right + Input.GetAxisRaw("Vertical") * Vector2.up;
        }

        public override Vector2 Look()
        {
            // not yet tested
            return Input.mousePosition - new Vector3(Screen.width, Screen.height) / 2;
        }
    }
}