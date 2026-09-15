using System.Collections;
using TMPro;
using UnityEngine;
public enum CookingState
{
    Empty,
    Cooking,
    Finished
}
public class CookingSlot : MonoBehaviour
{
    [SerializeField] private GameObject _timerPanel;
    [SerializeField] private TextMeshProUGUI _timerText;

    public Transform ItemPlacementPoint;

    public GameObject CookedItem { get; private set; }

    public CookingState State { get; private set; } = CookingState.Empty;

    private void Awake()
    {
        _timerPanel.SetActive(false);
    }

    public void StartCooking(float cookingTime)
    {
        State = CookingState.Cooking;
        StartCoroutine(StartTimer(cookingTime));
    }

    private IEnumerator StartTimer(float cookingTime)
    {
        _timerPanel.SetActive(true);

        float currentTime = cookingTime;

        while (currentTime > 0)
        {
            _timerText.text = Mathf.Ceil(currentTime).ToString();
            yield return new WaitForSeconds(1f);
            currentTime--;
        }

        //Replace raw item with cooked item
        if (ItemPlacementPoint.childCount > 0)
            Destroy(ItemPlacementPoint.GetChild(0).gameObject);
        yield return null;

        CookedItem = Instantiate(GameManager.Instance.GetIngredient(IngredientType.CookedMeat), ItemPlacementPoint);
        CookedItem.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        CookedItem.AddComponent<Ingredient>().ingredientType = IngredientType.CookedMeat;
        State = CookingState.Finished;
        _timerPanel.SetActive(false);
    }

    public void ClearSlot()
    {
        CookedItem = null;
        State = CookingState.Empty;
        _timerPanel.SetActive(false);
    }

    public void ResetStove()
    {
        StopAllCoroutines();
        ClearSlot();
        //Clear any object in the stove
        if (ItemPlacementPoint.childCount > 0)
            Destroy(ItemPlacementPoint.GetChild(0).gameObject);
    }

}