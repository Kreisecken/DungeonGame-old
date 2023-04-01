using System.Collections;
using UnityEngine;

namespace DungeonGame.InputSystem
{
    public class MobileInputDevice : InputDevice
    {
        public JoyStick MovementStick;
        public JoyStick LookStick;

        public override Vector2 Movement()
        {
            return MovementStick.Output;
        }

        public override Vector2 Look()
        {
            return LookStick.Output;
        }
    }
}