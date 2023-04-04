using System;
using System.Collections;
using UnityEngine;

namespace DungeonGame.InputSystem
{
    public abstract class InputDevice : MonoBehaviour
    {
        public abstract Vector2 Movement();
        public abstract Vector2 Look();

        // maybe something like "RequestFocus()" for changing GameInput.CurrentDevice?
        // maybe something like "UpdateUI()", e.g. to set the visibility of the JoySticks?
        // how could we select items in the inventory with controllers?
        // maybe a cursor that can be controlled with the JoyStick like in other games?
    }
}