using UnityEngine;

namespace Cannon
{
    [CreateAssetMenu(fileName = "CannonSettings", menuName = "Flexus Test/Cannon Settings")]
    public class CannonSettings : ScriptableObject
    {
        [Header("Aim")]
        [field: SerializeField] public float MouseSensitivity { get; private set; }
        [field: SerializeField] public float StickSensitivity { get; private set; }
        [field: SerializeField] public float KeyboardSensitivity { get; private set; }
        [field: SerializeField] public float MinYaw { get; private set; }
        [field: SerializeField] public float MaxYaw { get; private set; }
        [field: SerializeField] public float MinPitch { get; private set; }
        [field: SerializeField] public float MaxPitch { get; private set; }

        [Header("Power")]
        [field: SerializeField] public float MinPower { get; private set; }
        [field: SerializeField] public float MaxPower { get; private set; }
        [field: SerializeField] public float InitialPower { get; private set; }
        [field: SerializeField] public float PowerAdjustSpeed { get; private set; }
        [field: SerializeField] public float MouseWheelPowerScale { get; private set; }

        [Header("Shot")]
        [field: SerializeField] public float ShotCooldown { get; private set; }
    }
}