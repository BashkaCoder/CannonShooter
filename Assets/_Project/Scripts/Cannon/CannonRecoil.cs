using System.Collections;
using UnityEngine;

namespace Cannon
{
    public class CannonRecoil : MonoBehaviour
    {
        [SerializeField] private Transform _barrel;
        [SerializeField] private float _recoilDistance;
        [SerializeField] private float _backDuration;
        [SerializeField] private float _returnDuration;

        private Vector3 _initialLocalPosition;
        private Coroutine _routine;

        private void Awake()
        {
            _initialLocalPosition = _barrel.localPosition;
        }

        public void Play()
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
                _barrel.localPosition = _initialLocalPosition;
            }

            _routine = StartCoroutine(PlayRoutine());
        }

        private IEnumerator PlayRoutine()
        {
            var recoilPosition = _initialLocalPosition + Vector3.back * _recoilDistance;

            yield return MoveBarrel(_initialLocalPosition, recoilPosition, _backDuration);
            yield return MoveBarrel(recoilPosition, _initialLocalPosition, _returnDuration);

            _routine = null;
        }
        
        private IEnumerator MoveBarrel(Vector3 from, Vector3 to, float duration)
        {
            if (duration <= 0f)
            {
                _barrel.localPosition = to;
                yield break;
            }

            var elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                var normalizedTime = Mathf.Clamp01(elapsedTime / duration);
                _barrel.localPosition = Vector3.LerpUnclamped(from, to, normalizedTime);
                
                yield return null;
            }

            _barrel.localPosition = to;
        }
    }
}