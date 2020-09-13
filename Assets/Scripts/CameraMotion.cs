using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotion : MonoBehaviour
{
    [SerializeField] [Range(1,50)] float _movementSpeed = 20f;
    [SerializeField] [Range(1,10)] float _rotationSpeed = 2f;

    float _pitch = 0f;
    float _yaw = 0f;

    void Start() 
    {
        _pitch = transform.eulerAngles.x;
        _yaw = transform.eulerAngles.y;
    }

    void Update()
    {
        MoveCamera();
    }

    void MoveCamera()
    {
        if (Input.GetKey("w")) 
            transform.position += transform.forward * _movementSpeed * Time.deltaTime;

        if (Input.GetKey("a"))
            transform.position -= transform.right * _movementSpeed * Time.deltaTime;
        
        if (Input.GetKey("d"))
            transform.position += transform.right * _movementSpeed * Time.deltaTime;

        if (Input.GetKey("s"))
            transform.position -= transform.forward * _movementSpeed * Time.deltaTime;

        if (Input.GetKey("e"))
            transform.position += transform.up * _movementSpeed * Time.deltaTime;

        if (Input.GetKey("q"))
            transform.position -= transform.up * _movementSpeed * Time.deltaTime;

        if (Input.GetMouseButton(2))
        {
            _pitch -= _rotationSpeed * Input.GetAxis("Mouse Y"); 
            _yaw += _rotationSpeed * Input.GetAxis("Mouse X");

            transform.eulerAngles = new Vector3 (_pitch, _yaw, 0f);
        }
    }
}
