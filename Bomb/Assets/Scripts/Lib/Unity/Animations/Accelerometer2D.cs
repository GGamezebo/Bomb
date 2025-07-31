using UnityEngine;
using UnityEngine.InputSystem;

namespace Lib.Unity.Animations
{
    public class Accelerometer2D : MonoBehaviour
    {
        [SerializeField] private const float UpdateSpriteSpeed = 1f;
        [SerializeField] private const float UpdateLightSpeed = 20f;
        [SerializeField] private const float UpdateStoneCoef = 10f;

        [SerializeField] private Transform _backgroundSprite;
        [SerializeField] private Transform _frontgroundSprite;
        [SerializeField] private Transform _spriteMask;
        [SerializeField] private Transform _light;

        private Vector3 _deviceAccel;
        private Vector3 _smoothedAccel;

        void Start()
        {
            InputSystem.EnableDevice(Accelerometer.current);
        }

        void Update()
        {
            _deviceAccel = Accelerometer.current.acceleration.value;

            ApplyAccel(_deviceAccel);
        }

        private void ApplyAccel(Vector3 accel)
        {
            _backgroundSprite.transform.position = Smooth(_backgroundSprite.transform.position, accel);
            _frontgroundSprite.transform.position = Smooth(_frontgroundSprite.transform.position, accel / UpdateStoneCoef);
            _spriteMask.transform.position = Smooth(_spriteMask.transform.position, accel / UpdateStoneCoef);
            _light.transform.position = accel * UpdateLightSpeed;
        }

        private Vector3 Smooth(Vector3 sprite, Vector3 device)
        {
            _smoothedAccel = new Vector3(
                Mathf.Lerp(sprite.x, device.x, UpdateSpriteSpeed * Time.deltaTime),
                Mathf.Lerp(sprite.y, device.y, UpdateSpriteSpeed * Time.deltaTime),
                0f);
            return _smoothedAccel;
        }
    }
}