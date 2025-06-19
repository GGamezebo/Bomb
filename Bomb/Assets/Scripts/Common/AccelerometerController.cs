using UnityEngine;
using UnityEngine.InputSystem;

public class AccelerometerController : MonoBehaviour
{
    [SerializeField] private float _updateSpriteSpeed = 3;
    [SerializeField] private float _updateLightSpeed = 20;
    [SerializeField] private Transform _sprite;
    [SerializeField] private Transform _light;

    private Vector3 _deviceAccel;
    private Vector3 _smoothedAccel;

    void Start()
    {
        //InputSystem.EnableDevice(Accelerometer.current);
    }

    void Update()
    {
        //_deviceAccel = Accelerometer.current.acceleration.value;

        //SpriteMove(_deviceAccel);
    }

    private void SpriteMove(Vector3 accel)
    {
        _sprite.transform.position = Smooth(_sprite.transform.position, accel);
        _light.transform.position = _deviceAccel * _updateLightSpeed;
    }

    private Vector3 Smooth(Vector3 sprite, Vector3 device)
    {
        _smoothedAccel = new Vector3(
            Mathf.Lerp(sprite.x, device.x, _updateSpriteSpeed * Time.deltaTime), 
            Mathf.Lerp(sprite.y, device.y, _updateSpriteSpeed * Time.deltaTime), 
            0f);
        return _smoothedAccel;
    }
}