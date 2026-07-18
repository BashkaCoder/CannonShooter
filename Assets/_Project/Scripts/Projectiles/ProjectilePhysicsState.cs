using UnityEngine;

namespace Projectiles
{
    public struct ProjectilePhysicsState
    {
        public Vector3 Position;
        public Vector3 Velocity;

        public ProjectilePhysicsState(Vector3 position, Vector3 velocity)
        {
            Position = position;
            Velocity = velocity;
        }
    }
}