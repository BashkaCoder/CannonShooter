using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CannonHudView : MonoBehaviour
    {
        [SerializeField] private Slider _powerSlider;
        [SerializeField] private TMP_Text _powerText;
        [SerializeField] private TMP_Text _yawText;
        [SerializeField] private TMP_Text _pitchText;

        public void SetValues(float power, float minPower, float maxPower, float yaw, float pitch)
        {
            _powerSlider.minValue = minPower;
            _powerSlider.maxValue = maxPower;
            _powerSlider.value = power;

            _powerText.text = $"POWER {power:0}";
            
            _yawText.text = $"YAW {yaw:0.0}";
            
            _pitchText.text = $"PITCH {pitch:0.0}";
        }
    }
}