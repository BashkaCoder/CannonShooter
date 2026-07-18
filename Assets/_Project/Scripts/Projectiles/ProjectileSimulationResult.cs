using UnityEngine;

namespace Projectiles
{
    public readonly struct ProjectileSimulationResult
    {
        public readonly bool HasHit;
        public readonly RaycastHit Hit;

        public ProjectileSimulationResult(bool hasHit, RaycastHit hit)
        {
            HasHit = hasHit;
            Hit = hit;
        }
    }
}