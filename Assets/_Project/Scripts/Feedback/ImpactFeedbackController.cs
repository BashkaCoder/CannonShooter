using UnityEngine;

namespace Feedback
{
    public class ImpactFeedbackController : MonoBehaviour
    {
        [SerializeField] private ImpactFeedbackProfile _profile;
        [SerializeField] private CameraShake _cameraShake;
        [SerializeField] private AudioSource _audioSource;
        
        private const float NormalOffset = 0.5f;
        private const float DestroyDelay = 1f;
        
        public void PlayShot(AudioClip shotSound)
        {
            _audioSource.PlayOneShot(shotSound);
        }

        public void PlayImpact(RaycastHit hit, bool isFinalImpact)
        {
            var vfxPrefab = isFinalImpact ? _profile.FinalImpactVfx : _profile.RegularImpactVfx;
            var stamp = isFinalImpact ? _profile.FinalDecalStamp : _profile.RegularDecalStamp;
            var stampSize = isFinalImpact ? _profile.FinalStampSize : _profile.RegularStampSize;
            var sound = isFinalImpact ? _profile.FinalImpactSound : _profile.RegularImpactSound;
            var shakeAmplitude = isFinalImpact ? _profile.FinalShakeAmplitude : _profile.RegularShakeAmplitude;
            var shakeDuration = isFinalImpact ? _profile.FinalShakeDuration : _profile.RegularShakeDuration;

            var rotation = Quaternion.LookRotation(hit.normal);
            var instance = Instantiate(vfxPrefab, hit.point + hit.normal * NormalOffset, rotation);
            Destroy(instance, DestroyDelay);

            var painter = hit.collider.GetComponentInParent<WallDecalPainter>();
            if (painter != null)
            {
                painter.Paint(hit, stamp, stampSize);
            }

            _audioSource.PlayOneShot(sound);
            _cameraShake.Shake(shakeAmplitude, shakeDuration);
        }
    }
}