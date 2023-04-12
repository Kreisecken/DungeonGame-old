using DungeonGame.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public class TriangulationTest : MonoBehaviour
    {
        public List<Vector2> points;

        public List<Triangle<Vector2>> triangles;
        int a;

        [ContextMenu("Randomize")]
        public void Randomize()
        {
            SeedableRandom random = a++;

            for (int i = 0; i < 10; i++)
            {
                points.Add(random.PointInsideUnitCircle() * 100);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            foreach (var point in points)
            {
                Gizmos.DrawSphere(point, 2);
            }

            Gizmos.color = Color.blue;
            foreach (var triangle in triangles)
            {
                //Gizmos.DrawWireSphere(triangle.circumCircle.center, triangle.circumCircle.radius);

                Handles.color = Color.green;

                Handles.DrawLine(triangle.vertecies[0].position, triangle.vertecies[1].position);
                Handles.DrawLine(triangle.vertecies[0].position, triangle.vertecies[2].position);
                Handles.DrawLine(triangle.vertecies[1].position, triangle.vertecies[2].position);
            }
        }

        [ContextMenu("Update")]
        public void UpdateTriangle()
        {
            List<Vertex<Vector2>> vertecies = new();

            foreach (var point in points)
                vertecies.Add(new(point, point));

            triangles = Triangulation.Triangulate(vertecies);

            Debug.Log(triangles.Count);
        }
    }
}