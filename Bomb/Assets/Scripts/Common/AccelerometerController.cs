using UnityEngine;
using UnityEngine.InputSystem;

public class AccelerometerController : MonoBehaviour
{
    [SerializeField] private float _updateSpriteSpeed = 1f;
    [SerializeField] private float _updateLightSpeed = 20f;
    [SerializeField] private float _updateStoneCoef = 10f;

    [SerializeField] private Transform _spriteLava;
    [SerializeField] private Transform _spriteStone;
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
        _spriteLava.transform.position = Smooth(_spriteLava.transform.position, accel);
        _spriteStone.transform.position = Smooth(_spriteStone.transform.position, accel / _updateStoneCoef);
        _spriteMask.transform.position = Smooth(_spriteMask.transform.position, accel / _updateStoneCoef);
        _light.transform.position = accel * _updateLightSpeed;
    }

    private Vector3 Smooth(Vector3 sprite, Vector3 device)
    {
        _smoothedAccel = new Vector3(
            Mathf.Lerp(sprite.x, device.x, _updateSpeed * Time.deltaTime), 
            Mathf.Lerp(sprite.y, device.y, _updateSpeed * Time.deltaTime), 
            0f);
        return _smoothedAccel;
    }
}