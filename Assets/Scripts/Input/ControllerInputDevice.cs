using System.Collections;
using UnityEngine;

namespace DungeonGame.InputSystem
{
    public class ControllerInputDevice : InputDevice
    {
        public override Vector2 Movement()
        {
            return Vector2.zero;
        }

        public override Vector2 Look()
        {
            return Vector2.zero;
        }
    }
}