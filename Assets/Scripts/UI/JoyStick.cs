using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEditor;

namespace DungeonGame.InputSystem
{
    public class JoyStick : MonoBehaviour
    {
        [Header("UI Elements")]
        public Canvas canvas;
        public RectTransform innerPart;
        public RectTransform outerPart;

        [Header("Settings")]
        public Vector3 center;
        public float radius;

        // TODO: use AnimationCurve, pretty nice way to add dead zones and make the JoyStick smoother
        public float speed;

        [Header("Output")]
        public Vector3 Output;

        private Vector3 position;
        private Vector3 direction;

        private void Update()
        {
            // TODO: use touch input instead of the mouse
            // TODO: only make certain area clickable/usable

            MoveJoystick(Input.GetMouseButton(0) ? Input.mousePosition : center);
        }

        public void MoveJoystick(Vector3 destination)
        {
            direction = Vector3.ClampMagnitude(destination - center, radius);

            position = Vector3.Lerp(position, direction, speed * Time.deltaTime);

            Output = position / radius;

            innerPart.transform.position = center + position;
        }

        // EditorUtilities

        [ContextMenu("Set center to outerpart position")]
        public void SetCenterToOuterPartPosition()
        {
            center = outerPart.transform.position;
        }

        [ContextMenu("Set radius to outerpart width")]
        public void SetRadiusToOuterPartWidth()
        {
            radius = outerPart.rect.width * canvas.scaleFactor * 0.5f;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(center, radius);
        }
    }
}