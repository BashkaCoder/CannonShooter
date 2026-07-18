using UnityEngine;

namespace Projectiles
{
    public static class ProjectilePhysicsSimulator
    {
        public static ProjectileSimulationResult Step(ref ProjectilePhysicsState state, ProjectileShapeDefinition shape,
            ProjectilePhysicsSettings settings, float deltaTime)
        {
            var velocity = state.Velocity;
            velocity += settings.Gravity * deltaTime;
            velocity -= velocity * (shape.Drag * deltaTime);

            var step = velocity * deltaTime;
            var distance = step.magnitude;

            if (distance <= Mathf.Epsilon)
            {
                state.Velocity = velocity;
                return new ProjectileSimulationResult(false, default);
            }

            var direction = step / distance;

            if (Cast(state.Position, direction, distance, shape, settings, out var hit))
            {
                state.Position = hit.point + hit.normal * settings.ContactOffset;
                state.Velocity = Vector3.Reflect(velocity, hit.normal) * settings.BounceDamping;
                return new ProjectileSimulationResult(true, hit);
            }

            state.Position += step;
            state.Velocity = velocity;
            return new ProjectileSimulationResult(false, default);
        }

        private static bool Cast(Vector3 origin, Vector3 direction, float distance, ProjectileShapeDefinition shape,
            ProjectilePhysicsSettings settings, out RaycastHit hit)
        {
            if (shape.CollisionShape == ProjectileCollisionShape.Sphere)
            {
                return Physics.SphereCast(origin, shape.SphereCastRadius, direction, out hit, distance,
                    settings.CollisionMask, QueryTriggerInteraction.Ignore);
            }

            return Physics.BoxCast(origin, shape.BoxCastSize * 0.5f, direction, out hit, 
                Quaternion.identity, distance, settings.CollisionMask, QueryTriggerInteraction.Ignore);
        }
    }
}