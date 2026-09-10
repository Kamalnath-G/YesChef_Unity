using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    PlayerInputActions _inputActions;
    private Vector2 _moveInput;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 500f;

    [Header("Interaction")]
    [SerializeField] private float _interactionDistance = 3f;
    [SerializeField] private LayerMask _interactionLayer;

    #region Unity Functions

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _inputActions.Enable();

        _inputActions.Player.Move.performed += Move_performed;
        _inputActions.Player.Move.canceled += Move_canceled;

        _inputActions.Player.Interact.performed += Interact_performed;
    }


    private void OnDisable()
    {
        _inputActions.Disable();

        _inputActions.Player.Move.performed -= Move_performed;
        _inputActions.Player.Move.canceled -= Move_canceled;
    }

    private void FixedUpdate()
    {
        if (_moveInput == Vector2.zero) return;

        Vector3 movementDirection = new Vector3(_moveInput.x, 0f, _moveInput.y).normalized;
        transform.position += movementDirection * _moveSpeed * Time.fixedDeltaTime;

        //Rotate towards movement direction.
        Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.fixedDeltaTime);
    }

    #endregion
    private void Move_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _moveInput = obj.ReadValue<Vector2>();
    }
    private void Move_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _moveInput = Vector2.zero;
    }
    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        //Raycast to detect interactable objects in front of the player.
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, _interactionDistance, _interactionLayer))
        {


            //Debug.Log("Interacted with: " + hit.collider.gameObject.name);
        }
    }

    private void OnDrawGizmos()
    {
        //Draw a line in front of the player to visualize the interaction distance.
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * _interactionDistance);
    }

}