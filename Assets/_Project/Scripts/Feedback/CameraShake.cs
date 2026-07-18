using System.Collections;
using UnityEngine;

namespace Feedback
{
    public class CameraShake : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _dampingFactor;
        
        private Coroutine _shakeRoutine;
        private Vector3 _initialLocalPosition;

        private void Awake()
        {
            _initialLocalPosition = _target.localPosition;
        }

        public void Shake(float amplitude, float duration)
        {
            if (_shakeRoutine != null)
            {
                StopCoroutine(_shakeRoutine);
                _target.localPosition = _initialLocalPosition;
            }

            _shakeRoutine = StartCoroutine(ShakeRoutine(amplitude, duration));
        }

        private IEnumerator ShakeRoutine(float amplitude, float duration)
        {
            var time = 0f;

            while (time < duration)
            {
                var t = 1f - time / duration;
                var offset = Random.insideUnitSphere * (amplitude * t);
                offset.z *= _dampingFactor;
                _target.localPosition = _initialLocalPosition + offset;
                time += Time.deltaTime;
                yield return null;
            }

            _target.localPosition = _initialLocalPosition;
            _shakeRoutine = null;
        }
    }
}