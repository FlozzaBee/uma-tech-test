using UnityEngine;
using Utils;

// Simple first person flying movement
public class FirstPersonCharacterController : MonoBehaviour
{
    [SerializeField] private float _horizontalMoveSpeed = 1;
    [SerializeField] private float _verticalMoveSpeed = 1;
    [SerializeField] private float _lookSensitivity = 1;
    [SerializeField] private float _movementSmoothing = 16;
    [SerializeField] private Camera _cam;

    [SerializeField] private float _maxPitch = 89;

    private Vector3 _targetPosition;

    private float _pitch = 0;

    private Vector3Int GridSize => GameGrid.Instance.GridSize;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _targetPosition = transform.position;
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

        _targetPosition += moveDir * Time.deltaTime;
        
        //Clamp position within grid
        _targetPosition.x = Mathf.Clamp(_targetPosition.x, -0.5f, GridSize.x - 0.5f);
        _targetPosition.y = Mathf.Clamp(_targetPosition.y, -0.5f, GridSize.y - 0.5f);
        _targetPosition.z = Mathf.Clamp(_targetPosition.z, -0.5f, GridSize.z - 0.5f);
        
        // Smooth movement
        transform.position =
            MathUtils.ExpDecay(transform.position, _targetPosition, _movementSmoothing, Time.deltaTime);
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
