using System;
using UnityEngine;

//Simple first person flying movement
public class CharacterController : MonoBehaviour
{
    [SerializeField] private float _horizontalMoveSpeed = 1;
    [SerializeField] private float _verticalMoveSpeed = 1;
    [SerializeField] private float _lookSensitivity = 1;
    [SerializeField] private Camera _cam;

    [SerializeField] private float _maxPitch = 89;

    private float _pitch = 0;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMovement();
        HandleLook();
    }

    private void HandleMovement()
    {
        Vector2 horizontalMovement = Systems.InputSystem.HorizontalMovement * _horizontalMoveSpeed;
        float verticalMovement = Systems.InputSystem.VerticalMovement * _verticalMoveSpeed;

        Vector3 moveDir = 
            transform.right * horizontalMovement.x + 
            transform.forward * horizontalMovement.y + 
            Vector3.up * verticalMovement;

        transform.position += moveDir * Time.deltaTime;
    }

    private void HandleLook()
    {
        Vector2 look = Systems.InputSystem.LookDelta * _lookSensitivity; //TODO: not sure if this needs delta time
        
        transform.Rotate(Vector3.up * look.x);
        
        _pitch -= look.y * _lookSensitivity;
        _pitch = Mathf.Clamp(_pitch, -_maxPitch, _maxPitch);

        _cam.transform.localEulerAngles = new Vector3(_pitch, 0, 0);
    }
}
