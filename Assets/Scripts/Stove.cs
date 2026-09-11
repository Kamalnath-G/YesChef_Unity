using System.Collections;
using TMPro;
using UnityEngine;

public class Stove : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform _itemPlacementPoint;
    [SerializeField] private GameObject _timerPanel;
    [SerializeField] private TextMeshProUGUI _timerText;

    [SerializeField] private float _cookingTime = 6f; // Time in seconds to cook the meat

    private void Awake()
    {
        _timerPanel.SetActive(false);
        _timerText.text = _cookingTime.ToString();
    }

    public void Interact(PlayerController m_PlayerController)
    {
        //whether there's an item on the stove
        if (_itemPlacementPoint.childCount > 0)
        {
            //Try to grab the item from the stove.
            if (m_PlayerController.TryGrabItem(_itemPlacementPoint.GetChild(0).gameObject))
            {
                StopTimer(); // Stop the timer if the player grabs the item from the stove
            }
        }
        else
        {
            //If the player is already holding an item, place it on the stove instead only if it meat.
            if (m_PlayerController.GetHeldItemType() == IngredientType.Meat)
            {
                m_PlayerController.PlaceItem(_itemPlacementPoint);
                StartCoroutine(StartTimer());
            }
        }
    }

    IEnumerator StartTimer()
    {
        _timerPanel.SetActive(true);
        var currentTime = _cookingTime;
        while (currentTime > 0)
        {
            _timerText.text = Mathf.Ceil(currentTime).ToString();
            yield return new WaitForSeconds(1f);
            currentTime--;
        }

        // Replace the raw meat with cooked meat once cooking time is complete

        Destroy(_itemPlacementPoint.GetChild(0).gameObject);
        yield return null; // Wait for the next frame to ensure the raw meat is destroyed before instantiating the cooked meat

        var cookedItem = Instantiate(GameManager.Instance.GetIngredient(IngredientType.CookedMeat), _itemPlacementPoint);
        cookedItem.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        cookedItem.AddComponent<Ingredient>().ingredientType = IngredientType.CookedMeat;
        StopTimer();
    }

    void StopTimer()
    {
        StopAllCoroutines();
        _timerPanel.SetActive(false);
    }
}
