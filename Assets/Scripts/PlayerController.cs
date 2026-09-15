using System;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static event Action<GameObject> OnPlayerInteracted;

    [Header("Movement")]
    private PlayerInputActions _inputActions;
    private Vector2 _moveInput;
    [SerializeField] private float _moveSpeed = 7f;
    [SerializeField] private float _rotationSpeed = 500f;

    [Header("Interaction")]
    [SerializeField] private float _interactionDistance = 1f;
    [SerializeField] private LayerMask _interactionLayer;

    [SerializeField] private GameObject _objectAnchor;
    private GameObject _heldItem;


    #region Unity Functions
    private void Awake()
    {
        _inputActions = new PlayerInputActions();
    }
    private void OnEnable()
    {
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameOver += OnGameOver;

        _inputActions.Player.Move.performed += Move_performed;
        _inputActions.Player.Move.canceled += Move_canceled;

        _inputActions.Player.Interact.performed += Interact_performed;
    }
    private void OnDisable()
    {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOver -= OnGameOver;

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
            if (hit.collider.gameObject.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact(this);
            }
            OnPlayerInteracted?.Invoke(hit.collider.gameObject);
        }
    }
    private void OnGameStarted()
    {
        _inputActions.Enable();
        transform.SetLocalPositionAndRotation(Vector3.zero, quaternion.identity);
    }
    private void OnGameOver()
    {
        _inputActions.Disable();
    }
    public bool TryGrabItem(GameObject m_item)
    {
        if (_heldItem != null)
            return false;

        GrabItem(m_item);
        return true;
    }
    public bool IsHoldingItem()
    {
        return _heldItem != null;
    }
    public IngredientType GetHeldItemType()
    {
        if (_heldItem == null)
            return IngredientType.None;
        return _heldItem.GetComponent<Ingredient>().ingredientType;
    }
    private void GrabItem(GameObject m_item)
    {
        //Attach the m_item to the player.
        m_item.transform.SetParent(_objectAnchor.transform);
        m_item.transform.localPosition = Vector3.zero;
        m_item.transform.localRotation = Quaternion.identity;
        //Disable the m_item's collider and rigidbody to prevent physics interactions while held.
        Collider itemCollider = m_item.GetComponent<Collider>();
        if (itemCollider != null)
            itemCollider.enabled = false;
        Rigidbody itemRigidbody = m_item.GetComponent<Rigidbody>();
        if (itemRigidbody != null)
            itemRigidbody.isKinematic = true;

        _heldItem = m_item;
    }

    public void DropItem()
    {
        if (_heldItem == null)
            return;
        //Detach the m_item from the player.
        Destroy(_heldItem);
        _heldItem = null;
    }

    public void PlaceItem(Transform m_parent)
    {
        if (_heldItem == null)
            return;

        //Detach the m_item from the player.
        _heldItem.transform.SetParent(m_parent);
        _heldItem.transform.localPosition = Vector3.zero;
        _heldItem.transform.localRotation = Quaternion.identity;
        _heldItem = null;
    }

    public void ResetPlayer()
    {
        DropItem();
        _moveInput = Vector2.zero;
        transform.SetLocalPositionAndRotation(new Vector3(0, 0.5f, 0), Quaternion.identity);
    }

    private void OnDrawGizmos()
    {
        //Draw a line in front of the player to visualize the interaction distance.
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + (transform.forward * _interactionDistance));
    }

}