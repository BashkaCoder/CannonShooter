using Feedback;
using Projectiles;
using Trajectory;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cannon
{
    public class CannonController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private CannonSettings _settings;
        [SerializeField] private ProjectilePhysicsSettings _projectilePhysics;
        [SerializeField] private ProjectileShapeCatalog _shapeCatalog;

        [Header("Scene References")]
        [SerializeField] private Transform _yawPivot;
        [SerializeField] private Transform _pitchPivot;
        [SerializeField] private Transform _muzzle;
        [SerializeField] private ProjectileView _projectilePrefab;
        [SerializeField] private TrajectoryPreview _trajectoryPreview;
        [SerializeField] private CannonHudView _hud;
        [SerializeField] private CannonRecoil _recoil;
        [SerializeField] private ImpactFeedbackController _feedback;
        [SerializeField] private ParticleSystem _muzzleFlash;
        [SerializeField] private AudioClip _shotSound;

        private ProjectileShapeDefinition _nextShape;
        
        private float _yaw;
        private float _pitch;
        private float _power;
        
        private float _lastShotTime;
        private bool _outputsDirty = true;

        private void Awake()
        {
            _power = _settings.InitialPower;
            SelectNextShape();
            UpdateOutputs();
        }

        private void Update()
        {
            var changed = UpdateAim();
            changed |= UpdatePower();

            if (changed || _outputsDirty)
            {
                UpdateOutputs();
                _outputsDirty = false;
            }

            if (WasShootPressed())
            {
                Shoot();
            }
        }

        private bool UpdateAim()
        {
            var aim = ReadAimInput(out var isPointerInput);
            if (aim.sqrMagnitude <= float.Epsilon)
            {
                return false;
            }

            var multiplier = isPointerInput ? _settings.MouseSensitivity : GetContinuousAimSensitivity() * Time.deltaTime;
            _yaw += aim.x * multiplier;
            _pitch -= aim.y * multiplier;

            _yaw = Mathf.Clamp(_yaw, _settings.MinYaw, _settings.MaxYaw);
            _pitch = Mathf.Clamp(_pitch, _settings.MinPitch, _settings.MaxPitch);

            _yawPivot.localRotation = Quaternion.Euler(0f, _yaw, 0f);
            _pitchPivot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
            return true;
        }

        private static Vector2 ReadAimInput(out bool isPointerInput)
        {
            if (Mouse.current != null)
            {
                var mouseDelta = Mouse.current.delta.ReadValue();
                if (mouseDelta.sqrMagnitude > float.Epsilon)
                {
                    isPointerInput = true;
                    return mouseDelta;
                }
            }

            if (Gamepad.current != null)
            {
                var stick = Gamepad.current.rightStick.ReadValue();
                if (stick.sqrMagnitude > float.Epsilon)
                {
                    isPointerInput = false;
                    return stick;
                }
            }

            if (Keyboard.current != null)
            {
                var keyboard = Vector2.zero;
                if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
                {
                    keyboard.x -= 1f;
                }
                if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
                {
                    keyboard.x += 1f;
                }
                if (Keyboard.current.upArrowKey.isPressed || Keyboard.current.wKey.isPressed)
                {
                    keyboard.y += 1f;
                }
                if (Keyboard.current.downArrowKey.isPressed || Keyboard.current.sKey.isPressed)
                {
                    keyboard.y -= 1f;
                }

                if (keyboard.sqrMagnitude > 1f)
                {
                    keyboard.Normalize();
                }

                isPointerInput = false;
                return keyboard;
            }

            isPointerInput = false;
            return Vector2.zero;
        }

        private float GetContinuousAimSensitivity()
        {
            return Gamepad.current != null && Gamepad.current.rightStick.ReadValue().sqrMagnitude > float.Epsilon
                ? _settings.StickSensitivity
                : _settings.KeyboardSensitivity;
        }

        private bool UpdatePower()
        {
            var input = ReadPowerInput(out var isMouseWheel);
            if (Mathf.Abs(input) <= float.Epsilon)
            {
                return false;
            }

            var scale = isMouseWheel
                ? _settings.MouseWheelPowerScale
                : _settings.PowerAdjustSpeed * Time.deltaTime;

            _power = Mathf.Clamp(_power + input * scale, _settings.MinPower, _settings.MaxPower);
            return true;
        }

        private static float ReadPowerInput(out bool isMouseWheel)
        {
            if (Mouse.current != null)
            {
                var wheel = Mouse.current.scroll.ReadValue().y;
                if (Mathf.Abs(wheel) > 0.0001f)
                {
                    isMouseWheel = true;
                    return wheel;
                }
            }

            var input = 0f;

            if (Keyboard.current != null)
            {
                if (Keyboard.current.qKey.isPressed || Keyboard.current.minusKey.isPressed)
                {
                    input -= 1f;
                }
                if (Keyboard.current.eKey.isPressed || Keyboard.current.equalsKey.isPressed)
                {
                    input += 1f;
                }
            }

            if (Gamepad.current != null)
            {
                input += Gamepad.current.dpad.ReadValue().y;
                input += Gamepad.current.rightTrigger.ReadValue();
                input -= Gamepad.current.leftTrigger.ReadValue();
            }

            isMouseWheel = false;
            return Mathf.Clamp(input, -1f, 1f);
        }

        private static bool WasShootPressed()
        {
            var mouseShoot = Mouse.current.leftButton.wasPressedThisFrame;
            var keyboardShoot = Keyboard.current.spaceKey.wasPressedThisFrame ||
                                Keyboard.current.enterKey.wasPressedThisFrame;
            var gamepadShoot = Gamepad.current != null &&
                               (Gamepad.current.buttonSouth.wasPressedThisFrame ||
                                Gamepad.current.rightShoulder.wasPressedThisFrame);

            return mouseShoot || keyboardShoot || gamepadShoot;
        }

        private void Shoot()
        {
            if (Time.time < _lastShotTime + _settings.ShotCooldown)
            {
                return;
            }

            _lastShotTime = Time.time;
            var projectile = Instantiate(_projectilePrefab, _muzzle.position, _muzzle.rotation);
            projectile.Initialize(_nextShape, _projectilePhysics, _feedback, _muzzle.forward * _power);

            _recoil.Play();
            _muzzleFlash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _muzzleFlash.Play();
            _feedback.PlayShot(_shotSound);

            SelectNextShape();
            _outputsDirty = true;
        }

        private void SelectNextShape()
        {
            _nextShape = _shapeCatalog.GetRandom();
            _outputsDirty = true;
        }

        private void UpdateOutputs()
        {
            _hud.SetValues(_power, _settings.MinPower, _settings.MaxPower, _yaw, _pitch);
            _trajectoryPreview.Draw(_muzzle.position, _muzzle.forward * _power, _nextShape, _projectilePhysics);
        }
    }
}