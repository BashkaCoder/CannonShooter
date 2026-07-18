using System.Collections.Generic;
using UnityEngine;

namespace Projectiles
{
    public static class ProjectileMeshFactory
    {
        private static readonly Vector3[] BoxVertices =
        {
            new(-1f, -1f, 1f), new(1f, -1f, 1f), new(1f, 1f, 1f), new(-1f, 1f, 1f),
            new(-1f, -1f, -1f), new(1f, -1f, -1f), new(1f, 1f, -1f), new(-1f, 1f, -1f)
        };

        private static readonly int[] BoxTriangles =
        {
            0, 2, 1, 0, 3, 2,
            4, 5, 6, 4, 6, 7,
            0, 7, 3, 0, 4, 7,
            1, 2, 6, 1, 6, 5,
            3, 7, 6, 3, 6, 2,
            0, 1, 5, 0, 5, 4
        };

        public static Mesh Create(ProjectileShapeDefinition shape)
        {
            return shape.MeshKind == ProjectileMeshKind.JitteredSphere
                ? CreateJitteredSphere(shape)
                : CreateRandomizedBox(shape);
        }

        private static Mesh CreateRandomizedBox(ProjectileShapeDefinition shape)
        {
            var vertices = new Vector3[BoxVertices.Length];
            var scale = shape.VisualScale * 0.5f;
            var jitter = shape.MeshJitter;

            for (var i = 0; i < vertices.Length; i++)
            {
                var random = new Vector3(
                    Random.Range(-jitter, jitter),
                    Random.Range(-jitter, jitter),
                    Random.Range(-jitter, jitter));

                vertices[i] = Vector3.Scale(BoxVertices[i] + random, scale);
            }

            var mesh = new Mesh
            {
                name = shape.Name + "_GeneratedBox",
                vertices = vertices,
                triangles = BoxTriangles
            };
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        private static Mesh CreateJitteredSphere(ProjectileShapeDefinition shape)
        {
            const int latitudeSegments = 6;
            const int longitudeSegments = 10;
            const int lastRingStart = 1 + (latitudeSegments - 2) * longitudeSegments;
            
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            var radius = shape.SphereCastRadius;

            vertices.Add(Vector3.up * radius);

            for (var latitude = 1; latitude < latitudeSegments; latitude++)
            {
                var theta = Mathf.PI * latitude / latitudeSegments;
                var sinTheta = Mathf.Sin(theta);
                var cosTheta = Mathf.Cos(theta);

                for (var lon = 0; lon < longitudeSegments; lon++)
                {
                    var phi = 2f * Mathf.PI * lon / longitudeSegments;
                    var normal = new Vector3(Mathf.Cos(phi) * sinTheta, cosTheta, Mathf.Sin(phi) * sinTheta);

                    var jitteredRadius = radius * Random.Range(1f - shape.MeshJitter, 1f + shape.MeshJitter);
                    vertices.Add(normal * jitteredRadius);
                }
            }

            var bottomIndex = vertices.Count;
            vertices.Add(Vector3.down * radius);

            for (var longitude = 0; longitude < longitudeSegments; longitude++)
            {
                var nextLon = (longitude + 1) % longitudeSegments;
                triangles.Add(0);
                triangles.Add(1 + longitude);
                triangles.Add(1 + nextLon);
            }

            for (var latitude = 0; latitude < latitudeSegments - 2; latitude++)
            {
                var ringStart = 1 + latitude * longitudeSegments;
                var nextRingStart = ringStart + longitudeSegments;

                for (var longitude = 0; longitude < longitudeSegments; longitude++)
                {
                    var nextLon = (longitude + 1) % longitudeSegments;
                    triangles.Add(ringStart + longitude);
                    triangles.Add(nextRingStart + longitude);
                    triangles.Add(ringStart + nextLon);

                    triangles.Add(ringStart + nextLon);
                    triangles.Add(nextRingStart + longitude);
                    triangles.Add(nextRingStart + nextLon);
                }
            }
            
            for (var longitude = 0; longitude < longitudeSegments; longitude++)
            {
                var nextLon = (longitude + 1) % longitudeSegments;
                triangles.Add(lastRingStart + nextLon);
                triangles.Add(lastRingStart + longitude);
                triangles.Add(bottomIndex);
            }

            var mesh = new Mesh
            {
                name = shape.Name + "_GeneratedSphere"
            };
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}