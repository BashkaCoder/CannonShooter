using UnityEngine;

namespace Trajectory
{
    public class TrajectoryImpactMarker : MonoBehaviour
    {
        [SerializeField] private float _regularScale;
        [SerializeField] private float _finalScale;
        
        private const float NormalOffset = 0.025f;

        public void SetState(Vector3 position, Vector3 normal, bool isFinal)
        {
            transform.position = position + normal * NormalOffset;
            transform.rotation = Quaternion.LookRotation(normal);
            var scale = isFinal ? _finalScale : _regularScale;
            transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}