using UnityEngine;

namespace Feedback
{
    [CreateAssetMenu(fileName = "ImpactFeedbackProfile", menuName = "Flexus Test/Impact Feedback Profile")]
    public class ImpactFeedbackProfile : ScriptableObject
    {
        [Header("Regular Impact")]
        [field: SerializeField] public GameObject RegularImpactVfx { get; private set; }
        [field: SerializeField] public Texture2D RegularDecalStamp { get; private set; }
        [field: SerializeField] public float RegularStampSize { get; private set; }
        [field: SerializeField] public AudioClip RegularImpactSound { get; private set; }
        [field: SerializeField] public float RegularShakeAmplitude { get; private set; }
        [field: SerializeField] public float RegularShakeDuration { get; private set; }

        [Header("Final Impact")]
        [field: SerializeField] public GameObject FinalImpactVfx { get; private set; }
        [field: SerializeField] public Texture2D FinalDecalStamp { get; private set; }
        [field: SerializeField] public float FinalStampSize { get; private set; }
        [field: SerializeField] public AudioClip FinalImpactSound { get; private set; }
        [field: SerializeField] public float FinalShakeAmplitude { get; private set; }
        [field: SerializeField] public float FinalShakeDuration { get; private set; }
    }
}