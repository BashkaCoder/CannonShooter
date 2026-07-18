using System.Collections.Generic;
using Projectiles;
using UnityEngine;

namespace Trajectory
{
    public class TrajectoryPreview : MonoBehaviour
    {
        [SerializeField] private TrajectorySettings _settings;
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private TrajectoryImpactMarker _regularImpactMarkerPrefab;
        [SerializeField] private TrajectoryImpactMarker _finalImpactMarkerPrefab;

        private readonly List<TrajectoryImpactMarker> _activeMarkers = new();
        private readonly List<Vector3> _points = new();

        public void Draw(Vector3 startPosition, Vector3 startVelocity, ProjectileShapeDefinition shape, 
            ProjectilePhysicsSettings physicsSettings)
        {
            _points.Clear();
            HideMarkers();

            var state = new ProjectilePhysicsState(startPosition, startVelocity);
            _points.Add(startPosition);
            var bounceCount = 0;

            for (var i = 0; i < _settings.MaxSteps; i++)
            {
                var result = ProjectilePhysicsSimulator.Step(ref state, shape, physicsSettings, _settings.TimeStep);

                _points.Add(state.Position);

                if (!result.HasHit)
                {
                    continue;
                }

                bounceCount++;
                var isFinal = bounceCount >= physicsSettings.MaxBounces;
                ShowMarker(result.Hit.point, result.Hit.normal, isFinal);

                if (isFinal || bounceCount >= _settings.MaxPredictedBounces)
                {
                    break;
                }
            }

            _lineRenderer.positionCount = _points.Count;
            _lineRenderer.SetPositions(_points.ToArray());
            _lineRenderer.startWidth = _settings.LineWidth;
            _lineRenderer.endWidth = _settings.LineWidth;
        }

        private void ShowMarker(Vector3 position, Vector3 normal, bool isFinal)
        {
            var prefab = isFinal ? _finalImpactMarkerPrefab : _regularImpactMarkerPrefab;
            var marker = Instantiate(prefab, transform);
            marker.SetState(position, normal, isFinal);
            _activeMarkers.Add(marker);
        }

        private void HideMarkers()
        {
            foreach (var impactMarker in _activeMarkers)
            {
                Destroy(impactMarker.gameObject);
            }
            _activeMarkers.Clear();
        }
    }
}