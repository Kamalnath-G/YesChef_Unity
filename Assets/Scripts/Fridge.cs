using UnityEngine;
using UnityEngine.UI;

public class Fridge : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject _ingredientSelectionPanel;

    [SerializeField] Button _vegetablesButton;
    [SerializeField] Button _cheeseButton;
    [SerializeField] Button _meatButton;

    PlayerController _playerController;

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

    public void SelectIngredient(IngredientType ingredientType)
    {
        _ingredientSelectionPanel.SetActive(false);

        var ingredient = Instantiate(GameManager.Instance.GetIngredient(ingredientType));
        ingredient.AddComponent<Ingredient>().ingredientType = ingredientType;
        _playerController?.TryGrabItem(ingredient);
    }


}
