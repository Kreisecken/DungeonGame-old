using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace DungeonGame.InputSystem
{
    // just a rough idea of how it might be possible to implement multiple input devices

    public class GameInput : MonoBehaviour
    {
        public static GameInput Instance { get; private set; }

        public static Vector2 Movement { get => CurrentDevice.Movement(); }
        public static Vector2 Look     { get => CurrentDevice.Look    (); }

        public static InputDevice CurrentDevice { get; private set; }

        public static PCInputDevice         PC         { get; private set; }
        public static MobileInputDevice     Mobile     { get; private set; }
        public static ControllerInputDevice Controller { get; private set; }

        private void Awake()
        {
            Instance = this;

            TryGetComponent(out PCInputDevice         pc        );
            TryGetComponent(out MobileInputDevice     mobile    );
            TryGetComponent(out ControllerInputDevice controller);

            PC         = pc        ;
            Mobile     = mobile    ;
            Controller = controller;
        }
    }

    public abstract class InputDevice : MonoBehaviour
    {
        public abstract Vector2 Movement();
        public abstract Vector2 Look    ();

        // maybe something like "RequestFocus()" for changing GameInput.CurrentDevice?
        // maybe something like "UpdateUI()", e.g. to set the visibility of the JoySticks?
        // how could we select items in the inventory with controllers?
        // maybe a cursor that can be controlled with the JoyStick like in other games?
    }
}