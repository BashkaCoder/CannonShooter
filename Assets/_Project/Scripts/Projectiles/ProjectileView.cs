using Feedback;
using UnityEngine;

namespace Projectiles
{
    public class ProjectileView : MonoBehaviour
    {
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private TrailRenderer _trailRenderer;

        private ProjectileShapeDefinition _shape;
        private ProjectilePhysicsSettings _settings;
        private ImpactFeedbackController _feedback;
        private ProjectilePhysicsState _state;
        private Vector3 _spawnPosition;
        
        private int _bounceCount;
        private bool _isInitialized;

        public void Initialize(ProjectileShapeDefinition shape, ProjectilePhysicsSettings settings, 
            ImpactFeedbackController feedback, Vector3 velocity)
        {
            _shape = shape;
            _settings = settings;
            _feedback = feedback;
            _state = new ProjectilePhysicsState(transform.position, velocity);
            _spawnPosition = transform.position;
            _bounceCount = 0;
            _isInitialized = true;

            _meshFilter.sharedMesh = ProjectileMeshFactory.Create(shape);
            _meshRenderer.sharedMaterial = shape.Material;
            
            _trailRenderer.Clear();
            _trailRenderer.sharedMaterial = shape.TrailMaterial;
        }

        private void FixedUpdate()
        {
            if (!_isInitialized)
            {
                return;
            }

            if (Vector3.Distance(_spawnPosition, transform.position) > _settings.MaxTravelDistance)
            {
                Destroy(gameObject);
                return;
            }

            var result = ProjectilePhysicsSimulator.Step(ref _state, _shape, _settings, Time.fixedDeltaTime);
            transform.position = _state.Position;
            if (!result.HasHit)
            {
                return;
            }
            _bounceCount++;
            
            var isFinalImpact = _bounceCount >= _settings.MaxBounces;
            _feedback.PlayImpact(result.Hit, isFinalImpact);
            if (isFinalImpact)
            {
                Destroy(gameObject);
            }
        }
    }
}