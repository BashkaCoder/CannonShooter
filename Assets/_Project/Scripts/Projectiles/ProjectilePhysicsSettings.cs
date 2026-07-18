using UnityEngine;

namespace Projectiles
{
    [CreateAssetMenu(fileName = "ProjectilePhysicsSettings", menuName = "Flexus Test/Projectile Physics Settings")]
    public class ProjectilePhysicsSettings : ScriptableObject
    {
        [field: SerializeField] public Vector3 Gravity { get; private set; }
        [field: SerializeField] public float BounceDamping { get; private set; }
        [field: SerializeField] public int MaxBounces { get; private set; }
        [field: SerializeField] public float ContactOffset { get; private set; }
        [field: SerializeField] public float MaxTravelDistance { get; private set; }
        [field: SerializeField] public LayerMask CollisionMask { get; private set; } 
    }
}