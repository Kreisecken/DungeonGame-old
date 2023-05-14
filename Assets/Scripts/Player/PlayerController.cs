using DungeonGame.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGame.Player
{
    public class PlayerController : MonoBehaviour
    {
        public Rigidbody2D rigidBody;
        public float speed; // maybe use a AnimationCurve for the movement here?
        
        private PlayerScript playerScript;
        
        void Start()
        {
            playerScript = GetComponent<PlayerScript>();
        }
        
        void FixedUpdate()
        {
            rigidBody.MovePosition(rigidBody.position + GameInput.Movement * Time.fixedDeltaTime * speed);
            
            // weapon
            playerScript.SetWeaponDirection(GameInput.Look);
            // TODO @NextLegacy: add this to GameInput
            playerScript.SetWeaponTrigger(Input.GetKey(KeyCode.Space));
        }
    }
}