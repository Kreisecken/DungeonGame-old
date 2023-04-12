using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public class SuperTriangleTest : MonoBehaviour
    {
        public new BoxCollider2D collider;

        private void OnDrawGizmos()
        {
            Bounds bounds = collider.bounds;

            Gizmos.DrawCube(bounds.center, bounds.size);

            Vector3 min = bounds.center - bounds.size / 2;
            Vector3 max = bounds.center + bounds.size / 2;

            var tcenter = bounds.center + new Vector3(0, bounds.size.y / 2);
            var bcenter = bounds.center - new Vector3(0, bounds.size.y / 2);

            var diagonal = (max - min).magnitude;

            Vector3 a = bcenter + new Vector3(-1f , 0) * diagonal;
            Vector3 b = tcenter + new Vector3(0, 0.5f) * diagonal;
            Vector3 c = bcenter + new Vector3(1f  , 0) * diagonal;

            Handles.DrawLine(a, b);
            Handles.DrawLine(a, c);
            Handles.DrawLine(b, c);
        }
    }
}