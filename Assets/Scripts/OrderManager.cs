using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    [SerializeField] private float _newOrderCooldownTime = 5;

    [SerializeField] private List<CustomerWindowHandler> _customerWindowHandler;
    private readonly IngredientType[] _possibleIngredients = {
        IngredientType.Cheese,
        IngredientType.CutVegetables,
        IngredientType.CookedMeat
    };

    public void OnEnable()
    {
        GameManager.OnGameStarted += OnGameStarted;
        CustomerWindowHandler.OnOrderComplete += OnOrderComplete;
    }

    private void OnOrderComplete(CustomerWindowHandler m_customer)
    {
        GameManager.Instance.StartActionWithDelay(_newOrderCooldownTime, () =>
        {
            m_customer.InitializeNewOrder(GenerateOrder());
        });
    }

    private void OnDisable()
    {
        GameManager.OnGameStarted -= OnGameStarted;
        CustomerWindowHandler.OnOrderComplete -= OnOrderComplete;
    }

    public List<IngredientType> GenerateOrder()
    {
        // 50% chance of 2, 50% chance of 3 ingredients.
        int ingredientCount = Random.value < 0.5f ? 2 : 3;
        List<IngredientType> order = new(ingredientCount);
        for (int i = 0; i < ingredientCount; i++)
        {
            IngredientType ingredient = _possibleIngredients[Random.Range(0, _possibleIngredients.Length)];
            order.Add(ingredient);
        }
        return order;
    }
    private void OnGameStarted()
    {
        foreach (CustomerWindowHandler customer in _customerWindowHandler)
        {
            customer.InitializeNewOrder(GenerateOrder());
        }
    }
}