using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CustomerWindowHandler : MonoBehaviour, IInteractable
{
    public static Action<float> OnOrderComplete;

    [SerializeField] private GameObject _currentOrderPanel;
    [SerializeField] private TextMeshProUGUI _orderTimer;

    bool _isOrderActive = false;
    float _currentTime = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void InitializeNewOrder(List<GameObject> Ingredients)
    {
        //Clear Previos Order for safer case.
        ClearCurrentOrder();
        //Populate the current order with the new ingredients.
        foreach (GameObject ingredient in Ingredients)
        {
            Instantiate(ingredient, _currentOrderPanel.transform);
        }

        _isOrderActive = true;
        StartCoroutine(StartOrderTimer());


    }

    private IEnumerator StartOrderTimer()
    {
        while (_isOrderActive)
        {
            _orderTimer.text = TimeSpan.FromSeconds(_currentTime).ToString(@"m\:ss");
            yield return new WaitForSeconds(1f);
            _currentTime += 1f;
        }
    }

    void StopOrderTimer()
    {
        _isOrderActive = false;
        _currentTime = 0f;
        _orderTimer.text = "0.00";
    }

    void ClearCurrentOrder()
    {
        foreach (Transform child in _currentOrderPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public float GetCurrentOrderTime()
    {
        return _currentTime;
    }

    public void Interact(PlayerController m_PlayerController)
    {
        m_PlayerController.DropItem();
    }
}