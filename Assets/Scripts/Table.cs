using System.Collections;
using TMPro;
using UnityEngine;

public class Table : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform _itemPlacementPoint;
    [SerializeField] private GameObject _timerPanel;
    [SerializeField] private TextMeshProUGUI _timerText;

    [SerializeField] private float _vegetableCutTime = 2f; // Time in seconds to cut the vegetables

    private void Awake()
    {
        _timerPanel.SetActive(false);
        _timerText.text = _vegetableCutTime.ToString();
    }
    public void Interact(PlayerController m_playerController)
    {
        //whether there's an item on the Table
        if (_itemPlacementPoint.childCount > 0)
        {
            //Try to grab the item from the Table.
            if (m_playerController.TryGrabItem(_itemPlacementPoint.GetChild(0).gameObject))
            {
                StopTimer(); // Stop the timer if the player grabs the item from the Table
            }
        }
        else
        {
            //If the player is already holding an item, place it on the Table instead only if it Vegetable.
            if (m_playerController.GetHeldItemType() == IngredientType.Vegetables)
            {
                m_playerController.PlaceItem(_itemPlacementPoint);
                StartCoroutine(StartTimer());
            }
        }
    }

    private IEnumerator StartTimer()
    {
        _timerPanel.SetActive(true);
        float currentTime = _vegetableCutTime;
        while (currentTime > 0)
        {
            _timerText.text = Mathf.Ceil(currentTime).ToString();
            yield return new WaitForSeconds(1f);
            currentTime--;
        }

        // Replace the raw vegetables with cut vegetables once cutting time is complete
        if (_itemPlacementPoint.childCount > 0)
            Destroy(_itemPlacementPoint.GetChild(0).gameObject);
        yield return null; // Wait for the next frame to ensure the raw vegetables are destroyed before instantiating the cut vegetables

        GameObject cutItem = Instantiate(GameManager.Instance.GetIngredient(IngredientType.CutVegetables), _itemPlacementPoint);
        cutItem.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        cutItem.AddComponent<Ingredient>().ingredientType = IngredientType.CutVegetables;
        StopTimer();
    }

    private void StopTimer()
    {
        StopAllCoroutines();
        _timerPanel.SetActive(false);
    }

    public void ResetTable()
    {
        StopTimer();
        //Clear any object in the Table
        if (_itemPlacementPoint.childCount > 0)
            Destroy(_itemPlacementPoint.GetChild(0).gameObject);
    }

}