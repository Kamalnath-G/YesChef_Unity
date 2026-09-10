using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CustomerWindowHandler : MonoBehaviour
{
    [SerializeField] private GameObject _currentOrder;
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
            Instantiate(ingredient, _currentOrder.transform);
        }

        _isOrderActive = true;
        StartCoroutine(StartOrderTimer());


    }

    private IEnumerator StartOrderTimer()
    {
        while (_isOrderActive)
        {
            _orderTimer.text = _currentTime.ToString("F1");
            yield return new WaitForSeconds(0.1f);
            _currentTime += 0.1f;
            _orderTimer.text = "0.0";
        }
    }

    void StopOrderTimer()
    {
        _isOrderActive = false;
        _currentTime = 0f;
        _orderTimer.text = "0.0";
    }

    void ClearCurrentOrder()
    {
        foreach (Transform child in _currentOrder.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public float GetCurrentOrderTime()
    {
        return _currentTime;
    }

}