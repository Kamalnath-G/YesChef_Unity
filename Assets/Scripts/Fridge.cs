using UnityEngine;
using UnityEngine.UI;

public class Fridge : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _ingredientSelectionPanel;

    [SerializeField] private Button _vegetablesButton;
    [SerializeField] private Button _cheeseButton;
    [SerializeField] private Button _meatButton;

    private PlayerController _playerController;

    private void Awake()
    {
        _vegetablesButton.onClick.AddListener(() => SelectIngredient(IngredientType.Vegetables));
        _cheeseButton.onClick.AddListener(() => SelectIngredient(IngredientType.Cheese));
        _meatButton.onClick.AddListener(() => SelectIngredient(IngredientType.Meat));

        _ingredientSelectionPanel.SetActive(false);
    }

    public void Interact(PlayerController m_playerController)
    {
        _playerController = m_playerController;
        if (_playerController.IsHoldingItem())
            return;
        _ingredientSelectionPanel.SetActive(!_ingredientSelectionPanel.activeSelf);
    }

    public void SelectIngredient(IngredientType m_ingredientType)
    {
        _ingredientSelectionPanel.SetActive(false);

        GameObject ingredient = Instantiate(GameManager.Instance.GetIngredient(m_ingredientType));
        ingredient.AddComponent<Ingredient>().ingredientType = m_ingredientType;
        _playerController?.TryGrabItem(ingredient);
    }

    public void ResetFridge()
    {
        _ingredientSelectionPanel.SetActive(false);
    }

}
