using UnityEngine;

namespace Projectiles
{
    [System.Serializable]
    public class ProjectileShapeDefinition
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public ProjectileMeshKind MeshKind { get; private set; }
        [field: SerializeField] public ProjectileCollisionShape CollisionShape { get; private set; }
        [field: SerializeField] public Material Material { get; private set; }
        [field: SerializeField] public Material TrailMaterial { get; private set; }
        [field: SerializeField] public Vector3 VisualScale { get; private set; }
        [field: SerializeField] public Vector3 BoxCastSize { get; private set; }
        [field: SerializeField] public float SphereCastRadius { get; private set; }
        [field: SerializeField] public float Drag { get; private set; }
        [field: SerializeField] public float MeshJitter { get; private set; }
    }
}