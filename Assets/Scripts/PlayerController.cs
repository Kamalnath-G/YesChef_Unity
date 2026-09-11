using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static event Action<GameObject> OnPlayerInteracted;

    [Header("Movement")]
    PlayerInputActions _inputActions;
    private Vector2 _moveInput;
    [SerializeField] private float _moveSpeed = 5f;
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
        _inputActions.Enable(); //Temp
        GameManager.OnGameOver += OnGameStarted;
        GameManager.OnGameOver += OnGameOver;

        _inputActions.Player.Move.performed += Move_performed;
        _inputActions.Player.Move.canceled += Move_canceled;

        _inputActions.Player.Interact.performed += Interact_performed;
    }


    private void OnDisable()
    {
        _inputActions.Disable(); //Temp
        GameManager.OnGameOver -= OnGameStarted;
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
    }
    private void OnGameOver()
    {
        _inputActions.Disable();
    }

    public bool TryGrabItem(GameObject m_Item)
    {
        if (_heldItem != null)
            return false;

        GrabItem(m_Item);
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
    void GrabItem(GameObject m_Item)
    {
        //Attach the m_Item to the player.
        m_Item.transform.SetParent(_objectAnchor.transform);
        m_Item.transform.localPosition = Vector3.zero;
        m_Item.transform.localRotation = Quaternion.identity;
        //Disable the m_Item's collider and rigidbody to prevent physics interactions while held.
        Collider itemCollider = m_Item.GetComponent<Collider>();
        if (itemCollider != null)
            itemCollider.enabled = false;
        Rigidbody itemRigidbody = m_Item.GetComponent<Rigidbody>();
        if (itemRigidbody != null)
            itemRigidbody.isKinematic = true;

        _heldItem = m_Item;
    }

    public void DropItem()
    {
        if (_heldItem == null)
            return;
        //Detach the m_Item from the player.
        Destroy(_heldItem);
        _heldItem = null;
    }

    public void PlaceItem(Transform m_Parent)
    {
        if (_heldItem == null)
            return;

        //Detach the m_Item from the player.
        _heldItem.transform.SetParent(m_Parent);
        _heldItem.transform.localPosition = Vector3.zero;
        _heldItem.transform.localRotation = Quaternion.identity;
        _heldItem = null;
    }


    private void OnDrawGizmos()
    {
        //Draw a line in front of the player to visualize the interaction distance.
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * _interactionDistance);
    }

}