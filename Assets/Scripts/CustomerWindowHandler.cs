using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomerWindowHandler : MonoBehaviour, IInteractable
{
    public static Action<CustomerWindowHandler> OnOrderComplete;

    [SerializeField] private GameObject _currentOrderPanel;
    [SerializeField] private CanvasGroup _orderScorePanel;
    [SerializeField] private TextMeshProUGUI _orderScoreText;
    [SerializeField] private TextMeshProUGUI _orderTimer;

    [SerializeField] private GameObject _ingredientPrefab;
    [SerializeField] private Sprite _cookedMeatIcon;
    [SerializeField] private Sprite _cutVegetablesIcon;
    [SerializeField] private Sprite _cheeseIcon;

    private List<Ingredient> _activeOrder = new();

    private bool _isOrderActive = false;
    private float _currentTime = 0f;
    private int _currentOrderScore = 0;

    [SerializeField] private float _initialYPosition = 150;
    [SerializeField] private float _targetYPosition = 180;

    private void Start()
    {
        _orderScoreText.text = "Score: 0";
        _orderScorePanel.alpha = 0f;
        _currentOrderPanel.SetActive(false);
        _orderTimer.transform.parent.gameObject.SetActive(false);
    }

    public void InitializeNewOrder(List<IngredientType> m_ingredients)
    {
        //Clear Previos Order for safer case.
        ClearCurrentOrder();
        //Populate the current order with the new ingredients.
        _currentOrderPanel.SetActive(true);
        foreach (IngredientType ingredient in m_ingredients)
        {
            GameObject _ingredient = Instantiate(_ingredientPrefab, _currentOrderPanel.transform);
            Ingredient ingredientComponent = _ingredient.AddComponent<Ingredient>();
            ingredientComponent.ingredientType = ingredient;
            _activeOrder.Add(ingredientComponent);
            _ingredient.transform.GetChild(0).GetComponent<Image>().sprite = GetIngredientIcon(ingredient);
        }

        _currentTime = 0f;
        _currentOrderScore = 0;
        _isOrderActive = true;

        StartCoroutine(StartOrderTimer());


    }

    private Sprite GetIngredientIcon(IngredientType m_type)
    {
        return m_type switch
        {
            IngredientType.CutVegetables => _cutVegetablesIcon,
            IngredientType.Cheese => _cheeseIcon,
            IngredientType.CookedMeat => _cookedMeatIcon,
            _ => _cheeseIcon
        };
    }

    private IEnumerator StartOrderTimer()
    {
        _orderTimer.transform.parent.gameObject.SetActive(true);
        while (_isOrderActive)
        {
            _orderTimer.text = TimeSpan.FromSeconds(_currentTime).ToString(@"m\:ss");
            yield return new WaitForSeconds(1f);
            _currentTime += 1f;
        }
    }

    private void StopOrderTimer()
    {
        _isOrderActive = false;
        _currentTime = 0f;
        _currentOrderScore = 0;
        _orderTimer.text = "0.00";
        _orderTimer.transform.parent.gameObject.SetActive(false);
    }

    private void ClearCurrentOrder()
    {
        _currentOrderPanel.SetActive(false);
        _activeOrder.Clear();
        foreach (Transform child in _currentOrderPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public float GetCurrentOrderTime()
    {
        return _currentTime;
    }

    public void Interact(PlayerController m_playerController)
    {
        IngredientType heldItemType = m_playerController.GetHeldItemType();
        switch (heldItemType)
        {
            case IngredientType.CutVegetables:
            case IngredientType.Cheese:
            case IngredientType.CookedMeat:

                Ingredient matchingIngredient = _activeOrder.Find(ingredient => ingredient.ingredientType == heldItemType);
                if (matchingIngredient != null)
                {
                    Debug.Log($"Player delivered {heldItemType}.");

                    m_playerController.DropItem();
                    foreach (Transform child in _currentOrderPanel.transform)
                    {
                        if (child.TryGetComponent(out Ingredient ingredient) &&
                            ingredient.ingredientType == heldItemType)
                        {
                            Destroy(child.gameObject);
                            break;
                        }
                    }
                    _activeOrder.Remove(matchingIngredient);
                    AddCurrentOrderScore(heldItemType);
                    if (_activeOrder.Count == 0)
                    {
                        Debug.Log("Order completed!");
                        _currentOrderScore -= (int)MathF.Floor(_currentTime);
                        _currentOrderScore = Mathf.Max(0, _currentOrderScore);
                        GameManager.Instance.UpdateScore(_currentOrderScore);
                        DisplayScore();
                        OnOrderComplete?.Invoke(this);
                        StopOrderTimer();
                        ClearCurrentOrder();
                    }
                }
                else
                {
                    Debug.Log($"{heldItemType} was not part of the order.");
                }
                break;
            case IngredientType.None:
                Debug.Log("Player is not holding any item.");
                break;
            case IngredientType.Vegetables:
                Debug.Log("Player is holding raw vegetables, which cannot be delivered.");
                break;
            case IngredientType.Meat:
                Debug.Log("Player is holding raw meat, which cannot be delivered.");
                break;
            default:
                Debug.Log("Unknown ingredient type.");
                break;
        }
    }
    public void AddCurrentOrderScore(IngredientType m_servedOrder)
    {
        switch (m_servedOrder)
        {
            case IngredientType.CookedMeat:
                _currentOrderScore += 30;
                break;
            case IngredientType.CutVegetables:
                _currentOrderScore += 20;
                break;

            case IngredientType.Cheese:
                _currentOrderScore += 10;
                break;

        }
    }

    private void DisplayScore()
    {
        _orderScoreText.text = $"Score: {_currentOrderScore}";
        StartCoroutine(AnimateAndFadeScoreText());
    }

    private IEnumerator AnimateAndFadeScoreText(float m_animDuration = 1f)
    {
        float elapsedTime = 0f;

        _orderScorePanel.transform.localPosition = new Vector3(
            _orderScorePanel.transform.localPosition.x,
            _initialYPosition,
            _orderScorePanel.transform.localPosition.z
        );

        _orderScoreText.alpha = 1f;

        while (elapsedTime < m_animDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / m_animDuration;

            _orderScorePanel.transform.localPosition = new Vector3(
                _orderScorePanel.transform.localPosition.x,
                Mathf.Lerp(_initialYPosition, _targetYPosition, t),
                _orderScorePanel.transform.localPosition.z
            );

            _orderScorePanel.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        // Make sure the final state is exact
        _orderScorePanel.transform.localPosition = new Vector3(
            _orderScorePanel.transform.localPosition.x,
            _targetYPosition,
            _orderScorePanel.transform.localPosition.z
        );

        _orderScoreText.alpha = 0;
    }

    public void ResetOrder()
    {
        StopAllCoroutines();
        _isOrderActive = false;
        _currentTime = 0f;

        ClearCurrentOrder();
    }

}