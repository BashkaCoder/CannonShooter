using UnityEngine;

namespace Trajectory
{
    [CreateAssetMenu(fileName = "TrajectorySettings", menuName = "Flexus Test/Trajectory Settings")]
    public class TrajectorySettings : ScriptableObject
    {
        [field: SerializeField] public int MaxSteps { get; private set; }
        [field: SerializeField] public float TimeStep { get; private set; }
        [field: SerializeField] public int MaxPredictedBounces { get; private set; }
        [field: SerializeField] public float LineWidth { get; private set; }
    }
}